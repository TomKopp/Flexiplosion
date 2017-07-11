using System;
using System.Globalization;
using System.Windows.Data;
using FlexiWallUtilities.Core;

namespace FlexiWallUI.Converter
{
    [ValueConversion(typeof(Vector3), typeof(float), ParameterType = typeof(string))]
    public class TransformPropertyConverter : IValueConverter
    {
        private Vector3 _value;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var point = value as Vector3;
            var param = parameter as string;

            if (point == null || string.IsNullOrWhiteSpace(param))
                return -1;

            _value = point;

            if (Equals(param, "X"))            
                return point.X;

            if (Equals(param, "Y"))
                return point.Y;

            if (Equals(param, "Z"))
                return point.Z;

            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = System.Convert.ToSingle((double)value);
            var param = parameter as string;

            if (string.IsNullOrWhiteSpace(param))
                return -1;


            if (Equals(param, "X"))
                _value.X = val;

            if (Equals(param, "Y"))
                _value.Y = val;

            if (Equals(param, "Z"))
                _value.Z = val;

            return _value;
        }
    }
}
