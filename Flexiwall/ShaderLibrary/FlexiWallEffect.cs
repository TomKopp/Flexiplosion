using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace ShaderLibrary
{
    public class FlexiWallEffect : ShaderEffect
    {
        #region Fields

        private static readonly PixelShader Shader = new PixelShader
        {
            UriSource = new Uri(@"pack://application:,,,/ShaderLibrary;component/Shader/Compiled/FlexiWallEffect.ps")
        };

        #endregion

        #region DependencyProperties

        // Textures for layers
        public static readonly DependencyProperty Layer01Property = RegisterPixelShaderSamplerProperty("Layer01", typeof(FlexiWallEffect), 1);
        public static readonly DependencyProperty Layer02Property = RegisterPixelShaderSamplerProperty("Layer02", typeof(FlexiWallEffect), 2);
        public static readonly DependencyProperty Layer03Property = RegisterPixelShaderSamplerProperty("Layer03", typeof(FlexiWallEffect), 3);
        public static readonly DependencyProperty Layer04Property = RegisterPixelShaderSamplerProperty("Layer04", typeof(FlexiWallEffect), 4);
        public static readonly DependencyProperty Layer05Property = RegisterPixelShaderSamplerProperty("Layer05", typeof(FlexiWallEffect), 5);
        public static readonly DependencyProperty Layer06Property = RegisterPixelShaderSamplerProperty("Layer06", typeof(FlexiWallEffect), 6);
        public static readonly DependencyProperty Layer07Property = RegisterPixelShaderSamplerProperty("Layer07", typeof(FlexiWallEffect), 7);

        // depth map
        public static readonly DependencyProperty MaskProperty = RegisterPixelShaderSamplerProperty("Mask", typeof(FlexiWallEffect), 0);

        public static readonly DependencyProperty ShowDepthProperty = DependencyProperty.Register("ShowDepth", typeof(float), typeof(FlexiWallEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(0), CoerceBooleanValue));
        public static readonly DependencyProperty MinDepthProperty = DependencyProperty.Register("MinDepth", typeof(float), typeof(FlexiWallEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(1), CoerceDepthClipping));
        public static readonly DependencyProperty MaxDepthProperty = DependencyProperty.Register("MaxDepth", typeof(float), typeof(FlexiWallEffect), new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(2), CoerceDepthClipping));

        public static readonly DependencyProperty TextureWidthProperty = DependencyProperty.Register("TextureWidth", typeof(float), typeof(FlexiWallEffect), new UIPropertyMetadata(1024.0f, PixelShaderConstantCallback(3), CoerceTextureDimension));
        public static readonly DependencyProperty TextureHeightProperty = DependencyProperty.Register("TextureHeight", typeof(float), typeof(FlexiWallEffect), new UIPropertyMetadata(1024.0f, PixelShaderConstantCallback(4), CoerceTextureDimension));

        public static readonly DependencyProperty BlurRadiusProperty = DependencyProperty.Register("BlurRadius", typeof(float), typeof(FlexiWallEffect), new UIPropertyMetadata(0.0f, PixelShaderConstantCallback(5), CoerceBlurRadius));

        public static readonly DependencyProperty InterpolateColorProperty = DependencyProperty.Register("Interpolate", typeof(float), typeof(FlexiWallEffect), new UIPropertyMetadata(1.0f, PixelShaderConstantCallback(6), CoerceBooleanValue));

        #endregion

        #region Attached Properties

        public Brush Layer01
        {
            get { return (Brush)GetValue(Layer01Property); }
            set { SetValue(Layer01Property, value); }
        }

        public Brush Layer02
        {
            get { return (Brush)GetValue(Layer02Property); }
            set { SetValue(Layer02Property, value); }
        }

        public Brush Layer03
        {
            get { return (Brush)GetValue(Layer03Property); }
            set { SetValue(Layer03Property, value); }
        }

        public Brush Layer04
        {
            get { return (Brush)GetValue(Layer04Property); }
            set { SetValue(Layer04Property, value); }
        }

        public Brush Layer05
        {
            get { return (Brush)GetValue(Layer05Property); }
            set { SetValue(Layer05Property, value); }
        }

        public Brush Layer06
        {
            get { return (Brush)GetValue(Layer06Property); }
            set { SetValue(Layer06Property, value); }
        }

        public Brush Layer07
        {
            get { return (Brush)GetValue(Layer07Property); }
            set { SetValue(Layer07Property, value); }
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

        #endregion

        #region Constructor

        public FlexiWallEffect()
        {
            PixelShader = Shader;

            UpdateShaderValue(MaskProperty);
            UpdateShaderValue(Layer01Property);
            UpdateShaderValue(Layer02Property);
            UpdateShaderValue(Layer03Property);
            UpdateShaderValue(Layer04Property);
            UpdateShaderValue(Layer05Property);
            UpdateShaderValue(Layer06Property);
            UpdateShaderValue(Layer07Property);
            UpdateShaderValue(ShowDepthProperty);

            UpdateShaderValue(MinDepthProperty);
            UpdateShaderValue(MaxDepthProperty);
            UpdateShaderValue(TextureWidthProperty);
            UpdateShaderValue(TextureHeightProperty);
            UpdateShaderValue(BlurRadiusProperty);
            UpdateShaderValue(InterpolateColorProperty);
        }

        #endregion

        #region Auxiliary Methods

        private static object CoerceBooleanValue(DependencyObject d, object value)
        {
            var effect = d as FlexiWallEffect;
            if (effect == null)
                return null;

            var newFactor = (float)value;

            return newFactor;
        }

        private static object CoerceTextureDimension(DependencyObject d, object value)
        {
            var effect = d as FlexiWallEffect;
            if (effect == null)
                return null;

            var newFactor = (float)value;
            if (newFactor < 256.0f)
                newFactor = 256.0f;

            return newFactor;
        }

        private static object CoerceBlurRadius(DependencyObject d, object value)
        {
            var effect = d as FlexiWallEffect;
            if (effect == null)
                return null;

            var newFactor = (float)value;
            newFactor = newFactor > 8.0f ? 8.0f : newFactor;
            newFactor = newFactor < 0.0f ? 0.0f : newFactor;

            return newFactor;
        }

        private static object CoerceDepthClipping(DependencyObject d, object value)
        {
            var effect = d as FlexiWallEffect;
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
