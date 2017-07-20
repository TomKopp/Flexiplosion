using System;
using System.Drawing;
using System.Windows.Media;
using CommonClassesLib;
using FlexiWallUI.Models;
using FlexiWallUI.Utilities;
using FlexiWallUtilities.Core;
using FlexiWallUtilities.Filter.PostProcessing;
using FlexiWallUtilities.Filter.PreProcessing;
using Prism.Commands;
using Prism.Mvvm;
using static FlexiWallUI.Models.FlexiWall;
using FlexiWallUI.Properties;

namespace FlexiWallUI.ViewModels
{
    public class SensorViewModel : BindableBase
    {
        #region Fields

        private FPSCounter _fps;
        private RSCamera _rsCamera;
        private FlexiWall _flexiwall;
        private ImageSource _depthImage;
        private Point3 _interaction;
        private Size _size;
        private bool _useEmulator;

        private MovingOutliersFilter<Point3> _movingOutliersFilter;
        private LowPassBlur<Point3> _lowPassBlur;

        // Alternativen
        // private MovingAverageFilter<Point3> _movingAverageFilter;
        // private CoordinatesFilter<Point3> _cameraPositionFilter;
        // private BoxBlur<Point3> _boxBlur;
        // private BetterBoxBlur<Point3> _betterBoxBlur;

        #endregion

        #region Properties

        public FPSCounter FPS
        {
            get { return _fps; }
            set { SetProperty(ref _fps, value); }
        }

        public bool SensorConnected
        {
            get { return _rsCamera != null && _rsCamera.State != SensorState.NotFound; }
        }

        public ImageSource DepthImage
        {
            get { return _depthImage; }
            set { SetProperty(ref _depthImage, value); }
        }

        public FlexiWall FlexiWall
        {
            get { return _flexiwall; }
            set { SetProperty(ref _flexiwall, value); }
        }

        public Point3 Interaction
        {
            get { return _interaction; }
            set { SetProperty(ref _interaction, value); }
        }

        public bool UseEmulator
        {
            get { return _useEmulator; }
            set
            {
                SetProperty(ref _useEmulator, value);
                FlexiWall.IsEmulated = value;
            }
        }

        public DelegateCommand LoadCalibrationCommand { get; internal set; }

        #endregion

        #region events

        public EventHandler<InteractionEventArgs> InteractionChanged;

        #endregion

        #region Constructor

        public SensorViewModel()
        {
            // Calibrierung testen
            LoadCalibrationCommand = new DelegateCommand(LoadCalibrationResult);

            // Punktwolke
            _size = new Size(320, 240);
            _flexiwall = new FlexiWall(_size, 1, 600); // 0.6m travel distance front backs
            _flexiwall.EmulatorResolution = new Point(1920, 1080);
            _interaction = new Point3();
            _fps = new FPSCounter();

            // Filter            
            _movingOutliersFilter = new MovingOutliersFilter<Point3>(350); // entspricht 0,4m/frame
            _lowPassBlur = new LowPassBlur<Point3>(0.5f);

            // Alternativen
            // _cameraPositionFilter = new CoordinatesFilter<Point3>(0, 0, 0);
            // _boxBlur = new BoxBlur<Point3>(1);
            // _betterBoxBlur = new BetterBoxBlur<Point3>(2);

            // attach Filter to Flexiwall
            _flexiwall.AttachFilter(_movingOutliersFilter);
            _flexiwall.AttachFilter(_lowPassBlur);
            _flexiwall.NewInteraction += NewInteraction;

            // Sensor + Stream
            _rsCamera = new RSCamera();
            _rsCamera.DepthFrameReady += OnCameraDepthFrameArrived;
            var depthStreamDesc = new RSStreamProfile(new PXCMSizeI32(320, 240), 60,
                PXCMCapture.StreamType.STREAM_TYPE_DEPTH);

            try
            {
                _rsCamera.Initialize(depthStreamDesc);
            }
            catch (Exception exc)
            {
                // @todo
            }

            if (_rsCamera.State == SensorState.Initialized)
                _rsCamera.Start();
        }

        #endregion

        #region Methods

        public void RegisterSensor(RSCamera camera)
        {
            _rsCamera = camera;
            RaisePropertyChanged(nameof(SensorConnected));
            _rsCamera.DepthFrameReady += OnCameraDepthFrameArrived;
        }

        private void OnCameraDepthFrameArrived(object sender, RSCamera.DepthFrameEventArgs args)
        {
            if (UseEmulator) return;

            // starte Timer für FPS-Zähler
            _fps.SetFrameStart();

            if (_flexiwall.IsInitialized()) // => Main-Loop
            {
                lock (_flexiwall)
                {
                    for (int i = 0; i < _flexiwall.Size; i++)
                    {
                        // Neue Werte übergeben
                        _flexiwall.Update(args.DepthData[i], i);
                    }
                }

                // Eingabepunkte ermitteln
                Interaction.Set(_flexiwall.FindPointOfInteraction());

                // Beispiel für Tiefenbild als BitmapImage
                DepthImage = BitmapUtil.ConvertBitmap(args.DepthImage);
            }

            // Zeit für FPS-Zähler stoppen
            _fps.SetFrameEnd();
            RaisePropertyChanged(nameof(FPS));
        }

        private void LoadCalibrationResult()
        {
            try
            {
                _flexiwall.Calibration = FlexiWallCalibration.Utilities.SerialUtil.DeSerializeFile<FlexiWallCalibration.Models.Rectangle3>();
                var fn = FlexiWallCalibration.Utilities.SerialUtil.FileName;
                if (!string.IsNullOrWhiteSpace(fn))
                {
                    Settings.Default.ConfigFile = fn;
                    Settings.Default.Save();
                }
            }
            catch (Exception exc)
            {
                // @todo logging
            }
        }

        private void NewInteraction(object sender, InteractionEventArgs args)
        {
            if (UseEmulator == false && _flexiwall.Calibration == null)
                return;

            Interaction.Set(args.DisplayCoordinates);
            InteractionChanged?.Invoke(this, args);
        }

        #endregion

    }
}
