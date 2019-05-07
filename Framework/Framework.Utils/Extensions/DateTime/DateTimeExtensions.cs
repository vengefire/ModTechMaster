using System.Globalization;

namespace Framework.Utils.Extensions.DateTime
{
    public static class DateTimeExtensions
    {
        public static string ToStandardDateString(this System.DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static string ToStandardDateTimeString(this System.DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
        }
    }
}