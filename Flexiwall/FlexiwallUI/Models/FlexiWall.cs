using FlexiWallUtilities.Core;
using System.Drawing;
using System;
using System.Collections.Generic;
using FlexiWallUtilities.Filter.PreProcessing;
using FlexiWallUtilities.Filter.PostProcessing;
using FlexiWallCalibration.Models;
using FlexiWallUtilities.Filter.InputProcessing;

namespace FlexiWallUI.Models
{
    /// <summary>
    /// a structured pointcloud
    /// </summary>
    public class FlexiWall : IPointGrid<Point3>
    {

        #region fields

        private Size _size;
        private int _capacity;

        // Filter
        private List<IPreProcessor<Point3>> _filtering;
        private List<IPostProcessor<Point3>> _smoothing;

        // Werte verwenden Kalibrierung
        private float _distanceToCamera;
        private Rectangle3 _calibrationResult;

        // InputLayer
        private float _interactionDistance;
        private InteractionEventArgs _interactionEventArgs;

        // InputSmoothing
        private EinEuroFilter _xFilter, _yFilter, _zFilter;

        #endregion

        #region properties

        // single dimensional array performs better
        public Point3[] Values;
        public Point3 this[int index]
        {
            get { return Values[index]; }
            set { Values[index].Set(value); }
        }

        /// <summary>
        /// number of columns
        /// </summary>
        public int Width
        {
            get { return _size.Width; }
        }

        /// <summary>
        /// number of rows
        /// </summary>
        public int Height
        {
            get { return _size.Height; }
        }

        /// <summary>
        /// number of total points
        /// </summary>
        public int Size
        {
            get { return _capacity; }
        }

        /// <summary>
        /// maximum distance of interaction
        /// </summary>
        public float InteractionDistance
        {
            get { return _interactionDistance; }
            set { _interactionDistance = value; }
        }

        /// <summary>
        /// distance from camera to flexiwall
        /// </summary>
        public float DistanceToCamera
        {
            get { return _distanceToCamera; }
            set { _distanceToCamera = value; }
        }

        /// <summary>
        /// calibration to calculate interaction-offset
        /// </summary>
        public Rectangle3 Calibration
        {
            get { return _calibrationResult; }
            set
            {
                _calibrationResult = value;
                _distanceToCamera = Calibration.Translate.Z;
            }
        }

        #endregion

        #region events

        public delegate void InteractionEventHandler(object sender, InteractionEventArgs args);
        public event InteractionEventHandler NewInteraction;

        #endregion

        #region constructor

        /// <summary>
        /// structured pointcloud like a grid
        /// </summary>
        /// <param name="size"> number of columns and rows </param>
        /// <param name="framesToRound"> how much last frames to round </param>
        /// <param name="distanceToCamera"> distance from flexiwall to camera or vise versa </param>
        /// <param name="distanceOfInteraction"> maximum distance of interaction </param>
        public FlexiWall(Size size, int framesToRound, float distanceOfInteraction)
        {
            _size = size;
            _capacity = size.Width * size.Height;
            _distanceToCamera = 1422;

            // falls keine zeitliche Rundung nötig werden normale Punkte verwendet
            if (framesToRound <= 1)
            {
                Values = new Point3[size.Width * size.Height];
                for (int i = 0; i < Values.Length; i++)
                    Values[i] = new Point3(0, 0, _distanceToCamera);
            }
            // falls zeitliche Rundung nötig werden Punkte mit Verlauf verwendet
            else
            {
                Values = new Point3History[size.Width * size.Height];
                for (int i = 0; i < Values.Length; i++)
                {
                    var value = new Point3History(framesToRound, 0, 0, _distanceToCamera);
                    if (i > 0) value.Reset = Values[i - 1];
                    Values[i] = value;
                }
            }

            // Filter
            _filtering = new List<IPreProcessor<Point3>>();
            _smoothing = new List<IPostProcessor<Point3>>();

            // Interaktion
            _interactionDistance = distanceOfInteraction;
            _interactionEventArgs = new InteractionEventArgs();
            // Interaktion durch Ein-Euro Filter glätten
            _xFilter = new EinEuroFilter(1, 0.001f);
            _yFilter = new EinEuroFilter(1, 0.001f);
            _zFilter = new EinEuroFilter(1, 0.01f);
        }

        #endregion

        #region member

        /// <summary>
        /// returns a specific point
        /// </summary>
        /// <param name="index"> index of point </param>
        /// <returns></returns>
        public Point3 GetValue(int index)
        {
            return Values[index];
        }

        [System.Obsolete("Use Indexer or Values instead!")]
        public Point3 GetValue(int x, int y)
        {
            return Values[y * _size.Width * x];
        }

        [System.Obsolete("too expensive to use")]
        public void Update(Point3 source, int index)
        {
            Values[index].Set(source);
        }

        [System.Obsolete("too expensive to use")]
        public void Update(Point3 source, int x, int y)
        {
            GetValue(x, y).Set(source);
        }

        /// <summary>
        /// updates a point with new values
        /// </summary>
        /// <param name="source"> source point with new values </param>
        /// <param name="index"> index of the target point </param>
        public void Update(PXCMPoint3DF32 source, int index)
        {
            foreach (var fl in _filtering)
                if (!fl.Process(source.x, source.y, source.z, index))
                    return;

            Values[index].Set(source.x, source.y, source.z);

            foreach (var sm in _smoothing)
                sm.Process(index);
        }

