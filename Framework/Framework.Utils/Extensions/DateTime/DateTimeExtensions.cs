namespace Framework.Utils.Extensions.DateTime
{
    using System;
    using System.Globalization;

    public static class DateTimeExtensions
    {
        public static string ToStandardDateString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static string ToStandardDateTimeString(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd hh:mm:ss tt", CultureInfo.InvariantCulture);
        }
    }
}