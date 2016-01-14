using System.IO;
using System.Xml.Serialization;

namespace Robotix
{
    /// <summary>
    /// Containing static methods for serialize object
    /// </summary>
    public sealed class SerializeXML
    {
        /// <summary>
        /// Serialize a object of type T to xml
        /// </summary>
        /// <typeparam name="T">Class with parameter-free constructor</typeparam>
        /// <param name="objectToSerialize">A object to serialize</param>
        /// <param name="xmlfile">Url to file were the xml will be saved</param>
        public static void SerializeToFile<T>(T objectToSerialize, string xmlfile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StreamWriter(xmlfile))
            {
                serializer.Serialize(writer, objectToSerialize);
            }
            
        }
        /// <summary>
        /// Deserialize a xml file to object of type T
        /// </summary>
        /// <typeparam name="T">Class with parameter-free constructor</typeparam>
        /// <param name="xmlfile">Url to file were the xml are saved</param>
        /// <returns>Object of type T</returns>
        public static T DeserializeFromFile<T>(string xmlfile)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            TextReader reader = new StreamReader(xmlfile);
            return (T)deserializer.Deserialize(reader);
        }
        /// <summary>
        /// Deserialize a object of type T to xml
        /// </summary>
        /// <typeparam name="T">Class with parameter-free constructor</typeparam>
        /// <param name="toDeserialize">A object to deserialize</param>
        /// <returns></returns>
        public static T Deserialize<T>(string toDeserialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            object data;
            using (StringReader textReader = new StringReader(toDeserialize))
            {
                data = xmlSerializer.Deserialize(textReader);
            }
            return (T)data;
        }
        /// <summary>
        /// Serialize a object of type T to xml
        /// </summary>
        /// <typeparam name="T">Class with parameter-free constructor</typeparam>
        /// <param name="toSerialize">A object to serialize</param>
        /// <returns></returns>
        public static string Serialize<T>(T toSerialize)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                StringWriter textWriter = new StringWriter();
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}
