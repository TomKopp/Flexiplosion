using System.Windows;
using System.Windows.Input;
using CommonClassesLib.Model;
using FlexiWallUI.ViewModels;

namespace FlexiWallUI.Views
{
    /// <summary>
    /// Interaction logic for LogView.xaml
    /// </summary>
    public partial class LogView : Window
    {
        public LogView()
        {
            InitializeComponent();
            DataContext = new LogViewModel(this);
            WindowState = WindowState.Minimized;
            Log.LogMessage("LogWindow created.");
        }

        private void MoveWindow(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
