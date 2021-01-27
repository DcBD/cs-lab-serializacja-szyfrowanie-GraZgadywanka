using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace GraZaDuzoZaMalo
{
    public class DataContractSerialization
    {

        public static T DeserializeFromFile<T>(string fileName)
        {

            Aes key = Encryptor.GetKey();
 
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(fileName);

            Encryptor.Decrypt(xmlDoc, key);

            byte[] byteArray = Encoding.ASCII.GetBytes(xmlDoc.InnerXml);
            MemoryStream stream = new MemoryStream(byteArray);
            stream.Position = 0;

            XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, new XmlDictionaryReaderQuotas());
            
            stream.Flush();
         

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            return (T) serializer.ReadObject(reader, true);
        }

        public static void SerializeToFile<T>(T obj, string fileName)
        {

            DataContractSerializer serializer = new DataContractSerializer(typeof(T));

            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {

                serializer.WriteObject(stream, obj);

            }


            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true;
            xmlDoc.Load(fileName);
            

            List<string> elementsToEncrypt = new List<string>() { "liczbaDoOdgadniecia" };
            Aes key = Encryptor.GetKey();

            XmlDocument xml = Encryptor.Encrypt(xmlDoc, elementsToEncrypt, key);

            using (Stream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {

                xml.Save(stream);

            }
            
            
        }


    }
}
