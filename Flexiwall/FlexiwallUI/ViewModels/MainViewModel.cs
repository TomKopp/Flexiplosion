using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using FlexiWallUI.Models;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Input;
using CommonClassesLib.Model;
using FlexiWallUI.Commands;
using FlexiWallUI.Properties;
using FlexiWallUI.Views;
using FlexiWallUI.ViewModels.Interface;
using Prism.Commands;
using System.IO;
using FlexiWallCalibration.Utilities;

namespace FlexiWallUI.ViewModels
{
    public class MainViewModel : BindableBase, IFlexiWallApplicationActions, IActionCollection
    {
        #region fields

        private OpenGLView _debugView;

        private ActionPropertiesViewModel _selectedAction;
        private LogViewModel _logVm;
        private readonly EffectProperties _fxProps;
        private bool _showPropertyPanel;
        private bool _showHelp;
        private bool _isFullScreen;
        private bool _mapViewActive;
        private bool _bubbleViewActive;
        private LayeredTextureResourceViewModel _selectedLayeredTextureResource;
        private int _textureIdx;

        private IFlexiWallAction _action;
        private bool _useEumlatorImg = true;
        private float _maxEmuDiameter;
        private MenuViewModel _menuVm;
        private BubbleViewModel _bubbleVm;
        private Point _zoomCenter;
        private Point _offset;
        private float _zoom;
        private Rect _lenseRect;
        private double _lenseSize;
        private double _lenseTopOffset;
        private double _lenseLeftOffset;
        private float _ellipseOpacity;
        private float _rectangleOpacity;
        private double _cameraTopOffset;
        private double _cameraLeftOffset;
        private bool _selectRectangle;
        private bool _selectEllipse;
        private float _zoomFactor;
        private double _lenseMinDepth;
        private string _imgSource;

        #endregion

        #region properties


        public SensorViewModel SensorVm { get; private set; }

        public ICommand AppCmd { get; }

        public ObservableCollection<TextureResourceViewModel> TextureRepository { get; private set; }

        public bool UseEmulatorImage
        {
            get { return _useEumlatorImg; }
            set
            {
                SetProperty(ref _useEumlatorImg, value);
                RaisePropertyChanged(nameof(IsEmulatorActive));
            }
        }

        public bool IsEmulatorActive
        {
            get { return UseEmulatorImage || SensorVm == null || !SensorVm.SensorConnected; }
        }

        public int SelectedTextureIndex
        {
            get { return _textureIdx; }

            set
            {
                if (_textureIdx == value)
                    return;
                _textureIdx = value;
                SelectedLayeredTextureResource = TextureRepository[SelectedTextureIndex] as LayeredTextureResourceViewModel;
                RaisePropertyChanged(nameof(SelectedTextureIndex));
            }
        }

        public LayeredTextureResourceViewModel SelectedLayeredTextureResource
        {
            get { return _selectedLayeredTextureResource; }
            set
            {
                ChangeSelectedResourceVm(value, ref _selectedLayeredTextureResource);
                RaisePropertyChanged(nameof(SelectedLayeredTextureResource));
            }
        }

        public Boolean ShowDepth
        {
            get { return _fxProps != null && _fxProps.ShowDepth; }
            set
            {
                if (_fxProps?.ShowDepth == value)
                    return;
                _fxProps.ShowDepth = value;
                //if (KinectVm != null && KinectVm.SensorConnected)
                //    KinectVm.Sensor.ShowDepthImage = value;
                EffectPropertiesProvider.CurrentlyActiveProperties = _fxProps;
                RaisePropertyChanged(nameof(ShowDepth));
                RaisePropertyChanged(nameof(ShowDepthFloat));
            }
        }

        public float ShowDepthFloat
        {
            get { return ShowDepth ? 1.0f : 0.0f; }
            set
            {
                bool showDepth = value > 0.5f;

                if (_fxProps.ShowDepth.Equals(showDepth))
                    return;

                _fxProps.ShowDepth = showDepth;
                EffectPropertiesProvider.CurrentlyActiveProperties = _fxProps;
                RaisePropertyChanged(nameof(ShowDepth));
                RaisePropertyChanged(nameof(ShowDepthFloat));
            }
        }

        public float InteractionDepth
        {
            get; private set;
        }

        public float Zoom
        {
            get { return _zoom; }
            set { SetProperty(ref _zoom, value); }
        }

        public float ZoomFactor
        {
            get { return _zoomFactor; }
            set { SetProperty(ref _zoomFactor, value); }
        }

