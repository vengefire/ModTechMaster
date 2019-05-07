using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Framework.Utils.XML
{
    public class XmlHelper
    {
        public static T Deserialize<T>(string xmlString, Encoding encoding = null) where T : class
        {
            if (null == encoding)
            {
                encoding = new UTF8Encoding(false);
            }

            using (var stream = new MemoryStream(encoding.GetBytes(xmlString)))
            {
                return Deserialize<T>(stream);
            }
        }

        public static T Deserialize<T>(Stream stream) where T : class
        {
            stream.Seek(0, SeekOrigin.Begin);
            var xmlSerializer = new XmlSerializer(typeof(T));
            var returnObject = xmlSerializer.Deserialize(stream) as T;
            return returnObject;
        }

        public static T DataContractDeserialize<T>(string xml, Encoding encoding = null) where T : class
        {
            encoding = encoding ?? new UTF8Encoding();
            using (var memoryStream = new MemoryStream(encoding.GetBytes(xml)))
            {
                return DataContractDeserialize<T>(memoryStream);
            }
        }

        public static object DataContractDeserialize(Type typeInfo, string xml, Encoding encoding = null)
        {
            encoding = encoding ?? new UTF8Encoding();
            using (var memoryStream = new MemoryStream(encoding.GetBytes(xml)))
            {
                return DataContractDeserialize(typeInfo, memoryStream);
            }
        }

        public static T DataContractDeserialize<T>(Stream stream) where T : class
        {
            stream.Seek(0, SeekOrigin.Begin);
            var serializer = new DataContractSerializer(typeof(T));
            var returnObject = serializer.ReadObject(stream) as T;
            return returnObject;
        }

        public static object DataContractDeserialize(Type typeInfo, Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var serializer = new DataContractSerializer(typeInfo);
            var returnObject = serializer.ReadObject(stream);
            return returnObject;
        }

        public static T Deserialize<T>(Stream stream, string rootNode) where T : class
        {
            stream.Seek(0, SeekOrigin.Begin);
            var xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootNode));
            var returnObject = xmlSerializer.Deserialize(stream) as T;
            return returnObject;
        }

        public static T DeserializewithValidation<T>(
            Stream stream,
            Stream[] schemaStreams,
            out string[] warnings,
            out string[] errors) where T : class
        {
            Validate(stream, schemaStreams, out warnings, out errors);

            return errors.Any() ? null : Deserialize<T>(stream);
        }

        public static T DataContractDeserializewithValidation<T>(
            Stream stream,
            Stream[] schemaStreams,
            out string[] warnings,
            out string[] errors) where T : class
        {
            Validate(stream, schemaStreams, out warnings, out errors);

            return errors.Any() ? null : DataContractDeserialize<T>(stream);
        }

        public static MemoryStream Serialize(Type typeInfo, object classInstance, Encoding encoding = null)
        {
            encoding = encoding ?? new UTF8Encoding(false);
            var xmlSerializer = new XmlSerializer(typeInfo);
            using (var memStream = new MemoryStream())
            {
                using (var setParmsXml = new XmlTextWriter(memStream, encoding))
                {
                    setParmsXml.Formatting = Formatting.Indented;
                    xmlSerializer.Serialize(setParmsXml, classInstance);
                    return new MemoryStream(memStream.ToArray());
                }
            }
        }

        public static MemoryStream Serialize<T>(T xmlClassInstance, Encoding encoding = null) where T : class
        {
            encoding = encoding ?? new UTF8Encoding(false);
            var xmlSerializer = new XmlSerializer(typeof(T));
            using (var memStream = new MemoryStream())
            {
                using (var setParmsXml = new XmlTextWriter(memStream, encoding))
                {
                    setParmsXml.Formatting = Formatting.Indented;
                    xmlSerializer.Serialize(setParmsXml, xmlClassInstance);
                    return new MemoryStream(memStream.ToArray());
                }
            }
        }

        public static MemoryStream DataContractSerialize<T>(T xmlClassInstance, Encoding encoding = null)
            where T : class
        {
            var serializer = new DataContractSerializer(typeof(T));
            using (var memStream = new MemoryStream())
            {
                if (null == encoding)
                {
                    encoding = new UTF8Encoding(false);
                }

                using (var setParmsXml = new XmlTextWriter(memStream, encoding))
                {
                    setParmsXml.Formatting = Formatting.Indented;
                    serializer.WriteObject(setParmsXml, xmlClassInstance);
                    setParmsXml.Flush();

                    return new MemoryStream(memStream.ToArray());
                }
            }
        }

        public static MemoryStream DataContractSerialize(Type typeInfo, object typeInstance, Encoding encoding = null)
        {
            var serializer = new DataContractSerializer(typeInfo);
            using (var memStream = new MemoryStream())
            {
                if (null == encoding)
                {
                    encoding = new UTF8Encoding(false);
                }

                using (var setParmsXml = new XmlTextWriter(memStream, encoding))
                {
                    setParmsXml.Formatting = Formatting.Indented;
                    serializer.WriteObject(setParmsXml, typeInstance);
                    setParmsXml.Flush();

                    return new MemoryStream(memStream.ToArray());
                }
            }
        }

        public static void Validate(Stream stream, Stream[] schemaStreams, out string[] warnings, out string[] errors)
        {
            var warningsList = new List<string>();
            var errorsList = new List<string>();

            var schemaSet = new XmlSchemaSet();
            foreach (var schemaStream in schemaStreams)
            {
                schemaStream.Seek(0, SeekOrigin.Begin);
                schemaSet.Add(XmlSchema.Read(schemaStream, (sender, args) => { }));
                schemaStream.Seek(0, SeekOrigin.Begin);
            }

            var xmlReaderSettings = new XmlReaderSettings();
            xmlReaderSettings.ValidationType = ValidationType.Schema;
            xmlReaderSettings.ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings;
            xmlReaderSettings.Schemas = schemaSet;
            xmlReaderSettings.ValidationEventHandler += (sender, args) =>
            {
                switch (args.Severity)
                {
                    case XmlSeverityType.Warning:
                        warningsList.Add(args.Message);
                        break;
                    case XmlSeverityType.Error:
                        errorsList.Add(args.Message);
                        break;
                    default:
                        throw new InvalidProgramException("Unknown Xml Severity argument value detected.");
                }
            };

            var xmlReader = XmlReader.Create(stream, xmlReaderSettings);
            while (xmlReader.Read())
            {
            }

            warnings = warningsList.ToArray();
            errors = errorsList.ToArray();
            stream.Seek(0, SeekOrigin.Begin);
        }
    }
}