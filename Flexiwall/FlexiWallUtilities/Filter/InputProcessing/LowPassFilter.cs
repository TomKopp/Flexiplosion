namespace FlexiWallUtilities.Filter.InputProcessing
{
    public class LowPassFilter
    {
        private float _last;
        private bool _firstTime;

        public float Last
        {
            get { return _last; }
        }

        public LowPassFilter()
        {
            _firstTime = true;
        }

        public float Process(float value, float alpha)
        {
            float current = _firstTime == true ? value : alpha * value + (1 - alpha) * _last;
            if(_firstTime) _firstTime = false;

            _last = current;

            return current;
        }
    }
}
