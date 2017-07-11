using Microsoft.Win32;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FlexiWallCalibration.Utilities
{
    /// <summary>
    /// functions to serialize an deserialize objects and files
    /// </summary>
    public class SerialUtil
    {
        public static string FileName = "";

        /// <summary>
        /// save a serialized object to a file
        /// </summary>
        /// <typeparam name="T"> type of object to serialze </typeparam>
        /// <param name="serializableObject"> object to serialize </param>
        public static void SerializeFile<T>(T serializableObject)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "xml file|*.xml";

            if (saveFileDialog.ShowDialog() == true)
            {
                FileStream fileStream = (FileStream)saveFileDialog.OpenFile();
                SerializeObject<T>(serializableObject, fileStream);
            }
        }

        /// <summary>
        /// load a file an try to deserialize it
        /// </summary>
        /// <typeparam name="T"> type of object to deserialize </typeparam>
        /// <returns> deserialized object </returns>
        public static T DeSerializeFile<T>()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "xml file|*.xml";

            if (openFileDialog.ShowDialog() == true && openFileDialog.CheckFileExists)
            {
                FileName = openFileDialog.FileName;
                FileStream fileStream = (FileStream)openFileDialog.OpenFile();
                return DeSerializeObject<T>(fileStream);
            }

            return default(T);
        }

        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        public static void SerializeObject<T>(T serializableObject, FileStream fileStream)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    serializer.Serialize(memoryStream, serializableObject);
                    memoryStream.Position = 0;
                    xmlDocument.Load(memoryStream);
                    xmlDocument.Save(fileStream);
                    memoryStream.Close();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }


        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        public static T DeSerializeObject<T>(FileStream fileStream)
        {
            if (fileStream == null) { return default(T); }

            T objectOut = default(T);

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileStream);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return objectOut;
        }
    }
}
