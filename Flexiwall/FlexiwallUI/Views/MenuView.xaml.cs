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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FlexiWallUI.ViewModels;

namespace FlexiWallUI.Views
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    public partial class MenuView : UserControl
    {
        private Storyboard _sb;
        private Storyboard _sb_Child;
        private MenuViewModel _vm;

        private const int FramesToSkip = 1;
        private int _skipped = 0;

        public MenuView()
        {
            InitializeComponent();
        }

        private void UpdateStoryboard()
        {
            if (_vm.TransitionPosition < 0 || _vm.TransitionPosition > 1)
                return;

            _skipped++;
            _skipped = _skipped%FramesToSkip;
            if (_skipped != 0)
                return;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {

                var ts = TimeSpan.FromMilliseconds(_sb.Duration.TimeSpan.TotalMilliseconds * _vm.TransitionPosition);
                var tsChild = TimeSpan.FromMilliseconds(_sb_Child.Duration.TimeSpan.TotalMilliseconds * _vm.TransitionPosition);

                _sb.Resume();
                _sb_Child.Resume();

                _sb.Seek(ts, TimeSeekOrigin.BeginTime);
                _sb_Child.Seek(tsChild, TimeSeekOrigin.BeginTime);
                _sb.Pause();
                _sb_Child.Pause();
            }));
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = DataContext as MenuViewModel;
            if (_vm == null)
                return;
            _sb = Resources["MenuAnimation"] as Storyboard;
            _sb_Child = GTVView.Resources["MenuAnimation"] as Storyboard;

            _sb.Begin();
            _sb_Child.Begin();
            _sb.Pause();
            _sb_Child.Pause();
            UpdateStoryboard();

            _vm.TransitionUpdated += delegate { UpdateStoryboard(); };
        }
    }
}
