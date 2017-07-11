using FlexiWallUI.ViewModels;
using FlexiWallUtilities.Core;
using SharpGL;
using SharpGL.SceneGraph;
using System.Windows;
using System.Windows.Media;

namespace FlexiWallUI.Views
{
    /// <summary>
    /// Interaktionslogik für GLDebugView.xaml
    /// </summary>
    public partial class OpenGLView : Window
    {
        #region fields

        private OpenGL _gl;
        private IPointCloud<Point3> _renderTarget;
        private Point3 _interaction;
        private OpenGLViewModel _viewModel;

        #endregion

        #region properties

        public IPointCloud<Point3> RenderTarget
        {
            get { return _renderTarget; }
            set { _renderTarget = value; }
        }

        public Point3 Interaction
        {
            get { return _interaction; }
            set { _interaction = value; }
        }

        #endregion

        #region constructor

        public OpenGLView()
        {
            InitializeComponent();
            _viewModel = DataContext as OpenGLViewModel;
            Interaction = new Point3();
        }

        #endregion

        #region member

        private void OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            if (RenderTarget == null)
                return;

            _gl = args.OpenGL;
            _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            _gl.LoadIdentity();
            _gl.Translate(_viewModel.Translate.X, _viewModel.Translate.Y, _viewModel.Translate.Z); // Bild in sichtbaren Bereich setzen
            _gl.Rotate(_viewModel.Rotate.X, _viewModel.Rotate.Y, _viewModel.Rotate.Z);

            #region render flexiwall

            _gl.Begin(OpenGL.GL_POINTS);

            // FlexiWall
            for (int i = 0; i < RenderTarget.Size; i++)
            {
                Point3 p = RenderTarget[i];
                _gl.Color(1f, 1f, 1f);
                _gl.Vertex(p.X * _viewModel.Scale.X, p.Y * _viewModel.Scale.Y, p.Z * _viewModel.Scale.Z);
            }

            _gl.End();

            #endregion

            #region interaction

            _gl.Begin(OpenGL.GL_QUADS);

            // Interaktions-Punkt
            _gl.Color(1f, 0f, 0f);
            _gl.Vertex((Interaction.X + 12) * _viewModel.Scale.X, (Interaction.Y + 12) * _viewModel.Scale.Y, Interaction.Z * _viewModel.Scale.Z);
            _gl.Vertex((Interaction.X + 12) * _viewModel.Scale.X, (Interaction.Y - 12) * _viewModel.Scale.Y, Interaction.Z * _viewModel.Scale.Z);
            _gl.Vertex((Interaction.X - 12) * _viewModel.Scale.X, (Interaction.Y - 12) * _viewModel.Scale.Y, Interaction.Z * _viewModel.Scale.Z);
            _gl.Vertex((Interaction.X - 12) * _viewModel.Scale.X, (Interaction.Y + 12) * _viewModel.Scale.Y, Interaction.Z * _viewModel.Scale.Z);

            _gl.End();

            #endregion

            _gl.Flush();
        }

        #endregion

    }
}
