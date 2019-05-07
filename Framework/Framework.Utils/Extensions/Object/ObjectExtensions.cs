namespace Framework.Utils.Extensions.Object
{
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Linq;

    public static class ObjectExtensions
    {
        public static XElement ToXml(this object o)
        {
            var t = o.GetType();

            var extraTypes = t.GetProperties()
                              .Where(p => p.PropertyType.IsInterface)
                              .Select(p => p.GetValue(o, null).GetType())
                              .ToArray();

            var serializer = new DataContractSerializer(t, extraTypes);
            var sw = new StringWriter();
            var xw = new XmlTextWriter(sw);
            serializer.WriteObject(xw, o);
            return XElement.Parse(sw.ToString());
        }
    }
}