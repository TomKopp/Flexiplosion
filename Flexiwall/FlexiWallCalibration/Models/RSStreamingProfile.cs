using StreamType = PXCMCapture.StreamType;
using StreamOption = PXCMCapture.Device.StreamOption;

namespace FlexiWallCalibration.Models
{
    /// <summary>
    /// description of a stream
    /// </summary>
    public struct RSStreamProfile
    {

        #region properties

        public PXCMSizeI32 Size;
        public int FrameRate;
        public StreamType StreamType;
        public StreamOption StreamOption;

        #endregion

        #region constructor

        /// <summary>
        /// description of a stream
        /// </summary>
        /// <param name="size"> resolution of the capture </param>
        /// <param name="frameRate"> framerate of the capture </param>
        /// <param name="streamType"> type of stream like color or depth </param>
        /// <param name="streamOption"> various predefined options </param>
        public RSStreamProfile(PXCMSizeI32 size, int frameRate = 30, StreamType streamType = StreamType.STREAM_TYPE_ANY, StreamOption streamOption = StreamOption.STREAM_OPTION_ANY)
        {
            Size = size;
            FrameRate = frameRate;
            StreamType = streamType;
            StreamOption = streamOption;
        }

        #endregion

    }
}