        public Point OffsetPoint
        {
            get { return _offset; }
            set { SetProperty(ref _offset, value); }
        }

        public Point ZoomCenterPoint
        {
            get { return _zoomCenter; }
            set { SetProperty(ref _zoomCenter, value); }
        }

        public double LenseSize
        {
            get { return _lenseSize; }
            set { SetProperty(ref _lenseSize, value); }
        }

        public double LenseMinDepth
        {
            get { return _lenseMinDepth; }
            set { SetProperty(ref _lenseMinDepth, value); }
        }

        public double LenseTopOffset
        {
            get { return _lenseTopOffset; }
            set { SetProperty(ref _lenseTopOffset, value); }
        }

        public double LenseLeftOffset
        {
            get { return _lenseLeftOffset; }
            set { SetProperty(ref _lenseLeftOffset, value); }
        }

        public double CameraTopOffset
        {
            get { return _cameraTopOffset; }
            set { SetProperty(ref _cameraTopOffset, value); }
        }

        public double CameraLeftOffset
        {
            get { return _cameraLeftOffset; }
            set { SetProperty(ref _cameraLeftOffset, value); }
        }

        public Rect LenseRect
        {
            get { return _lenseRect; }
            set { SetProperty(ref _lenseRect, value); }
        }

        public float EllipseOpacity
        {
            get { return _ellipseOpacity; }
            set { SetProperty(ref _ellipseOpacity, value); }
        }

        public float RectangleOpacity
        {
            get { return _rectangleOpacity; }
            set { SetProperty(ref _rectangleOpacity, value); }
        }

        public bool SelectRectangle
        {
            get { return _selectRectangle; }
            set { SetProperty(ref _selectRectangle, value); }
        }

        public bool SelectEllipse
        {
            get { return _selectEllipse; }
            set { SetProperty(ref _selectEllipse, value); }
        }

        public float MinDepth
        {
            get { return _fxProps != null ? _fxProps.ClampDepthMin : 0; }
            set
            {
                if (_fxProps.ClampDepthMin.Equals(value))
                    return;
                _fxProps.ClampDepthMin = value;
                EffectPropertiesProvider.CurrentlyActiveProperties = _fxProps;
                // KinectVm.SetDepthValueRange(MinDepth, MaxDepth);
                RaisePropertyChanged(nameof(MinDepth));
            }
        }

        public float MaxDepth
        {
            get { return _fxProps != null ? _fxProps.ClampDepthMax : 0; }
            set
            {
                if (_fxProps.ClampDepthMax.Equals(value))
                    return;
                _fxProps.ClampDepthMax = value;
                EffectPropertiesProvider.CurrentlyActiveProperties = _fxProps;
                // KinectVm.SetDepthValueRange(MinDepth, MaxDepth);
                RaisePropertyChanged(nameof(MaxDepth));
            }
        }

        public float BlurRadius
        {
            get { return _fxProps != null ? _fxProps.BlurRadius : 0; }
            set
            {
                if (Math.Abs(_fxProps.BlurRadius - value) < double.Epsilon)
                    return;
                _fxProps.BlurRadius = value;
                EffectPropertiesProvider.CurrentlyActiveProperties = _fxProps;
                RaisePropertyChanged(nameof(BlurRadius));
            }
        }

        readonly string XRAY = "pack://application:,,,/Resources/img/Layer_xray.jpg";
        readonly string IR = "pack://application:,,,/Resources/img/Layer_ir.jpg";

        public string ImgSource
        {
            get { return _imgSource; }
            set { SetProperty(ref _imgSource, value); }
        }

        public float MaxEmulatorDiameter
        {
            get { return _maxEmuDiameter; }
            set { SetProperty(ref _maxEmuDiameter, value); }
        }

        public bool InterpolateDepthLayers
        {
            get { return _fxProps.InterpolateDepthLayers; }
            set
            {
                if (_fxProps.InterpolateDepthLayers == value)
                    return;
                _fxProps.InterpolateDepthLayers = value;
                EffectPropertiesProvider.CurrentlyActiveProperties = _fxProps;
                RaisePropertyChanged(nameof(InterpolateDepthLayers));
                RaisePropertyChanged(nameof(InterpolateDepthLayersFloat));
            }
        }

        public float InterpolateDepthLayersFloat
        {
            get
            {
                if (_fxProps != null)
                    return _fxProps.InterpolateDepthLayers ? 1.0f : 0.0f;
                else
                    return 0;
            }
        }


