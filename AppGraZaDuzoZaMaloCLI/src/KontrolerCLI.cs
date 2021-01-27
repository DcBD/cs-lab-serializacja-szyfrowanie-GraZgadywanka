using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GraZaDuzoZaMalo;
using GraZaDuzoZaMalo.Model;

using static GraZaDuzoZaMalo.Model.Gra.Odpowiedz;

namespace AppGraZaDuzoZaMaloCLI
{

    public class KontrolerCLI
    {
        public const char ZNAK_ZAKONCZENIA_GRY = 'X';
        public const string SAVE_FILENAME = @"./ZaDuzoZaMalo.xml";

        private Gra gra;



        private WidokCLI widok;


        public int MinZakres { get; private set; } = 1;

        public int MaxZakres { get; private set; } = 100;

        private bool IsSaveFile => File.Exists(SAVE_FILENAME);

        private Thread AutoZapisThread { get; set; }

        public IReadOnlyList<Gra.Ruch> ListaRuchow
        {

            get
            { return gra.ListaRuchow; }
        }

        public KontrolerCLI()
        {

            gra = new Gra();


            widok = new WidokCLI(this);
        }

        public void Uruchom()
        {
            widok.OpisGry();
            while (widok.ChceszKontynuowac("Czy chcesz kontynuować aplikację (t/n)? "))
                UruchomRozgrywke();
        }

        public void ZapiszRozgrywke()
        {
         
            //BinarySerialization.SerializeToFile<Gra>(gra, SAVE_FILENAME);
            DataContractSerialization.SerializeToFile<Gra>(gra, SAVE_FILENAME);
        }


        private Gra ZapisanaRozgrywka()
        {
            try
            {
                //return BinarySerialization.DeserializeFromFile<Gra>(SAVE_FILENAME);
                return DataContractSerialization.DeserializeFromFile<Gra>(SAVE_FILENAME);

            }
            catch (Exception e)
            {
                return null;
            }
        }

        private void WczytajRozgrywke()
        {
            Gra savedGame = ZapisanaRozgrywka();

            if (savedGame != null)
            {
                gra = savedGame;

                widok.KomunikatWczytajZapis();
                widok.HistoriaGry();

                if (!widok.ChceszKontynuowac("Czy chcesz kontynuować rozgrywkę od poprzedniego stanu (t/n)? "))
                {
                    UsunZapis();
                    RozpocznijNowaRozgrywke();
                }
            }

        }

        private void AutoZapis()
        {
            while(gra.StatusGry == Gra.Status.WTrakcie)
            {
                Thread.Sleep(10000);
          
                ZapiszRozgrywke();
            }

        }

        private void RozpocznijAutoZapis()
        {
            AutoZapisThread = new Thread(new ThreadStart(AutoZapis));

            AutoZapisThread.Start();
        } 

        private void UsunZapis()
        {
            if (IsSaveFile)
            {
                File.Delete(SAVE_FILENAME);
            }
        }



        private void RozpocznijNowaRozgrywke()
        {
            gra = new Gra(MinZakres, MaxZakres);
        }

        public void UruchomRozgrywke()
        {
            widok.CzyscEkran();
        
            // Sprobuj wczytac rozgrywke lub rozpocznij nowa.
            if (IsSaveFile)
                { WczytajRozgrywke(); } 
            else 
                { RozpocznijNowaRozgrywke(); };


            RozpocznijAutoZapis();

            do
            {
                //wczytaj propozycję
                int propozycja = 0;
                try
                {
                    propozycja = widok.WczytajPropozycje();
                }
                catch (KoniecGryException)
                {
                    gra.Przerwij();
                    ZakonczGre();
                }


                Console.WriteLine(propozycja);



                if (gra.StatusGry == Gra.Status.Poddana) break;

                //Console.WriteLine( gra.Ocena(propozycja) );
                //oceń propozycję, break
                switch (gra.Ocena(propozycja))
                {
                    case ZaDuzo:
                        widok.KomunikatZaDuzo();
                        break;
                    case ZaMalo:
                        widok.KomunikatZaMalo();
                        break;
                    case Trafiony:
                        widok.KomunikatTrafiono();
                        break;
                    default:
                        break;
                }
                widok.HistoriaGry();
            }
            while (gra.StatusGry == Gra.Status.WTrakcie);

            //if StatusGry == Przerwana wypisz poprawną odpowiedź
            //if StatusGry == Zakończona wypisz statystyki gry
        }

        ///////////////////////

        public void UstawZakresDoLosowania(ref int min, ref int max)
        {

        }

        public int LiczbaProb() => gra.ListaRuchow.Count();

        public void ZakonczGre()
        {
            //np. zapisuje stan gry na dysku w celu późniejszego załadowania
            //albo dopisuje wynik do Top Score
            //sprząta pamięć

            ZapiszRozgrywke();

            gra = null;
            widok.CzyscEkran(); //komunikat o końcu gry
            widok = null;
            System.Environment.Exit(0);
        }

        public void ZakonczRozgrywke()
        {
            gra.Przerwij();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <exception cref="KoniecGryException"></exception>
        /// <exception cref="FormatException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <returns></returns>
        public int WczytajLiczbeLubKoniec(string value, int defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            value = value.TrimStart().ToUpper();
            if (value.Length > 0 && value[0].Equals(ZNAK_ZAKONCZENIA_GRY))
                throw new KoniecGryException();

            //UWAGA: ponizej może zostać zgłoszony wyjątek 
            return Int32.Parse(value);
        }
    }

    [Serializable]
    internal class KoniecGryException : Exception
    {
        public KoniecGryException()
        {
        }

        public KoniecGryException(string message) : base(message)
        {
        }

        public KoniecGryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected KoniecGryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
