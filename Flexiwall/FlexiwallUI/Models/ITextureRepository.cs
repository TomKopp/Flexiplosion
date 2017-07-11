using System.Collections.Generic;

namespace FlexiWallUI.Models
{
    public interface ITextureRepository<T> where T: ITextureResource
    {
        List<T> TextureResources { get; set; }
    }
}