namespace Framework.Utils.Enums
{
    using System.Collections.Generic;

    public static class EnumHelper
    {
        public static T? MapStringToEnum<T>(string val, Dictionary<string, T> dict) where T : struct
        {
            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            return dict[val];
        }
    }
}