using FlexiWallUtilities.Core;

namespace FlexiWallUtilities.Filter.PostProcessing
{
    /// <summary>
    /// modify data in attached pointcloud after update
    /// </summary>
    /// <typeparam name="T"> 3D point type </typeparam>
    public interface IPostProcessor<T> where T : Point3
    {
        /// <summary>
        /// attached pointcloud
        /// </summary>
        IPointCloud<T> Target { get; set; }

        /// <summary>
        /// modify existing data in attached pointcloud
        /// </summary>
        /// <param name="index"> target position in attached pointcloud </param>
        void Process(int index);
    }
}
