using System;
using System.Collections.Generic;
using System.Linq;
using CommonClassesLib.Model;
using FlexiWallUI.Properties;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Threading;
using FlexiWallUI.Commands;
using FlexiWallUI.Utilities;
using FlexiWallUI.ViewModels;

namespace FlexiWallUI.Models
{
    public class FlexiWallAppStateManager : BindableBase
    {
        private FlexiWallAppState _state;
        private readonly Stack<FlexiWall.InteractionEventArgs> _history = new Stack<FlexiWall.InteractionEventArgs>();
        private readonly MainViewModel _vm;
        private readonly DispatcherTimer _switchStateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(Settings.Default.StateManagerActionDelay) };
        private readonly DispatcherTimer _switchMapTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(50) };
        private bool _actionsEnabled = true;
        private bool _actionDetected;
        private DateTime _mapTimerStarted;

        private double _sizeX = Settings.Default.ResolutionX;
        private double _sizeY = Settings.Default.ResolutionY;

        private static double SqrDistThreshold = 400*400;

        private static readonly Dictionary<AnimationType, Point> AnimationControls = new Dictionary<AnimationType, Point>
        {
            { AnimationType.Interfaces, new Point(750,300)},
            { AnimationType.Data, new Point(1350,400)},
            { AnimationType.Systems, new Point(800,750)},
            { AnimationType.Innovation, new Point(1250,800)}
        };

        private static readonly Dictionary<AnimationType, Point> AnimationDistances = new Dictionary<AnimationType, Point>
        {
            { AnimationType.Interfaces, new Point(-1,-1)},
            { AnimationType.Data, new Point(-1,-1)},
            { AnimationType.Systems, new Point(-1,-1)},
            { AnimationType.Innovation, new Point(-1,-1)}
        };


        public FlexiWallAppState AppState
        {
            get { return _state; }
            private set
            {
                SetProperty(ref _state, value);
                OnAppStateChanged();
            }
        }

        public bool ActionDetected
        {
            get { return _actionDetected; }
            private set
            {
                TriggerSwitchMapTimer(_actionDetected, value);
                SetProperty(ref _actionDetected, value);
            }
        }

        public bool MapTimerRunning
        {
            get { return _switchMapTimer.IsEnabled; }
        }

        public double MapTimerProgress
        {
            get
            {
                if (!MapTimerRunning)
                    return 0;

                var dt = DateTime.Now;
                var diff = (_mapTimerStarted - dt).TotalMilliseconds;
                return -(diff / TimeSpan.FromSeconds(Settings.Default.StateManagementLayerTimer).TotalMilliseconds);
            }
        }

        public event EventHandler<EventArgs> AppStateChanged;
        public event EventHandler<EventArgs> SwitchMapTimerChanged;


        public FlexiWallAppStateManager(MainViewModel vm)
        {
            _vm = vm;
            _vm.SensorVm.InteractionChanged += OnInteractionChanged;
            _switchStateTimer.Tick += OnSwitchStateTimerTick;
            _switchMapTimer.Tick += OnSwitchMapTimerTick;

        }

        private void OnSwitchStateTimerTick(object sender, EventArgs e)
        {

            _switchStateTimer.Stop();
            _actionsEnabled = true;
        }

        private void OnSwitchMapTimerTick(object sender, EventArgs e)
        {
            if ((DateTime.Now - _mapTimerStarted) >= TimeSpan.FromSeconds(Settings.Default.StateManagementLayerTimer))
            {
                _vm.ChangeTextureCmd.Execute("inc");
                _switchMapTimer.Stop();
                TriggerSwitchMapTimer(true, false);
            }

            RaisePropertyChanged(nameof(MapTimerProgress));
        }

        public void EvaluateInteraction(FlexiWall.InteractionEventArgs args)
        {
            _history.Push(args);
            if (_history.Count > 100)
                _history.Pop();


            var pulledForSpecificSamples = CheckHistoryValidity(FlexiWall.InteractionType.PULLED, 2.5 * Settings.Default.DepthThreshold);
            var pushedForSpecificSamples = CheckHistoryValidity(FlexiWall.InteractionType.PUSHED, 2.5 * Settings.Default.DepthThreshold);

            if (IsValid(FlexiWall.InteractionType.PULLED, args))
            {
                if (CheckHistoryValidity(FlexiWall.InteractionType.PULLED))
                    GoBack();
            }

            if (IsValid(FlexiWall.InteractionType.PUSHED, args))
            {
                if (CheckHistoryValidity(FlexiWall.InteractionType.PUSHED))
                    Advance();
            }

            ActionDetected = pulledForSpecificSamples || pushedForSpecificSamples;
        }

        public void AdvanceToState(FlexiWallAppState targetState)
        {
            AppState = targetState;
        }

        public void GoBack()
        {
            if (!_actionsEnabled)
                return;

            FlexiWallAppState targetState = FlexiWallAppState.Idle;
            var oldState = AppState;

            switch (AppState)
            {
                case FlexiWallAppState.Idle:
                    return;
                case FlexiWallAppState.SelectAppType:
                    targetState = FlexiWallAppState.Idle;
                    AdvanceToState(targetState);
                    break;
                case FlexiWallAppState.ExploreMaps:
                case FlexiWallAppState.ExploreCompany:
                    targetState = FlexiWallAppState.SelectAppType;
                    AdvanceToState(targetState);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            StartStateChangedTimer(FlexiWallActionType.Pull, targetState, oldState);


        }

        public void Advance()
        {
            FlexiWallAppState targetState = FlexiWallAppState.Idle;
            var oldState = AppState;

            switch (AppState)
            {
                case FlexiWallAppState.Idle:
                    targetState = FlexiWallAppState.SelectAppType;
                    AdvanceToState(targetState);
                    break;
                case FlexiWallAppState.SelectAppType:
                    targetState =
                        _history.Peek().DisplayCoordinates.X > 0.5
                            ? FlexiWallAppState.ExploreCompany
                            : FlexiWallAppState.ExploreMaps;
                    AdvanceToState(targetState);
                    break;
                case FlexiWallAppState.ExploreMaps:
                case FlexiWallAppState.ExploreCompany:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            StartStateChangedTimer(FlexiWallActionType.Push, targetState, oldState);

        }

        private void StartStateChangedTimer(FlexiWallActionType type, FlexiWallAppState targetState, FlexiWallAppState oldState)
        {
            _history.Clear();


            _actionsEnabled = false;
            _switchStateTimer.Start();

            Application.Current.Dispatcher.Invoke(new Action(() =>
                Log.LogMessage($"Detected {type}, Switch to state {targetState}. Old State was: {oldState}.")));
        }

        private void TriggerSwitchMapTimer(bool oldState, bool newState)
        {
            if (oldState != newState && !newState && !_switchMapTimer.IsEnabled)
            {
                _mapTimerStarted = DateTime.Now;
                _switchMapTimer.Start();
                SwitchMapTimerChanged?.Invoke(this, new EventArgs());
                RaisePropertyChanged(nameof(MapTimerProgress));
            }

            if (newState && _switchMapTimer.IsEnabled)
            {
                _switchMapTimer.Stop();
                SwitchMapTimerChanged?.Invoke(this, new EventArgs());
                RaisePropertyChanged(nameof(MapTimerProgress));
            }
        }

        private void OnAppStateChanged()
        {
            TriggerSwitchMapTimer(false, true);
            AppStateChanged?.Invoke(this, new EventArgs());
        }

        private bool IsValid(FlexiWall.InteractionType type, FlexiWall.InteractionEventArgs args, double? specificThreshold = null)
        {
            var threshold = type == FlexiWall.InteractionType.PULLED
                ? Settings.Default.StateManagementPullThreshold
                : Settings.Default.StateManagementPushThreshold;

            if (specificThreshold != null)
                threshold = specificThreshold.Value;

            return args.TypeOfInteraction == type &&
        Math.Abs(args.DisplayCoordinates.Z) > threshold;
        }

        private void OnInteractionChanged(object sender, FlexiWall.InteractionEventArgs e)
        {
            EvaluateInteraction(e);

            if (AppState == FlexiWallAppState.ExploreMaps)
            {
                var action = _vm.Actions.FirstOrDefault();

                if (action == null)
                {
                    action = new ActionPropertiesViewModel(_vm);
                    Application.Current.Dispatcher.Invoke(new Action(() => _vm.Actions.Add(action)));
                }

                var interaction = e;

                var d = (interaction.DisplayCoordinates.Z);

                if (d < Settings.Default.DepthThreshold || interaction.TypeOfInteraction == FlexiWall.InteractionType.PULLED)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => _vm.Actions.Clear()));
                    return;
                }

                //if (interaction.TypeOfInteraction == InteractionType.PULLED)
                //    d *= -1.0f;

                action.Depth = (float)Settings.Default.DefaultDepthColorValue +
                               (d * (float)Settings.Default.DefaultDepthColorValue);
                action.CenterOffset = Settings.Default.EmulatorCenterSize;
                var centerPosition =
                    new Point(
                        interaction.DisplayCoordinates.X * Settings.Default.ResolutionX,
                        interaction.DisplayCoordinates.Y * Settings.Default.ResolutionY);
                action.Diameter = _vm.MaxEmulatorDiameter * (d);

                action.CenterOffset = Settings.Default.EmulatorCenterSize;

                action.StartAction(centerPosition);
            }

            if (AppState == FlexiWallAppState.SelectAppType)
            {
                var scale = e.DisplayCoordinates.X < 0.5f ? -1.0 : 1.0;
                if (e.TypeOfInteraction == FlexiWall.InteractionType.PULLED ||
                    e.DisplayCoordinates.Z < Settings.Default.DepthThreshold)
                    return;

                var pos = ((e.DisplayCoordinates.Z - Settings.Default.DepthThreshold) * 1.0 /
                           Settings.Default.StateManagementPushThreshold);

                pos *= 0.5 * scale;

                pos = 0.5 + pos;

                _vm.MenuVm.TransitionPosition = pos;

            }

            if (AppState == FlexiWallAppState.ExploreCompany)
            {
                if (e.TypeOfInteraction == FlexiWall.InteractionType.PULLED ||
                    e.DisplayCoordinates.Z < Settings.Default.DepthThreshold)
                    return;

                var minDist = -1.0;
                var associatedtype = AnimationType.Data;

                AnimationControls.Keys.ToList().ForEach(key =>
                {
                    var controlPt = AnimationControls[key];
                    var x = e.DisplayCoordinates.X * _sizeX;
                    var y = e.DisplayCoordinates.Y * _sizeY;

                    var sqrDist = (controlPt.X - x)*(controlPt.X - x) +
                                  (controlPt.Y - y)*(controlPt.Y - y);

                    if (minDist < 0 || minDist > sqrDist)
                    {
                        minDist = sqrDist;
                        associatedtype = key;
                    }
                });

                if (minDist > SqrDistThreshold)
                    return;


                var pos = ((e.DisplayCoordinates.Z - Settings.Default.DepthThreshold) * 1.0 /
                           Settings.Default.StateManagementPushThreshold);

                _vm.BubbleVm.CurrentType = associatedtype;
                _vm.BubbleVm.TransitionPosition = pos;

            }
        }

        private bool CheckHistoryValidity(FlexiWall.InteractionType type, double? specificThreshold = null)
        {
            int count = 1;

            if (_history.Count <= Settings.Default.StateManagementStackSize)
                return false;

            for (int i = 1; i < Settings.Default.StateManagementStackSize; i++)
            {
                //if (IsValid(type, _history.ToList()[_history.Count - i], specificThreshold))
                  //  count += 1;
            }

            return count >= Settings.Default.StateManagementStackSize / 2;
        }
    }
}
