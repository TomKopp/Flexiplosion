using CommonClassesLib;
using FlexiWallUtilities.Core;
using System;
using System.Drawing;

namespace FlexiWallCalibration.Models
{
    public class RSCamera : ICamera<RSStreamProfileSet>
    {
        #region fields

        private PXCMSenseManager _senseManager;
        private PXCMSenseManager.Handler _handler;
        private PXCMCapture.Device _device;
        private PXCMCapture.Sample _sample;
        private PXCMPoint3DF32[] _vertices;
        private DepthFrameEventArgs _depthArgs;
        private ColorFrameEventArgs _colorArgs;

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

        public delegate void ColorFrameEventHandler(object sender, ColorFrameEventArgs args);
        public event ColorFrameEventHandler ColorFrameReady;

        #endregion

        #region public member

        public void Initialize(RSStreamProfileSet parameter)
        {
            Profiles = parameter;

            if (_senseManager == null)
                throw new Exception("Camera isn't able to initialize.");

            PXCMVideoModule.DataDesc ddesc = new PXCMVideoModule.DataDesc();
            ddesc.deviceInfo.streams = PXCMCapture.StreamType.STREAM_TYPE_COLOR | PXCMCapture.StreamType.STREAM_TYPE_DEPTH;
            _senseManager.EnableStreams(ddesc);

            _handler.onNewSample += onNewSample;

            if (_senseManager.Init(_handler) < pxcmStatus.PXCM_STATUS_NO_ERROR)
                throw new Exception("Handler isn't able to initialize.");

            _depthArgs = new DepthFrameEventArgs(0, null);
            _colorArgs = new ColorFrameEventArgs(0, null);

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

        public PXCMPointF32 GetColorFocalLength()
        {
            if (State == SensorState.Initialized)
                return _device.QueryColorFocalLength();
            else
                return new PXCMPointF32(0, 0);
        }

        public PXCMPointF32 GetDepthFocalLength()
        {
            if (State == SensorState.Initialized)
                return _device.QueryDepthFocalLength();
            else
                return new PXCMPointF32(0, 0);
        }

        public PXCMPoint3DF32 MapColorToCamera(PXCMPointF32 posij)
        {
            PXCMPointF32[] posijArr = new PXCMPointF32[1];
            posijArr[0] = posij;
            return mapColorToCamera(posijArr)[0];
        }

        public PXCMPoint3DF32[] MapColorToCamera(PXCMPointF32[] posij)
        {
            return mapColorToCamera(posij);
        }

        #endregion

        #region private member

        private PXCMPoint3DF32[] mapColorToCamera(PXCMPointF32[] posij)
        {
            if (_sample.depth == null) return null;

            PXCMImage depth = _sample.depth;
            PXCMProjection projection = _device.CreateProjection();
            PXCMPointF32[] posuv = new PXCMPointF32[posij.Length];
            PXCMPoint3DF32[] posxyz = new PXCMPoint3DF32[posij.Length];
            PXCMPoint3DF32[] vertices = new PXCMPoint3DF32[depth.info.height * depth.info.width];

            projection.MapColorToDepth(depth, posij, posuv);
            projection.QueryVertices(depth, vertices);

            for (int i = 0; i < posij.Length; i++)
            {
                if (posuv[i].x >= 0 && posuv[i].y >= 0)
                    posxyz[i] = vertices[(int)(posuv[i].x + depth.info.width * posuv[i].y)];
            }

            return posxyz;
        }

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

        private Bitmap getColorData(PXCMCapture.Sample sample)
        {
            PXCMImage colorImage = sample.color;
            PXCMImage.ImageData colorData;
            colorImage.AcquireAccess(PXCMImage.Access.ACCESS_READ, PXCMImage.PixelFormat.PIXEL_FORMAT_RGB32, out colorData);
            Bitmap bitmap = colorData.ToBitmap(0, colorImage.info.width, colorImage.info.height);
            colorImage.ReleaseAccess(colorData);

            return bitmap;
        }

        private pxcmStatus onNewSample(int mid, PXCMCapture.Sample sample)
        {
            _sample = sample;

            if (State == SensorState.Initialized)
            {
                if (sample.depth != null)
                {
                    _depthArgs.MID = mid;
                    _depthArgs.DepthData = getVertices(sample);
                    OnDepthFrameReady(this, _depthArgs);
                }

                if (sample.color != null)
                {
                    _colorArgs.MID = mid;
                    _colorArgs.ColorData = getColorData(sample);
                    OnColorFrameReady(this, _colorArgs);
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

        protected virtual void OnColorFrameReady(object sender, ColorFrameEventArgs args)
        {
            ColorFrameReady?.Invoke(sender, args);
        }

        public class DepthFrameEventArgs : EventArgs
        {
            public PXCMPoint3DF32[] DepthData { set; get; }
            public int MID { set; get; }

            public DepthFrameEventArgs(int mid, PXCMPoint3DF32[] depthData)
            {
                MID = mid;
                DepthData = depthData;
            }
        }

        public class ColorFrameEventArgs : EventArgs
        {
            public Bitmap ColorData { set; get; }
            public int MID { set; get; }

            public ColorFrameEventArgs(int mid, Bitmap colorData)
            {
                MID = mid;
                ColorData = colorData;
            }
        }

        #endregion

    }
}
