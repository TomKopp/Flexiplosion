using System.Windows;

namespace FlexiWallCalibration.Views
{
    /// <summary>
    /// Interaktionslogik für MarkerView.xaml
    /// </summary>
    public partial class MarkerView : Window
    {
        public MarkerView()
        {
            InitializeComponent();
        }

        private void CommandBinding_Executed(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Close();
        }
    }
}
