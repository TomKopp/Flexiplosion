using FlexiWallUtilities.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlexiWallUtilities.Filter.PostProcessing
{
    public class FastestBoxBlur<T> : IPostProcessor<T> where T : Point3
    {

        private IPointGrid<T> _pointgrid;
        private int _width, _size;
        private int _radius, _lowerBound, _upperBound;
        private float[] _boxes;

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

        public int Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                _lowerBound = _radius * _pointgrid.Width;
            }
        }

        public FastestBoxBlur(float sigma, int numberOfBoxes)
        {
            _boxes = calculateBoxes(sigma, numberOfBoxes);
        }

        private float[] calculateBoxes(float sigma, int n)
        {
            var wIdeal = Math.Sqrt((12 * sigma * sigma / n) + 1);
            var wl = Math.Floor(wIdeal); if (wl % 2 == 0) wl--;
            var wu = wl + 2;

            var mIdeal = (12 * sigma * sigma - n * wl * wl - 4 * n * wl - 3 * n) / (-4 * wl - 4);
            var m = Math.Round(mIdeal);

            var sizes = new float[n];
            for (var i = 0; i < n; i++)
                sizes[i] = (float)(i < m ? wl : wu);

            return sizes;
        }

        public void Process(int index)
        {
            throw new NotImplementedException();
        }
    }
}
