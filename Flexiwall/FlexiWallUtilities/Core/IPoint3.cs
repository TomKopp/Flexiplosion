namespace FlexiWallUtilities.Core
{
    [System.Obsolete("Use Point3 instead!")]
    public interface IPoint3
    {
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }

        void Set(float x, float y, float z);
    }
}
