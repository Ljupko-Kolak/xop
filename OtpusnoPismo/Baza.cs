using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Obrazac_za_Otpusnice
{
    public class Lijecnik
    {
        public string imePrezime;
        public string titula1;
        public string titula2;
    }

    public class Odjel
    {
        public string naziv;
        public string broj;
        public string sef;
        public string titula1;
        public string titula2;

        public List<Lijecnik> lijecnici = new List<Lijecnik>();
    }

    public class Baza
    {
        public List<Odjel> odjeli = new List<Odjel>();
        public List<string> odjelniLijecnici = new List<string>();
        public List<string> imena = new List<string>();
        public List<string> prezimena = new List<string>();
        public List<string> gradovi = new List<string>();
        public List<string> ulice = new List<string>();
        public List<string> dijagnoze = new List<string>();
        public List<string> zahvati = new List<string>();

        public string path = "";


        public void UcitajOdjele()
        {
            XDocument doc = XDocument.Load(path);

            XElement bazaOdjela = doc.Descendants("odjeli").First();
            foreach (XElement odjel in bazaOdjela.Elements("odjel"))
            {
                Odjel o = new Odjel();

                XAttribute naziv = odjel.Attribute("naziv");
                o.naziv = naziv.Value;

                XAttribute broj = odjel.Attribute("broj");
                o.broj = broj.Value;

                XAttribute sef = odjel.Attribute("sef");
                o.sef = sef.Value;

                XAttribute t1 = odjel.Attribute("titula1");
                o.titula1 = t1.Value;

                XAttribute t2 = odjel.Attribute("titula2");
                o.titula2 = t2.Value;

                XElement lijecnici = odjel.Descendants("lijecnici").First();
                foreach (XElement lijecnik in lijecnici.Elements("lijecnik"))
                {
                    Lijecnik l = new Lijecnik();

                    l.imePrezime = lijecnik.Value;

                    XAttribute lt1 = lijecnik.Attribute("titula1");
                    l.titula1 = lt1.Value;

                    XAttribute lt2 = lijecnik.Attribute("titula2");
                    l.titula2 = lt2.Value;

                    o.lijecnici.Add(l);
                }

                odjeli.Add(o);
            }
        }

        public void UctajOdjelneLijecnike()
        {
            XDocument doc = XDocument.Load(path);

            XElement bazaOdjelnikLijecnika = doc.Descendants("odjelnilijecnici").First();
            foreach (XElement lijecnik in bazaOdjelnikLijecnika.Elements("lijecnik"))
            {
                odjelniLijecnici.Add(lijecnik.Value);
            }
        }

        public void UcitajImena()
        {
            XDocument doc = XDocument.Load(path);

            XElement bazaImena = doc.Descendants("imena").First();
            foreach (XElement ime in bazaImena.Elements("ime"))
            {
                imena.Add(ime.Value);
            }
        }

        public void UcitajPrezimena()
        {
            XDocument doc = XDocument.Load(path);

            XElement bazaPrezimena = doc.Descendants("prezimena").First();
            foreach (XElement prezime in bazaPrezimena.Elements("prezime"))
            {
                prezimena.Add(prezime.Value);
            }
        }

        public void UcitajGradove()
        {
            XDocument doc = XDocument.Load(path);

            XElement bazaGradova = doc.Descendants("gradovi").First();
            foreach (XElement grad in bazaGradova.Elements("grad"))
            {
                gradovi.Add(grad.Value);
            }
        }

        public void UcitajUlice()
        {
            XDocument doc = XDocument.Load(path);

            XElement bazaUlica = doc.Descendants("ulice").First();
            foreach (XElement ulica in bazaUlica.Elements("ulica"))
            {
                ulice.Add(ulica.Value);
            }
        }

        public void UcitajDijagnoze()
        {
            XDocument doc = XDocument.Load(path);

            XElement bazaDijagnoza = doc.Descendants("dijagnoze").First();
            foreach (XElement dijagnoza in bazaDijagnoza.Elements("dijagnoza"))
            {
                dijagnoze.Add(dijagnoza.Value);
            }
        }

        public void UcitajZahvate()
        {
            XDocument doc = XDocument.Load(path);

            XElement bazaZahvata = doc.Descendants("zahvati").First();
            foreach (XElement zahvat in bazaZahvata.Elements("zahvat"))
            {
                zahvati.Add(zahvat.Value);
            }
        }

        public void UcitajBazu()
        {
            XDocument doc = XDocument.Load(path);

            XElement bazaOdjela = doc.Descendants("odjeli").First();
            foreach (XElement odjel in bazaOdjela.Elements("odjel"))
            {
                Odjel o = new Odjel();

                XAttribute naziv = odjel.Attribute("naziv");
                o.naziv = naziv.Value;

                XAttribute broj = odjel.Attribute("broj");
                o.broj = broj.Value;

                XAttribute sef = odjel.Attribute("sef");
                o.sef = sef.Value;

                XAttribute t1 = odjel.Attribute("titula1");
                o.titula1 = t1.Value;

                XAttribute t2 = odjel.Attribute("titula2");
                o.titula2 = t2.Value;

                XElement lijecnici = odjel.Descendants("lijecnici").First();
                foreach (XElement lijecnik in lijecnici.Elements("lijecnik"))
                {
                    Lijecnik l = new Lijecnik();

                    l.imePrezime = lijecnik.Value;

                    XAttribute lt1 = lijecnik.Attribute("titula1");
                    l.titula1 = lt1.Value;

                    XAttribute lt2 = lijecnik.Attribute("titula2");
                    l.titula2 = lt2.Value;

                    o.lijecnici.Add(l);
                }

                odjeli.Add(o);
            }

            XElement bazaOdjelnihLijecnika = doc.Descendants("odjelnilijecnici").First();
            foreach (XElement lijecnik in bazaOdjelnihLijecnika.Elements("lijecnik"))
            {
                odjelniLijecnici.Add(lijecnik.Value);
            }

            XElement bazaImena = doc.Descendants("imena").First();
            foreach (XElement ime in bazaImena.Elements("ime"))
            {
                imena.Add(ime.Value);
            }

            XElement bazaPrezimena = doc.Descendants("prezimena").First();
            foreach (XElement prezime in bazaPrezimena.Elements("prezime"))
            {
                prezimena.Add(prezime.Value);
            }

            XElement bazaGradova = doc.Descendants("gradovi").First();
            foreach (XElement grad in bazaGradova.Elements("grad"))
            {
                gradovi.Add(grad.Value);
            }

            XElement bazaUlica = doc.Descendants("ulice").First();
            foreach (XElement ulica in bazaUlica.Elements("ulica"))
            {
                ulice.Add(ulica.Value);
            }

            XElement bazaDijagnoza = doc.Descendants("dijagnoze").First();
            foreach (XElement dijagnoza in bazaDijagnoza.Elements("dijagnoza"))
            {
                dijagnoze.Add(dijagnoza.Value);
            }

            XElement bazaZahvata = doc.Descendants("zahvati").First();
            foreach (XElement zahvat in bazaZahvata.Elements("zahvat"))
            {
                zahvati.Add(zahvat.Value);
            }
        }


        public void PohraniOdjelneLijecnike(Baza azuriranaBaza)
        {
            azuriranaBaza.odjelniLijecnici.Sort();

            XDocument doc = XDocument.Load(path);
            XElement bazaOdjelnihLijecnika = doc.Descendants("odjelnilijecnici").First();
            bazaOdjelnihLijecnika.Elements().Remove();
            foreach (string lijecnik in azuriranaBaza.odjelniLijecnici)
            {
                bazaOdjelnihLijecnika.Add(new XElement("lijecnik", lijecnik));
            }
        }

        public void PohraniImena(Baza azuriranaBaza)
        {
            azuriranaBaza.imena.Sort();

            XDocument doc = XDocument.Load(path);
            XElement bazaImena = doc.Descendants("imena").First();
            bazaImena.Elements().Remove();
            foreach (string ime in azuriranaBaza.imena)
            {
                bazaImena.Add(new XElement("ime", ime));
            }

            doc.Save(path);
        }

        public void PohraniPrezmena(Baza azuriranaBaza)
        {
            azuriranaBaza.prezimena.Sort();

            XDocument doc = XDocument.Load(path);
            XElement bazaPrezimena = doc.Descendants("prezimena").First();
            bazaPrezimena.Elements().Remove();
            foreach (string prezime in azuriranaBaza.prezimena)
            {
                bazaPrezimena.Add(new XElement("prezime", prezime));
            }

            doc.Save(path);
        }

        public void PohraniGradove(Baza azuriranaBaza)
        {
            azuriranaBaza.gradovi.Sort();

            XDocument doc = XDocument.Load(path);
            XElement bazaGradova = doc.Descendants("gradovi").First();
            bazaGradova.Elements().Remove();
            foreach (string grad in azuriranaBaza.gradovi)
            {
                bazaGradova.Add(new XElement("grad", grad));
            }

            doc.Save(path);
        }

        public void PohraniUlice(Baza azuriranaBaza)
        {
            azuriranaBaza.ulice.Sort();

            XDocument doc = XDocument.Load(path);
            XElement bazaUlica = doc.Descendants("ulice").First();
            bazaUlica.Elements().Remove();
            foreach (string ulica in azuriranaBaza.ulice)
            {
                bazaUlica.Add(new XElement("ulica", ulica));
            }

            doc.Save(path);
        }

        public void PohraniDijagnoze(Baza azuriranaBaza)
        {
            azuriranaBaza.dijagnoze.Sort();

            XDocument doc = XDocument.Load(path);
            XElement bazaDijagnoza = doc.Descendants("dijagnoze").First();
            bazaDijagnoza.Elements().Remove();
            foreach (string dijagnoza in azuriranaBaza.dijagnoze)
            {
                bazaDijagnoza.Add(new XElement("dijagnoza", dijagnoza));
            }

            doc.Save(path);
        }

        public void PohraniZahvate(Baza azuriranaBaza)
        {
            azuriranaBaza.zahvati.Sort();

            XDocument doc = XDocument.Load(path);
            XElement bazaZahvata = doc.Descendants("zahvati").First();
            bazaZahvata.Elements().Remove();
            foreach (string zahvat in azuriranaBaza.zahvati)
            {
                bazaZahvata.Add(new XElement("zahvat", zahvat));
            }

            doc.Save(path);
        }

        public void PohraniBazu(Baza azuriranaBaza)
        {
            XDocument doc = XDocument.Load(path);

            azuriranaBaza.odjelniLijecnici.Sort();
            XElement bazaOdjelnihLijecnika = doc.Descendants("odjelnilijecnici").First();
            bazaOdjelnihLijecnika.Elements().Remove();
            foreach (string lijecnik in azuriranaBaza.odjelniLijecnici)
            {
                bazaOdjelnihLijecnika.Add(new XElement("lijecnik", lijecnik));
            }

            azuriranaBaza.imena.Sort();
            XElement bazaImena = doc.Descendants("imena").First();
            bazaImena.Elements().Remove();
            foreach (string ime in azuriranaBaza.imena)
            {
                bazaImena.Add(new XElement("ime", ime));
            }

            azuriranaBaza.prezimena.Sort();
            XElement bazaPrezimena = doc.Descendants("prezimena").First();
            bazaPrezimena.Elements().Remove();
            foreach (string prezime in azuriranaBaza.prezimena)
            {
                bazaPrezimena.Add(new XElement("prezime", prezime));
            }

            azuriranaBaza.gradovi.Sort();
            XElement bazaGradova = doc.Descendants("gradovi").First();
            bazaGradova.Elements().Remove();
            foreach (string grad in azuriranaBaza.gradovi)
            {
                bazaGradova.Add(new XElement("grad", grad));
            }

            azuriranaBaza.ulice.Sort();
            XElement bazaUlica = doc.Descendants("ulice").First();
            bazaUlica.Elements().Remove();
            foreach (string ulica in azuriranaBaza.ulice)
            {
                bazaUlica.Add(new XElement("ulica", ulica));
            }

            azuriranaBaza.dijagnoze.Sort();
            XElement bazaDijagnoza = doc.Descendants("dijagnoze").First();
            bazaDijagnoza.Elements().Remove();
            foreach (string dijagnoza in azuriranaBaza.dijagnoze)
            {
                bazaDijagnoza.Add(new XElement("dijagnoza", dijagnoza));
            }

            azuriranaBaza.zahvati.Sort();
            XElement bazaZahvata = doc.Descendants("zahvati").First();
            bazaZahvata.Elements().Remove();
            foreach (string zahvat in azuriranaBaza.zahvati)
            {
                bazaZahvata.Add(new XElement("zahvat", zahvat));
            }

            doc.Save(path);
        }

        public Baza(string putanja)
        {
            path = putanja;
            UcitajBazu();
        }
    }
}