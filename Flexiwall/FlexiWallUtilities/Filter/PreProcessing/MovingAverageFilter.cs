using FlexiWallUtilities.Core;
using System;

namespace FlexiWallUtilities.Filter.PreProcessing
{
    public class MovingAverageFilter<T> : IPreProcessor<T> where T : Point3
    {
        private float _maxStride;
        private IPointGrid<T> _pointgrid;
        private int _lowerBound, _upperBound, _size, _width;

        public IPointCloud<T> Target
        {
            get { return _pointgrid; }
            set
            {
                _pointgrid = value as IPointGrid<T>;
                _size = _pointgrid.Size;
                _width = _pointgrid.Width;
                _lowerBound = _width + 1;
                _upperBound = _size - _lowerBound;
            }
        }

        public MovingAverageFilter(float maxStride)
        {
            _maxStride = maxStride;
        }

        public bool Process(float x, float y, float z, int index)
        {
            if (index < _lowerBound || index > _upperBound)
                return false;

            var zAverage = (
                _pointgrid[index - 1].Z
                + _pointgrid[index + 1].Z
                + _pointgrid[index + _width].Z
                + _pointgrid[index - _width].Z) / 4;

            return Math.Abs(zAverage - _pointgrid[index].Z) >= _maxStride ? false : true;
        }
    }
}
