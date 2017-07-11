using System;

namespace FlexiWallUtilities.Core
{
    public class Vector3 : Point3
    {

        public Vector3(float x = 0, float y = 0, float z = 0) : base(x, y, z) { }

        public float Length
        {
            get
            {
                return (float)Math.Abs(Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2) + Math.Pow(Z, 2)));
            }
        }

        public static Vector3 Divide(Vector3 vector, float scalar)
        {
            return new Vector3(vector.X / scalar, vector.Y / scalar, vector.Z / scalar);
        }

        public void Normalize()
        {
            Set(Divide(this, Length));
        }

        public Point3 ToPoint3()
        {
            return this as Point3;
        }
    }
}
