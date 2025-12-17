using System;
using System.IO;
using System.Windows; // Pour MessageBox
using LocationVoiture.Core.Models;
using QRCoder;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
// J'ai retiré 'using iText.Layout.Properties;' pour éviter le conflit.
// Nous allons appeler les propriétés directement par leur nom complet ci-dessous.

namespace LocationVoiture.WPF.Services
{
    public class PdfService
    {
        public void GenererBonReservation(Location location)
        {
            try
            {
                // 1. Définir le chemin sur le Bureau
                string dossier = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "BonsLocations");
                if (!Directory.Exists(dossier)) Directory.CreateDirectory(dossier);

                // --- CORRECTION ICI : Ajout d'un timestamp pour rendre le nom unique ---
                string dateHeure = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string cheminFichier = Path.Combine(dossier, $"Bon_{location.Id}_{dateHeure}.pdf");

                // 2. Initialiser le PDF
                PdfWriter writer = new PdfWriter(cheminFichier);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // 3. Titre
                document.Add(new Paragraph("BON DE RÉSERVATION")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SetBold());

                document.Add(new Paragraph("\n"));

                // 4. Tableau
                Table table = new Table(2);
                table.SetWidth(iText.Layout.Properties.UnitValue.CreatePercentValue(100));

                table.AddCell("Numéro de Réservation :");
                table.AddCell(location.Id.ToString());

                table.AddCell("Client :");
                table.AddCell(location.NomClient);

                table.AddCell("Véhicule :");
                table.AddCell(location.VoitureModele);

                table.AddCell("Période :");
                table.AddCell($"Du {location.DateDebut:dd/MM/yyyy} Au {location.DateFin:dd/MM/yyyy}");

                table.AddCell("Prix Total :");
                table.AddCell($"{location.PrixTotal} DH");

                document.Add(table);

                // 5. Générer QR Code
                string contenuQR = $"LOCATION-ID:{location.Id}|CLIENT:{location.NomClient}|STATUT:VALIDE";
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(contenuQR, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeBytes = qrCode.GetGraphic(20);

                // 6. Ajouter Image QR
                ImageData imageData = ImageDataFactory.Create(qrCodeBytes);
                Image qrImage = new Image(imageData)
                    .SetWidth(150)
                    .SetHorizontalAlignment(iText.Layout.Properties.HorizontalAlignment.CENTER);

                document.Add(new Paragraph("\nScannez ce code pour vérifier la réservation :")
                    .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER));

                document.Add(qrImage);

                // 7. Fermer
                document.Close();

                // 8. Ouvrir le PDF automatiquement
                var p = new System.Diagnostics.Process();
                p.StartInfo = new System.Diagnostics.ProcessStartInfo(cheminFichier) { UseShellExecute = true };
                p.Start();
            }
            catch (Exception ex)
            {
                // On affiche l'erreur complète pour mieux comprendre si ça replante
                MessageBox.Show("Erreur PDF : " + ex.ToString());
            }
        }
    }
}