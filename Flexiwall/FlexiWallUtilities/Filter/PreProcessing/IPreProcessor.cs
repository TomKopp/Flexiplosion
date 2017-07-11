using FlexiWallUtilities.Core;

namespace FlexiWallUtilities.Filter.PreProcessing
{
    /// <summary>
    /// processes input before updating the attached pointcloud
    /// </summary>
    public interface IPreProcessor<T> where T : Point3
    {
        /// <summary>
        /// attached pointcloud
        /// </summary>
        IPointCloud<T> Target { get; set; }

        /// <summary>
        /// filters or modify input
        /// </summary>
        /// <param name="source"> new input </param>
        /// <param name="index"> target position in pointcloud </param>
        /// <returns> modified or filtered input </returns>
        bool Process(float x, float y, float z, int index);
    }
}
