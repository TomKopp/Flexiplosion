using System.Diagnostics;

namespace FlexiWallUI.Utilities
{
    /// <summary>
    /// calculates lengzh of one frame
    /// </summary>
    public class FPSCounter
    {
        private float _fps;
        private Stopwatch _stopWatch;

        /// <summary>
        /// how many frames in one second
        /// </summary>
        public float FPS
        {
            get { return _fps; }
            private set { _fps = value; }
        }

        public FPSCounter()
        {
            _fps = 0;
        }

        /// <summary>
        /// set time of frame-start
        /// </summary>
        public void SetFrameStart()
        {
            _stopWatch = new Stopwatch();
            _stopWatch.Start();
        }

        /// <summary>
        /// set time of frame-end
        /// </summary>
        public void SetFrameEnd()
        {
            _stopWatch.Stop();
            long time = _stopWatch.ElapsedMilliseconds;

            // fps durch gewichtetes Mittel runden
            FPS = time != 0 ? ((1000 / time) * 0.001f) + _fps * 0.999f : 1000;
        }
    }
}
