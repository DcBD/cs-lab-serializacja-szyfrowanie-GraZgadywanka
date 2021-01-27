using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace GraZaDuzoZaMalo
{
    public class DataContractSerialization
    {

        public static T DeserializeFromFile<T>(string fileName)
        {

            using Stream stream = new FileStream(fileName, FileMode.Open);

            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            return (T) serializer.ReadObject(reader, true);
        }

        public static void SerializeToFile<T>(T obj, string fileName)
        {

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            using Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write);

            serializer.WriteObject(stream, obj);
        }


    }
}
