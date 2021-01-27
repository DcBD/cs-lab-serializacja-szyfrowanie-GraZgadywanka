using GraZaDuzoZaMalo.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace GraZaDuzoZaMalo
{
    public class BinarySerialization
    {

        public static T DeserializeFromFile<T>(string fileName)
        {

            using Stream stream = File.OpenRead(fileName);

            BinaryFormatter deserializer = new BinaryFormatter();

            T obj = (T) deserializer.Deserialize(stream);

            return obj;
        }

        public static void SerializeToFile<T>(T obj, string fileName)
        {

            IFormatter formatter = new BinaryFormatter();
            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                formatter.Serialize(stream, obj);
            }
        }

    }
}
