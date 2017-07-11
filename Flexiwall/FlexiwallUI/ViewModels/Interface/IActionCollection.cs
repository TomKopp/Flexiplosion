using System.Collections.ObjectModel;

namespace FlexiWallUI.ViewModels.Interface
{
    public interface IActionCollection
    {
        ObservableCollection<ActionPropertiesViewModel> Actions { get; }

        ActionPropertiesViewModel SelectedAction { get; set; }
    }
}