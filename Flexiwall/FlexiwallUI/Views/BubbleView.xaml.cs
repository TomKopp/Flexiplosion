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
        /// <summary>
        /// Initializes a new instance of the <see cref="BubbleView"/> class.
        /// </summary>
        public BubbleView()
        {
            InitializeComponent();

            _storyboard.Add(AnimationType.Interfaces, Resources["Storyboard1"] as Storyboard);

            StartAllStoryboards();
            UpdateStoryboard();
        }

        private const int FramesToSkip = 1;
        private readonly List<Storyboard> _stoppedSbs = new List<Storyboard>();
        private readonly Dictionary<AnimationType, Storyboard> _storyboard = new Dictionary<AnimationType, Storyboard>();
        private TimeSpan _billTimeSpan = TimeSpan.FromSeconds(22);
        private BubbleViewModel _vm;

        /// <summary>
        /// Called when [data context changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = DataContext as BubbleViewModel;
            if (_vm == null)
                return;

            _vm.AnimationUpdated += delegate { UpdateStoryboard(); };
        }

        /// <summary>
        /// Starts all storyboards.
        /// </summary>
        private void StartAllStoryboards()
        {
            _storyboard.Keys.ToList().ForEach(key =>
            {
                _storyboard[key].Begin();
                _storyboard[key].Pause();
            });
        }

        /// <summary>
        /// Stops all storyboards.
        /// </summary>
        private void StopAllStoryboards()
        {
            _stoppedSbs.Clear();
            _storyboard.Keys.ToList().ForEach(key =>
            {
                var stopSb = _storyboard[key];

                _stoppedSbs.Add(stopSb);
                stopSb.Stop();
            });
        }

        /// <summary>
        /// Stops the other story boards.
        /// </summary>
        /// <param name="type">The type.</param>
        private void StopOtherStoryBoards(AnimationType type)
        {
            _storyboard.Keys.Where(key => !Equals(key, type)).ToList().ForEach(key =>
            {
                var stopSb = _storyboard[key];

                if (_stoppedSbs.Contains(stopSb))
                    return;

                _stoppedSbs.Add(stopSb);
                stopSb.Stop();
            });
        }

        /// <summary>
        /// Updates the storyboard.
        /// </summary>
        private void UpdateStoryboard()
        {
            if (_vm == null
                || _vm.IsLocked)
            {
                return;
            }
            // can NEVER happen - because interaction depth must be lower than Settings.Default.DepthThreshold, but this will not trigger this update function
            //if (_vm.TransitionPosition < 0)
            //{
            //    StopAllStoryboards();
            //    return;
            //}

            /// keep pos between 0 and 1
            double pos = _vm.TransitionPosition > 1 ? 1 : _vm.TransitionPosition;
            /// if interaction depth is just above the threshold set the animation position to 0
            pos = pos < (Properties.Settings.Default.DepthThreshold / 2) + 0.01 ? 0 : pos;

            /// freeze animation at last frame if interaction depth is deep enough
            /// if interaction is "pull" the animation lock will reset
            if (_vm.TransitionPosition > 1)
            {
                _vm.IsLocked = true;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                StopOtherStoryBoards(_vm.CurrentType);

                var currentSb = _storyboard[_vm.CurrentType];
                //if (currentSb == null) // WHEN can current Sb be NULL?? -> NEVER (else exeption in line above)
                //    return;

                var ts = TimeSpan.FromMilliseconds(currentSb.Duration.TimeSpan.TotalMilliseconds * pos);

                currentSb.Begin(this, true);
                currentSb.Pause(this);
                currentSb.Seek(this, ts, TimeSeekOrigin.BeginTime);
                currentSb.Pause(this);
            });
        }
    }
}
