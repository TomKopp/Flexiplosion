using System;

namespace FlexiWallUtilities.Filter.InputProcessing
{
    public class EinEuroFilter
    {
        private bool _firstTime;
        private float _minCutoff, _cutoffSlope, _derivateCutoff;
        private LowPassFilter _lowPass, _derivateLowPass;
        private static float PI = (float)Math.PI;

        public float MinCutoff
        {
            get { return _minCutoff; }
            set { _minCutoff = value; }
        }

        public float CutoffSlope
        {
            get { return _cutoffSlope; }
            set { _cutoffSlope = value; }
        }

        public EinEuroFilter(float minCutoff, float cutoffSlope)
        {
            _firstTime = true;
            _minCutoff = minCutoff;
            _cutoffSlope = cutoffSlope;

            _lowPass = new LowPassFilter();
            _derivateLowPass = new LowPassFilter();
            _derivateCutoff = 1;
        }

        public float Process(float value, float updateRate)
        {
            float derivate = _firstTime ? 0 : (value - _lowPass.Last) * updateRate;
            if (_firstTime) _firstTime = false;

            var edx = _derivateLowPass.Process(derivate, Alpha(updateRate, _derivateCutoff));
            var cutoff = _minCutoff + _cutoffSlope * Math.Abs(edx);

            return _lowPass.Process(value, Alpha(updateRate, cutoff));
        }

        protected float Alpha(float updateRate, float cutoff)
        {
            float tau = 1.0f / (2 * PI * cutoff);
            float te = 1.0f / updateRate;
            return 1.0f / (1.0f + tau / te);
        }
    }
}
