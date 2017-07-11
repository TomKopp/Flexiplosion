namespace FlexiWallUtilities.Core
{
    /// <summary>
    /// structured pointcloud like a grid of Point3
    /// </summary>
    /// <typeparam name="T"> 3D-point-type </typeparam>
    public interface IPointGrid<T> : IPointCloud<T> where T : Point3
    {
        /// <summary>
        /// returns the number of columns
        /// </summary>
        /// <returns> number of columns </returns>
        int Width { get; }

        /// <summary>
        /// returns the number of lines
        /// </summary>
        /// <returns> number of lines </returns>
        int Height { get; }

        /// <summary>
        /// returns the value at position x,y from the collection of PointType-Objects
        /// </summary>
        /// <param name="x"> index of x dimension </param>
        /// <param name="y"> index of y dimension </param>
        /// <returns> value at position x,y from the collection of PointType-Objects </returns>
        T GetValue(int x, int y);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void Update(T source, int x, int y);
    }
}
