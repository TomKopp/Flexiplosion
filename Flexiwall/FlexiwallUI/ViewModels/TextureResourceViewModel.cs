using FlexiWallUI.Models;
using Prism.Mvvm;

namespace FlexiWallUI.ViewModels
{
    public abstract class TextureResourceViewModel : BindableBase
    {
        #region Fields

        protected readonly ITextureResource TextureResource;
        protected readonly string FormatString;

        #endregion

        #region Properties

        public string Title => TextureResource.Name;

        public bool ShowLegend => TextureResource.ShowLegend;

        #endregion

        #region Constructor

        protected TextureResourceViewModel(ITextureResource resource, string formatString)
        {
            TextureResource = resource;
            FormatString = formatString;

            RaisePropertyChanged(nameof(Title));
            RaisePropertyChanged(nameof(ShowLegend));
            
        }

        #endregion

        #region Abstract Methods

        public abstract void Load();

        public abstract void Unload();

        #endregion

        #region Auxiliary Methods

        protected string RetrievePath(string format)
        {
            var idx = TextureResource.Formats.Contains(format) ? TextureResource.Formats.IndexOf(format) : 0;

            var fn = TextureResource.Folder + "/" + TextureResource.Formats[idx] + "/";

            return fn;
        }

        #endregion
    }
}
