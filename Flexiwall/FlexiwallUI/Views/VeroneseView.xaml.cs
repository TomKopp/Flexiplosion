using FlexiWallUI.ViewModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace FlexiWallUI.Views
{
    /// <summary>
    /// Interaction logic for Veronese.xaml
    /// </summary>
    public partial class Veronese : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Veronese"/> class.
        /// </summary>
        public Veronese()
        {
            InitializeComponent();
            // You cannot define your data here..
            // You have to do it in xaml DataContext - because of Prism i guess
            //ViewModel = new VeroneseViewModel();
            //ViewModel.AnimationUpdate += OnAnimationUpdate;

            VeroneseViewModel.AnimationUpdate += OnAnimationUpdate;

            Storyboards.Add("Storyboard_Antonio", FindResource("Storyboard1") as Storyboard);
            Storyboards.Add("Storyboard_Zuanna", FindResource("Storyboard2") as Storyboard);

            // Start and then stop all storyboards to stop the anoying warning, that stop is called on a storyboad before it started
            StartAllStoryboards();
            StopAllStoryboards();
        }

        /// <summary>
        /// The storyboards
        /// </summary>
        internal Dictionary<string, Storyboard> Storyboards = new Dictionary<string, Storyboard>();

        /// <summary>
        /// Gets the view model.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        internal VeroneseViewModel ViewModel { get; private set; }

        /// <summary>
        /// Determines the storyboard.
        /// </summary>
        /// <returns>Storyboard <see cref="Storyboard"/></returns>
        internal Storyboard DetermineStoryboard(Point3D point3D)
        {
            if (ShapeContainesPoint(Hand_Zuanna, point3D))
            {
                return Storyboards["Storyboard_Zuanna"];
            }

            return Storyboards["Storyboard_Antonio"];
        }

        /// <summary>
        /// Called when [animation update].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="UpdateAnimationEventArgs" /> instance containing the event data.</param>
        internal void OnAnimationUpdate(object sender, UpdateAnimationEventArgs e)
        {
            StopAllStoryboards();
            var sb = DetermineStoryboard(e.Point3D);
            var ts = TimeSpan.FromMilliseconds(sb.Duration.TimeSpan.TotalMilliseconds * e.Point3D.Z);

            sb.Begin();
            sb.Seek(ts);
            sb.Pause();
        }

        /// <summary>
        /// Called when [data context changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            // empty
        }

        /// <summary>
        /// Shapes the containes point.
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <param name="point3D">The point3 d.</param>
        /// <returns></returns>
        private bool ShapeContainesPoint(Shape shape, Point3D point3D)
        {
            var position2D = new Point(
                point3D.X * ActualWidth - (ActualWidth - grid.ActualWidth) / 2 - shape.Margin.Left
                , point3D.Y * ActualHeight - (ActualHeight - grid.ActualHeight) / 2 - shape.Margin.Top
                );
            return shape.RenderedGeometry.Bounds.Contains(position2D);
        }

        /// <summary>
        /// Starts all storyboards.
        /// </summary>
        private void StartAllStoryboards()
        {
            foreach (Storyboard sb in Storyboards.Values)
            {
                sb.Begin();
            }
        }

        /// <summary>
        /// Stops all storyboards.
        /// </summary>
        private void StopAllStoryboards()
        {
            foreach (Storyboard sb in Storyboards.Values)
            {
                sb.Seek(TimeSpan.Zero);
                sb.Stop();
            }
        }
    }
}
