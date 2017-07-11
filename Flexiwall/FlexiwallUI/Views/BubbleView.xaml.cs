using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using FlexiWallUI.Utilities;
using FlexiWallUI.ViewModels;
using FlexiWallUI.Properties;

namespace FlexiWallUI.Views
{
    /// <summary>
    /// Interaction logic for BubbleView.xaml
    /// </summary>
    public partial class BubbleView : UserControl
    {
        private readonly Dictionary<AnimationType, Storyboard> _sb = new Dictionary<AnimationType, Storyboard>();
        private readonly List<Storyboard> _stoppedSbs = new List<Storyboard>();
        private BubbleViewModel _vm;

        private const int FramesToSkip = 1;
        private int _skipped = 0;

        private bool _flexiReady;
        private TimeSpan _billTimeSpan = TimeSpan.FromSeconds(22);

        public BubbleView()
        {
            InitializeComponent();

            FlexiWall_Video.Loaded += delegate
            {
                _flexiReady = true;
            };

            FlexiWall_Video.MediaEnded += delegate
            {
                FlexiWall_Video.Position = TimeSpan.Zero;
                FlexiWall_Video.Play();
                FlexiWall_Video.Pause();
            };


            _sb.Add(AnimationType.Interfaces, Resources["AnimationInterfaces"] as Storyboard);
            _sb.Add(AnimationType.Data, Resources["AnimationData"] as Storyboard);
            _sb.Add(AnimationType.Systems, Resources["AnimationSystems"] as Storyboard);
            _sb.Add(AnimationType.Innovation, Resources["AnimationInnovation"] as Storyboard);

            StartAllStoryboards();

            UpdateStoryboard();
        }

        private void UpdateStoryboard()
        {
            if (_vm == null)
                return;



            if (_vm.TransitionPosition < 0)
            {
                StopAllStoryboards();
                return;
            }

            var pos = _vm.TransitionPosition;
            if (pos > 1)
                pos = 1;

            _skipped++;
            _skipped = _skipped % FramesToSkip;
            if (_skipped != 0)
                return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                StopOtherStoryBoards(_vm.CurrentType);

                var currentSb = _sb[_vm.CurrentType];
                if (currentSb == null)
                    return;

                var ts = TimeSpan.FromMilliseconds(currentSb.Duration.TimeSpan.TotalMilliseconds * pos);



                if (_stoppedSbs.Contains(currentSb))
                {
                    currentSb.Begin();
                    _stoppedSbs.Remove(currentSb);
                }
                else
                {
                    currentSb.Resume();
                }                    

                currentSb.Seek(ts, TimeSeekOrigin.BeginTime);
                currentSb.Pause();

                var showFlexi = _vm.CurrentType == AnimationType.Systems && _flexiReady;


                if (showFlexi)
                {
                    var videoPos = (pos - 0.25) / 0.75;
                    if (videoPos < 0)
                        videoPos = 0;


                    if (showFlexi)
                    {
                        FlexiWall_Video.Play();
                        FlexiWall_Video.Position = TimeSpan.FromMilliseconds(_billTimeSpan.TotalMilliseconds * videoPos);

                        FlexiWall_Video.Pause();
                    }
                }

            });
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = DataContext as BubbleViewModel;
            if (_vm == null)
                return;

            _vm.AnimationUpdated += delegate { UpdateStoryboard(); };
        }

        private void StartAllStoryboards()
        {
            _sb.Keys.ToList().ForEach(key =>
            {
                _sb[key].Begin();
                _sb[key].Pause();
            });
        }

        private void StopAllStoryboards()
        {
            _stoppedSbs.Clear();
            _sb.Keys.ToList().ForEach(key =>
            {
                var stopSb = _sb[key];

                _stoppedSbs.Add(stopSb);
                stopSb.Stop();

                //stopSb.Resume();
                //stopSb.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
                //stopSb.Pause();
            });
        }


        private void StopOtherStoryBoards(AnimationType type)
        {
            _sb.Keys.Where(key => !Equals(key, type)).ToList().ForEach(key =>
            {
                var stopSb = _sb[key];

                if (_stoppedSbs.Contains(stopSb))
                    return;

                _stoppedSbs.Add(stopSb);
                stopSb.Stop();

                //stopSb.Resume();
                //stopSb.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
                //stopSb.Pause();
            });
        }
    }

}
