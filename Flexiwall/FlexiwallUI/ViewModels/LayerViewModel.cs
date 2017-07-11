using System.Collections.ObjectModel;
using FlexiWallUI.ViewModels.Interface;
using Prism.Mvvm;

namespace FlexiWallUI.ViewModels
{
    public class LayerViewModel : BindableBase, IActionCollection
    {
        private ActionPropertiesViewModel _selectedAction;


        public ObservableCollection<ActionPropertiesViewModel> Actions { get; private set; }

        public ActionPropertiesViewModel SelectedAction
        {
            get { return _selectedAction; }
            set
            {
                if (_selectedAction == value)
                    return;

                if (_selectedAction != null)
                    _selectedAction.IsSelectionLocked = false;

                _selectedAction = value;

                if (_selectedAction != null)
                {
                    _selectedAction.IsSelectionLocked = true;
                    // _selectedAction.PropertyChanged += sendActionPoint;
                }


                RaisePropertyChanged();
            }
        }
    }
}
