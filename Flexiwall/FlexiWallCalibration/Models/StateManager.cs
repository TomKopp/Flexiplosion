namespace FlexiWallCalibration.Models
{
    public class StateManager
    {
        private static StateManager instance;

        public delegate void StateChangedEventHandler(CalibrationState newState);
        public static event StateChangedEventHandler StateChanged;

        private StateManager()
        {
            None();
        }

        public static StateManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new StateManager();
                }
                return instance;
            }
        }

        private static CalibrationState _state;
        public static CalibrationState CurrentState
        {
            get { return _state; }
            private set
            {
                _state = value;
                StateChanged?.Invoke(_state);
            }
        }

        public static void None() => CurrentState = CalibrationState.NONE;

        public static void Measure() => CurrentState = CalibrationState.MEASURE;

        public static void Error() => CurrentState = CalibrationState.ERROR;

    }

    public enum CalibrationState
    {
        NONE,
        MEASURE,
        ERROR
    }
}
