using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace YMM_ToneCurve.View.Converter
{
    class EqualToVisibilityConverter : IValueConverter
    {
        public bool Inverted { get; set; }

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var result = false;
            if (value != null && parameter != null)
            {
                result = value.Equals(parameter);
            }
            else
            {
                result = value == parameter;
            }

            result = Inverted ? !result : result;

            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
