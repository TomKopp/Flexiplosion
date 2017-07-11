using FlexiWallUtilities.Core;
using Prism.Mvvm;

namespace FlexiWallUI.ViewModels
{
    public class OpenGLViewModel : BindableBase
    {
        private Vector3 _translate;
        private Vector3 _rotate;
        private Vector3 _scale;

        public Vector3 Translate
        {
            get { return _translate; }
            set { SetProperty(ref _translate, value); }
        }

        public Vector3 Rotate
        {
            get { return _rotate; }
            set { SetProperty(ref _rotate, value); }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set { SetProperty(ref _scale, value); }
        }

        public OpenGLViewModel()
        {
            Translate = new Vector3(0, 0, -30);
            Rotate = new Vector3();
            Scale = new Vector3(0.01f, 0.01f, 0.01f);
        }
    }
}
