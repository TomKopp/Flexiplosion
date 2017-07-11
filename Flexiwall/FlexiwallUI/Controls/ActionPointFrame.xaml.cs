using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlexiWallUI.ViewModels.Interface;

namespace FlexiWallUI.Controls
{
    /// <summary>
    /// Interaction logic for ActionPointFrame.xaml
    /// </summary>
    public partial class ActionPointFrame : UserControl
    {
        private Point _dragStart = new Point(-1, -1);
        private bool _isDragging;

        public ActionPointFrame()
        {
            InitializeComponent();
        }

        private void Drag(object sender, MouseEventArgs e)
        {
            if (!(e.MiddleButton == MouseButtonState.Pressed && _isDragging))
                return;
            var vm = DataContext as IFlexiWallAction;
            // TODO: quite dirty --> clean solution ? (remark: sender is not possible as position updates of element interfere with MouseEvent-Position)
            Point newPos = e.GetPosition(Application.Current.MainWindow);
            if (vm != null)
                vm.Move(newPos - _dragStart);
            _dragStart = newPos;
        }

        private void StartDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle && e.ButtonState == MouseButtonState.Pressed)
            {
                // TODO: quite dirty --> clean solution ? (remark: sender is not possible as position updates of element interfere with MouseEvent-Position)
                _dragStart = e.GetPosition(Application.Current.MainWindow);
                _isDragging = true;
            }
        }

        private void StopDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                _isDragging = false;
        }

        private void EnterDrag(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
                _isDragging = false;
        }

        private void LeaveDrag(object sender, MouseEventArgs e)
        {
            _isDragging = false;
        }

        private void Scale(object sender, MouseWheelEventArgs e)
        {
            var action = DataContext as IFlexiWallAction;
            if (action != null)
                action.Scale(e.Delta);
        }
    }
}