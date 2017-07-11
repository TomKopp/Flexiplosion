using FlexiWallUtilities.Core;
using System;

namespace FlexiWallCalibration.Models
{
    [Serializable]
    public class Rectangle3
    {

        #region properties

        public double Width { get; set; }
        public double Height { get; set; }
        public SerializableVector3 Translate { get; set; }
        public SerializableVector3 Rotate { get; set; }

        #endregion

        #region constructor

        public Rectangle3()
        {
            Width = 0;
            Height = 0;
            Translate = new SerializableVector3();
            Rotate = new SerializableVector3();
        }

        public Rectangle3(float width, float height, Vector3 translate, Vector3 rotate)
        {
            Width = width;
            Height = height;
            Translate = translate as SerializableVector3;
            Rotate = rotate as SerializableVector3;
        }

        #endregion

        #region member

        public override string ToString() => $"Width: {Width} ; Height: {Height} ; Translate: {Translate.ToString()} ; Rotate: {Rotate.ToString()}";

        #endregion
    }
}
