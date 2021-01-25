using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using GraZaDuzoZaMalo.Model;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace AppGraZaDuzoZaMaloCLI
{
    class Program
    {

        static void Main(string[] args)
        {


            if (File.Exists(KontrolerCLI.SAVE_FILENAME))
            {
                Console.WriteLine("Wczytywanie rozgrywki z pliku binarnego :)");
                
                WczytajRozgrywke().Uruchom();
            }
            else
            {
                (new KontrolerCLI()).Uruchom();

            }



        }


        public static KontrolerCLI WczytajRozgrywke()
        {
            using Stream stream = File.OpenRead(KontrolerCLI.SAVE_FILENAME);

            BinaryFormatter deserializer = new BinaryFormatter();

            KontrolerCLI kontroler = (KontrolerCLI)deserializer.Deserialize(stream);

            return kontroler;
        }



    }
}
