namespace FlexiWallUtilities.Core
{
    public class Point3History : Point3
    {

        private Point3[] _history;

        public Point3 Reset;

        public Point3History(int length) : base()
        {
            _history = new Point3[length];
            for (int i = 0; i < length; i++)
                _history[i] = new Point3();

            Reset = new Point3();
        }

        public Point3History(int length, float x, float y, float z) : base(x, y, z)
        {
            _history = new Point3[length];
            for (int i = 0; i < length; i++)
                _history[i] = new Point3();

            Reset = new Point3();
        }

        public override void Set(float x, float y, float z)
        {
            for (int i = 1; i < _history.Length; i++)
            {
                var desc = _history[i];
                _history[i - 1].Set(desc.X, desc.Y, desc.Z);
            }

            _history[_history.Length - 1].Set(x, y, z);

            int count = 0;
            float sumX = 0;
            float sumY = 0;
            float sumZ = 0;

            for (int i = 0; i < _history.Length; i++)
            {
                if (_history[i].Z == 0) continue;

                sumX += _history[i].X;
                sumY += _history[i].Y;
                sumZ += _history[i].Z;
                count++;
            }

            // Wenn Punkt zulange an einer Position verweilt wird er zurückgesetzt
            /*
            if (_history[0].Z == _history[_history.Length - 1].Z)
                base.Set(Reset);
            else
            */
            base.Set(sumX / count, sumY / count, sumZ / count);
        }

        public override void Set(Point3 point)
        {
            Set(point.X, point.Y, point.Z);
        }
    }

}
