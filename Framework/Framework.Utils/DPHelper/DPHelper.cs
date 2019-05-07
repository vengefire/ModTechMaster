using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using Castle.DynamicProxy;

namespace Framework.Utils.DPHelper
{
    public static class DPHelper
    {
        public static string CreateInvocationCompleteString(IInvocation invocation)
        {
            var sb = new StringBuilder();
            if (null == invocation.ReturnValue)
            {
                return sb.AppendFormat("Returned null").ToString();
            }

            var retType = invocation.ReturnValue.GetType();
            if (retType == typeof(void))
            {
                sb.AppendFormat("Returned void.");
            }
            else if (retType == typeof(string))
            {
                sb.AppendFormat("Returned string [\"{0}\"].", invocation.ReturnValue);
            }
            else if (retType == typeof(char))
            {
                sb.AppendFormat("Returned char [\'{0}\'].", invocation.ReturnValue);
            }
            else if (retType.IsPrimitive || !retType.IsClass)
            {
                sb.AppendFormat("Returned {0} [\'{1}\'].", retType, invocation.ReturnValue);
            }
            else
            {
                sb.AppendFormat(
                    "Returned {0} = [{1}]",
                    retType,
                    CreateObjectLogString(invocation.ReturnValue));
            }

            return sb.ToString();
        }

        public static string CreateInvocationLogString(IInvocation invocation)
        {
            var sb = new StringBuilder(100);
            var targetTypeName = null != invocation.TargetType
                ? invocation.TargetType.Name
                : invocation.Method.DeclaringType.Name;
            sb.AppendFormat("{0}.{1}(", targetTypeName, invocation.Method.Name);
            foreach (var argument in invocation.Arguments)
            {
                var argumentDescription = argument == null ? "null" : DumpObject(argument);
                sb.Append(argumentDescription /*.Substring(0, Math.Min(100, argumentDescription.Length))*/).Append(",");
            }

            if (invocation.Arguments.Length > 0)
            {
                sb.Length--;
            }

            sb.Append(")");
            return sb.ToString();
        }

        public static string CreateObjectLogString(object obj)
        {
            string rawXml;
            using (var writer = new StringWriter())
            {
                using (XmlWriter xmlWriter = new XmlTextWriter(writer))
                {
                    var ser = new DataContractSerializer(obj.GetType(), null, int.MaxValue, false, true, null);
                    ser.WriteObject(xmlWriter, obj);
                    rawXml = writer.ToString();
                }
            }

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawXml);

            var curnode = xmlDoc.DocumentElement;
            SterilizeElement(curnode);

            return xmlDoc.InnerXml;
        }

        public static void SterilizeElement(XmlNode node)
        {
            if (null != node.Value && node.Value.Length > 100)
            {
                node.Value = "Value exceeds max length (100)";
            }

            if (node.NextSibling != null)
            {
                SterilizeElement(node.NextSibling);
            }

            foreach (var childNode in node.ChildNodes)
            {
                SterilizeElement((XmlNode) childNode);
            }
        }

        private static string DumpObject(object argument)
        {
            var objtype = argument.GetType();
            if (objtype == typeof(string) || objtype.IsPrimitive || !objtype.IsClass
                || objtype.BaseType == typeof(MulticastDelegate) || !objtype.IsSerializable
                || objtype.Namespace.Contains("Castle."))
            {
                if (objtype == typeof(string))
                {
                    return string.Format("\"{0}\"", argument);
                }
                if (objtype == typeof(char))
                {
                    return string.Format("\'{0}\'", argument);
                }
                return argument.ToString();
            }

            return CreateObjectLogString(argument);
        }
    }
}