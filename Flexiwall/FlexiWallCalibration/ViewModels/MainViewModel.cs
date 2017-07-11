using Emgu.CV;
using FlexiWallCalibration.Models;
using FlexiWallCalibration.Views;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Media.Imaging;
using FlexiWallCalibration.Utilities;
using Prism.Commands;
using FlexiWallUtilities.Core;
using System.Drawing;
using System;
using System.Windows.Input;
using CommonClassesLib;

namespace FlexiWallCalibration.ViewModels
{
    public class MainViewModel : BindableBase
    {

        #region fields

        private RSCamera _camera;
        private FeatureDetector _detector;
        private MarkerView _markerView;
        private Mat _modelImage;
        private int _tries;
        private WindowState _currentWindowState;
        private Rectangle3 _result;
        private BitmapImage _colorStream;

        #endregion

        #region properties

        // aktueller Fensterstatus
        public WindowState CurrentWindowState
        {
            get { return _currentWindowState; }
            set { SetProperty(ref _currentWindowState, value); }
        }

        // Gibt an ob das Programm gerade die Fläche berrechnet
        public bool IsMeasuring
        {
            get { return StateManager.CurrentState == CalibrationState.MEASURE ? true : false; }
        }

        // Gibt an ob Messung verfügbar ist
        public bool IsResult
        {
            get { return !IsMeasuring && Result != null; }
        }

        public bool IsError
        {
            get { return StateManager.CurrentState == CalibrationState.ERROR ? true : false; }
        }

        // Ergebnis für die letzte Messung
        public Rectangle3 Result
        {
            get { return _result; }
            set { SetProperty(ref _result, value); }
        }

        // Farbstream der Kamera
        public BitmapImage ColorStream
        {
            get { return _colorStream; }
            set { SetProperty(ref _colorStream, value); }
        }

        #endregion

        #region commands

        public DelegateCommand SwitchWindowStateCommand { get; internal set; }
        public DelegateCommand OpenMarkerWindowCommand { get; internal set; }
        public DelegateCommand StartToMeasureCommand { get; internal set; }
        public DelegateCommand SaveResultCommand { get; internal set; }
        public DelegateCommand CancelMeasuringCommand { get; internal set; }
        public DelegateCommand CloseAppCommand { get; internal set; }

        #endregion

        #region constructor

        public MainViewModel()
        {
            // StateManager wirft Event wenn neuer Status gesetzt wird
            StateManager.StateChanged += stateChanged;

            // Fensterstatus wird als normal initialisiert
            CurrentWindowState = WindowState.Normal;
            _tries = 0;

            // Commands zur Interaktion mit der view
            SwitchWindowStateCommand = new DelegateCommand(switchWindowState);
            OpenMarkerWindowCommand = new DelegateCommand(OpenMarkerWindow);
            StartToMeasureCommand = new DelegateCommand(StartToMeasure);
            SaveResultCommand = new DelegateCommand(SaveResult);
            CancelMeasuringCommand = new DelegateCommand(CancelMeasuring);
            CloseAppCommand = new DelegateCommand(CloseApp);

            // Kamera wird initialisiert und Eventlistener angehangen
            _camera = new RSCamera();
            _camera.ColorFrameReady += ColorFrameReady;
            var colorStreamDesc = new RSStreamProfile(new PXCMSizeI32(640, 480), 60, PXCMCapture.StreamType.STREAM_TYPE_COLOR);
            var depthStreamDesc = new RSStreamProfile(new PXCMSizeI32(320, 240), 60, PXCMCapture.StreamType.STREAM_TYPE_DEPTH);
            var streamingParameter = new RSStreamProfileSet(colorStreamDesc, depthStreamDesc);
            _camera.Initialize(streamingParameter);
            _camera.Start();

            // Marker-Detection aktivieren und Marker wählen
            _detector = new FeatureDetector();
            var modelImage = BitmapUtil.LoadImage("Media/model.png");
            _modelImage = BitmapUtil.ToMat(modelImage);

            // Event beim schließen der Anwendung
            Application.Current.Exit += onClosingApp;
        }

        #endregion

        #region member

        private void switchWindowState()
        {
            if (CurrentWindowState == WindowState.Maximized)
                CurrentWindowState = WindowState.Normal;
            else if (CurrentWindowState == WindowState.Normal)
                CurrentWindowState = WindowState.Maximized;
            else
                CurrentWindowState = WindowState.Normal;
        }

        private void OpenMarkerWindow()
        {
            if (_markerView != null)
                CloseMarkerWindow();

            _markerView = new MarkerView();
            _markerView.Show();
        }

        private void CloseMarkerWindow()
        {
            if (_markerView != null)
            {
                _markerView.Close();
                _markerView = null;
            }
        }

        private void StartToMeasure()
        {
            if (_camera.State == SensorState.Initialized
                && (_markerView != null && _markerView.IsInitialized))
                StateManager.Measure();
        }

        private void SaveResult()
        {
            if (_result != null)
                SerialUtil.SerializeFile(_result);
        }

        private void CancelMeasuring()
        {
            StateManager.None();
        }

        private void CloseApp()
        {
            _camera.Stop();
            CloseMarkerWindow();
            Application.Current.Shutdown();
        }

