using CommonClassesLib;
using System;

namespace FlexiWallUtilities.Core
{
    /// <summary>
    /// interface for various depth-cameras
    /// </summary>
    /// <typeparam name="T"> parameter-type for setup </typeparam>
    public interface ICamera<T> : IDisposable
    {
        /// <summary>
        /// returns state of camera-device
        /// </summary>
        SensorState State { get; }

        /// <summary>
        /// Initialize camera with some parameter
        /// </summary>
        /// <param name="parameter"> parameter for setup </param>
        void Initialize(T parameter);

        /// <summary>
        /// starts the stream
        /// </summary>
        void Start();

        /// <summary>
        /// stopps the stream
        /// </summary>
        void Stop();
    }
}
