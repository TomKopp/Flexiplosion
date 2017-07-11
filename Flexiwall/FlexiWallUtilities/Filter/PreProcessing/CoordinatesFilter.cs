using System;
using FlexiWallUtilities.Core;

namespace FlexiWallUtilities.Filter.PreProcessing
{
    public class CoordinatesFilter<T> : IPreProcessor<T> where T : Point3
    {
        private IPointCloud<T> _pointcloud;
        private float _x, _y, _z;

        public IPointCloud<T> Target
        {
            get { return _pointcloud; }
            set { _pointcloud = value; }
        }

        public CoordinatesFilter(float x, float y, float z)
        {
            _x = x;
            _y = y;
            _z = z;
        }

        public bool Process(float x, float y, float z, int index)
        {
            return _x == x && _y == y && _z == z ? false : true;
        }
    }
}
