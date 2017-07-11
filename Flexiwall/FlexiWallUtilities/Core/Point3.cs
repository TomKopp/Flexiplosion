
namespace FlexiWallUtilities.Core
{
    public class Point3
    {
        public float X, Y, Z;

        public Point3(float x = 0, float y = 0, float z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public virtual void Set(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public virtual void Set(Point3 point)
        {
            X = point.X;
            Y = point.Y;
            Z = point.Z;
        }

        public override string ToString() => $"X: {X} ; Y: {Y} ; Z: {Z}";

    }
}
