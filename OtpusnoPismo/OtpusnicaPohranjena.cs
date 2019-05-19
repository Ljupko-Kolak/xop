using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Obrazac_za_Otpusnice
{
    public class OtpusnicaPohranjena
    {
        public string Ime = "";
        public string ImeRoditelja = "";
        public string Prezime = "";
        public string DjevPrezime = "";
        public string DanRodjenja = "";
        public string MjesecRodjenja = "";
        public string GodinaRodjenja = "";
        public string Grad = "";
        public string Ulica = "";
        public string Broj = "";
        public string MaticniBroj = "";
        public string DatumPrijema = "";
        public string DatumOtpusta = "";
        public string PrijemnaDijagnoza = "";
        public string ZavrsnaDijagnoza = "";
        public string OperativniZahvati = "";
        public string Napomena = "";
        public string OrdinirajuciLijecnik = "";
        public string OdjelniLijecnik = "";

        public string NeoPrezime = "";
        public bool NeoM = false;
        public bool NeoZ = false;
        public string NeoImeMajke = "";
        public string NeoDatumRodjenja = "";
        public string NeoSatRodjenja = "";
        public string NeoMinutaRodjenja = "";

        private string path = "";

        public void UcitajOtpusnicu()
        {
            XDocument otp = XDocument.Load(path);

            Ime = new XElement(otp.Descendants("Ime").First()).Value;
            ImeRoditelja = new XElement(otp.Descendants("ImeRoditelja").First()).Value;
            Prezime = new XElement(otp.Descendants("Prezime").First()).Value;
            DjevPrezime = new XElement(otp.Descendants("DjevPrezime").First()).Value;
            DanRodjenja = new XElement(otp.Descendants("DanRodjenja").First()).Value;
            MjesecRodjenja = new XElement(otp.Descendants("MjesecRodjenja").First()).Value;
            GodinaRodjenja = new XElement(otp.Descendants("GodinaRodjenja").First()).Value;
            Grad = new XElement(otp.Descendants("Grad").First()).Value;
            Ulica = new XElement(otp.Descendants("Ulica").First()).Value;
            Broj = new XElement(otp.Descendants("Broj").First()).Value;
            MaticniBroj = new XElement(otp.Descendants("MaticniBroj").First()).Value;
            DatumPrijema = new XElement(otp.Descendants("DatumPrijema").First()).Value;
            DatumOtpusta = new XElement(otp.Descendants("DatumOtpusta").First()).Value;
            PrijemnaDijagnoza = new XElement(otp.Descendants("PrijemnaDijagnoza").First()).Value;
            ZavrsnaDijagnoza = new XElement(otp.Descendants("ZavrsnaDijagnoza").First()).Value;
            OperativniZahvati = new XElement(otp.Descendants("OperativniZahvati").First()).Value;
            Napomena = new XElement(otp.Descendants("Napomena").First()).Value;
            OrdinirajuciLijecnik = new XElement(otp.Descendants("OrdinirajuciLijecnik").First()).Value;
            OdjelniLijecnik = new XElement(otp.Descendants("OdjelniLijecnik").First()).Value;

            NeoPrezime = new XElement(otp.Descendants("NeoPrezime").First()).Value;
            if (new XElement(otp.Descendants("NeoM").First()).Value == "1")
            {
                NeoM = true;
            }
            if (new XElement(otp.Descendants("NeoZ").First()).Value == "1")
            {
                NeoZ = true;
            }
            NeoImeMajke = new XElement(otp.Descendants("NeoImeMajke").First()).Value;
            NeoDatumRodjenja = new XElement(otp.Descendants("NeoDatumRodjenja").First()).Value;
            NeoSatRodjenja = new XElement(otp.Descendants("NeoSatRodjenja").First()).Value;
            NeoMinutaRodjenja = new XElement(otp.Descendants("NeoMinutaRodjenja").First()).Value;
        }

        public void PohraniOtpusnicu()
        {
            XDocument otp = new XDocument(new XElement("root"));
            XElement root = otp.Element("root");

            root.Add(new XElement("Ime", Ime));
            root.Add(new XElement("ImeRoditelja", ImeRoditelja));
            root.Add(new XElement("Prezime", Prezime));
            root.Add(new XElement("DjevPrezime", DjevPrezime));
            root.Add(new XElement("DanRodjenja", DanRodjenja));
            root.Add(new XElement("MjesecRodjenja", MjesecRodjenja));
            root.Add(new XElement("GodinaRodjenja", GodinaRodjenja));
            root.Add(new XElement("Grad", Grad));
            root.Add(new XElement("Ulica", Ulica));
            root.Add(new XElement("Broj", Broj));
            root.Add(new XElement("MaticniBroj", MaticniBroj));
            root.Add(new XElement("DatumPrijema", DatumPrijema));
            root.Add(new XElement("DatumOtpusta", DatumOtpusta));
            root.Add(new XElement("PrijemnaDijagnoza", PrijemnaDijagnoza));
            root.Add(new XElement("ZavrsnaDijagnoza", ZavrsnaDijagnoza));
            root.Add(new XElement("OperativniZahvati", OperativniZahvati));
            root.Add(new XElement("Napomena", Napomena));
            root.Add(new XElement("OrdinirajuciLijecnik", OrdinirajuciLijecnik));
            root.Add(new XElement("OdjelniLijecnik", OdjelniLijecnik));

            root.Add(new XElement("NeoPrezime", NeoPrezime));
            if (this.NeoM)
            {
                root.Add(new XElement("NeoM", "1"));
                root.Add(new XElement("NeoZ", "0"));
            }
            else if (this.NeoZ)
            {
                root.Add(new XElement("NeoM", "0"));
                root.Add(new XElement("NeoZ", "1"));
            }
            else
            {
                root.Add(new XElement("NeoM", "0"));
                root.Add(new XElement("NeoZ", "0"));
            }

            root.Add(new XElement("NeoImeMajke", NeoImeMajke));
            root.Add(new XElement("NeoDatumRodjenja", NeoDatumRodjenja));
            root.Add(new XElement("NeoSatRodjenja", NeoSatRodjenja));
            root.Add(new XElement("NeoMinutaRodjenja", NeoMinutaRodjenja));
            otp.Save(path);
        }

        public OtpusnicaPohranjena(string putanja)
        {
            path = putanja;
        }
    }
}
