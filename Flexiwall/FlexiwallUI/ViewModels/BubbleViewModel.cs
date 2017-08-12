using FlexiWallUI.Utilities;
using Prism.Mvvm;
using System;

namespace FlexiWallUI.ViewModels
{
    public class BubbleViewModel : BindableBase
    {
        public AnimationType CurrentType
        {
            get { return _currentAnimType; }
            set { SetProperty(ref _currentAnimType, value); }
        }

        public bool IsLocked { get => _isLocked; set => _isLocked = value; }

        public double TransitionPosition
        {
            get { return _transitionPosition; }
            set
            {
                SetProperty(ref _transitionPosition, value);
                AnimationUpdated?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler<EventArgs> AnimationUpdated;

        private AnimationType _currentAnimType;
        private bool _isLocked = false;
        private double _transitionPosition = 0.5;
    }
}
