namespace Framework.Utils.Extensions.Dictionary
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using Newtonsoft.Json;

    public static class DictionaryExtensionMethods
    {
        /// <summary>
        ///     Safely add contextual data to exception data dictionary.
        /// </summary>
        /// <param name="dictionary">Target dictionary.</param>
        /// <param name="key">Dictionary key.</param>
        /// <param name="value">Contextual value to reference.</param>
        public static void SafeAdd(this IDictionary dictionary, string key, object value)
        {
            if (value == null)
            {
                value = "[NULL]";
            }
            else if (!(value.GetType().IsPrimitive || value is string))
            {
                try
                {
                    var sb = new StringBuilder();
                    var settings = new JsonSerializerSettings {Formatting = Formatting.Indented};
                    var serializer = JsonSerializer.Create(settings);
                    using (var stringWriter = new StringWriter(sb))
                    {
                        serializer.Serialize(stringWriter, value);
                    }

                    value = sb.ToString();
                }
                catch (Exception ex)
                {
                    value = string.Format("[Serialization failed: {0}]", ex.Message);
                }
            }

            if (dictionary.Contains(key))
            {
                dictionary[key] += Environment.NewLine + "---" + Environment.NewLine + value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}