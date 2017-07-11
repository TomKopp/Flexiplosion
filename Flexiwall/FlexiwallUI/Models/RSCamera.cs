using CommonClassesLib;
using FlexiWallUtilities.Core;
using System;
using System.Drawing;

namespace FlexiWallUI.Models
{
    public class RSCamera : ICamera<RSStreamProfileSet>
    {
        #region fields

        private PXCMSenseManager _senseManager;
        private PXCMSenseManager.Handler _handler;
        private PXCMCapture.Device _device;
        private PXCMCapture.Sample _sample;
        private PXCMPoint3DF32[] _vertices;
        private DepthFrameEventArgs _depthFrameEventArgs;

        #endregion

        #region properties

        public RSStreamProfileSet Profiles { get; private set; }
        public SensorState State { get; private set; }

        #endregion

        #region constructor

        public RSCamera()
        {
            _senseManager = PXCMSenseManager.CreateInstance();
            _handler = new PXCMSenseManager.Handler();
            _handler.onConnect += onConnect;
        }

        #endregion

        #region eventhandling

        public delegate void DepthFrameEventHandler(object sender, DepthFrameEventArgs args);
        public event DepthFrameEventHandler DepthFrameReady;

        #endregion

        #region public member

        public void Initialize(RSStreamProfile profile)
        {
            var profileSet = new RSStreamProfileSet();
            profileSet.SetStreamProfile(profile);
            Initialize(profileSet);
        }

        public void Initialize(RSStreamProfileSet parameter)
        {
            Profiles = parameter;

            if (_senseManager == null)
                throw new Exception("Camera isn't able to initialize.");

            foreach (var profile in Profiles.StreamProfiles)
                if (profile != null)
                    _senseManager.EnableStream(
                        profile.Value.StreamType,
                        profile.Value.Size.width,
                        profile.Value.Size.height,
                        profile.Value.FrameRate,
                        profile.Value.StreamOption
                        );

            _handler.onNewSample += onNewSample;

            if (_senseManager.Init(_handler) < pxcmStatus.PXCM_STATUS_NO_ERROR)
            {
                State = SensorState.NotFound;
                return;
            }
            // throw new Exception("Handler isn't able to initialize.");

            _device.SetMirrorMode(PXCMCapture.Device.MirrorMode.MIRROR_MODE_HORIZONTAL);

            _depthFrameEventArgs = new DepthFrameEventArgs();

            State = SensorState.Initialized;
        }

        public void Start()
        {
            if (State == SensorState.Initialized)
                _senseManager.StreamFrames(false);
            else
                throw new Exception("Camera isn't able to stream , if it isn't initialized.");
        }

        public void Stop()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_senseManager != null)
                _senseManager.Dispose();
            else
                throw new Exception("Camera isn't able to dispose, if it isn't initialized.");
        }

        #endregion

        #region private member

        private PXCMPoint3DF32[] getVertices(PXCMCapture.Sample sample)
        {
            PXCMImage depthImage = sample.depth;
            var frameSize = depthImage.info.width * depthImage.info.height;
            if (_vertices == null || _vertices.Length != frameSize)
                _vertices = new PXCMPoint3DF32[frameSize];

            using (var projection = _device.CreateProjection())
            {
                projection.QueryVertices(depthImage, _vertices);
            }

            return _vertices;
        }

        private Bitmap getDepthImage(PXCMCapture.Sample sample)
        {
            PXCMImage depthImage = sample.depth;
            PXCMImage.ImageData imageData;
            depthImage.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32, out imageData);
            Bitmap bitmap = imageData.ToBitmap(0, depthImage.info.width, depthImage.info.height);
            depthImage.ReleaseAccess(imageData);

            return bitmap;
        }

        private pxcmStatus onNewSample(int mid, PXCMCapture.Sample sample)
        {
            _sample = sample;

            if (State == SensorState.Initialized)
            {
                if (Profiles.Depth != null)
                {
                    _depthFrameEventArgs.MID = mid;
                    _depthFrameEventArgs.DepthImage = getDepthImage(sample);
                    _depthFrameEventArgs.DepthData = getVertices(sample);
                    OnDepthFrameReady(this, _depthFrameEventArgs);
                }
            }

            return pxcmStatus.PXCM_STATUS_NO_ERROR;
        }

        private pxcmStatus onConnect(PXCMCapture.Device device, bool connected)
        {
            if (connected)
            {
                State = SensorState.Connected;
                _device = device;
                return pxcmStatus.PXCM_STATUS_NO_ERROR;
            }
            else
            {
                State = SensorState.NotFound;
                return pxcmStatus.PXCM_STATUS_DEVICE_FAILED;
            }
        }

        #endregion

        #region eventhandling

        protected virtual void OnDepthFrameReady(object sender, DepthFrameEventArgs args)
        {
            DepthFrameReady?.Invoke(sender, args);
        }

        public class DepthFrameEventArgs : EventArgs
        {
            public PXCMPoint3DF32[] DepthData { set; get; }
            public Bitmap DepthImage { set; get; }
            public int MID { set; get; }

            public DepthFrameEventArgs()
            {
                MID = 0;
                DepthData = null;
                DepthImage = null;
            }
        }

        #endregion

    }
}
