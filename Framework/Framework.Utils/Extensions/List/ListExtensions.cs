namespace Framework.Utils.Extensions.List
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Linq;

    public static class ListExtensions
    {
        public static XElement ToXml<T>(this List<T> list)
        {
            var t = typeof(List<T>);

            var extraTypes = list.Select(obj => obj.GetType()).Distinct().ToList();
            extraTypes.Add(typeof(T));

            var serializer = new DataContractSerializer(t, extraTypes);
            var sw = new StringWriter();
            var xw = new XmlTextWriter(sw);
            serializer.WriteObject(xw, list);
            return XElement.Parse(sw.ToString());
        }
    }
}