        /// <summary>
        /// calculates the spot which seems to be pulled or pushed
        /// </summary>
        /// <returns></returns>
        public Point3 FindPointOfInteraction()
        {
            Point3 interaction = new Point3(0, 0, _distanceToCamera);
            Point3 candidate = new Point3();

            var minZ = _distanceToCamera - _interactionDistance;
            var maxZ = _distanceToCamera + _interactionDistance;

            lock (Values)
                for (int i = Width; i < _capacity - Width; i++)
                {
                    candidate.Set(Values[i]);

                    // prüfen ob Messpunkte innerhalb der FlexiWall liegen
                    if (candidate.Z > minZ && candidate.Z < maxZ)
                    {
                        // Confidence testen @todo reichen Grenzen?
                        var zAverage = (Values[i - 1].Z + Values[i + 1].Z + Values[i + Width].Z + Values[i - Width].Z) / 4;
                        var averageDistance = Math.Abs(zAverage - candidate.Z);
                        if (averageDistance >= 10)
                        {
                            if (averageDistance > 100) Values[i].Z = _distanceToCamera;
                            continue;
                        }

                        // Punkt finden der am weitesten von Flexiwall entfernt
                        if (Math.Abs(candidate.Z - _distanceToCamera) > Math.Abs(interaction.Z - _distanceToCamera))
                        {
                            interaction.Set(candidate);
                        }
                    }
                }

            // Ein-Euro Filter anwenden
            interaction.X = _xFilter.Process(interaction.X, 500);
            interaction.Y = _yFilter.Process(interaction.Y, 500);
            interaction.Z = _zFilter.Process(interaction.Z, 500);

            // Event mit Informationen über Interaktion feuern
            RaiseInteractionEvent(interaction);

            return interaction;
        }

        /// <summary>
        /// apply calibration on point
        /// </summary>
        /// <param name="target"> target of calibration </param>
        /// <returns> calibrated point </returns>
        public Point3 ApplyCalibration(Point3 target)
        {
            var calibratedPoint = new Point3();
            calibratedPoint.Set(target);

            // Offset einrechnen falls Flexiwall kalibriert wurde
            if (Calibration != null)
            {
                // oberer und linken Rand des projizierten Bildes
                var left = Calibration.Translate.X + (Calibration.Width / 2);
                var top = Calibration.Translate.Y + (Calibration.Height / 2);

                // Abstand zwischen Interaktion und oberen bzw linken Rand finden
                var targetX = Math.Abs(target.X - left);
                var targetY = Math.Abs(target.Y - top);

                // prozentualer offset
                var xOffset = (targetX / Calibration.Width);
                var yOffset = (targetY / Calibration.Height);

                calibratedPoint.Set((float)xOffset, (float)yOffset, target.Z);
            }

            return calibratedPoint;
        }

        /// <summary>
        /// Fire InterctionEvent with new args
        /// </summary>
        /// <param name="location"> measured location of Interaction </param>
        private void RaiseInteractionEvent(Point3 location)
        {
            _interactionEventArgs.ID++;
            _interactionEventArgs.WorldCoordinates.Set(location);

            var depth = location.Z - _distanceToCamera;
            var depthPercentage = Math.Abs(depth) / _interactionDistance;
            var calibratedLocation = ApplyCalibration(location);

            _interactionEventArgs.DisplayCoordinates.Set(calibratedLocation.X, calibratedLocation.Y, depthPercentage);

            _interactionEventArgs.TypeOfInteraction = InteractionType.NONE;

            if (depth < 0)
            {
                _interactionEventArgs.TypeOfInteraction = InteractionType.PUSHED;
            }
            else if (depth > 0)
            {
                _interactionEventArgs.TypeOfInteraction = InteractionType.PULLED;
            }

            OnNewInteraction(this, _interactionEventArgs);
        }

        /// <summary>
        /// returns true if every value is initialized
        /// </summary>
        /// <returns> true if every value is initialized and vise versa </returns>
        public bool IsInitialized() => this[_capacity - 1] != null;

        /// <summary>
        /// register a Filter to the update-process
        /// </summary>
        /// <param name="filter"></param>
        public void AttachFilter(IPreProcessor<Point3> filter)
        {
            _filtering.Add(filter);
            filter.Target = this;
        }

        /// <summary>
        /// register a Filter to the update-process
        /// </summary>
        /// <param name="filter"></param>
        public void AttachFilter(IPostProcessor<Point3> filter)
        {
            _smoothing.Add(filter);
            filter.Target = this;
        }

        /// <summary>
        /// unregister a Filter to the update-process
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool DetachFilter(IPreProcessor<Point3> filter) => _filtering.Remove(filter);

        /// <summary>
        /// unregister a Filter to the update-process
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public bool DetachFilter(IPostProcessor<Point3> filter) => _smoothing.Remove(filter);

        #endregion

        #region eventhandling

        /// <summary>
        /// to attach InteractionEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnNewInteraction(object sender, InteractionEventArgs args)
        {
            NewInteraction?.Invoke(sender, args);
        }

        /// <summary>
        /// Event-Arguments that describes an Interaction
        /// </summary>
        public class InteractionEventArgs : EventArgs
        {
            public int ID;
            public Point3History WorldCoordinates;
            public Point3 DisplayCoordinates;
            public InteractionType TypeOfInteraction;

            public InteractionEventArgs()
            {
                ID = 0;
                WorldCoordinates = new Point3History(5);
                DisplayCoordinates = new Point3();
                TypeOfInteraction = InteractionType.NONE;
            }
        }

        /// <summary>
        /// An type of Interaction on flexible display
        /// </summary>
        public enum InteractionType
        {
            PULLED, PUSHED, NONE
        }

        #endregion

    }
}
