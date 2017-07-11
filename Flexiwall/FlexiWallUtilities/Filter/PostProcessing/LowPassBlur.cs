using FlexiWallUtilities.Core;
using FlexiWallUtilities.Filter.InputProcessing;

namespace FlexiWallUtilities.Filter.PostProcessing
{
    public class LowPassBlur<T> : IPostProcessor<T> where T : Point3
    {
        private IPointCloud<T> _pointcloud;
        private LowPassFilter _lowPassImpl;
        private float _alpha;

        public float Alpha
        {
            get { return _alpha; }
            set { _alpha = value; }
        }

        public IPointCloud<T> Target
        {
            get { return _pointcloud; }
            set
            {
                _pointcloud = value as IPointCloud<T>;
            }
        }

        public LowPassBlur(float alpha)
        {
            _lowPassImpl = new LowPassFilter();
            _alpha = alpha;
        }

        public void Process(int index)
        {
            var current = _pointcloud[index];
            current.Z = _lowPassImpl.Process(current.Z, _alpha);
        }
    }
}
