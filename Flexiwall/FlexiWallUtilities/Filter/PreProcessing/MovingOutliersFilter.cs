using FlexiWallUtilities.Core;
using System;

namespace FlexiWallUtilities.Filter.PreProcessing
{
    public class MovingOutliersFilter<T> : IPreProcessor<T> where T : Point3
    {
        private float _maxStride;
        private IPointCloud<T> _pointcloud;

        public IPointCloud<T> Target
        {
            get { return _pointcloud; }
            set { _pointcloud = value; }
        }

        public MovingOutliersFilter(float maxStride)
        {
            _maxStride = maxStride;
        }

        public bool Process(float x, float y, float z, int index) => Math.Abs(z - _pointcloud[index].Z) < _maxStride;
    }
}
