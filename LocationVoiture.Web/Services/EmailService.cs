using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace LocationVoiture.Web.Services
{
    public class EmailService
    {
        
        
        private string _monEmail = "oussama.zahidafaqbarca@gmail.com";
        private string _monMotDePasse = "ambk pzuq psxz ofcp";

        public async Task EnvoyerEmailAsync(string emailDestinataire, string sujet, string messageHtml)
        {
            try
            {
                // 1. Configuration du Client SMTP (Le facteur)
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true; // Sécurisé
                smtp.Credentials = new NetworkCredential(_monEmail, _monMotDePasse);

                // 2. Création du message
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress(_monEmail, "LocationVoiture App");
                mail.To.Add(emailDestinataire);
                mail.Subject = sujet;
                mail.Body = messageHtml;
                mail.IsBodyHtml = true; // Important pour le design HTML

                // 3. Envoi (Asynchrone pour ne pas bloquer le site)
                await smtp.SendMailAsync(mail);
            }
            catch (System.Exception ex)
            {
                // En cas d'erreur (ex: mauvais mot de passe), on ne fait rien pour ne pas planter le site
                // Dans un vrai projet, on loguerait l'erreur dans un fichier.
                System.Diagnostics.Debug.WriteLine("Erreur Email : " + ex.Message);
            }
        }
    }
}