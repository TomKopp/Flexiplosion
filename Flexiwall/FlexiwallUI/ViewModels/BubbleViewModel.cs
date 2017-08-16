using FlexiWallUI.Utilities;
using Prism.Mvvm;
using System;
using FlexiWallUI.Models;

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
        /// Gets or sets a value indicating whether this instance is locked.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is locked; otherwise, <c>false</c>.
        /// </value>
        public bool IsLocked {
            get { return _isLocked; }
            set { _isLocked = value; }
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
                AnimationUpdated?.Invoke(this, new EventArgs());
            }
        }

        private SensorViewModel _sensorVm;

        public SensorViewModel SensorVm
        {
            get { return _sensorVm; }
            set
            {
                _sensorVm = value;
                SensorVm.InteractionChanged += OnInteractionChanged;
            }
        }

        private void OnInteractionChanged(object sender, FlexiWall.InteractionEventArgs e)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Occurs when [animation updated].
        /// </summary>
        public event EventHandler<EventArgs> AnimationUpdated;

        /// <summary>
        /// The current anim type
        /// </summary>
        private AnimationType _currentAnimType;

        /// <summary>
        /// The is locked
        /// </summary>
        private bool _isLocked = false;

        /// <summary>
        /// The transition position
        /// </summary>
        private double _transitionPosition = 0.5;
    }
}
