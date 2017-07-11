using System;
using System.Collections.Generic;

namespace FlexiWallUI.Models
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITextureResource
    {
        List<String> Formats { get; }

        List<String> Textures { get; }

        String Name { get; set; }

        String Folder { get; set; }

        bool ShowLegend { get; set; }
    }
}
