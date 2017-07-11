namespace FlexiWallUI.Models
{
    public class EffectProperties
    {
        public bool ShowDepth { get; set; }
        public float ClampDepthMin { get; set; }
        public float ClampDepthMax { get; set; }
        public float BlurRadius { get; set; }
        public bool InterpolateDepthLayers { get; set; }
    }
}
