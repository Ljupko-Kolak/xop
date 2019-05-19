using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Globalization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics;
using Microsoft.Win32;

namespace Obrazac_za_Otpusnice
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Baza baza;
        OtpusnicaPohranjena otpusnica;
        string podatciFolder = "";
        bool programLock = false;
        bool obrazacUcitan = false;

        public MainWindow()
        {
            InitializeComponent();

            this.WindowState = WindowState.Maximized;

            CultureInfo ci = new CultureInfo("hr-HR");
            ci.DateTimeFormat.ShortDatePattern = "d. M. yyyy.";
            ci.DateTimeFormat.LongDatePattern = "d. MMMM yyyy.";
            Thread.CurrentThread.CurrentCulture = ci;
            Thread.CurrentThread.CurrentUICulture = ci;

            InitializeData();

            txtOperacijaUnos.Text = "Prvo dodajte datum operacije";
        }

        private void InitializeData()
        {
            string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            podatciFolder = documentsFolder + "\\OP podatci";
            Directory.CreateDirectory(podatciFolder);
            Directory.CreateDirectory(podatciFolder + "\\Resursi");
            Directory.CreateDirectory(podatciFolder + "\\temp");

            DirectoryInfo di = new DirectoryInfo(podatciFolder);
            FileInfo fi = new FileInfo(podatciFolder + "\\Baza.xml");
            if (File.Exists(podatciFolder + "\\Resursi\\Baza.xml") == false)
            {
                File.Copy(Environment.CurrentDirectory + "\\Resursi\\Baza.xml", podatciFolder + "\\Resursi\\Baza.xml");
            }

            baza = new Baza(podatciFolder + "\\Resursi\\Baza.xml");
            foreach (Odjel odjel in baza.odjeli)
            {
                cbxOdjel.Items.Add(odjel.naziv);
            }
            cbxOdjel.Items.Add("Pedijatrija - neonatologija");
        }

        private void OcistiObrazac()
        {
            txtIme.Clear();
            txtImeRoditelja.Clear();
            txtPrezime.Clear();
            txtNeoImeMajke.Clear();
            txtNeoPrezime.Clear();
            rbtnM.IsChecked = false;
            rbtnZ.IsChecked = false;
            txtDanRodjenja.Clear();
            txtMjesecRodjenja.Clear();
            txtGodinaRodjenja.Clear();
            datNeoDatumRodjenja.Text = "";
            txtSatRodjenja.Clear();
            txtMinutaRodjenja.Clear();
            txtDjevojackoPrezime.Clear();
            txtOpcinaGrad.Clear();
            txtUlica.Clear();
            txtBroj.Clear();
            cbxOrdinirajuciLijecnik.SelectedIndex = -1;
            txtOdjelniLijecnik.Clear();
            txtMaticniBrojPovijesti.Clear();
            datDatumPrijema.Text = "";
            datDatumOtpusta.Text = "";
            txtPrijemnaDUnos.Clear();
            txtPrijemnaDijagnoza.Clear();
            txtZavrsnaDUnos.Clear();
            txtZavrsnaDijagnoza.Clear();
            datDatumOperacije.Text = "";
            txtOperacijaUnos.Clear();
            txtOperacija.Clear();
            txtNapomena.Clear();

            borderIme.BorderThickness = new Thickness(0);
            borderNeoDatumRodjenja.BorderThickness = new Thickness(0);
            borderNeoImeMajke.BorderThickness = new Thickness(0);
            borderNeoPrezime.BorderThickness = new Thickness(0);
            borderNeoVrijemeRodjenja.BorderThickness = new Thickness(0);
            borderOrdinirajuciLijecnik.BorderThickness = new Thickness(0);
            borderOtpust.BorderThickness = new Thickness(0);
            borderPrezime.BorderThickness = new Thickness(0);
            borderPrijem.BorderThickness = new Thickness(0);
            borderSpol.BorderThickness = new Thickness(0);
        }

        private void AutoSuggest(TextBox polje, ListBox prijedlozi, List<string> podatci)
        {
            CloseSuggestionBox(prijedlozi);
            if (polje.Text != "")
            {
                foreach (string podatak in podatci)
                {
                    if (podatak.StartsWith(polje.Text))
                    {
                        prijedlozi.Items.Add(podatak);
                        prijedlozi.Height += 23;
                        prijedlozi.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        private void SuggestFromTextBox(TextBox ovajTekst, ListBox prijedlozi, TextBox izvorTekst)
        {
            string noviPrijedlog = "";
            if (izvorTekst.Text != "")
            {
                char[] c = new char[ovajTekst.Text.Length];
                for (int i = 0; i < c.Length; i++)
                {
                    try
                    {
                        c[i] = izvorTekst.Text[i];
                    }
                    catch
                    { }
                }
                foreach (char element in c)
                {
                    noviPrijedlog += element.ToString();
                }
            }
            if (noviPrijedlog != "" && ovajTekst.Text.StartsWith(noviPrijedlog) && (prijedlozi.Items.Contains(izvorTekst.Text) == false))
            {
                prijedlozi.Items.Insert(0, izvorTekst.Text);
                prijedlozi.Height += 23;
                prijedlozi.Visibility = Visibility.Visible;
            }
        }

        private void ScrollSuggestion(Key tipka, ListBox prijedlozi)
        {
            if (tipka == Key.Down)
            {
                try
                {
                    prijedlozi.SelectedIndex++;
                    prijedlozi.ScrollIntoView(prijedlozi.SelectedItem);
                }
                catch { }
            }
            else if (tipka == Key.Up)
            {
                if (prijedlozi.SelectedIndex > 0)
                {
                    try
                    {
                        prijedlozi.SelectedIndex--;
                        prijedlozi.ScrollIntoView(prijedlozi.SelectedItem);
                    }
                    catch { }
                }
            }
        }

        private void CloseSuggestionBox(ListBox prijedlozi)
        {
            prijedlozi.Items.Clear();
            prijedlozi.Height = 0;
            prijedlozi.Visibility = Visibility.Hidden;
        }

        private void CloseOtherSuggestionBoxes(ListBox prijedlozi)
        {
            List<ListBox> boxes = new List<ListBox>
            {
                lstImePrijedlozi,
                lstImeRoditeljaPrijedlozi,
                lstPrezimePrijedlozi,
                lstDjevojackoPrezimePrijedlozi,
                lstOpcinaGradPrijedlozi,
                lstUlicaPrijedlozi,
                lstPrijemnaDijagnozaPrijedlozi,
                lstZavrsnaDijagnozaPrijedlozi,
                lstOperacijaPrijedlozi
            };

            foreach (ListBox box in boxes)
            {
                if (box != prijedlozi)
                {
                    CloseSuggestionBox(box);
                }
            }
        }

        private void SelectSuggestion(TextBox polje, ListBox prijedlozi)
        {
            if (prijedlozi.Items.Count > 0 && prijedlozi.SelectedIndex >= 0)
            {
                polje.Text = prijedlozi.SelectedItem.ToString();
                polje.CaretIndex = polje.Text.Length;
                CloseSuggestionBox(prijedlozi);
            }
        }

        private void AddNewSuggestion(ListBox prijedlozi, string podatak, List<string> podatci)
        {
            if (podatak != "" && podatci.Contains(podatak) == false && (prijedlozi.SelectedIndex >= 0) == false)
            {
                podatci.Add(podatak);
            }
        }

        private bool CheckFieldValidity()
        {
            bool validity = true;

            if (cbxOdjel.SelectedItem.ToString() == "Pedijatrija - neonatologija")
            {
                #region NeoPrezime
                if (txtNeoPrezime.Text == "")
                {
                    validity = false;
                    borderNeoPrezime.BorderThickness = new Thickness(2);
                }
                else
                {
                    borderNeoPrezime.BorderThickness = new Thickness(0);
                }
                #endregion
                #region Spol
                if (rbtnM.IsChecked == false && rbtnZ.IsChecked == false)
                {
                    validity = false;
                    borderSpol.BorderThickness = new Thickness(2);
                }
                else
                {
                    borderSpol.BorderThickness = new Thickness(0);
                }
                #endregion
                #region NeoImeMajke
                if (txtNeoImeMajke.Text == "")
                {
                    validity = false;
                    borderNeoImeMajke.BorderThickness = new Thickness(2);
                }
                else
                {
                    borderNeoImeMajke.BorderThickness = new Thickness(0);
                }
                #endregion
                #region NeoDatumRođenja
                if (datNeoDatumRodjenja.Text == "")
                {
                    validity = false;
                    borderNeoDatumRodjenja.BorderThickness = new Thickness(2);
                }
                else
                {
                    borderNeoDatumRodjenja.BorderThickness = new Thickness(0);
                }
                #endregion
                #region NeoVrijemeRođenja
                if (txtSatRodjenja.Text == "" || txtMinutaRodjenja.Text == "")
                {
                    validity = false;
                    borderNeoVrijemeRodjenja.BorderThickness = new Thickness(2);
                }
                else
                {
                    borderNeoVrijemeRodjenja.BorderThickness = new Thickness(0);
                }
                #endregion
            }
            else
            {
                #region Ime
                if (txtIme.Text == "")
                {
                    validity = false;
                    borderIme.BorderThickness = new Thickness(2);
                }
                else
                {
                    borderIme.BorderThickness = new Thickness(0);
                }
                #endregion
                #region Prezime
                if (txtPrezime.Text == "")
                {
                    validity = false;
                    borderPrezime.BorderThickness = new Thickness(2);
                }
                else
                {
                    borderPrezime.BorderThickness = new Thickness(0);
                }
                #endregion
            }

            #region Liječnik
            if (cbxOrdinirajuciLijecnik.Text == "")
            {
                validity = false;
                borderOrdinirajuciLijecnik.BorderThickness = new Thickness(2);
            }
            else
            {
                borderOrdinirajuciLijecnik.BorderThickness = new Thickness(0);
            }
            #endregion
            #region Prijem
            if (datDatumPrijema.Text == "")
            {
                validity = false;
                borderPrijem.BorderThickness = new Thickness(2);
            }
            else
            {
                borderPrijem.BorderThickness = new Thickness(0);
            }
            #endregion
            #region Otpust
            if (datDatumOtpusta.Text == "")
            {
                validity = false;
                borderOtpust.BorderThickness = new Thickness(2);
            }
            else
            {
                borderOtpust.BorderThickness = new Thickness(0);
            }
            #endregion

            return validity;
        }

        private iTextSharp.text.pdf.PdfPCell CreateCell(iTextSharp.text.Paragraph paragraph, int alignment, int border)
        {
            PdfPCell cell = new PdfPCell(paragraph);
            cell.HorizontalAlignment = alignment;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.Border = border;
            return cell;
        }

        private iTextSharp.text.Paragraph CreateParagraph(string text, iTextSharp.text.Font font)
        {
            iTextSharp.text.Paragraph par = new iTextSharp.text.Paragraph("", font);
            if (text != "")
            {
                par = new iTextSharp.text.Paragraph(text, font);
            }
            return par;
        }

        public bool IsFileLocked(string filename)
        {
            bool fileLocked = false;
            try
            {
                FileStream fs =
                    File.Open(filename, FileMode.OpenOrCreate,
                    FileAccess.ReadWrite, FileShare.Read);
                fs.Close();
            }
            catch (IOException ex)
            {
                fileLocked = true;
            }
            return fileLocked;
        }

        private void CreatePDFin(string putanja)
        {
            #region
            string putanjaOtpusnice = putanja + "\\" + txtIme.Text + " " + txtPrezime.Text + " (" + DateTime.Now.ToShortDateString() + ").pdf";
            string winFolder = Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.System)).ToString();
            string myFont = winFolder + "\\Fonts\\verdana.ttf";
            iTextSharp.text.Document otpusnica = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 50, 50, 50, 50);
            iTextSharp.text.pdf.PdfWriter wrtr;
            wrtr = iTextSharp.text.pdf.PdfWriter.GetInstance(otpusnica,
                new System.IO.FileStream(putanjaOtpusnice, System.IO.FileMode.Create));
            otpusnica.Open();
            otpusnica.NewPage();
            iTextSharp.text.pdf.BaseFont bfR;
            bfR = iTextSharp.text.pdf.BaseFont.CreateFont(myFont,
              iTextSharp.text.pdf.BaseFont.IDENTITY_H,
              iTextSharp.text.pdf.BaseFont.EMBEDDED);

            iTextSharp.text.Color clrBlack =
                new iTextSharp.text.Color(0, 0, 0);
            iTextSharp.text.Font fntBold =
                new iTextSharp.text.Font(bfR, 10, iTextSharp.text.Font.BOLD, clrBlack);
            iTextSharp.text.Font fntNormal =
                new iTextSharp.text.Font(bfR, 10, iTextSharp.text.Font.NORMAL, clrBlack);
            iTextSharp.text.Font fntItalic =
                new iTextSharp.text.Font(bfR, 10, iTextSharp.text.Font.ITALIC, clrBlack);
            #endregion

            #region Header
            iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(Environment.CurrentDirectory + "\\Resursi\\bolnica_logo.png");
            logo.SetAbsolutePosition(otpusnica.Right - logo.Width, otpusnica.Top - logo.Height - 15);
            otpusnica.Add(logo);

            otpusnica.Add(CreateParagraph("HRVATSKA BOLNICA", fntNormal));
            otpusnica.Add(CreateParagraph("DR. FRA MATO NIKOLIĆ", new Font(bfR, 16, iTextSharp.text.Font.BOLD)));
            otpusnica.Add(CreateParagraph("DUBRAVE B.B. NOVA BILA", fntNormal));
            otpusnica.Add(CreateParagraph("TRAVNIK", fntNormal));
            otpusnica.Add(CreateParagraph("tel. 030/708-500    fax. 030/707-421", fntNormal));

            iTextSharp.text.Paragraph linija = new iTextSharp.text.Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, iTextSharp.text.Color.BLACK, Element.ALIGN_LEFT, 1)));
            otpusnica.Add(linija);

            otpusnica.Add(CreateParagraph(cbxOdjel.SelectedItem.ToString() + "    " + lblOdjel.Text, fntNormal));
            otpusnica.Add(CreateParagraph("URED ZA PRIJEM I OTPUST PACIJENATA    tel. 030/708-556", fntNormal));
            iTextSharp.text.Paragraph par =
                new iTextSharp.text.Paragraph("OTPUSNO PISMO", new Font(bfR, 16, iTextSharp.text.Font.BOLD));
            par.Alignment = 1;
            otpusnica.Add(par);
            otpusnica.Add(CreateParagraph("\n", fntBold));
            #endregion

            if (cbxOdjel.SelectedItem.ToString() != "Pedijatrija - neonatologija")
            {
                #region Ime, Prezime, Datum rođenja
                string imePrezime = "";
                imePrezime += txtIme.Text;
                if (txtImeRoditelja.Text != "")
                {
                    imePrezime += " (" + txtImeRoditelja.Text + ")";
                }
                if (txtPrezime.Text != "")
                {
                    imePrezime += " " + txtPrezime.Text;
                }
                if (txtDjevojackoPrezime.Text != "")
                {
                    imePrezime += " (djev. " + txtDjevojackoPrezime.Text + ")";
                }

                string datRodjenja = "";
                if (txtDanRodjenja.Text != "")
                {
                    datRodjenja += "rođ. " + txtDanRodjenja.Text.Replace(".", "") + ".";
                }
                if (txtMjesecRodjenja.Text != "")
                {
                    if (datRodjenja != "")
                    {
                        datRodjenja += " ";
                    }
                    else
                    {
                        datRodjenja += "rođ. ";
                    }
                    switch (txtMjesecRodjenja.Text)
                    {
                        case "01":
                            datRodjenja += "siječnja";
                            break;
                        case "1":
                            datRodjenja += "siječnja";
                            break;
                        case "02":
                            datRodjenja += "veljače";
                            break;
                        case "2":
                            datRodjenja += "veljače";
                            break;
                        case "03":
                            datRodjenja += "ožujka";
                            break;
                        case "3":
                            datRodjenja += "ožujka";
                            break;
                        case "04":
                            datRodjenja += "travnja";
                            break;
                        case "4":
                            datRodjenja += "travnja";
                            break;
                        case "05":
                            datRodjenja += "svibnja";
                            break;
                        case "5":
                            datRodjenja += "svibnja";
                            break;
                        case "06":
                            datRodjenja += "lipnja";
                            break;
                        case "6":
                            datRodjenja += "lipnja";
                            break;
                        case "07":
                            datRodjenja += "srpnja";
                            break;
                        case "7":
                            datRodjenja += "srpnja";
                            break;
                        case "08":
                            datRodjenja += "kolovoza";
                            break;
                        case "8":
                            datRodjenja += "kolovoza";
                            break;
                        case "09":
                            datRodjenja += "rujna";
                            break;
                        case "9":
                            datRodjenja += "rujna";
                            break;
                        case "10":
                            datRodjenja += "listopada";
                            break;
                        case "11":
                            datRodjenja += "studenog";
                            break;
                        case "12":
                            datRodjenja += "prosinca";
                            break;
                        default:
                            datRodjenja += txtMjesecRodjenja.Text + ".";
                            break;
                    }
                }
                if (txtGodinaRodjenja.Text != "")
                {
                    if (datRodjenja != "")
                    {
                        datRodjenja += " ";
                    }
                    else
                    {
                        datRodjenja += "rođ. ";
                    }
                    datRodjenja += txtGodinaRodjenja.Text + ".";
                }

                string vrijemeRodjenja = "";
                if (txtSatRodjenja.Text != "")
                {
                    vrijemeRodjenja += txtSatRodjenja.Text + ":" + txtMinutaRodjenja.Text + " h";
                }

                iTextSharp.text.Paragraph parImePrezimeDatum = CreateParagraph(imePrezime + "               " + datRodjenja + "   " + vrijemeRodjenja, fntNormal);
                otpusnica.Add(parImePrezimeDatum);
                #endregion
            }
            else
            {
                #region Prezime, Spol, Ime majke, Datum i Vrijeme rođenja
                string neoImePrezime = "";

                neoImePrezime += txtNeoPrezime.Text;

                if (rbtnM.IsChecked == true)
                {
                    neoImePrezime += ", muško novorođenče";
                }
                else if (rbtnZ.IsChecked == true)
                {
                    neoImePrezime += ", žensko novorođenče";
                }

                neoImePrezime += " (" + txtNeoImeMajke.Text + ")";

                string neoDatumVrijeme = "";
                neoDatumVrijeme += "rođ. " + datNeoDatumRodjenja.Text + "   " + txtSatRodjenja.Text + ":" + txtMinutaRodjenja.Text + " h";

                iTextSharp.text.Paragraph parNeoPodatci = CreateParagraph(neoImePrezime + "               " + neoDatumVrijeme, fntNormal);
                otpusnica.Add(parNeoPodatci);
                #endregion
            }

            #region Prebivalište
            string prebivaliste = txtOpcinaGrad.Text;
            if (txtUlica.Text != "")
            {
                prebivaliste += ", " + txtUlica.Text;
            }
            if (txtBroj.Text != "")
            {
                prebivaliste += " " + txtBroj.Text;
            }
            iTextSharp.text.Paragraph parPrebivaliste = CreateParagraph(prebivaliste, fntNormal);
            otpusnica.Add(parPrebivaliste);
            #endregion

            #region Matični broj povijesti
            iTextSharp.text.Paragraph parMatTitle = CreateParagraph("Matični broj povijesti", fntBold);
            parMatTitle.Alignment = 2;
            iTextSharp.text.Paragraph parMaticniBrojPovijesti = CreateParagraph(txtMaticniBrojPovijesti.Text, fntNormal);
            parMaticniBrojPovijesti.Alignment = 2;
            if (txtMaticniBrojPovijesti.Text != "")
            {
                otpusnica.Add(CreateParagraph(" ", fntNormal));
                otpusnica.Add(parMatTitle);
                otpusnica.Add(parMaticniBrojPovijesti);
            }
            #endregion

            PdfPCell space = CreateCell(CreateParagraph(" ", fntNormal), 1, 0);

            #region Prijem i Otpust
            otpusnica.Add(CreateParagraph(" ", fntNormal));
            PdfPTable tabelaPrijemOtpust = new PdfPTable(5);
            float[] widthspo = new float[] { 28.5f, 28.5f, 2f, 28.5f, 28.5f };
            tabelaPrijemOtpust.SetWidths(widthspo);
            tabelaPrijemOtpust.HorizontalAlignment = 1;
            tabelaPrijemOtpust.TotalWidth = otpusnica.Right - otpusnica.Left;
            tabelaPrijemOtpust.LockedWidth = true;
            tabelaPrijemOtpust.AddCell(CreateCell(CreateParagraph("Datum prijema:", fntBold), 0, 0));
            if (datDatumPrijema.Text != "")
            { tabelaPrijemOtpust.AddCell(CreateCell(CreateParagraph(datDatumPrijema.Text, fntNormal), 0, 0)); }
            else
            { tabelaPrijemOtpust.AddCell(CreateCell(CreateParagraph("/", fntNormal), 0, 0)); }
            tabelaPrijemOtpust.AddCell(space);
            tabelaPrijemOtpust.AddCell(CreateCell(CreateParagraph("Datum otpusta:", fntBold), 0, 0));
            if (datDatumOtpusta.Text != "")
            { tabelaPrijemOtpust.AddCell(CreateCell(CreateParagraph(datDatumOtpusta.Text, fntNormal), 0, 0)); }
            else
            { tabelaPrijemOtpust.AddCell(CreateCell(CreateParagraph("/", fntNormal), 2, 0)); }
            if (datDatumPrijema.Text != "" || datDatumOtpusta.Text != "")
            {
                otpusnica.Add(tabelaPrijemOtpust);
            }
            #endregion

            #region Dijagnoze
            iTextSharp.text.Paragraph parPrijemnaDijagnoza = CreateParagraph(txtPrijemnaDijagnoza.Text, fntNormal);
            iTextSharp.text.Paragraph parZavrsnaDijagnoza = CreateParagraph(txtZavrsnaDijagnoza.Text, fntNormal);

            PdfPTable tableDijagnoza = new PdfPTable(3);
            float[] widths = new float[] { 49f, 2f, 49f };
            tableDijagnoza.SetWidths(widths);
            tableDijagnoza.HorizontalAlignment = 1;
            tableDijagnoza.TotalWidth = otpusnica.Right - otpusnica.Left;
            tableDijagnoza.LockedWidth = true;
            tableDijagnoza.AddCell(CreateCell(CreateParagraph("Prijemna dijagnoza", fntBold), 0, 0));
            tableDijagnoza.AddCell(space);
            tableDijagnoza.AddCell(CreateCell(CreateParagraph("Završna dijagnoza", fntBold), 0, 0));
            tableDijagnoza.AddCell(CreateCell(parPrijemnaDijagnoza, 0, 15));
            tableDijagnoza.AddCell(space);
            tableDijagnoza.AddCell(CreateCell(parZavrsnaDijagnoza, 0, 15));
            if (txtPrijemnaDijagnoza.Text != "" || txtZavrsnaDijagnoza.Text != "")
            {
                otpusnica.Add(tableDijagnoza);
            }
            #endregion

            #region Operacije
            iTextSharp.text.Paragraph parOperacije = CreateParagraph(txtOperacija.Text, fntNormal);
            PdfPTable tableOperacija = new PdfPTable(1);
            tableOperacija.TotalWidth = otpusnica.Right - otpusnica.Left;
            tableOperacija.LockedWidth = true;
            tableOperacija.AddCell(CreateCell(CreateParagraph("Operativni zahvati - intervencije", fntBold), 0, 0));
            tableOperacija.AddCell(CreateCell(parOperacije, 0, 15));
            if (txtOperacija.Text != "")
            {
                otpusnica.Add(CreateParagraph(" ", fntNormal));
                otpusnica.Add(tableOperacija);
            }
            #endregion

            iTextSharp.text.Paragraph parNapomena = CreateParagraph("\n" + txtNapomena.Text, fntNormal);
            otpusnica.Add(parNapomena);

            #region Potpisi
            PdfPTable tablePotpis = new PdfPTable(5);
            float[] widths2 = new float[] { 32f, 2f, 32f, 2f, 32f };
            tablePotpis.SetWidths(widths2);
            tablePotpis.TotalWidth = otpusnica.Right - otpusnica.Left;
            tablePotpis.LockedWidth = true;
            tablePotpis.KeepTogether = true;

            string sef = "";
            string sefTitula = "";
            string lijecnikTitula = " ";

            try
            {
                sef = baza.odjeli[cbxOdjel.SelectedIndex].sef;
                sefTitula = baza.odjeli[cbxOdjel.SelectedIndex].titula1 + "\n" + baza.odjeli[cbxOdjel.SelectedIndex].titula2;

                try
                {
                    lijecnikTitula = baza.odjeli[cbxOdjel.SelectedIndex].lijecnici[cbxOrdinirajuciLijecnik.SelectedIndex].titula1 +
                        "\n" + baza.odjeli[cbxOdjel.SelectedIndex].lijecnici[cbxOrdinirajuciLijecnik.SelectedIndex].titula2;
                }
                catch
                { }
            }
            catch
            {
                sef = baza.odjeli[cbxOdjel.SelectedIndex - 1].sef;
                sefTitula = baza.odjeli[cbxOdjel.SelectedIndex - 1].titula1 + "\n" + baza.odjeli[cbxOdjel.SelectedIndex - 1].titula2;

                try
                {
                    lijecnikTitula = baza.odjeli[cbxOdjel.SelectedIndex - 1].lijecnici[cbxOrdinirajuciLijecnik.SelectedIndex].titula1 +
                        "\n" + baza.odjeli[cbxOdjel.SelectedIndex - 1].lijecnici[cbxOrdinirajuciLijecnik.SelectedIndex].titula2;
                }
                catch
                { }
            }

            iTextSharp.text.Image potpis = iTextSharp.text.Image.GetInstance(Environment.CurrentDirectory + "\\Resursi\\" + sef + ".png");
            potpis.ScalePercent(25);

            tablePotpis.AddCell(CreateCell(CreateParagraph("\n\n\nOrdinirajući liječnik", fntNormal), 1, 0));
            tablePotpis.AddCell(space);
            if (txtOdjelniLijecnik.Text != "")
            { tablePotpis.AddCell(CreateCell(CreateParagraph("\n\n\nOdjelni liječnik", fntNormal), 1, 0)); }
            else
            { tablePotpis.AddCell(CreateCell(CreateParagraph("\n\n\n ", fntNormal), 1, 0)); }
            tablePotpis.AddCell(space);
            tablePotpis.AddCell(CreateCell(CreateParagraph("\n\n\nŠef odjela", fntNormal), 1, 0));
            tablePotpis.AddCell(CreateCell(CreateParagraph(" ", fntNormal), 0, 0));
            tablePotpis.AddCell(space);
            tablePotpis.AddCell(CreateCell(CreateParagraph(" ", fntNormal), 0, 0));
            tablePotpis.AddCell(space);
            PdfPCell slika = new PdfPCell(potpis);
            slika.Border = 0;
            slika.HorizontalAlignment = 1;
            tablePotpis.AddCell(slika);
            tablePotpis.AddCell(CreateCell(CreateParagraph(lijecnikTitula, fntItalic), 1, 1));
            tablePotpis.AddCell(space);
            if (txtOdjelniLijecnik.Text != "")
            {
                tablePotpis.AddCell(CreateCell(CreateParagraph(txtOdjelniLijecnik.Text, fntItalic), 1, 1));
            }
            else
            {
                tablePotpis.AddCell(CreateCell(CreateParagraph(" ", fntNormal), 0, 0));
            }
            tablePotpis.AddCell(space);
            tablePotpis.AddCell(CreateCell(CreateParagraph(sefTitula, fntItalic), 1, 1));

            otpusnica.Add(tablePotpis);
            #endregion


            otpusnica.Close();

            var prikaziPDF = new Process();
            prikaziPDF.StartInfo.FileName = putanjaOtpusnice;
            prikaziPDF.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            prikaziPDF.EnableRaisingEvents = true;
            prikaziPDF.Start();

            bool canDeleteFile = false;
            programLock = true;
            while (programLock)
            {
                if (File.Exists(putanjaOtpusnice))
                {
                    if (IsFileLocked(putanjaOtpusnice) && canDeleteFile == false)
                    {
                        canDeleteFile = true;
                    }
                    if (canDeleteFile)
                    {
                        try
                        {
                            File.Delete(putanjaOtpusnice);
                            programLock = false;
                        }
                        catch
                        { }
                    }
                }
                Thread.Sleep(250);
            };
        }

        #region Ime
        private void txtIme_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtIme, lstImePrijedlozi, baza.imena);
            SuggestFromTextBox(txtIme, lstImePrijedlozi, txtImeRoditelja);
        }

        private void txtIme_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstImePrijedlozi);

            if (e.Key == Key.Enter)
            {
                SelectSuggestion(txtIme, lstImePrijedlozi);
            }
        }

        private void txtIme_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstImePrijedlozi);
        }

        private void lstImePrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstImePrijedlozi.SelectedIndex >= 0)
            {
                txtIme.Text = lstImePrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstImePrijedlozi);
            }
        }

        private void lstImePrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtIme, lstImePrijedlozi);
        }
        #endregion

        #region Ime roditelja
        private void txtImeRoditelja_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtImeRoditelja, lstImeRoditeljaPrijedlozi, baza.imena);
            SuggestFromTextBox(txtImeRoditelja, lstImeRoditeljaPrijedlozi, txtIme);
        }

        private void txtImeRoditelja_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstImeRoditeljaPrijedlozi);

            if (e.Key == Key.Enter)
            {
                SelectSuggestion(txtImeRoditelja, lstImeRoditeljaPrijedlozi);
            }
        }

        private void txtImeRoditelja_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstImeRoditeljaPrijedlozi);
        }

        private void lstImeRoditeljaPrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstImeRoditeljaPrijedlozi.SelectedIndex >= 0)
            {
                txtImeRoditelja.Text = lstImeRoditeljaPrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstImeRoditeljaPrijedlozi);
            }
        }

        private void lstImeRoditeljaPrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtImeRoditelja, lstImeRoditeljaPrijedlozi);
        }
        #endregion

        #region Neo Prezime
        private void txtNeoPrezime_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtNeoPrezime, lstImePrijedlozi, baza.prezimena);
        }

        private void txtNeoPrezime_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstImePrijedlozi);

            if (e.Key == Key.Enter)
            {
                SelectSuggestion(txtNeoPrezime, lstImePrijedlozi);
            }
        }

        private void txtNeoPrezime_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstImePrijedlozi);
        }
        #endregion

        #region Neo Spol
        private void rbtnM_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rbtnM.IsChecked = true;
                rbtnZ.IsChecked = false;
            }
        }

        private void rbtnZ_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                rbtnZ.IsChecked = true;
                rbtnM.IsChecked = false;
            }
        }

        #endregion

        #region Neo Ime majke
        private void txtNeoImeMajke_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtNeoImeMajke, lstPrezimePrijedlozi, baza.imena);
        }

        private void txtNeoImeMajke_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstPrezimePrijedlozi);

            if (e.Key == Key.Enter)
            {
                SelectSuggestion(txtNeoImeMajke, lstPrezimePrijedlozi);
            }
        }

        private void txtNeoImeMajke_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstPrezimePrijedlozi);
        }
        #endregion

        #region Prezime
        private void txtPrezime_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtPrezime, lstPrezimePrijedlozi, baza.prezimena);
            SuggestFromTextBox(txtPrezime, lstPrezimePrijedlozi, txtDjevojackoPrezime);
        }

        private void txtPrezime_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstPrezimePrijedlozi);

            if (e.Key == Key.Enter)
            {
                SelectSuggestion(txtPrezime, lstPrezimePrijedlozi);
            }
        }

        private void txtPrezime_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstPrezimePrijedlozi);
        }

        private void lstPrezimePrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstPrezimePrijedlozi.SelectedIndex >= 0)
            {
                txtPrezime.Text = lstPrezimePrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstPrezimePrijedlozi);
            }
        }

        private void lstPrezimePrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtPrezime, lstPrezimePrijedlozi);
        }
        #endregion

        #region Djevojačko prezime
        private void txtDjevojackoPrezime_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtDjevojackoPrezime, lstDjevojackoPrezimePrijedlozi, baza.prezimena);
            SuggestFromTextBox(txtDjevojackoPrezime, lstDjevojackoPrezimePrijedlozi, txtPrezime);
        }

        private void txtDjevojackoPrezime_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstDjevojackoPrezimePrijedlozi);

            if (e.Key == Key.Enter)
            {
                SelectSuggestion(txtDjevojackoPrezime, lstDjevojackoPrezimePrijedlozi);
            }
        }

        private void txtDjevojackoPrezime_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstDjevojackoPrezimePrijedlozi);
        }

        private void lstDjevojackoPrezimePrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstDjevojackoPrezimePrijedlozi.SelectedIndex >= 0)
            {
                txtDjevojackoPrezime.Text = lstDjevojackoPrezimePrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstDjevojackoPrezimePrijedlozi);
            }
        }

        private void lstDjevojackoPrezimePrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtDjevojackoPrezime, lstDjevojackoPrezimePrijedlozi);
        }
        #endregion

        #region Grad
        private void txtOpcinaGrad_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtOpcinaGrad, lstOpcinaGradPrijedlozi, baza.gradovi);
        }

        private void txtOpcinaGrad_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstOpcinaGradPrijedlozi);

            if (e.Key == Key.Enter)
            {
                SelectSuggestion(txtOpcinaGrad, lstOpcinaGradPrijedlozi);
            }
        }

        private void txtOpcinaGrad_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstOpcinaGradPrijedlozi);
        }

        private void lstOpcinaGradPrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstOpcinaGradPrijedlozi.SelectedIndex >= 0)
            {
                txtOpcinaGrad.Text = lstOpcinaGradPrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstOpcinaGradPrijedlozi);
            }
        }

        private void lstOpcinaGradPrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtOpcinaGrad, lstOpcinaGradPrijedlozi);
        }
        #endregion

        #region Ulice
        private void txtUlica_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtUlica, lstUlicaPrijedlozi, baza.ulice);
        }

        private void txtUlica_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstUlicaPrijedlozi);

            if (e.Key == Key.Enter)
            {
                SelectSuggestion(txtUlica, lstUlicaPrijedlozi);
            }
        }

        private void txtUlica_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstUlicaPrijedlozi);
        }

        private void lstUlicaPrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstUlicaPrijedlozi.SelectedIndex >= 0)
            {
                txtUlica.Text = lstUlicaPrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstUlicaPrijedlozi);
            }
        }

        private void lstUlicaPrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtUlica, lstUlicaPrijedlozi);
        }
        #endregion

        #region Odjelni liječnik
        private void txtOdjelniLijecnik_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtOdjelniLijecnik, lstOdjelniLijecniciPrijedlozi, baza.odjelniLijecnici);
        }

        private void txtOdjelniLijecnik_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstOdjelniLijecniciPrijedlozi);

            if (e.Key == Key.Enter)
            {
                SelectSuggestion(txtOdjelniLijecnik, lstOdjelniLijecniciPrijedlozi);
            }
        }

        private void txtOdjelniLijecnik_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstOdjelniLijecniciPrijedlozi);
        }

        private void lstOdjelniLijecniciPrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstOdjelniLijecniciPrijedlozi.SelectedIndex >= 0)
            {
                txtOdjelniLijecnik.Text = lstOdjelniLijecniciPrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstOdjelniLijecniciPrijedlozi);
            }
        }

        private void lstOdjelniLijecniciPrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtOdjelniLijecnik, lstOdjelniLijecniciPrijedlozi);
        }
        #endregion

        #region Prijemna dijagnoza
        private void txtPrijemnaDUnos_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtPrijemnaDUnos, lstPrijemnaDijagnozaPrijedlozi, baza.dijagnoze);
            SuggestFromTextBox(txtPrijemnaDUnos, lstPrijemnaDijagnozaPrijedlozi, txtZavrsnaDUnos);
        }

        private void txtPrijemnaDUnos_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstPrijemnaDijagnozaPrijedlozi);

            if (e.Key == Key.Enter)
            {
                AddNewSuggestion(lstPrijemnaDijagnozaPrijedlozi, txtPrijemnaDUnos.Text, baza.dijagnoze);
            }
            if (e.Key == Key.Enter && lstPrijemnaDijagnozaPrijedlozi.Items.Count > 0)
            {
                SelectSuggestion(txtPrijemnaDUnos, lstPrijemnaDijagnozaPrijedlozi);
            }
            else if (e.Key == Key.Enter && txtPrijemnaDUnos.Text != "")
            {
                if (txtPrijemnaDijagnoza.Text != "")
                {
                    txtPrijemnaDijagnoza.Text += "\n";
                }
                txtPrijemnaDijagnoza.Text += txtPrijemnaDUnos.Text;
                txtPrijemnaDUnos.Clear();
                txtPrijemnaDijagnoza.ScrollToEnd();
            }
        }

        private void txtPrijemnaDUnos_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstPrijemnaDijagnozaPrijedlozi);
        }

        private void lstPrijemnaDijagnozaPrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstPrijemnaDijagnozaPrijedlozi.SelectedIndex >= 0)
            {
                txtPrijemnaDUnos.Text = lstPrijemnaDijagnozaPrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstPrijemnaDijagnozaPrijedlozi);
            }
        }

        private void lstPrijemnaDijagnozaPrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtPrijemnaDUnos, lstPrijemnaDijagnozaPrijedlozi);
        }

        private void btnDodajPrijemnuDijagnozu_Click(object sender, RoutedEventArgs e)
        {
            if (txtPrijemnaDUnos.Text != "")
            {
                AddNewSuggestion(lstPrijemnaDijagnozaPrijedlozi, txtPrijemnaDUnos.Text, baza.dijagnoze);
                if (txtPrijemnaDijagnoza.Text != "")
                {
                    txtPrijemnaDijagnoza.Text += "\n";
                }
                txtPrijemnaDijagnoza.Text += txtPrijemnaDUnos.Text;
                txtPrijemnaDUnos.Clear();
                txtPrijemnaDijagnoza.ScrollToEnd();
            }
        }
        #endregion

        #region Završna dijagnoza
        private void txtZavrsnaDUnos_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtZavrsnaDUnos, lstZavrsnaDijagnozaPrijedlozi, baza.dijagnoze);
            SuggestFromTextBox(txtZavrsnaDUnos, lstZavrsnaDijagnozaPrijedlozi, txtPrijemnaDUnos);
        }

        private void txtZavrsnaDUnos_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstZavrsnaDijagnozaPrijedlozi);

            if (e.Key == Key.Enter)
            {
                AddNewSuggestion(lstZavrsnaDijagnozaPrijedlozi, txtZavrsnaDUnos.Text, baza.dijagnoze);
            }
            if (e.Key == Key.Enter && lstZavrsnaDijagnozaPrijedlozi.Items.Count > 0)
            {
                AddNewSuggestion(lstZavrsnaDijagnozaPrijedlozi, txtZavrsnaDUnos.Text, baza.dijagnoze);
                SelectSuggestion(txtZavrsnaDUnos, lstZavrsnaDijagnozaPrijedlozi);
            }
            else if (e.Key == Key.Enter && txtZavrsnaDUnos.Text != "")
            {
                if (txtZavrsnaDijagnoza.Text != "")
                {
                    txtZavrsnaDijagnoza.Text += "\n";
                }
                txtZavrsnaDijagnoza.Text += txtZavrsnaDUnos.Text;
                txtZavrsnaDUnos.Clear();
                txtZavrsnaDijagnoza.ScrollToEnd();
            }
        }

        private void txtZavrsnaDUnos_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstZavrsnaDijagnozaPrijedlozi);
        }

        private void lstZavrsnaDijagnozaPrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstZavrsnaDijagnozaPrijedlozi.SelectedIndex >= 0)
            {
                txtZavrsnaDUnos.Text = lstZavrsnaDijagnozaPrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstZavrsnaDijagnozaPrijedlozi);
            }
        }

        private void lstZavrsnaDijagnozaPrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtZavrsnaDUnos, lstZavrsnaDijagnozaPrijedlozi);
        }

        private void btnDodajZavrsnuDijagnozu_Click(object sender, RoutedEventArgs e)
        {
            AddNewSuggestion(lstZavrsnaDijagnozaPrijedlozi, txtZavrsnaDUnos.Text, baza.dijagnoze);
            if (txtZavrsnaDUnos.Text != "")
            {
                if (txtZavrsnaDijagnoza.Text != "")
                {
                    txtZavrsnaDijagnoza.Text += "\n";
                }
                txtZavrsnaDijagnoza.Text += txtZavrsnaDUnos.Text;
                txtZavrsnaDUnos.Clear();
                txtZavrsnaDijagnoza.ScrollToEnd();
            }
        }
        #endregion

        #region Operacije
        private void txtOperacijaUnos_TextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSuggest(txtOperacijaUnos, lstOperacijaPrijedlozi, baza.zahvati);
        }

        private void txtOperacijaUnos_KeyUp(object sender, KeyEventArgs e)
        {
            ScrollSuggestion(e.Key, lstOperacijaPrijedlozi);

            if (e.Key == Key.Enter)
            {
                AddNewSuggestion(lstOperacijaPrijedlozi, txtOperacijaUnos.Text, baza.zahvati);
            }
            if (e.Key == Key.Enter && lstOperacijaPrijedlozi.Items.Count > 0)
            {
                AddNewSuggestion(lstOperacijaPrijedlozi, txtOperacijaUnos.Text, baza.zahvati);
                SelectSuggestion(txtOperacijaUnos, lstOperacijaPrijedlozi);
            }
            else if (e.Key == Key.Enter && txtOperacijaUnos.Text != "")
            {
                txtOperacija.Text += "\n                      " + txtOperacijaUnos.Text;
                txtOperacijaUnos.Clear();
                txtOperacija.ScrollToEnd();
            }
        }

        private void txtOperacijaUnos_GotFocus(object sender, RoutedEventArgs e)
        {
            CloseOtherSuggestionBoxes(lstOperacijaPrijedlozi);
        }

        private void lstOperacijaPrijedlozi_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && lstOperacijaPrijedlozi.SelectedIndex >= 0)
            {
                txtOperacijaUnos.Text = lstOperacijaPrijedlozi.SelectedItem.ToString();
                CloseSuggestionBox(lstOperacijaPrijedlozi);
            }
        }

        private void lstOperacijaPrijedlozi_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectSuggestion(txtOperacijaUnos, lstOperacijaPrijedlozi);
        }

        private void btnDodajDatumOperacije_Click(object sender, RoutedEventArgs e)
        {
            if (datDatumOperacije.Text != "")
            {
                if (txtOperacijaUnos.Text.Contains("Prvo dodajte datum operacije"))
                {
                    txtOperacijaUnos.Text = "";
                }
                if (txtOperacija.Text == "" || obrazacUcitan)
                {
                    txtOperacija.IsEnabled = true;
                    txtOperacijaUnos.IsEnabled = true;
                    btnDodajZahvat.IsEnabled = true;
                    if (txtOperacija.Text != "")
                    {
                        txtOperacija.Text += "\n\n";
                    }
                    txtOperacija.Text += datDatumOperacije.Text;
                    datDatumOperacije.Text = "";
                }
                else
                {
                    txtOperacija.Text += "\n\n" + datDatumOperacije.Text;
                    datDatumOperacije.Text = "";
                }
            }
            txtOperacija.ScrollToEnd();
        }

        private void datDatumOperacije_KeyUp(object sender, KeyEventArgs e)
        {
            if (datDatumOperacije.Text != "" && e.Key == Key.Enter)
            {
                if (txtOperacijaUnos.Text.Contains("Prvo dodajte datum operacije"))
                {
                    txtOperacijaUnos.Text = "";
                }
                if (txtOperacija.Text == "" || obrazacUcitan)
                {
                    txtOperacija.IsEnabled = true;
                    txtOperacijaUnos.IsEnabled = true;
                    btnDodajZahvat.IsEnabled = true;
                    {
                        txtOperacija.Text += "\n\n";
                    }
                    txtOperacija.Text += datDatumOperacije.Text;
                    datDatumOperacije.Text = "";
                }
                else
                {
                    txtOperacija.Text += "\n\n" + datDatumOperacije.Text;
                    datDatumOperacije.Text = "";
                }
                txtOperacija.ScrollToEnd();
            }
        }

        private void btnDodajZahvat_Click(object sender, RoutedEventArgs e)
        {
            if (txtOperacijaUnos.Text != "")
            {
                AddNewSuggestion(lstOperacijaPrijedlozi, txtOperacijaUnos.Text, baza.zahvati);
                txtOperacija.Text += "\n                      " + txtOperacijaUnos.Text;
                txtOperacijaUnos.Clear();
                txtOperacija.ScrollToEnd();
            }
        }

        private void txtOperacija_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtOperacija.Text == "")
            {
                txtOperacijaUnos.IsEnabled = false;
                btnDodajZahvat.IsEnabled = false;
                txtOperacija.IsEnabled = false;
                txtOperacijaUnos.Text = "Prvo dodajte datum operacije";
            }
        }
        #endregion

        private void cbxOdjel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //OcistiObrazac();

            cbxOrdinirajuciLijecnik.Items.Clear();
            cbxOrdinirajuciLijecnik.SelectedItem = null;

            Odjel trenutniOdjel = new Odjel();
            foreach (Odjel o in baza.odjeli)
            {
                if (o.naziv == cbxOdjel.SelectedItem.ToString() || (cbxOdjel.SelectedItem.ToString() == "Pedijatrija - neonatologija" && o.naziv == "Pedijatrija"))
                {
                    lblOdjel.Text = o.broj;

                    foreach (Lijecnik l in o.lijecnici)
                    {
                        cbxOrdinirajuciLijecnik.Items.Add(l.imePrezime);
                    }
                }
            }

            canvasSadrzaj.Visibility = Visibility.Visible;
            heightGrid.Height = 1375;

            if (cbxOdjel.SelectedItem.ToString() != "Ginekologija")
            {
                grpDjevojackoPrezime.Visibility = Visibility.Hidden;
                txtDjevojackoPrezime.Clear();
            }
            else
            {
                grpDjevojackoPrezime.Visibility = Visibility.Visible;
            }

            if (
                cbxOdjel.SelectedItem.ToString() != "Ginekologija" &&
                cbxOdjel.SelectedItem.ToString() != "Kirurgija" &&
                cbxOdjel.SelectedItem.ToString() != "Minimalno invazivna kirurgija" &&
                cbxOdjel.SelectedItem.ToString() != "Odjel anestezije i intenzivnog liječenja"
                )
            {
                grpOperacije.Visibility = Visibility.Hidden;
                txtOperacija.Clear();

                Canvas.SetTop(grpNapomena, 753);
                Canvas.SetTop(btnZavrsi, 1031);
                Canvas.SetTop(btnOcisti, 1031);
                Canvas.SetTop(btnUcitajOtpusnicu, 1031);
                Canvas.SetTop(btnPohraniOtpusnicu, 1031);
                heightGrid.Height = 1150;
            }
            else
            {
                heightGrid.Height = 1375;
                Canvas.SetTop(grpNapomena, 980);
                Canvas.SetTop(btnZavrsi, 1258);
                Canvas.SetTop(btnOcisti, 1258);
                Canvas.SetTop(btnUcitajOtpusnicu, 1258);
                Canvas.SetTop(btnPohraniOtpusnicu, 1258);

                grpOperacije.Visibility = Visibility.Visible;
            }

            if (cbxOdjel.SelectedItem.ToString() == "Pedijatrija - neonatologija")
            {
                grpVrijemeRodjenja.Visibility = Visibility.Visible;
                gridOsnovniPodatci.Visibility = Visibility.Hidden;
                gridNeoPodatci.Visibility = Visibility.Visible;
                grpDatumRodjenja.Visibility = Visibility.Collapsed;
                grpNeoDatumRodjenja.Visibility = Visibility.Visible;
                lblOdjel.Text = "tel. 030/708-557";
            }
            else
            {
                grpVrijemeRodjenja.Visibility = Visibility.Hidden;
                gridNeoPodatci.Visibility = Visibility.Hidden;
                gridOsnovniPodatci.Visibility = Visibility.Visible;
                grpDatumRodjenja.Visibility = Visibility.Visible;
                grpNeoDatumRodjenja.Visibility = Visibility.Collapsed;
                txtNeoPrezime.Clear();
                txtNeoImeMajke.Clear();
                rbtnM.IsChecked = false;
                rbtnZ.IsChecked = false;
                datNeoDatumRodjenja.Text = "";
                txtSatRodjenja.Clear();
                txtMinutaRodjenja.Clear();
            }
        }

        private void btnZavrsi_Click(object sender, RoutedEventArgs e)
        {
            if (CheckFieldValidity())
            {
                #region Ažuriranje baze
                if (txtIme.Text != "" && baza.imena.Contains(txtIme.Text) == false)
                { baza.imena.Add(txtIme.Text); }

                if (txtNeoPrezime.Text != "" && baza.prezimena.Contains(txtNeoPrezime.Text) == false)
                { baza.prezimena.Add(txtNeoPrezime.Text); }

                if (txtImeRoditelja.Text != "" && baza.imena.Contains(txtImeRoditelja.Text) == false)
                { baza.imena.Add(txtImeRoditelja.Text); }

                if (txtPrezime.Text != "" && baza.prezimena.Contains(txtPrezime.Text) == false)
                { baza.prezimena.Add(txtPrezime.Text); }

                if (txtNeoImeMajke.Text != "" && baza.imena.Contains(txtNeoImeMajke.Text) == false)
                { baza.imena.Add(txtNeoImeMajke.Text); }

                if (txtDjevojackoPrezime.Text != "" && baza.prezimena.Contains(txtDjevojackoPrezime.Text) == false)
                { baza.prezimena.Add(txtDjevojackoPrezime.Text); }

                if (txtOpcinaGrad.Text != "" && baza.gradovi.Contains(txtOpcinaGrad.Text) == false)
                { baza.gradovi.Add(txtOpcinaGrad.Text); }

                if (txtUlica.Text != "" && baza.ulice.Contains(txtUlica.Text) == false)
                { baza.ulice.Add(txtUlica.Text); }

                if (txtOdjelniLijecnik.Text != "" && baza.odjelniLijecnici.Contains(txtOdjelniLijecnik.Text) == false)
                { baza.odjelniLijecnici.Add(txtOdjelniLijecnik.Text); }



                baza.PohraniBazu(baza);
                #endregion

                try
                {
                    CreatePDFin(podatciFolder + "\\temp");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Popunite obavezna polja!", "Nepotpuni podatci!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnOcisti_Click(object sender, RoutedEventArgs e)
        {
            OcistiObrazac();
        }

        private void btnPohraniOtpusnicu_Click(object sender, RoutedEventArgs e)
        {
            if (cbxOrdinirajuciLijecnik.SelectedIndex >= 0)
            {
                string folder = podatciFolder + "\\" + cbxOdjel.Text + "\\" + cbxOrdinirajuciLijecnik.Text;
                Directory.CreateDirectory(folder);

                string filePath = "";
                if (cbxOdjel.Text == "Pedijatrija - neonatologija")
                {
                    filePath = folder + "\\" + txtNeoPrezime.Text + " " + txtNeoImeMajke.Text + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".xml";
                }
                else
                {
                    filePath = folder + "\\" + txtIme.Text + " " + txtPrezime.Text + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + ".xml";
                }
                otpusnica = new OtpusnicaPohranjena(filePath);

                otpusnica.Ime = txtIme.Text;
                otpusnica.ImeRoditelja = txtImeRoditelja.Text;
                otpusnica.Prezime = txtPrezime.Text;
                otpusnica.DjevPrezime = txtDjevojackoPrezime.Text;
                otpusnica.DanRodjenja = txtDanRodjenja.Text;
                otpusnica.MjesecRodjenja = txtMjesecRodjenja.Text;
                otpusnica.GodinaRodjenja = txtGodinaRodjenja.Text;
                otpusnica.Grad = txtOpcinaGrad.Text;
                otpusnica.Ulica = txtUlica.Text;
                otpusnica.Broj = txtBroj.Text;
                otpusnica.MaticniBroj = txtMaticniBrojPovijesti.Text;
                otpusnica.DatumPrijema = datDatumPrijema.Text;
                otpusnica.DatumOtpusta = datDatumOtpusta.Text;
                otpusnica.PrijemnaDijagnoza = txtPrijemnaDijagnoza.Text;
                otpusnica.ZavrsnaDijagnoza = txtZavrsnaDijagnoza.Text;
                otpusnica.OperativniZahvati = txtOperacija.Text;
                otpusnica.Napomena = txtNapomena.Text;
                otpusnica.OrdinirajuciLijecnik = cbxOrdinirajuciLijecnik.Text;
                otpusnica.OdjelniLijecnik = txtOdjelniLijecnik.Text;

                otpusnica.NeoPrezime = txtNeoPrezime.Text;
                if (rbtnM.IsChecked == true)
                {
                    otpusnica.NeoM = true;
                }
                else if (rbtnZ.IsChecked == true)
                {
                    otpusnica.NeoZ = true;
                }
                otpusnica.NeoImeMajke = txtNeoImeMajke.Text;
                otpusnica.NeoDatumRodjenja = datNeoDatumRodjenja.Text;
                otpusnica.NeoSatRodjenja = txtSatRodjenja.Text;
                otpusnica.NeoMinutaRodjenja = txtMinutaRodjenja.Text;

                otpusnica.PohraniOtpusnicu();

                if (File.Exists(filePath))
                {
                    MessageBox.Show("Pohrana uspješna!", "Pohrana", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Pohrana neuspješna!", "Pohrana", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Odaberite ordinirajućeg liječnika!", "Pohrana", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnUcitajOtpusnicu_Click(object sender, RoutedEventArgs e)
        {
            string filePath = podatciFolder + "\\" + cbxOdjel.Text + "\\" + cbxOrdinirajuciLijecnik.Text;
            Directory.CreateDirectory(filePath);

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = filePath;
            ofd.Filter = "XML|*.xml";
            var result = ofd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                filePath = ofd.FileName;

                otpusnica = new OtpusnicaPohranjena(filePath);
                otpusnica.UcitajOtpusnicu();

                OcistiObrazac();
                txtIme.Text = otpusnica.Ime;
                txtImeRoditelja.Text = otpusnica.ImeRoditelja;
                txtPrezime.Text = otpusnica.Prezime;
                txtDjevojackoPrezime.Text = otpusnica.DjevPrezime;
                txtDanRodjenja.Text = otpusnica.DanRodjenja;
                txtMjesecRodjenja.Text = otpusnica.MjesecRodjenja;
                txtGodinaRodjenja.Text = otpusnica.GodinaRodjenja;
                txtOpcinaGrad.Text = otpusnica.Grad;
                txtUlica.Text = otpusnica.Ulica;
                txtBroj.Text = otpusnica.Broj;
                txtMaticniBrojPovijesti.Text = otpusnica.MaticniBroj;
                datDatumPrijema.Text = otpusnica.DatumPrijema;
                datDatumOtpusta.Text = otpusnica.DatumOtpusta;
                txtPrijemnaDijagnoza.Text = otpusnica.PrijemnaDijagnoza;
                txtZavrsnaDijagnoza.Text = otpusnica.ZavrsnaDijagnoza;
                txtOperacija.Text = otpusnica.OperativniZahvati;
                txtNapomena.Text = otpusnica.Napomena;
                cbxOrdinirajuciLijecnik.Text = otpusnica.OrdinirajuciLijecnik;
                txtOdjelniLijecnik.Text = otpusnica.OdjelniLijecnik;

                txtNeoPrezime.Text = otpusnica.NeoPrezime;
                rbtnM.IsChecked = otpusnica.NeoM;
                rbtnZ.IsChecked = otpusnica.NeoZ;
                txtNeoImeMajke.Text = otpusnica.NeoImeMajke;
                datNeoDatumRodjenja.Text = otpusnica.NeoDatumRodjenja;
                txtSatRodjenja.Text = otpusnica.NeoSatRodjenja;
                txtMinutaRodjenja.Text = otpusnica.NeoMinutaRodjenja;


                lstDjevojackoPrezimePrijedlozi.Height = 0;
                lstImePrijedlozi.Height = 0;
                lstImeRoditeljaPrijedlozi.Height = 0;
                lstOdjelniLijecniciPrijedlozi.Height = 0;
                lstOpcinaGradPrijedlozi.Height = 0;
                lstOperacijaPrijedlozi.Height = 0;
                lstPrezimePrijedlozi.Height = 0;
                lstPrijemnaDijagnozaPrijedlozi.Height = 0;
                lstUlicaPrijedlozi.Height = 0;
                lstZavrsnaDijagnozaPrijedlozi.Height = 0;

                obrazacUcitan = true;
            }
        }
    }
}