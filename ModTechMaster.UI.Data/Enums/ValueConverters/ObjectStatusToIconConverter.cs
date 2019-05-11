namespace ModTechMaster.UI.Data.Enums.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(ObjectStatus), typeof(string))]
    public class ObjectStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((ObjectStatus) value)
            {
                case ObjectStatus.Nominal:
                    return "./Resources/Images/checked.png";
                case ObjectStatus.Warning:
                    return "./Resources/Images/exclamation-mark.png";
                case ObjectStatus.Error:
                    return "./Resources/Images/cancel.png";
            }

            return "./Resources/Images/cancel.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}