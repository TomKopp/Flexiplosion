using System;
using System.Collections.Generic;

using StreamType = PXCMCapture.StreamType;
using StreamOption = PXCMCapture.Device.StreamOption;

namespace FlexiWallUI.Models
{
    /// <summary>
    /// a set of stream-profiles
    /// </summary>
    public class RSStreamProfileSet
    {

        #region properties

        /// <summary>
        /// List of the five stream-types
        /// </summary>
        public List<RSStreamProfile?> StreamProfiles { get; private set; }

        /// <summary>
        /// description of the color-stream
        /// set null if the color-stream isn't needed
        /// </summary>
        public RSStreamProfile? Color
        {
            get { return StreamProfiles[0]; }
            set
            {
                if (value == null)
                    StreamProfiles.Insert(0, null);
                else if (value.Value.StreamType == StreamType.STREAM_TYPE_COLOR)
                    StreamProfiles.Insert(0, value);
                else
                    throw new Exception("Streamtype isn't supported!");
            }
        }

        /// <summary>
        /// description of the depth-stream
        /// set null if the depth-stream isn't needed
        /// </summary>
        public RSStreamProfile? Depth
        {
            get { return StreamProfiles[1]; }
            set
            {
                if (value == null)
                    StreamProfiles.Insert(1, null);
                else if (value.Value.StreamType == StreamType.STREAM_TYPE_DEPTH)
                    StreamProfiles.Insert(1, value);
                else
                    throw new Exception("Streamtype isn't supported!");
            }
        }

        /// <summary>
        /// description of the ir-stream
        /// set null if the ir-stream isn't needed
        /// </summary>
        public RSStreamProfile? IR
        {
            get { return StreamProfiles[2]; }
            set
            {
                if (value == null)
                    StreamProfiles.Insert(2, null);
                else if (value.Value.StreamType == StreamType.STREAM_TYPE_IR)
                    StreamProfiles.Insert(2, value);
                else
                    throw new Exception("Streamtype isn't supported!");
            }
        }

        /// <summary>
        /// description of the left-stream
        /// set null if the left-stream isn't needed
        /// </summary>
        public RSStreamProfile? Left
        {
            get { return StreamProfiles[3]; }
            set
            {
                if (value == null)
                    StreamProfiles.Insert(3, null);
                else if (value.Value.StreamType == StreamType.STREAM_TYPE_LEFT)
                    StreamProfiles.Insert(3, value);
                else
                    throw new Exception("Streamtype isn't supported!");
            }
        }

        /// <summary>
        /// description of the right-stream
        /// set null if the right-stream isn't needed
        /// </summary>
        public RSStreamProfile? Right
        {
            get { return StreamProfiles[4]; }
            set
            {
                if (value == null)
                    StreamProfiles.Insert(4, null);
                else if (value.Value.StreamType == StreamType.STREAM_TYPE_RIGHT)
                    StreamProfiles.Insert(4, value);
                else
                    throw new Exception("Streamtype isn't supported!");
            }
        }

        #endregion

        #region constructor

        /// <summary>
        /// a set of stream-profiles
        /// </summary>
        /// <param name="color"> sets the color-stream or null if the stream isn't needed </param>
        /// <param name="depth"> sets the depth-stream or null if the stream isn't needed </param>
        /// <param name="ir"> sets the ir-stream or null if the stream isn't needed </param>
        /// <param name="left"> sets the left-stream or null if the stream isn't needed </param>
        /// <param name="right"> sets the right-stream or null if the stream isn't needed </param>
        public RSStreamProfileSet(RSStreamProfile? color = null, RSStreamProfile? depth = null, RSStreamProfile? ir = null, RSStreamProfile? left = null, RSStreamProfile? right = null)
        {
            StreamProfiles = new List<RSStreamProfile?>(5);

            Color = color;
            Depth = depth;
            IR = ir;
            Left = left;
            Right = right;
        }

        #endregion

        #region member

        /// <summary>
        /// sets one stream description
        /// </summary>
        /// <param name="profile"> stream description </param>
        public void SetStreamProfile(RSStreamProfile profile)
        {
            switch (profile.StreamType)
            {
                case StreamType.STREAM_TYPE_COLOR:
                    StreamProfiles.Insert(0, profile);
                    break;
                case StreamType.STREAM_TYPE_DEPTH:
                    StreamProfiles.Insert(1, profile);
                    break;
                case StreamType.STREAM_TYPE_IR:
                    StreamProfiles.Insert(2, profile);
                    break;
                case StreamType.STREAM_TYPE_LEFT:
                    StreamProfiles.Insert(3, profile);
                    break;
                case StreamType.STREAM_TYPE_RIGHT:
                    StreamProfiles.Insert(4, profile);
                    break;
                default:
                    throw new Exception("Streamtype isn't supported!");
            }
        }

        /// <summary>
        /// creates and sets one stream description
        /// </summary>
        /// <param name="streamType"> type of stream like color or depth </param>
        /// <param name="size"> resolution of the stream </param>
        /// <param name="frameRate"> framerate of the capture </param>
        /// <param name="streamOption"> various stream-options </param>
        public void SetStreamProfile(StreamType streamType, PXCMSizeI32 size, int frameRate = 30, StreamOption streamOption = StreamOption.STREAM_OPTION_ANY)
        {
            var streamProfile = new RSStreamProfile(size, frameRate, streamType, streamOption);
            SetStreamProfile(streamProfile);
        }

        #endregion

    }
}