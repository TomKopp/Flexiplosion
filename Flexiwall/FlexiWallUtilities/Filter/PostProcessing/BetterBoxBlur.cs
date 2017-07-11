using FlexiWallUtilities.Core;

namespace FlexiWallUtilities.Filter.PostProcessing
{
    public class BetterBoxBlur<T> : IPostProcessor<T> where T : Point3
    {
        private int _radius, _lowerBound, _upperBound, _width, _size;
        private IPointGrid<T> _pointgrid;

        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                _lowerBound = _radius * _pointgrid.Width;
            }
        }

        public IPointCloud<T> Target
        {
            get { return _pointgrid; }
            set
            {
                _pointgrid = value as IPointGrid<T>;
                _size = _pointgrid.Size;
                _width = _pointgrid.Width;
                _lowerBound = _radius * _width;
                _upperBound = _size - _lowerBound;
            }
        }

        public BetterBoxBlur(int radius)
        {
            _radius = radius;
        }

        public void Process(int index)
        {
            if (index < _lowerBound || index > _upperBound)
                return;

            // X-Richtung

            int count = 0;
            float sum = 0;

            for (int i = -_radius; i < _radius; i++)
            {
                sum += _pointgrid[index + i].Z;
                count++;
            }

            // Y-Richtung

            for (int i = -_radius; i < _radius; i++)
            {
                sum += _pointgrid[index + i * _width].Z;
                count++;
            }

            _pointgrid[index].Z = sum / count;
        }
    }
}