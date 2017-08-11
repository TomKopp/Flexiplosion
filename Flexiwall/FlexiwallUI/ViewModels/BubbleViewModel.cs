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

        public bool IsActive
        {
            get { return _isActive; }
            set { SetProperty(ref _isActive, value); }
        }

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
        private bool _isActive = false;
        private double _transitionPosition = 0.5;
    }
}
