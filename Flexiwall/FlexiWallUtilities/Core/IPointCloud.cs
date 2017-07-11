
namespace FlexiWallUtilities.Core
{
    /// <summary>
    /// unstructured pointcloud like a simple list of Point3
    /// </summary>
    /// <typeparam name="T"> 3D-point-type </typeparam>
    public interface IPointCloud<T> where T : Point3
    {
        T this[int index] { get; set; }

        /// <summary>
        /// returns the size of Poin3-array
        /// </summary>
        /// <returns> size of Point3-array </returns>
        int Size { get; }

        /// <summary>
        /// returns the value at position index from the collection of PointType-Objects
        /// </summary>
        /// <param name="index"></param>
        /// <returns> value at position index from the collection of PointType-Objects </returns>
        T GetValue(int index);

        /// <summary>
        /// updates an array of Point3 with the information of source
        /// </summary>
        /// <param name="source"> 3D-point </param>
        void Update(T source, int index);
    }
}
