namespace Launcher.Common
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;
            if (parameter != null)
            {
                Boolean.TryParse(parameter.ToString(), out result);
            }

            if (value != null && (value is Boolean))
            {
                if (System.Convert.ToBoolean(value) == result)
                {
                    return Visibility.Visible;
                }
                else return Visibility.Collapsed;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
