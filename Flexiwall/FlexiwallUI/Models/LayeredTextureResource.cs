using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace FlexiWallUI.Models
{
    [Serializable]
    public class LayeredTextureResource : ITextureResource
    {
        [XmlArray(IsNullable = false)]
        public List<String> Formats { get; set; }

        public List<String> Textures { get; set; }

        [XmlAttribute]
        public String Name { get; set; }

        [XmlAttribute]
        public String Folder { get; set; }

        [XmlAttribute]
        public bool ShowLegend { get; set; }
    }
}
