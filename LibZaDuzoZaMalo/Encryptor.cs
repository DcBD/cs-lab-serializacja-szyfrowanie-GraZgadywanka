using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;

namespace GraZaDuzoZaMalo
{
    public class Encryptor
    {

        public static XmlDocument Encrypt(XmlDocument doc, List<string> elementsToEnccrypt, SymmetricAlgorithm key)
        {

            EncryptedXml eXml = new EncryptedXml();

            foreach (string elementName in elementsToEnccrypt)
            {
                XmlElement elementToEncrypt = doc.GetElementsByTagName(elementName)[0] as XmlElement;

                if(elementToEncrypt == null)
                {
                    throw new XmlException("The specified element was not found");
                }



                byte[] encryptedElement = eXml.EncryptData(elementToEncrypt, key, false);

                EncryptedData edElement = new EncryptedData();
                edElement.Type = EncryptedXml.XmlEncElementUrl;

                edElement.EncryptionMethod = new EncryptionMethod(EncryptedXml.XmlEncAES256Url);

                edElement.CipherData.CipherValue = encryptedElement;

                EncryptedXml.ReplaceElement(elementToEncrypt, edElement, false);

                
            }

            return doc;
        }

        

        public static Aes GetKey()
        {
            var key = new Rfc2898DeriveBytes("testpw", new byte[32], 50000);
            Aes AES = Aes.Create();
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
     

            return AES;
        }

        public static void Decrypt(XmlDocument Doc, SymmetricAlgorithm Alg)
        {
            XmlElement encryptedElement = Doc.GetElementsByTagName("EncryptedData")[0] as XmlElement;

            // If the EncryptedData element was not found, throw an exception.
            if (encryptedElement == null)
            {
                throw new XmlException("The EncryptedData element was not found.");
            }

            // Create an EncryptedData object and populate it.
            EncryptedData edElement = new EncryptedData();
            edElement.LoadXml(encryptedElement);

            // Create a new EncryptedXml object.
            EncryptedXml exml = new EncryptedXml();

            // Decrypt the element using the symmetric key.
            byte[] rgbOutput = exml.DecryptData(edElement, Alg);

            // Replace the encryptedData element with the plaintext XML element.
            exml.ReplaceData(encryptedElement, rgbOutput);

        }
        

    }
}
