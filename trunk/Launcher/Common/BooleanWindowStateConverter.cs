namespace Launcher.Common
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    public class BooleanWindowStateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is Boolean))
            {
                throw new ArgumentException("", "value");
            }

            return ((bool)value) ? WindowState.Normal : WindowState.Minimized;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is WindowState))
            {
                throw new ArgumentException("", "value");
            }

            switch (((WindowState)value))
            {
                case WindowState.Maximized:
                case WindowState.Normal:
                    {
                        return true;
                    }
                default:
                    {
                        return false;
                    }
            }
        }
    }
}
