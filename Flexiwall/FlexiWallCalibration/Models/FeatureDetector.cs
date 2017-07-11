using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System.Drawing;

namespace FlexiWallCalibration.Models
{
    public class FeatureDetector
    {
        private Mat _mask;
        private Mat _homography;
        private VectorOfVectorOfDMatch _matches;
        private VectorOfKeyPoint _modelKeyPoints;
        private VectorOfKeyPoint _observedKeyPoints;

        public PointF[] FindImage(Mat modelImage, Mat observedImage)
        {
            FindMatch(modelImage, observedImage);

            if (_homography == null) return null;

            Size size = modelImage.Size;
            PointF[] corners = new PointF[]
            {
                new PointF(0,0), // links - unten
                new PointF(size.Width, 0), // rechts - unten
                new PointF(size.Width, size.Height), // rechts - oben
                new PointF(0, size.Height) // links - oben
            };
            corners = CvInvoke.PerspectiveTransform(corners, _homography);

            return corners;
        }

        private void FindMatch(Mat modelImage, Mat observedImage)
        {
            int k = 2;
            double uniquenessThreshold = 0.80;

            _homography = null;
            _matches = new VectorOfVectorOfDMatch();
            _modelKeyPoints = new VectorOfKeyPoint();
            _observedKeyPoints = new VectorOfKeyPoint();

            using (UMat uModelImage = modelImage.ToUMat(AccessType.Read))
            using (UMat uObservedImage = observedImage.ToUMat(AccessType.Read))
            {
                KAZE featureDetector = new KAZE(false, false);

                //extract features from the object image
                Mat modelDescriptors = new Mat();
                featureDetector.DetectAndCompute(uModelImage, null, _modelKeyPoints, modelDescriptors, false);

                // extract features from the observed image
                Mat observedDescriptors = new Mat();
                featureDetector.DetectAndCompute(uObservedImage, null, _observedKeyPoints, observedDescriptors, false);
                BFMatcher matcher = new BFMatcher(DistanceType.L2);

                matcher.Add(modelDescriptors);

                matcher.KnnMatch(observedDescriptors, _matches, k, null);
                _mask = new Mat(_matches.Size, 1, DepthType.Cv8U, 1);
                _mask.SetTo(new MCvScalar(255));
                Features2DToolbox.VoteForUniqueness(_matches, uniquenessThreshold, _mask);

                int nonZeroCount = CvInvoke.CountNonZero(_mask);
                if (nonZeroCount >= 4)
                {
                    nonZeroCount = Features2DToolbox.VoteForSizeAndOrientation(_modelKeyPoints, _observedKeyPoints,
                       _matches, _mask, 1.5, 20);
                    if (nonZeroCount >= 4)
                        _homography = Features2DToolbox.GetHomographyMatrixFromMatchedFeatures(_modelKeyPoints,
                           _observedKeyPoints, _matches, _mask, 2);
                }
            }
        }
    }
}
