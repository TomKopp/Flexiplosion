using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FlexiWallUI.Models;

namespace FlexiWallUI.ViewModels
{
    public class LayeredTextureResourceViewModel : TextureResourceViewModel
    {
        public List<ImageSource> Textures { get; }

        public LayeredTextureResourceViewModel(ITextureResource resource, string formatString)
            : base(resource, formatString)  
        {
            Textures = new List<ImageSource>();
            RaisePropertyChanged(nameof(Textures));
        }

        public override void Load()
        {
            string path = RetrievePath(FormatString);

            // TODO: Load should be done elsewhere --> are DDS textures loaded this way ? are they of type ImageSource ?
            TextureResource.Textures.ForEach(img =>
            {
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnDemand;
                bmp.UriSource = new Uri(path + img, UriKind.Relative);
                bmp.EndInit();
                bmp.Freeze();
                Textures.Add(bmp);
            });
        }

        public override void Unload()
        {
            Textures.Clear();
        }
    }
}
