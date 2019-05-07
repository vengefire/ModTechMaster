namespace Framework.Utils.Extensions.Enums
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    public static class EnumExtensions
    {
        public static TEnumType EnumValueFromString<TEnumType>(this TEnumType value, string val)
        {
            return (TEnumType)Enum.Parse(typeof(TEnumType), val);
        }

        public static T GetAttribute<T>(this Enum value) where T : Attribute
        {
            var info = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            if (info == null)
            {
                return null;
            }

            var attribute = (T)info.GetCustomAttributes(typeof(T), false).FirstOrDefault();
            return attribute;
        }

        public static string Description(this Enum value)
        {
            return value.GetAttribute<DescriptionAttribute>().Description;
        }
    }
}