        public Visibility PropertyPanelVisibility => _showPropertyPanel ? Visibility.Visible : Visibility.Collapsed;

        public Visibility HelpPanelVisibility => _showHelp ? Visibility.Visible : Visibility.Collapsed;

        public Visibility TitleBarVisibility => _isFullScreen ? Visibility.Collapsed : Visibility.Visible;

        public Visibility MapViewVisible => _mapViewActive ? Visibility.Visible : Visibility.Collapsed;

        public Visibility BubbleViewVisible => _bubbleViewActive ? Visibility.Visible : Visibility.Collapsed;

        public string AppVersion => "Version " + Assembly.GetExecutingAssembly().GetName().Version;

        public ObservableCollection<ActionPropertiesViewModel> Actions { get; private set; }

        public ActionPropertiesViewModel SelectedAction
        {
            get { return _selectedAction; }
            set
            {
                if (_selectedAction == value)
                    return;
                if (_selectedAction != null)
                    _selectedAction.IsSelectionLocked = false;

                _selectedAction = value;

                if (_selectedAction != null)
                {
                    _selectedAction.IsSelectionLocked = true;
                    // _selectedAction.PropertyChanged += sendActionPoint;
                }

                RaisePropertyChanged(nameof(SelectedAction));
            }
        }

        public LogViewModel LogVm
        {
            get { return _logVm; }
            set
            {
                if (_logVm == value)
                    return;
                _logVm = value;
                RaisePropertyChanged(nameof(LogVm));
            }
        }

        public MenuViewModel MenuVm
        {
            get { return _menuVm; }
            set { SetProperty(ref _menuVm, value); }
        }

        public BubbleViewModel BubbleVm
        {
            get { return _bubbleVm; }
            set { SetProperty(ref _bubbleVm, value); }
        }

        public FlexiWallAppStateManager StateManager { get; private set; }

        public ICommand ActionCmd { get; }

        public ICommand ClearCmd { get; }

        public ICommand ChangeTextureCmd { get; }

        public ICommand ToggleDepthImageCmd { get; }

        public ICommand SaveSettingsCmd { get; }

        public ICommand TogglePropertyCmd { get; }

        public ICommand ShowOpenGlCmd { get; }

        #endregion

        #region Events

        public event EventHandler SelectedTextureChanged;

        #endregion

        #region constructor

        public MainViewModel()
        {
            _fxProps = new EffectProperties
            {
                ClampDepthMin = 0.0f,
                ClampDepthMax = 1.0f,
                ShowDepth = false
            };

            Actions = new ObservableCollection<ActionPropertiesViewModel>();
            AppCmd = new ApplicationCommand(this);
            ActionCmd = new DelegateCommand<object>(ExecuteFlexiWallAction);
            ClearCmd = new DelegateCommand<object>(ClearAction);
            ChangeTextureCmd = new DelegateCommand<object>(ChangeTextureResources);
            ToggleDepthImageCmd = new DelegateCommand<object>(ToggleDepthImage);
            SaveSettingsCmd = new DelegateCommand<object>(SaveSettings);
            TogglePropertyCmd = new DelegateCommand<object>(ToggleProperty);
            ShowOpenGlCmd = new DelegateCommand<object>(ShowOpenGlView);

            InitTextureRepo();

            SensorVm = new SensorViewModel();
            StateManager = new FlexiWallAppStateManager(this);
            StateManager.AppStateChanged += OnAppStateChanged;

            MenuVm = new MenuViewModel();
            BubbleVm = new BubbleViewModel();

            var logWindow = new LogView();
            _logVm = logWindow.DataContext as LogViewModel;

            ApplySettings();

            ImgSource = XRAY;

            StateManager.AdvanceToState(FlexiWallAppState.SelectAppType);

            SensorVm.InteractionChanged += OnInteractionChanged;
        }

