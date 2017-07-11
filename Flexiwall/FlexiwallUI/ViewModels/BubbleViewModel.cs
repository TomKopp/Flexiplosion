using System;
using FlexiWallUI.Utilities;
using Prism.Mvvm;

namespace FlexiWallUI.ViewModels
{
    public class BubbleViewModel : BindableBase
    {
        private double _transPos = 0.5;
        private bool _isActive = false;
        private AnimationType _currentAnimType;


        public double TransitionPosition
        {
            get { return _transPos; }
            set
            {
                SetProperty(ref _transPos, value);
                AnimationUpdated?.Invoke(this, new EventArgs());

            }
        }

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

        public event EventHandler<EventArgs> AnimationUpdated;

    }
}
