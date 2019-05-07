namespace Framework.Utils.Extensions.String
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Castle.Core.Internal;

    public static class StringExtensions
    {
        public static string StripWhiteSpace(this string theString)
        {
            return !theString.IsNullOrEmpty()
                ? new string(theString.ToCharArray().Where(c => !char.IsWhiteSpace(c)).ToArray())
                : theString;
        }

        public static string StripChars(this string theString, List<char> stripChars)
        {
            return !theString.IsNullOrEmpty()
                ? new string(theString.ToCharArray().Where(c => !stripChars.Contains(c)).ToArray())
                : theString;
        }

        /// <summary>
        ///     Constructs a result DateTime from a string with the format "1900-01-01"
        /// </summary>
        /// <param name="theString">A string in the format of "1900-01-01" (yyyy-MM-dd).</param>
        /// <returns>An instance of DateTime constructed from the input string.</returns>
        public static DateTime DateStringToToDateTime(this string theString)
        {
            return DateTime.ParseExact(theString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public static DateTime CompactDateStringToToDateTime(this string theString)
        {
            return DateTime.ParseExact(theString, "yyyyMMdd", CultureInfo.InvariantCulture);
        }

        public static bool IsAlphaNumeric(this string strToCheck)
        {
            var rg = new Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        public static bool IsNumeric(this string strToCheck)
        {
            var rg = new Regex(@"^[0-9\s,]*$");
            return rg.IsMatch(strToCheck);
        }

        public static bool IsAlpha(this string strToCheck)
        {
            var rg = new Regex(@"^[a-zA-Z\s,]*$");
            return rg.IsMatch(strToCheck);
        }
    }
}