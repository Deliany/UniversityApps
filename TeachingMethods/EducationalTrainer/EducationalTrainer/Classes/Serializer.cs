using System.Xml.Serialization;

namespace EducationalTrainer.Classes
{
    using System.IO;

    public class Serializer
    {
        public static T Deserialize<T>(string fileName)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(fileName))
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }
    }
}
