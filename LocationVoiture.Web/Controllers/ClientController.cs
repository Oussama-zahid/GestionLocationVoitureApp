using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Pour la Session
using LocationVoiture.Data;
using LocationVoiture.Core.Models;

namespace LocationVoiture.Web.Controllers
{
    public class ClientController : Controller
    {
        // INSCRIPTION (GET : Affiche le formulaire)
        public IActionResult Inscription()
        {
            return View();
        }

        // INSCRIPTION (POST : Traite le formulaire)
        [HttpPost]
        public IActionResult Inscription(Client client, string MotDePasseConfirmation)
        {
            if (client.MotDePasse != MotDePasseConfirmation)
            {
                ViewBag.Erreur = "Les mots de passe ne correspondent pas.";
                return View();
            }

            try
            {
                // On utilise le ClientRepository (à créer/vérifier juste après)
                ClientRepository repo = new ClientRepository();
                repo.Ajouter(client); // Il faudra ajouter cette méthode dans le Repo !

                return RedirectToAction("Connexion");
            }
            catch (System.Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                return View();
            }
        }

        // CONNEXION (GET)
        public IActionResult Connexion()
        {
            return View();
        }

        // CONNEXION (POST)
        [HttpPost]
        public IActionResult Connexion(string email, string motDePasse)
        {
            ClientRepository repo = new ClientRepository();
            Client client = repo.ValiderClient(email, motDePasse); // Méthode à créer

            if (client != null)
            {
                // On stocke les infos du client en Session
                HttpContext.Session.SetInt32("ClientId", client.Id);
                HttpContext.Session.SetString("ClientNom", client.Nom);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Erreur = "Email ou mot de passe incorrect.";
                return View();
            }
        }

        // DECONNEXION
        public IActionResult Deconnexion()
        {
            HttpContext.Session.Clear(); // On vide la session
            return RedirectToAction("Index", "Home");
        }
    }
}