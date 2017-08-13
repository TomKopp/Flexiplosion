using FlexiWallUI.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace FlexiWallUI.Views
{
    /// <summary>
    /// Interaction logic for MenuView.xaml
    /// </summary>
    /// <seealso cref="System.Windows.Controls.UserControl" />
    /// <seealso cref="System.Windows.Markup.IComponentConnector" />
    public partial class MenuView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MenuView"/> class.
        /// </summary>
        public MenuView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The Storyboard
        /// </summary>
        private Storyboard _sb;

        /// <summary>
        /// The MenuViewModel
        /// </summary>
        private MenuViewModel _vm;

        /// <summary>
        /// Called when [data context changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _vm = DataContext as MenuViewModel;
            if (_vm == null)
            {
                return;
            }

            _sb = Resources["MenuAnimation"] as Storyboard;

            UpdateStoryboard();

            _vm.TransitionUpdated += delegate { UpdateStoryboard(); };
        }

        /// <summary>
        /// Updates the storyboard.
        /// </summary>
        private void UpdateStoryboard()
        {
            /// keep pos between 0 and 1
            double pos = _vm.TransitionPosition > 1 ? 1 : _vm.TransitionPosition;
            /// if interaction depth is just above the threshold set the animation position to 0
            pos = pos < (Properties.Settings.Default.DepthThreshold / 2) + 0.01 ? 0 : pos;

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var ts = TimeSpan.FromMilliseconds(_sb.Duration.TimeSpan.TotalMilliseconds * pos);

                _sb.Begin(this, true);
                _sb.Pause(this);
                _sb.Seek(this, ts, TimeSeekOrigin.BeginTime);
                _sb.Pause(this);
            }));
        }
    }
}
