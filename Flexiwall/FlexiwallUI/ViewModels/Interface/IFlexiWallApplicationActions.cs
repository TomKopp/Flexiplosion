using FlexiWallUI.Models;

namespace FlexiWallUI.ViewModels.Interface
{
    public interface IFlexiWallApplicationActions
    {
        void TogglePropertyPanelVisibility();
        void ToggleFullScreen();
        void ToggleAppMinimized();
        void ToggleHelp();
        void ToggleLogVisibility();
        void Exit();
        void Play(double offset);
        void SwitchAppState(FlexiWallAppState targetState);
    }
}