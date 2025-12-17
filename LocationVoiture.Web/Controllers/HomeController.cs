using LocationVoiture.Core.Models;
using LocationVoiture.Data;
using LocationVoiture.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LocationVoiture.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly VoitureRepository _voitureRepo;

        public HomeController()
        {
            // On utilise notre Repository existant !
            _voitureRepo = new VoitureRepository();
        }

        public IActionResult Index()
        {
            // Récupérer toutes les voitures
            var voitures = _voitureRepo.GetAll();

            // On ne veut afficher que les voitures "Disponibles" ou "Louées" (pas celles en Entretien)
            // Et on peut filtrer côté code si besoin. Pour l'instant, on envoie tout.
            return View(voitures);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult GetImage(int id)
        {
            // 1. On récupère la voiture
            var voiture = _voitureRepo.GetAll().FirstOrDefault(v => v.Id == id);

            // 2. Vérifications de sécurité
            if (voiture == null || string.IsNullOrEmpty(voiture.ImageChemin))
            {
                return Redirect("https://via.placeholder.com/300x200?text=Pas+d'image");
            }

            // 3. Vérifier si le fichier existe physiquement sur le disque
            if (!System.IO.File.Exists(voiture.ImageChemin))
            {
                return Redirect($"https://via.placeholder.com/300x200?text={voiture.Marque}+Image+Introuvable");
            }

            // 4. Lire le fichier et le renvoyer comme une image
            var imageBytes = System.IO.File.ReadAllBytes(voiture.ImageChemin);

            // On devine le type (jpg ou png)
            string contentType = "image/jpeg";
            if (voiture.ImageChemin.EndsWith(".png")) contentType = "image/png";

            return File(imageBytes, contentType);
        }
    }
}