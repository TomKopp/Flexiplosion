using System;
using System.Collections.Generic;

namespace FlexiWallUI.Models
{
    [Serializable]
    public class LayeredTextureRepository : ITextureRepository<LayeredTextureResource>
    {
        public List<LayeredTextureResource> TextureResources { get; set; }
    }
}
