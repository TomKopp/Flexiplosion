using Prism.Commands;
using Prism.Mvvm;
using System.Windows;
using System;

namespace FlexiWallCalibration.ViewModels
{
    public class MarkerViewModel : BindableBase
    {

        private int _mirror;
        private WindowState _currentWindowState;

        public int Mirror
        {
            get { return _mirror; }
            set { SetProperty(ref _mirror, value); }
        }

        public WindowState CurrentWindowState
        {
            get { return _currentWindowState; }
            set { SetProperty(ref _currentWindowState, value); }
        }

        public DelegateCommand SwitchWindowStateCommand { get; internal set; }
        public DelegateCommand MirrorMarkerCommand { get; internal set; }
        public Action CloseAction { get; set; }

        public MarkerViewModel()
        {
            CurrentWindowState = WindowState.Maximized;
            SwitchWindowStateCommand = new DelegateCommand(switchWindowState);
            MirrorMarkerCommand = new DelegateCommand(mirrorMarker);          
            _mirror = 1;
        }

        private void mirrorMarker()
        {
            Mirror = _mirror == 1 ? -1 : 1;
        }

        private void switchWindowState()
        {
            if (CurrentWindowState == WindowState.Maximized)
                CurrentWindowState = WindowState.Normal;
            else if (CurrentWindowState == WindowState.Normal)
                CurrentWindowState = WindowState.Maximized;
            else
                CurrentWindowState = WindowState.Normal;
        }

    }
}