        private void stateChanged(CalibrationState newState) => RaisePropertyChanged(nameof(IsMeasuring));

        private void onClosingApp(object sender, ExitEventArgs e) => CloseApp();

        /// <summary>
        /// function is fired when camera record new image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ColorFrameReady(object sender, RSCamera.ColorFrameEventArgs args)
        {
            if (args.ColorData == null) return;

            var image = BitmapUtil.ConvertBitmap(args.ColorData);

            // Nach 2 Versuchen abbrechen und Fehler anzeigen
            if (_tries > 2)
            {
                CancelMeasuring();
                StateManager.Error();
            }
            RaisePropertyChanged(nameof(IsError));

            if (StateManager.CurrentState != CalibrationState.MEASURE)
            {
                // Bild anzeigen solange nicht gemessen wird
                ColorStream = image;
                // 0 versuche Marker zu finden
                _tries = 0;
            }
            else
            {
                // Neuer Versuch Marker zu finden
                _tries++;
                // Neue Messung
                Rectangle2 markerPositionInImage = findMarker(image);
                // Bricht weitere Berechnungen ab, falls bei der Messung etwas schief ging oder zu ungenau war
                if (markerPositionInImage == null) return;
                // Berechnet die 3D Positionen zu den gegebenen 2D Koordinaten
                Rectangle3 markerPositionInWorld = calculateMarkerPosition(markerPositionInImage);
                // Bricht weitere Berechnungen ab, falls bei der Messung etwas schief ging oder zu ungenau war
                if (markerPositionInWorld == null) return;
                // Berechnet die 3D Position zu gegeben 2D Koordinaten
                Result = markerPositionInWorld;
                RaisePropertyChanged(nameof(IsResult));
            }
        }

        /// <summary>
        /// find position of marker in observed image
        /// </summary>
        /// <param name="observedImage"></param>
        /// <returns></returns>
        private Rectangle2 findMarker(BitmapImage observedImage)
        {
            if (observedImage == null || _detector == null) return null;

            var detection = _detector.FindImage(_modelImage, BitmapUtil.ToMat(observedImage));
            return new Rectangle2(detection);
        }

        /// <summary>
        /// calculates position of marker in camera-coordinates
        /// </summary>
        /// <param name="markerPositionInImage"></param>
        /// <returns></returns>
        private Rectangle3 calculateMarkerPosition(Rectangle2 markerPositionInImage)
        {
            // Mittelpunkt auf der Diagonalen wird bestimmt     
            var centre = markerPositionInImage.GetCenter();
            // Mittelpunkt wird auf Tiefenbild der Kamera gemappt
            PXCMPoint3DF32 center3D = _camera.MapColorToCamera(new PXCMPointF32(centre.X, centre.Y));
            // Bricht weitere Berechnungen ab falls Mittelpunkt falsch gemessen wurde
            if (center3D.z == 0) return null;
            // Messung beenden
            CancelMeasuring();
            // Fläche wird intern berechnet
            var result = calculateMarkerPosition(markerPositionInImage, new Point3(center3D.x, center3D.y, center3D.z), _camera.GetColorFocalLength().y);

            return result;
        }

        /// <summary>
        /// calculates Rectangle3 with dimensions, location and orientation
        /// </summary>
        /// <param name="corners"></param>
        /// <param name="centre"></param>
        /// <param name="focalLength"></param>
        private Rectangle3 calculateMarkerPosition(Rectangle2 corners, Point3 centre, double focalLength)
        {
            var bottomLeftCorner = corners.Corners[0];
            var bottomRightCorner = corners.Corners[1];
            var TopRightCorner = corners.Corners[2];
            var TopLeftCorner = corners.Corners[3];

            // Mittelpunkt
            var centerPoint = centre;

            // Normalenvektor bestimmen und normalisieren
            Vector3 normalVector = new Vector3(centre.X, centre.Y, centre.Z);
            normalVector.Normalize();

            // Orientierungsvektor bestimmen und normalisieren -> verbesserungswürdig
            Vector3 upVector = new Vector3();

            // Breite bestimmen 
            // Da aufgenommenes Rechteck nicht unbedingt rechtwinklig, werden untere und obere Kantenlänge gerundet
            var widthBottom = DistanceBetweenPointF(bottomLeftCorner, bottomRightCorner) * centre.Z / focalLength;
            var widthTop = DistanceBetweenPointF(TopLeftCorner, TopRightCorner) * centre.Z / focalLength;
            var width = (widthBottom + widthTop) / 2;

            // Hoehe bestimmen
            var heightLeft = DistanceBetweenPointF(bottomLeftCorner, TopLeftCorner) * centre.Z / focalLength;
            var heightRight = DistanceBetweenPointF(bottomLeftCorner, TopLeftCorner) * centre.Z / focalLength;
            var height = (heightLeft + heightRight) / 2;

            // Marker als dreidimensionales Objekt anlegen
            var marker = new Rectangle3();
            marker.Width = width;
            marker.Height = height;
            marker.Translate.Set(centre);
            marker.Rotate.Set(normalVector);

            return marker;
        }

        /// <summary>
        /// calculates the distance between two Points
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        private static double DistanceBetweenPointF(PointF p1, PointF p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2));
        }

        #endregion

    }
}