        // important
        private void OnInteractionChanged(object sender, FlexiWall.InteractionEventArgs e)
        {
            InteractionDepth = e.DisplayCoordinates.Z;

            // Zoomfaktor
            Zoom = ZoomFactor * e.DisplayCoordinates.Z;

            RectangleOpacity = Zoom >= LenseMinDepth && SelectRectangle ? 1 : 0;
            EllipseOpacity = Zoom >= LenseMinDepth && SelectEllipse ? 1 : 0;

            var posX = 1920.0 - ((1.0f - e.DisplayCoordinates.X) * CameraLeftOffset);
            var posY = 1080.0 - ((1.0f - e.DisplayCoordinates.Y) * CameraTopOffset);
            
            // realer Druckpunkt
            ZoomCenterPoint = new Point(e.DisplayCoordinates.X * posX,
                e.DisplayCoordinates.Y * posY);

            // Punkt mit offset            
            var offsetX = ZoomCenterPoint.X + LenseLeftOffset - (ZoomCenterPoint.X);
            var offsetY = ZoomCenterPoint.Y + LenseTopOffset - (ZoomCenterPoint.Y);
            OffsetPoint = new Point(offsetX, offsetY);

            LenseRect = new Rect(new Point(ZoomCenterPoint.X, ZoomCenterPoint.Y), new Size(LenseSize / Zoom, LenseSize / Zoom));

            if (InteractionDepth < 0.3f)
                ImgSource = XRAY;
            else
                ImgSource = IR;

            RaisePropertyChanged(nameof(InteractionDepth));
        }

        #endregion

        private void OnAppStateChanged(object sender, EventArgs e)
        {
            var newState = StateManager.AppState;

            MenuVm.IsActive = (newState == FlexiWallAppState.SelectAppType || newState == FlexiWallAppState.Idle);
            if (newState == FlexiWallAppState.SelectAppType)
            {
                MenuVm.TransitionPosition = 0.5;
            }
            _mapViewActive = newState == FlexiWallAppState.ExploreMaps;
            _bubbleViewActive = newState == FlexiWallAppState.ExploreCompany;
            RaisePropertyChanged(nameof(MapViewVisible));
            RaisePropertyChanged(nameof(BubbleViewVisible));

            Application.Current.Dispatcher.Invoke(new Action(() => Log.LogMessage($"New App State handled in {GetType().FullName}: {newState}.")));
        }

        #region Methods

        /// <summary>
        /// Kamera hat neue Tiefenwerte
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DepthFrameReady(object sender, RSCamera.DepthFrameEventArgs args)
        {

        }

