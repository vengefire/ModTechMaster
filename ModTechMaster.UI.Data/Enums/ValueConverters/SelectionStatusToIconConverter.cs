namespace ModTechMaster.UI.Data.Enums.ValueConverters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(SelectionStatus), typeof(string))]
    public class SelectionStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((SelectionStatus)value)
            {
                case SelectionStatus.Unselected:
                    return "/ModTechMaster.UI.Core;component/Resources/Images/trash.png";
                case SelectionStatus.PartiallySelected:
                    return "/ModTechMaster.UI.Core;component/Resources/Images/minus.png";
                case SelectionStatus.Selected:
                    return "/ModTechMaster.UI.Core;component/Resources/Images/plus.png";
            }

            return "./Resources/Images/cancel.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}