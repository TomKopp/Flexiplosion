using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace FlexiWallUI.Models
{
    public class XmlTextureResourceAccess<T> : ITextureAccess<T> where T: class
    {
        public T Load(string connectionString)
        {
            var ser = new XmlSerializer(typeof(T));
            if (!File.Exists(connectionString))
            {
                string className = GetType().FullName;
                string methodName = new StackTrace().GetFrame(1).GetMethod().Name;

                throw new ArgumentException("Error loading Textures from File. File " + connectionString +
                                            "does not exist. In " + className + "." + methodName + ".");
            }
            var reader = XmlReader.Create(connectionString);
            return ser.Deserialize(reader) as T;
        }

        public void Save(string connectionString, T repository)
        {
            var ser = new XmlSerializer(typeof (LayeredTextureRepository));
            var writer = XmlWriter.Create(connectionString);
            
            ser.Serialize(writer, repository);
        }
    }
}
