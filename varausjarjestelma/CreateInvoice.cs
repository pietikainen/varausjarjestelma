using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using varausjarjestelma.Controller;

namespace varausjarjestelma
{
    class CreateInvoice
    {

        public SKBitmap LoadImageFromFile(string imagePath)
        {
            try
            {
                return SKBitmap.Decode(imagePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading image: {ex.Message}");
                return null;
            }
        }


        public SKBitmap ResizeImage(SKBitmap originalImage, int targetWidth, int targetHeight)
        {
            try
            {
                var resizedImage = new SKBitmap(targetWidth, targetHeight);
                originalImage.ScalePixels(resizedImage.PeekPixels(), SKFilterQuality.High);
                return resizedImage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resizing image: {ex.Message}");
                return null;
            }
        }




        private void LoadInvoices(object sender, EventArgs e)
        {
            // Esimerkki laskujen lisäämisestä listalle
            Invoices.Add(new Invou { FullName = "Asiakas Yksi", InvoiceId = "10001", InvoiceDate = DateTime.Now });
            Invoices.Add(new InvoiceData { FullName = "Asiakas Kaksi", InvoiceId = "10002", InvoiceDate = DateTime.Now });
            InvoicesListView.ItemsSource = null;
            InvoicesListView.ItemsSource = Invoices;
        }
        private void CreateInvoicePdf(object sender, EventArgs e)
        {
            if (InvoicesListView.SelectedItem is InvoiceData selectedInvoice)
            {
                var filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Invoice.pdf");
                CreatePdfAsync(selectedInvoice, filePath);
            }
        }
        public void CreateFooter(SKCanvas canvas, int pageWidth, int pageHeight)
        {
            // Oletetaan, että canvas on SKCanvas ja sivun koko on A4 (595 x 842 pistettä)
            var textPaint = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.Black,
                TextAlign = SKTextAlign.Center, // Teksti keskitetään jokaisen alueen keskelle
                TextSize = 15,
            };
            var linePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2,
                Color = SKColors.Black
            };
            var textPaint2 = new SKPaint
            {
                IsAntialias = true,
                Color = SKColors.Black,
                TextAlign = SKTextAlign.Left,
                TextSize = 15
            };

            int footerHeight = 30; // Footerin korkeus
            int footerY = pageHeight + 175; // Footerin Y-koordinaatti sivun alalaidassa

            int sectionWidth = pageWidth / 3;
            canvas.DrawText("Eräpäivä", sectionWidth / 2 - 50, footerY, textPaint);
            canvas.DrawText("Viitenumero", 350, footerY, textPaint);
            canvas.DrawText("YHTEENSÄ", 3 * sectionWidth + (sectionWidth / 2) + 90, footerY, textPaint);
            canvas.DrawText("14.10.2024", sectionWidth / 2 - 42, footerY + 18, textPaint);
            canvas.DrawText("Viitenumero", 350, footerY, textPaint);
            canvas.DrawText("123456789", 345, footerY + 18, textPaint);
            canvas.DrawText("YHTEENSÄ", 3 * sectionWidth + (sectionWidth / 2) + 90, footerY, textPaint);
            canvas.DrawText("1000,00 €", 775, footerY + 18, textPaint);

            float upperY = 1050;  // Laatikon ylärajan Y-koordinaatti
            float lowerY = upperY - 50; // Laatikon alarajan Y-koordinaatti (30 pisteet ylärajan alapuolella)

            float leftX = 15;    // Laatikon vasemman reunan X-koordinaatti
            float rightX = 825;  // Laatikon oikean reunan X-koordinaatti
            // yläreuna
            canvas.DrawLine(leftX, upperY, rightX, upperY, linePaint);
            // alareuna
            canvas.DrawLine(leftX, lowerY, rightX, lowerY, linePaint);
            // vasen reuna
            canvas.DrawLine(leftX, upperY, leftX, lowerY, linePaint);
            // oikea reuna
            canvas.DrawLine(rightX, upperY, rightX, lowerY, linePaint);



            float xMargin = 20;
            float yStart = 1100;
            // Piirrä yhteystiedot
            canvas.DrawLine(15, 1070, 825, 1070, linePaint);
            canvas.DrawText("Village Newbies Oy", xMargin + 63, yStart, textPaint);
            canvas.DrawText("Siilokatu 1", xMargin, yStart + 25, textPaint2);
            canvas.DrawText("90700 OULU", xMargin, yStart + 50, textPaint2);
            canvas.DrawText("Y-tunnus: 1234567-8", xMargin + 280, yStart - 5, textPaint2);
            canvas.DrawText("Puh: 040 123 4567", xMargin + 280, yStart + 20, textPaint2);
            canvas.DrawText("laskutus@villagenewbies.fi", xMargin + 280, yStart + 45, textPaint2);
            canvas.DrawText("www.villagenewbies.fi", xMargin + 560, yStart - 5, textPaint2);
            canvas.DrawText("IBAN: FI12 3456 7890 1234 56", xMargin + 560, yStart + 20, textPaint2);
            canvas.DrawText("BIC: NDEAFIHH", xMargin + 560, yStart + 45, textPaint2);
        }

