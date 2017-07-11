using System;
using System.Xml.Serialization;

namespace CommonClassesLib.Model
{
    [Serializable]
    public class LogEntry
    {
        [XmlAttribute]
        public LoggingLevel Level { get; set; }
        public String Message { get; set; }

        [XmlAttribute]
        public String Date { get; set; }

        public LogEntry()
        {
            Date = DateTime.Now.ToLongTimeString();
        }
    }
}
