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

        public IActionResult Index(string recherche, string tri, int page = 1)
        {
            int taillePage = 6; // Nombre de voitures par page
            var voitures = _voitureRepo.GetAll(); // Récupère tout (idéalement, faites le filtre en SQL, mais ça ira pour l'instant)

            // 1. RECHERCHE
            if (!string.IsNullOrEmpty(recherche))
            {
                recherche = recherche.ToLower();
                voitures = voitures.Where(v => v.Marque.ToLower().Contains(recherche) ||
                                               v.Modele.ToLower().Contains(recherche)).ToList();
            }

            // 2. TRI
            switch (tri)
            {
                case "prix_croissant":
                    voitures = voitures.OrderBy(v => v.PrixParJour).ToList();
                    break;
                case "prix_decroissant":
                    voitures = voitures.OrderByDescending(v => v.PrixParJour).ToList();
                    break;
                default: // Par défaut : les plus récentes
                    voitures = voitures.OrderByDescending(v => v.Id).ToList();
                    break;
            }

            // 3. PAGINATION
            int totalVoitures = voitures.Count();
            var voituresAffichees = voitures.Skip((page - 1) * taillePage).Take(taillePage).ToList();

            // On passe les infos à la Vue via ViewBag pour gérer les boutons Suivant/Précédent
            ViewBag.PageActuelle = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalVoitures / taillePage);
            ViewBag.RechercheActuelle = recherche; // Pour garder le texte dans la barre
            ViewBag.TriActuel = tri; // Pour garder le tri sélectionné

            return View(voituresAffichees);
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