        private async void CreatePdfAsync(InvoiceData invoiceData, string filePath)
        {
            int pageWidth = 840; // A4-leveys pisteinä
            int pageHeight = 1188; // A4-korkeus pisteinä


            string imagePath = @"C:\Users\NikoJ\source\repos\pdfapp\pdfapp\Resources\Images\mokkimasterlogo.png";
            SKBitmap logoBitmap = LoadImageFromFile(imagePath);


            Debug.WriteLine("logoBitmap " + logoBitmap);
            using (var document = SKDocument.CreatePdf(filePath))
            {
                // Sivun koko
                var canvas = document.BeginPage(840, 1188);
                var backgroundColor = SKColor.Parse("#bdc8d6");
                var backgroundpaint = new SKPaint
                { Style = SKPaintStyle.Fill, Color = backgroundColor };

                var textPaint = new SKPaint
                {
                    IsAntialias = true,
                    Color = SKColors.Black,
                    TextAlign = SKTextAlign.Left,
                    TextSize = 20
                };

                var titlePaint = new SKPaint
                {
                    IsAntialias = true,
                    Color = SKColors.Black,
                    TextAlign = SKTextAlign.Center,
                    TextSize = 30,
                    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                };

                var linePaint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 2,
                    Color = SKColors.Black
                };
                var assembly = Assembly.GetExecutingAssembly();
                var resourceNames = assembly.GetManifestResourceNames();
                foreach (var name in resourceNames)
                {
                    Debug.WriteLine("Mitä resursseja käytössä", name);  // Tulosta kaikki resurssinimet
                }
                // Taustaväri
                canvas.DrawRect(0, 0, pageWidth, pageHeight, backgroundpaint);

                // Lataa ja piirrä logo

                if (logoBitmap != null)
                {
                    canvas.DrawBitmap(logoBitmap, new SKPoint(110, 230)); // Aseta logo sivun vasempaan yläkulmaan
                }
                else
                {
                    Debug.WriteLine("Logo could not be loaded.");
                }
                // Otsikko
                canvas.DrawText("LASKU", 550, 50, titlePaint);
                canvas.DrawText("Laskun numero:", 500, 90, textPaint);
                canvas.DrawText("Laskun:", 500, 120, textPaint);
                canvas.DrawText("päivämäärä:", 500, 150, textPaint);
                canvas.DrawText("Eräpäivä:", 500, 185, textPaint);
                // Alusta y-akselin sijainti
                int baseY = 350; // Voit muuttaa tätä arvoa liikuttaaksesi kaikkia rivejä ylös tai alas

                // asiakkaan ja laskun tiedot
                canvas.DrawText("Asiakkaan nimi:", 20, baseY, textPaint);
                canvas.DrawText("Osoite:", 20, baseY + 30, textPaint);
                canvas.DrawText("Postinumero ja Kaupunki:", 20, baseY + 60, textPaint);

                // Y-koordinaatin päivitys otsikoille
                int titlesY = baseY + 150; // Lisätään 150 etäisyyttä asiakastietoihin nähden

                // otsikot
                canvas.DrawText("Selite", 20, titlesY, textPaint);
                canvas.DrawText("Määrä", 240, titlesY, textPaint);
                canvas.DrawText("Hinta", 440, titlesY, textPaint);
                canvas.DrawText("ALV", 620, titlesY, textPaint);
                canvas.DrawText("Yhteensä", 730, titlesY, textPaint);
                canvas.DrawLine(15, titlesY + 20, 825, titlesY + 20, linePaint);

                // Rivi-itemsit: alkavat 20 pistettä viivan alapuolelta
                int itemsStartY = titlesY + 40;

                // Piirrä tuoterivit
                for (int i = 0; i < 5; i++)
                {
                    canvas.DrawText("Tuote " + (i + 1), 20, itemsStartY, textPaint);
                    canvas.DrawText("2", 240, itemsStartY, textPaint);
                    canvas.DrawText("100,00 €", 440, itemsStartY, textPaint);
                    canvas.DrawText("24%", 620, itemsStartY, textPaint);
                    canvas.DrawText("248,00 €", 730, itemsStartY, textPaint);
                    itemsStartY += 30; // Joka rivi siirtyy 30 pistettä alaspäin
                }

                // FOOTER
                CreateFooter(canvas, 595, 842);

                document.EndPage();
                document.Close();
            }

            // Avaa PDF-tiedosto
            if (File.Exists(filePath))
            {
                try
                {
                    await Launcher.OpenAsync(new OpenFileRequest
                    {
                        File = new ReadOnlyFile(filePath)
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Failed to open PDF: " + ex.Message);
                }
            }
        }
    }
}
