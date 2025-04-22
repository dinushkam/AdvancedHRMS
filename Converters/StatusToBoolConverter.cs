using System;
using System.Globalization;
using System.Windows.Data;

namespace AdvancedHRMS.Converters
{
    public class StatusToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Compare the current status with the parameter
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // For two-way binding, convert back if needed
            return (bool)value ? parameter : Binding.DoNothing;
        }
    }
}