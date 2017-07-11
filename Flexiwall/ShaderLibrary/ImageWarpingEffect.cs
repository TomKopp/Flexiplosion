using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace ShaderLibrary
{
    public class ImageWarpingEffect : ShaderEffect
    {
        private static readonly PixelShader Shader = new PixelShader
        {
            UriSource = new Uri(@"pack://application:,,,/ShaderLibrary;component/Shader/Compiled/ImageWarpingEffect.ps")
        };

        public static readonly DependencyProperty FlexiInputProperty = RegisterPixelShaderSamplerProperty("FlexiInput", typeof(ImageWarpingEffect), 0);
        public static readonly DependencyProperty KinectInputProperty = RegisterPixelShaderSamplerProperty("KinectInput", typeof(ImageWarpingEffect), 1);

        public static readonly DependencyProperty Warp1Property = DependencyProperty.Register("Warp1", typeof(Point4D), typeof(ImageWarpingEffect), new UIPropertyMetadata(new Point4D(0.0f, 0.0f, 0.0f, 0.0f), PixelShaderConstantCallback(0), CoercePoint4dValue));
        public static readonly DependencyProperty Warp2Property = DependencyProperty.Register("Warp2", typeof(Point4D), typeof(ImageWarpingEffect), new UIPropertyMetadata(new Point4D(0.0f, 0.0f, 0.0f, 0.0f), PixelShaderConstantCallback(1), CoercePoint4dValue));
        public static readonly DependencyProperty Warp3Property = DependencyProperty.Register("Warp3", typeof(Point4D), typeof(ImageWarpingEffect), new UIPropertyMetadata(new Point4D(0.0f, 0.0f, 0.0f, 0.0f), PixelShaderConstantCallback(2), CoercePoint4dValue));

        public Brush FlexiInput
        {
            get { return (Brush)GetValue(FlexiInputProperty); }
            set { SetValue(FlexiInputProperty, value); }
        }
        
        public Brush KinectInput
        {
            get { return (Brush)GetValue(KinectInputProperty); }
            set { SetValue(KinectInputProperty, value); }
        }

        public Point4D Warp1
        {
            get { return (Point4D)GetValue(Warp1Property); }
            set { SetValue(Warp1Property, value); }
        }

        public Point4D Warp2
        {
            get { return (Point4D)GetValue(Warp2Property); }
            set { SetValue(Warp2Property, value); }
        }

        public Point4D Warp3
        {
            get { return (Point4D)GetValue(Warp3Property); }
            set { SetValue(Warp3Property, value); }
        }

        public ImageWarpingEffect()
        {
            PixelShader = Shader;

            UpdateShaderValue(KinectInputProperty);
            UpdateShaderValue(FlexiInputProperty);

            UpdateShaderValue(Warp1Property);
            UpdateShaderValue(Warp2Property);
            UpdateShaderValue(Warp3Property);
        }

        private static object CoercePoint4dValue(DependencyObject d, object value)
        {
            var effect = d as ImageWarpingEffect;
            if (effect == null)
                return null;

            var newFactor = (Point4D)value;

            return newFactor;
        }
    }
}
