using System.Drawing;

namespace FlexiWallCalibration.Models
{
    public class Rectangle2
    {
        public PointF[] Corners { get; private set; }

        public Rectangle2(PointF[] corners)
        {
            Corners = corners;
        }

        /// <summary>
        /// Berechnet den Mittelpunkt der gemessenen Fläche aus vier Punkten
        /// </summary>
        /// <returns> Mittelpunkt der Fläche </returns>
        public PointF GetCenter()
        {
            var center1 = new PointF((Corners[1].X + Corners[3].X) / 2, (Corners[1].Y + Corners[3].Y) / 2);
            var center2 = new PointF((Corners[0].X + Corners[2].X) / 2, (Corners[0].Y + Corners[2].Y) / 2);

            return new PointF((center1.X + center2.X) / 2, (center1.Y + center2.Y) / 2);
        }
    }
}
