using FlexiWallUI.Utilities;
using Prism.Mvvm;
using System;
using FlexiWallUI.Models;
using FlexiWallUI.Properties;

namespace FlexiWallUI.ViewModels
{
    public class BubbleViewModel : BindableBase
    {
        /// <summary>
        /// Gets or sets the type of the current.
        /// </summary>
        /// <value>
        /// The type of the current.
        /// </value>
        public AnimationType CurrentType
        {
            get { return _currentAnimType; }
            set { SetProperty(ref _currentAnimType, value); }
        }

        /// <summary>
        /// Gets or sets the flexi wall application state manager.
        /// </summary>
        /// <value>
        /// The flexi wall application state manager.
        /// </value>
        public FlexiWallAppStateManager FlexiWallAppStateManager { get => _flexiWallAppStateManager; set => _flexiWallAppStateManager = value; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocked
        {
            get { return _isLocked; }
            set { _isLocked = value; }
        }

        public SensorViewModel SensorViewModel
        {
            get { return _sensorVm; }
            set
            {
                _sensorVm = value;
                SensorViewModel.InteractionChanged += OnInteractionChanged;
            }
        }

        /// <summary>
        /// Gets or sets the transition position.
        /// </summary>
        /// <value>
        /// The transition position.
        /// </value>
        public double TransitionPosition
        {
            get { return _transitionPosition; }
            set
            {
                SetProperty(ref _transitionPosition, value);
                //AnimationUpdated?.Invoke(this, new EventArgs());
            }
        }

        /// <summary>
        /// Occurs when [animation updated].
        /// </summary>
        public event EventHandler<EventArgs> AnimationUpdated;

        public void UpdateAnimation()
        {
            AnimationUpdated?.Invoke(this, new EventArgs());
        }

        private AnimationType _currentAnimationType;

        /// <summary>
        /// The current anim type
        /// </summary>
        private AnimationType _currentAnimType;

        /// <summary>
        /// The flexi wall application state manager
        /// </summary>
        private FlexiWallAppStateManager _flexiWallAppStateManager;

        /// <summary>
        /// The is locked
        /// </summary>
        private bool _isLocked = false;

        /// <summary>
        /// The sensor vm
        /// </summary>
        private SensorViewModel _sensorVm;

        /// <summary>
        /// The transition position
        /// </summary>
        private double _transitionPosition = 0.5;

        private void OnInteractionChanged(object sender, FlexiWall.InteractionEventArgs e)
        {
            //throw new NotImplementedException();
            if (e.TypeOfInteraction == FlexiWall.InteractionType.PULLED ||
                e.DisplayCoordinates.Z < Settings.Default.DepthThreshold)
            {
                return;
            }

            if (e.TypeOfInteraction == FlexiWall.InteractionType.PULLED)
            {
                IsLocked = false;
            }

            /// set start animation
            AnimationType associatedtype = AnimationType.Interfaces;
            /// lock bubbleview if cursor is not within "action area"
            //_vm.BubbleVm.IsLocked = true;

            /// determine which "action area" to select
            //if (e.DisplayCoordinates.X > 0.506246875
            //    && e.DisplayCoordinates.X < 0.58241875)
            //{
            //    // rectangle 1
            //    if (e.DisplayCoordinates.Y > 0.448611111
            //        && e.DisplayCoordinates.Y < 0.678472222)
            //    {
            //        associatedtype = AnimationType.Data; // Zuanna
            //        _vm.BubbleVm.IsLocked = false; // unlock bubbleview when cursor is in action area
            //    }
            //}
            //else
            if (e.DisplayCoordinates.X > 0.7003875
                && e.DisplayCoordinates.X < 0.780075)
            {
                // rectangle 2
                if (e.DisplayCoordinates.Y > 0.469444444
                    && e.DisplayCoordinates.Y < 0.678472222)
                {
                    associatedtype = AnimationType.Interfaces; // Antonio
                                                               //_vm.BubbleVm.IsLocked = false; // unlock bubbleview when cursor is in action area
                }
            }

            /// determine animation position based on interaction depth
            double pos = (e.DisplayCoordinates.Z - Settings.Default.DepthThreshold) * ((1 + Settings.Default.DepthThreshold) / Settings.Default.StateManagementPushThreshold) - Settings.Default.DepthThreshold;

            //double pos = ((e.DisplayCoordinates.Z - Settings.Default.DepthThreshold) * 1.0 /
            //           Settings.Default.StateManagementPushThreshold);
            //if (pos > .5)
            //    pos = pos;

            CurrentType = associatedtype;
            TransitionPosition = pos;
        }
    }
}
