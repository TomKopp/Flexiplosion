using FlexiWallUI.Utilities;
using FlexiWallUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FlexiWallUI.Views
{
    /// <summary>
    /// Interaction logic for BubbleView.xaml
    /// </summary>
    public partial class BubbleView : UserControl
    {
        public BubbleView()
        {
            InitializeComponent();

            //FlexiWall_Video.Loaded += delegate
            //{
            //    _flexiReady = true;
            //};

            //FlexiWall_Video.MediaEnded += delegate
            //{
            //    FlexiWall_Video.Position = TimeSpan.Zero;
            //    FlexiWall_Video.Play();
            //    FlexiWall_Video.Pause();
            //};

            _storyboard.Add(AnimationType.Interfaces, Resources["Storyboard1"] as Storyboard);
            //_storyboard.Add(AnimationType.Interfaces, Resources["AnimationInterfaces"] as Storyboard);
            //_storyboard.Add(AnimationType.Data, Resources["AnimationData"] as Storyboard);
            //_storyboard.Add(AnimationType.Systems, Resources["AnimationSystems"] as Storyboard);
            //_storyboard.Add(AnimationType.Innovation, Resources["AnimationInnovation"] as Storyboard);

            StartAllStoryboards();

            UpdateStoryboard();
        }

        private const int FramesToSkip = 1;
        private readonly List<Storyboard> _stoppedSbs = new List<Storyboard>();
        private readonly Dictionary<AnimationType, Storyboard> _storyboard = new Dictionary<AnimationType, Storyboard>();
        private TimeSpan _billTimeSpan = TimeSpan.FromSeconds(22);
        private bool _flexiReady;
        private int _skipped = 0;
        private BubbleViewModel _vm;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = DataContext as BubbleViewModel;
            if (_vm == null)
                return;

            _vm.AnimationUpdated += delegate { UpdateStoryboard(); };
        }

        private void StartAllStoryboards()
        {
            _storyboard.Keys.ToList().ForEach(key =>
            {
                _storyboard[key].Begin();
                _storyboard[key].Pause();
            });
        }

        private void StopAllStoryboards()
        {
            _stoppedSbs.Clear();
            _storyboard.Keys.ToList().ForEach(key =>
            {
                var stopSb = _storyboard[key];

                _stoppedSbs.Add(stopSb);
                stopSb.Stop();

                //stopSb.Resume();
                //stopSb.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
                //stopSb.Pause();
            });
        }

        private void StopOtherStoryBoards(AnimationType type)
        {
            _storyboard.Keys.Where(key => !Equals(key, type)).ToList().ForEach(key =>
            {
                var stopSb = _storyboard[key];

                if (_stoppedSbs.Contains(stopSb))
                    return;

                _stoppedSbs.Add(stopSb);
                stopSb.Stop();

                //stopSb.Resume();
                //stopSb.Seek(TimeSpan.FromMilliseconds(0), TimeSeekOrigin.BeginTime);
                //stopSb.Pause();
            });
        }

        /// <summary>
        /// Updates the storyboard.
        /// </summary>
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

            //_skipped++;
            //_skipped = _skipped % FramesToSkip;
            //if (_skipped != 0)
            //    return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                StopOtherStoryBoards(_vm.CurrentType);

                var currentSb = _storyboard[_vm.CurrentType];
                if (currentSb == null)
                    return;

                var ts = TimeSpan.FromMilliseconds(currentSb.Duration.TimeSpan.TotalMilliseconds * pos);

                //if (_stoppedSbs.Contains(currentSb))
                //{
                //    currentSb.Begin();
                //    _stoppedSbs.Remove(currentSb);
                //}
                //else
                //{
                //    currentSb.Resume();
                //}

                currentSb.Begin(this, true);
                currentSb.Pause(this);
                currentSb.Seek(this, ts, TimeSeekOrigin.BeginTime);
                currentSb.Pause(this);

                //var showFlexi = _vm.CurrentType == AnimationType.Systems && _flexiReady;

                //if (showFlexi)
                //{
                //    var videoPos = (pos - 0.25) / 0.75;
                //    if (videoPos < 0)
                //        videoPos = 0;

                //    if (showFlexi)
                //    {
                //        FlexiWall_Video.Play();
                //        FlexiWall_Video.Position = TimeSpan.FromMilliseconds(_billTimeSpan.TotalMilliseconds * videoPos);

                //        FlexiWall_Video.Pause();
                //    }
                //}
            });
        }
    }
}
