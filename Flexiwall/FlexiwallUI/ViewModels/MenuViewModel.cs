using System;
using Prism.Mvvm;

namespace FlexiWallUI.ViewModels
{
    public class MenuViewModel : BindableBase
    {
        private double _transPos = 0.5;
        private bool _isActive = false;

        public double TransitionPosition
        {
            get { return _transPos; }
            set
            {
                SetProperty(ref _transPos, value);
                TransitionUpdated?.Invoke(this, new EventArgs());

            }
        }

        public bool IsActive
        {
            get { return _isActive;}
            set { SetProperty(ref _isActive, value); }
        }

        public event EventHandler TransitionUpdated;
    }
}
