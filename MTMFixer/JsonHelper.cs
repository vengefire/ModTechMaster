namespace MTMFixer
{
    using System.IO;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal static class JsonHelper
    {
        internal static JObject DeserializeJson(string filePath)
        {
            var rgx = new Regex(@"(\]|\}|""|[A-Za-z0-9])\s*\n\s*(\[|\{|"")", RegexOptions.Singleline);
            var commasAdded = rgx.Replace(File.ReadAllText(filePath), "$1,\n$2");
            var jsonObject = (JObject)JsonConvert.DeserializeObject(commasAdded);
            return jsonObject;
        }
    }
}