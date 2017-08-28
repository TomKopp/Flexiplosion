using FlexiWallUI.Models;
using FlexiWallUI.Properties;
using System;
using System.Windows.Media.Media3D;

namespace FlexiWallUI.ViewModels
{
    public class UpdateAnimationEventArgs : EventArgs
    {
        public Point3D Point3D { get; set; }
    }

    public class VeroneseViewModel
    {
        /// <summary>
        /// Gets or sets the FlexiWall application state manager.
        /// </summary>
        /// <value>
        /// The FlexiWall application state manager.
        /// </value>
        public FlexiWallAppStateManager FlexiWallAppStateManager { get; set; }

        /// <summary>
        /// Gets or sets the sensor view model.
        /// </summary>
        /// <value>
        /// The sensor view model.
        /// </value>
        public SensorViewModel SensorViewModel
        {
            get { return _sensorViewModel; }
            set
            {
                _sensorViewModel = value;
                SensorViewModel.InteractionChanged += OnInteractionChanged;
            }
        }

        /// <summary>
        /// Occurs when [animation update].
        /// </summary>
        public static event EventHandler<UpdateAnimationEventArgs> AnimationUpdate;

        /// <summary>
        /// Updates the animation.
        /// </summary>
        public void UpdateAnimation()
        {
            if (!_isAnimationLocked)
            {
                AnimationUpdate?.Invoke(this, new UpdateAnimationEventArgs() { Point3D = AnimationArgs });
            }
        }

        /// <summary>
        /// The is locked
        /// </summary>
        private bool _isAnimationLocked = false;

        /// <summary>
        /// The sensor view model
        /// </summary>
        private SensorViewModel _sensorViewModel;

        /// <summary>
        /// The animation arguments
        /// </summary>
        private Point3D AnimationArgs = new Point3D(0.0, 0.0, 0.0);

        /// <summary>
        /// Calculates the animation percent.
        /// </summary>
        /// <param name="e">The <see cref="FlexiWall.InteractionEventArgs"/> instance containing the event data.</param>
        /// <returns></returns>
        private double CalculateAnimationPercent(FlexiWall.InteractionEventArgs e)
        {
            var x = (e.DisplayCoordinates.Z - Settings.Default.DepthThreshold) * ((1 + Settings.Default.DepthThreshold) / Settings.Default.StateManagementPushThreshold) - Settings.Default.DepthThreshold;
            if (x >= 1)
                x = 1;
            if (x <= 0)
                x = 0;

            return x;
        }

        /// <summary>
        /// Called when [interaction changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="FlexiWall.InteractionEventArgs"/> instance containing the event data.</param>
        private void OnInteractionChanged(object sender, FlexiWall.InteractionEventArgs e)
        {
            if (FlexiWallAppStateManager.AppState != FlexiWallAppState.ExploreCompany)
            {
                return;
            }

            if (e.TypeOfInteraction == FlexiWall.InteractionType.PULLED)
            {
                _isAnimationLocked = false;
                return;
            }

            AnimationArgs.X = e.DisplayCoordinates.X;
            AnimationArgs.Y = e.DisplayCoordinates.Y;
            AnimationArgs.Z = CalculateAnimationPercent(e);

            if (AnimationArgs.Z <= 0)
            {
                _isAnimationLocked = false;
            }

            UpdateAnimation();

            if (AnimationArgs.Z >= 1)
            {
                _isAnimationLocked = true;
            }
        }
    }
}
