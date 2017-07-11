using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using FlexiWallUI.Models;
using FlexiWallUI.ViewModels.Interface;
using Prism.Commands;
using Prism.Mvvm;

namespace FlexiWallUI.ViewModels
{
    public class ActionPropertiesViewModel : BindableBase, IFlexiWallAction
    {
        #region Fields

        private static uint _globalIdCounter;
        private readonly uint _id;

        protected readonly ActionProperties _props = new ActionProperties { Diameter = 20, Depth = 25, Center = new Point(0, 0), CenterOffset = 0.25 };

        private bool _isPushing;
        private bool _isPulling;

        private Point _startPoint;

        private bool _isVisible;
        private bool _isSelectionLocked;

        private readonly DispatcherTimer _actionTimer;

        private readonly IActionCollection _actionCollection;

        #endregion

        #region Properties

        public double Diameter
        {
            get { return _props.Diameter; }
            set
            {
                _props.Diameter = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(TransformCenterPosition));
                RaisePropertyChanged(nameof(CenterPosition));
                RaisePropertyChanged(nameof(ColorIntensity));
                RaisePropertyChanged(nameof(Name));
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set { SetProperty(ref _isVisible, value); }
        }

        public bool IsSelectionLocked
        {
            get { return _isSelectionLocked; }
            set
            {
                if (_isSelectionLocked == value)
                    return;
                _isSelectionLocked = value;
                _actionCollection.SelectedAction = this;
                RaisePropertyChanged();
            }
        }

        public double CenterOffset
        {
            get { return _props.CenterOffset; }
            set
            {
                _props.CenterOffset = value;
                RaisePropertyChanged();
            }
        }

        public Point CenterPosition
        {
            get { return _props.Center; }
        }

        public Point TransformCenterPosition
        {
            get { return _props.Center - new Vector(0.5*Diameter, 0.5*Diameter); }
        }

        public float Depth
        {
            get
            {
                float depth = (_props.Depth * 5);
                depth = depth <= 255 ? depth : 255;
                return depth >= 0 ? depth : 0;
            }
            set
            {
                _props.Depth = (float) Math.Floor(value/5);
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(ColorIntensity));
                RaisePropertyChanged(nameof(Name));
            }
        }

        public Color ColorIntensity
        {
            get
            {
                var c = (byte) Convert.ToInt32(Depth);
                Color result = Color.FromArgb(255, c, c, c);
                return result;
            }
        }

        public String Name
        {
            get
            {
                String pos = String.Format(" [{0:0} | {1:0}]", CenterPosition.X, CenterPosition.Y);
                String d = String.Format(" d = {0:0.#}", Diameter);
                String p = String.Format(" p = {0}", Depth);
                return "P " + _id + pos + d + p;
            }
        }

        public ICommand DeleteCmd { get; private set; }

        public ICommand LockSelectionCmd { get; private set; }

        #endregion

        #region Constructor

        public ActionPropertiesViewModel(IActionCollection actionCollection)
        {
            _actionTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            _actionTimer.Tick += OnExecuteAction;
            

            _id = _globalIdCounter++;

            DeleteCmd = new DelegateCommand<object>(DeleteAction);
            LockSelectionCmd = new DelegateCommand(ToggleSelectionLocked);
            _actionCollection = actionCollection;
        }
        #endregion

        #region Timer-Events

        private void OnExecuteAction(object sender, EventArgs e)
        {
            if (_isPushing)
                IncreaseDepth();
            if (_isPulling)
                DecreaseDepth();
        }

        #endregion

        #region IFlexiWallAction Implementation

        public void StartPush(Point startPos)
        {
            if (_isPushing)
                return;
            _isPushing = true;
            StartAction(startPos);
        }

        public void ContinuePush(Point currPos)
        {
            if (!_isPushing)
                return;

            UpdateAction(currPos);
        }

        public void StopPush(Point endPos)
        {
            if (!_isPushing)
                return;

            _isPushing = false;
            _actionTimer.Stop();

            UpdateAction(endPos);
        }

        public void StartPull(Point startPos)
        {
            if (_isPulling)
                return;
            _isPulling = true;
 
            StartAction(startPos);
        }

        public void ContinuePull(Point currPos)
        {
            if (!_isPulling)
                return;

            UpdateAction(currPos);
        }

        public void StopPull(Point endPos)
        {
            if (!_isPulling)
                return;

            _isPulling = false;
            _actionTimer.Stop();

            UpdateAction(endPos);
        }

        #endregion

        #region Auxiliary Methods

        public void StartAction(Point startPos)
        {
            _actionTimer.Start();
            _props.Center = startPos;

            _startPoint = startPos;

            RaisePropertyChanged(nameof(Diameter));
            RaisePropertyChanged(nameof(TransformCenterPosition));
            RaisePropertyChanged(nameof(CenterPosition));
            RaisePropertyChanged(nameof(Depth));
            RaisePropertyChanged(nameof(ColorIntensity));
            RaisePropertyChanged(nameof(Name));
        }

        private void UpdateAction(Point currPos)
        {
            Vector rVec = (currPos - _startPoint);
            double r = rVec.Length;
            Diameter = 2.0 * r;
            // _props.Center = _startPoint - new Vector(r, r);

            RaisePropertyChanged(nameof(Diameter));
            RaisePropertyChanged(nameof(TransformCenterPosition));
            RaisePropertyChanged(nameof(CenterPosition));
            RaisePropertyChanged(nameof(Depth));
            RaisePropertyChanged(nameof(ColorIntensity));
            RaisePropertyChanged(nameof(Name));
        }


        protected void IncreaseDepth()
        {
            Depth += 1;
        }

        protected void DecreaseDepth()
        {
            Depth -= 1;
        }

        protected void DeleteAction(object obj)
        {
            _actionCollection.Actions.Remove(obj as ActionPropertiesViewModel);
        }

        protected void ToggleSelectionLocked()
        {
            IsSelectionLocked = !IsSelectionLocked;
        }

        #endregion

        public void Move(Vector vector)
        {
            _props.Center += vector;
            RaisePropertyChanged(nameof(CenterPosition));
            RaisePropertyChanged(nameof(TransformCenterPosition));
            RaisePropertyChanged(nameof(Name));
        }

        public void Scale(int factor)
        {
            double f = factor > 0 ? 0.95 : 1.05;
            _props.Diameter *= f;

            RaisePropertyChanged(nameof(TransformCenterPosition));
            RaisePropertyChanged(nameof(Diameter));
            RaisePropertyChanged(nameof(Name));
        }
    }
}