        private void InitTextureRepo()
        {
            if (TextureRepository == null)
            {
                TextureRepository = new ObservableCollection<TextureResourceViewModel>();
            }
            else
            {
                TextureRepository.ToList().ForEach(txRepo => txRepo.Unload());
                TextureRepository.Clear();
            }

            //TODO: outsourcing
            var acc = new XmlTextureResourceAccess<LayeredTextureRepository>();
            ITextureRepository<LayeredTextureResource> repo = acc.Load("resources/data.xml");


            //TODO: Refactor: create  Property (DependencyProperty ?) or define in Settings.xml instead of intializing formatString here
            double h = SystemParameters.PrimaryScreenHeight;
            string formatString = "1080p";

            if (h < 800)
                formatString = "720p";
            else if (h < 1000)
                formatString = "800p";

            repo.TextureResources.ForEach(res => TextureRepository.Add(new LayeredTextureResourceViewModel(res, formatString)));
            if (SelectedLayeredTextureResource == null)
                SelectedLayeredTextureResource = TextureRepository[0] as LayeredTextureResourceViewModel;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// why use generic type parameter instead of simply using base class 
        /// (== "private void ChangeSelectedResource(TextureResourceViewModel newValue, ref TextureResourceViewModel property)) ?
        /// "ref" in C# does not allow to be used with any type different from the type specified (no derived types etc.). Reason for this:
        /// allowing derived types to be passed by reference would wallow you to do something like this:
        /// <code>
        /// class Base { }
        /// 
        /// class Derived : Base { }
        /// 
        /// class Program
        /// {
        ///     static void f(ref Base b) { }
        /// 
        ///     public static void Main()
        ///     {
        ///         Derived d = new Derived();
        ///         f(ref d);
        ///     }
        /// }
        /// 
        /// static void f(ref Base b) { b = new Base(); }
        /// </code>
        /// which would completely break type-safety (consider: calling
        /// <code>
        /// d.OnlyInDerived()
        /// </code>
        /// somewhere else in code...)
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="newValue"></param>
        /// <param name="property"></param>
        private void ChangeSelectedResourceVm<T>(T newValue, ref T property) where T : TextureResourceViewModel
        {
            if (property == newValue)
                return;
            if (property != null)
                property.Unload();
            property = newValue;
            if (property != null)
                property.Load();

            OnSelectedTextureChanged();
        }

        private void OnSelectedTextureChanged()
        {
            if (SelectedTextureChanged != null)
                SelectedTextureChanged(this, null);
        }

        #endregion

        #region IFlexiWallApplicationActions Implementation

        public void TogglePropertyPanelVisibility()
        {
            _showPropertyPanel = !_showPropertyPanel;
            RaisePropertyChanged(nameof(PropertyPanelVisibility));
        }

        public void ToggleFullScreen()
        {
            _isFullScreen = !_isFullScreen;
            Application.Current.MainWindow.WindowState = _isFullScreen ? WindowState.Maximized : WindowState.Normal;
            RaisePropertyChanged(nameof(TitleBarVisibility));
        }

        public void ToggleAppMinimized()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void ToggleHelp()
        {
            _showHelp = !_showHelp;
            RaisePropertyChanged(nameof(HelpPanelVisibility));
        }

        public void ToggleLogVisibility()
        {
            _logVm.LogWindowCmd.Execute("ToggleVisibility");
        }

        public void Play(double duration)
        {
            MenuVm.TransitionPosition += duration;
        }

        public void SwitchAppState(FlexiWallAppState targetState)
        {
            StateManager.AdvanceToState(targetState);
        }

        #endregion

        #region Command Implementations

        private void ExecuteFlexiWallAction(object parameter)
        {
            if (parameter == null)
            {
                Log.LogCommandExecuteFailedNoParam(ActionCmd);
                throw new ArgumentNullException("Cannot execute " + GetType().Name + ". No parameter provided.");
            }

            var param = parameter as FlexiWallActionArguments;

            if (param == null)
            {
                Log.LogCommandExecuteFailedNoParam(ActionCmd);
                throw new ArgumentException("Cannot execute " + GetType().Name +
                                            ". Invalid parameter type. Provided param is of type: " +
                                            parameter.GetType() + ", expected paramerter type is: " +
                                            typeof(FlexiWallActionArguments).Name + ".");
            }

            if (param.State == FlexiWallActionState.Start)
            {
                _action = new ActionPropertiesViewModel(this);
                Actions.Add(_action as ActionPropertiesViewModel);

                if (param.Type == FlexiWallActionType.Push)
                    _action.StartPush(param.Position);

                if (param.Type == FlexiWallActionType.Pull)
                    _action.StartPull(param.Position);

                Log.LogCommandSucessfullyExecuted(ActionCmd, param);
            }

            if (_action != null && param.State == FlexiWallActionState.Continue)
            {
                _action.ContinuePush(param.Position);
                _action.ContinuePull(param.Position);
                Log.LogCommandSucessfullyExecuted(ActionCmd, param);
            }

            if (_action != null && param.State == FlexiWallActionState.Stop)
            {
                if (param.Type == FlexiWallActionType.Push)
                    _action.StopPush(param.Position);

                if (param.Type == FlexiWallActionType.Pull)
                    _action.StopPull(param.Position);

                _action = null;
                Log.LogCommandSucessfullyExecuted(ActionCmd, param);
            }

        }

        private void ClearAction(object parameter)
        {
            Actions.Clear();
            Log.LogCommandSucessfullyExecuted(ClearCmd);
        }

        private void ChangeTextureResources(object parameter)
        {
            var param = parameter as String;

            if (String.IsNullOrEmpty(param))
            {
                Log.LogCommandExecuteFailedNoParam(ChangeTextureCmd);
                return;
            }

            int idx = SelectedTextureIndex;
            int max = TextureRepository.Count;

            if (param.Equals("inc"))
            {
                idx = (idx > max - 2) ? 0 : idx + 1;
            }

            if (param.Equals("dec"))
            {
                idx = (idx < 1) ? max - 1 : idx - 1;
            }

            SelectedTextureIndex = idx;

            Application.Current.Dispatcher.Invoke(() =>
                Log.LogCommandSucessfullyExecuted(ChangeTextureCmd, param, $"New Texture Index is:{idx}"));
        }

        private void ToggleDepthImage(object parameter)
        {
            var param = parameter as String;

            if (String.IsNullOrEmpty(param))
            {
                Log.LogCommandExecuteFailedNoParam(ToggleDepthImageCmd);
                return;
            }

            if (param.Equals("toggleDepth"))
            {
                ShowDepth = !ShowDepth;
                Log.LogCommandSucessfullyExecuted(ToggleDepthImageCmd, param);
            }
            else
            {
                throw new NotImplementedException("Value " + param + " is not connected with an implemented action.");
            }
        }

        private void SaveSettings(object parameter)
        {
            if (parameter == null)
            {
                Log.LogCommandExecuteFailedNoParam(SaveSettingsCmd);
                return;
            }

            if (parameter.Equals("Save"))
            {
                SaveSettings();
                Log.LogCommandSucessfullyExecuted(SaveSettingsCmd, parameter);
                return;
            }

            if (parameter.Equals("Apply"))
            {
                ApplySettings();
                Log.LogCommandSucessfullyExecuted(SaveSettingsCmd, parameter);
                return;
            }

            throw new NotImplementedException("Value " + parameter + " is not connected with an implemented action.");
        }

        private void SaveSettings()
        {
            Settings.Default.UseEmulator = UseEmulatorImage;
            Settings.Default.ShowDepth = ShowDepth;
            Settings.Default.MaxDepth = MaxDepth;
            Settings.Default.MinDepth = MinDepth;
            Settings.Default.BlurRadius = BlurRadius;
            Settings.Default.MaxEmulatorDiameter = MaxEmulatorDiameter;
            Settings.Default.InterpolateDepthLayers = InterpolateDepthLayers;

            Settings.Default.InteractionDepth = InteractionDepth;
            Settings.Default.SelectEllipse = SelectEllipse;
            Settings.Default.SelectRectangle = SelectRectangle;
            Settings.Default.ZoomFactor = ZoomFactor;
            Settings.Default.LenseSize = LenseSize;
            Settings.Default.LenseMinDepth = LenseMinDepth;
            Settings.Default.LenseTopOffset = LenseTopOffset;
            Settings.Default.LenseLeftOffset = LenseLeftOffset;
            Settings.Default.CameraTopOffset = CameraTopOffset;
            Settings.Default.CameraLeftOffset = CameraLeftOffset;

            Settings.Default.Save();
        }

        private void ApplySettings()
        {
            // @todo: Load Settings and set according variables in vm
            MinDepth = Settings.Default.MinDepth;
            MaxDepth = Settings.Default.MaxDepth;
            BlurRadius = Settings.Default.BlurRadius;
            InterpolateDepthLayers = Settings.Default.InterpolateDepthLayers;
            ShowDepth = Settings.Default.ShowDepth;
            MaxEmulatorDiameter = Settings.Default.MaxEmulatorDiameter;
            UseEmulatorImage = Settings.Default.UseEmulator;

            InteractionDepth = Settings.Default.InteractionDepth;
            SelectEllipse = Settings.Default.SelectEllipse;
            SelectRectangle = Settings.Default.SelectRectangle;
            ZoomFactor = Settings.Default.ZoomFactor;
            LenseSize = Settings.Default.LenseSize;
            LenseMinDepth = Settings.Default.LenseMinDepth;
            LenseTopOffset = Settings.Default.LenseTopOffset;
            LenseLeftOffset = Settings.Default.LenseLeftOffset;
            CameraTopOffset = Settings.Default.CameraTopOffset;
            CameraLeftOffset = Settings.Default.CameraLeftOffset;

            //LoadCalibaration

            var fileName = Settings.Default.ConfigFile;
            if (!string.IsNullOrWhiteSpace(fileName) && File.Exists(fileName))
            {
                SensorVm.FlexiWall.Calibration = SerialUtil.DeSerializeObject<FlexiWallCalibration.Models.Rectangle3>(new FileStream(fileName, FileMode.Open, FileAccess.Read));
            }
        }

        // @TODO: refactoring: generic command
        private void ToggleProperty(object parameter)
        {
            //TODO: use Reflection to retrieve Properties from Name (extend UtilityLibrary.ReflectionUtility --> GetMember)
            var param = parameter as String;

            if (String.IsNullOrWhiteSpace(param))
            {
                Log.LogCommandExecuteFailedNoParam(TogglePropertyCmd);
                return;
            }

            PropertyInfo pi = GetType().GetProperty(param);
            if (pi == null || pi.PropertyType != typeof(bool))
            {
                throw new NotImplementedException(GetType().FullName + " can only be used to toggle boolean values as parameters.");
            }

            pi.SetValue(this, !(bool)pi.GetValue(this, null), null);

            Log.LogCommandSucessfullyExecuted(TogglePropertyCmd, param, $"New value for{pi.Name} is: {pi.GetValue(this, null)}");
        }

        /// <summary>
        /// shows pointcloud and interaction via openGl
        /// </summary>
        /// <param name="obj"></param>
        private void ShowOpenGlView(object obj)
        {
            _debugView = new OpenGLView();
            _debugView.RenderTarget = SensorVm.FlexiWall;
            _debugView.Interaction = SensorVm.Interaction;
            _debugView.Show();

            // @Todo: logging
        }

        #endregion
    }
}
