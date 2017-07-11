using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ShaderLibrary
{
    public class ZoomEffect : ShaderEffect
    {
        #region Fields

        private static readonly PixelShader Shader = new PixelShader
        {
            UriSource = new Uri(@"pack://application:,,,/ShaderLibrary;component/Shader/Compiled/ZoomEffect.ps")
        };

        #endregion

        #region DependencyProperties

        // image
        public static readonly DependencyProperty Image_LayerProperty = RegisterPixelShaderSamplerProperty("Image_Layer", typeof(ZoomEffect), 1);

        // depth map
        public static readonly DependencyProperty MaskProperty = RegisterPixelShaderSamplerProperty("Mask", typeof(ZoomEffect), 0);

        public static readonly DependencyProperty ShowDepthProperty = DependencyProperty.Register("ShowDepth", typeof(float), typeof(ZoomEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0), CoerceBooleanValue));
        public static readonly DependencyProperty MinDepthProperty = DependencyProperty.Register("MinDepth", typeof(float), typeof(ZoomEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(1), CoerceDepthClipping));
        public static readonly DependencyProperty MaxDepthProperty = DependencyProperty.Register("MaxDepth", typeof(float), typeof(ZoomEffect), new UIPropertyMetadata(0.5f, PixelShaderConstantCallback(2), CoerceDepthClipping));

        public static readonly DependencyProperty TextureWidthProperty = DependencyProperty.Register("TextureWidth", typeof(float), typeof(ZoomEffect), new UIPropertyMetadata(1024.0f, PixelShaderConstantCallback(3), CoerceTextureDimension));
        public static readonly DependencyProperty TextureHeightProperty = DependencyProperty.Register("TextureHeight", typeof(float), typeof(ZoomEffect), new UIPropertyMetadata(1024.0f, PixelShaderConstantCallback(4), CoerceTextureDimension));

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register("BlurRadius", typeof(float), typeof(ZoomEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(5), CoerceBlurRadius));

        public static readonly DependencyProperty InterpolateColorProperty = DependencyProperty.Register("Interpolate", typeof(float), typeof(ZoomEffect), new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(6), CoerceBooleanValue));

        public static readonly DependencyProperty lensCenterXProperty = DependencyProperty.Register("LensCenterX", typeof(float), typeof(ZoomEffect), new UIPropertyMetadata(960f, PixelShaderConstantCallback(7), CoerceBooleanValue));
        public static readonly DependencyProperty lensCenterYProperty = DependencyProperty.Register("LensCenterY", typeof(float), typeof(ZoomEffect), new UIPropertyMetadata(540f, PixelShaderConstantCallback(8), CoerceBooleanValue));

        #endregion

        #region Attached Properties

        public Brush Image_Layer
        {
            get { return (Brush)GetValue(Image_LayerProperty); }
            set { SetValue(Image_LayerProperty, value); }
        }

        public Brush Mask
        {
            get { return (Brush)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        public float ShowDepth
        {
            get { return (float)GetValue(ShowDepthProperty); }
            set { SetValue(ShowDepthProperty, value); }
        }

        public float MinDepth
        {
            get { return (float)GetValue(MinDepthProperty); }
            set { SetValue(MinDepthProperty, value); }
        }

        public float MaxDepth
        {
            get { return (float)GetValue(MaxDepthProperty); }
            set { SetValue(MaxDepthProperty, value); }
        }

        public float TextureWidth
        {
            get { return (float)GetValue(TextureWidthProperty); }
            set { SetValue(TextureWidthProperty, value); }
        }

        public float TextureHeight
        {
            get { return (float)GetValue(TextureHeightProperty); }
            set { SetValue(TextureHeightProperty, value); }
        }


        public float BlurRadius
        {
            get { return (float)GetValue(BlurRadiusProperty); }
            set { SetValue(BlurRadiusProperty, value); }
        }

        public float Interpolate
        {
            get { return (float)GetValue(InterpolateColorProperty); }
            set { SetValue(InterpolateColorProperty, value); }
        }

        public float LensCenterX
        {
            get
            {
                return (float)GetValue(lensCenterXProperty);
            }
            set
            {
                SetValue(lensCenterXProperty, value);
            }
        }

        public float LensCenterY
        {
            get { return (float)GetValue(lensCenterYProperty);}
            set
            {
                SetValue(lensCenterYProperty, value);
            }
        }

        #endregion

        #region Constructor

        public ZoomEffect()
        {
            PixelShader = Shader;

            UpdateShaderValue(MaskProperty);
            UpdateShaderValue(Image_LayerProperty);
            UpdateShaderValue(ShowDepthProperty);

            UpdateShaderValue(MinDepthProperty);
            UpdateShaderValue(MaxDepthProperty);
            UpdateShaderValue(TextureWidthProperty);
            UpdateShaderValue(TextureHeightProperty);
            UpdateShaderValue(BlurRadiusProperty);
            UpdateShaderValue(InterpolateColorProperty);
            UpdateShaderValue(lensCenterXProperty);
            UpdateShaderValue(lensCenterYProperty);
        }

        #endregion

        #region Auxiliary Methods

        private static object CoerceBooleanValue(DependencyObject d, object value)
        {
            var effect = d as ZoomEffect;
            if (effect == null)
                return null;

            var newFactor = (float)value;

            return newFactor;
        }

        private static object CoerceTextureDimension(DependencyObject d, object value)
        {
            var effect = d as ZoomEffect;
            if (effect == null)
                return null;

            var newFactor = (float)value;
            if (newFactor < 256.0f)
                newFactor = 256.0f;

            return newFactor;
        }

        private static object CoerceBlurRadius(DependencyObject d, object value)
        {
            var effect = d as ZoomEffect;
            if (effect == null)
                return null;

            var newFactor = (float)value;
            newFactor = newFactor > 8.0f ? 8.0f : newFactor;
            newFactor = newFactor < 0.0f ? 0.0f : newFactor;

            return newFactor;
        }

        private static object CoerceDepthClipping(DependencyObject d, object value)
        {
            var effect = d as ZoomEffect;
            if (effect == null)
                return null;

            var newFactor = (float)value;
            newFactor = newFactor > 1.0f ? 1.0f : newFactor;
            newFactor = newFactor < 0.0f ? 0.0f : newFactor;

            return newFactor;
        }

        #endregion
    }
}
