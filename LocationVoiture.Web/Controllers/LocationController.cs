using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Pour la Session
using LocationVoiture.Data;
using LocationVoiture.Core.Models;
using System.Linq;
using System;
using LocationVoiture.Web.Services;

namespace LocationVoiture.Web.Controllers
{
    public class LocationController : Controller
    {
        // 1. AFFICHER LE FORMULAIRE DE RÉSERVATION
        public IActionResult Reserver(int id)
        {
            // Vérifier si le client est connecté
            if (HttpContext.Session.GetInt32("ClientId") == null)
            {
                // Si pas connecté, on le renvoie vers la page de connexion
                return RedirectToAction("Connexion", "Client");
            }

            // Récupérer les infos de la voiture pour les afficher
            VoitureRepository repo = new VoitureRepository();
            var voiture = repo.GetAll().FirstOrDefault(v => v.Id == id);

            if (voiture == null) return RedirectToAction("Index", "Home");

            // On prépare une location vide avec l'ID de la voiture
            Location location = new Location
            {
                VoitureId = voiture.Id,
                VoitureModele = $"{voiture.Marque} {voiture.Modele}",
                PrixTotal = voiture.PrixParJour, // On stocke le prix unitaire temporairement pour le calcul JS
                DateDebut = DateTime.Today.AddDays(1),
                DateFin = DateTime.Today.AddDays(3)
            };

            return View(location);
        }

        // 2. TRAITER LA DEMANDE (POST)
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> ValiderReservation(Location location) // Notez le "async Task"
        {
            int? clientId = HttpContext.Session.GetInt32("ClientId");
            // On récupère aussi l'email du client stocké en session (si vous l'aviez mis)
            // Sinon, on va le chercher en base (plus sûr).

            if (clientId == null) return RedirectToAction("Connexion", "Client");

            try
            {
                // ... (Votre code existant pour le calcul des jours/prix) ...
                VoitureRepository vRepo = new VoitureRepository();
                var voiture = vRepo.GetAll().FirstOrDefault(v => v.Id == location.VoitureId);
                TimeSpan duree = location.DateFin - location.DateDebut;
                location.PrixTotal = duree.Days * voiture.PrixParJour;
                location.ClientId = clientId.Value;
                location.Statut = "En attente";

                // 1. Sauvegarde en BDD
                LocationRepository lRepo = new LocationRepository();
                lRepo.Ajouter(location);

                // 2. --- ENVOI DE L'EMAIL ---

                // On récupère l'email du client
                ClientRepository cRepo = new ClientRepository();
                // Petite astuce : on récupère tous les clients et on filtre (ou ajoutez GetById dans le Repo)
                var client = cRepo.GetAll().FirstOrDefault(c => c.Id == clientId.Value);

                if (client != null)
                {
                    EmailService emailService = new EmailService();
                    string sujet = "Confirmation de votre demande de réservation";
                    string corps = $@"
                <h1>Bonjour {client.Prenom},</h1>
                <p>Votre demande pour la <strong>{voiture.Marque} {voiture.Modele}</strong> a bien été reçue.</p>
                <p><strong>Dates :</strong> Du {location.DateDebut:dd/MM/yyyy} au {location.DateFin:dd/MM/yyyy}</p>
                <p><strong>Total estimé :</strong> {location.PrixTotal} DH</p>
                <p>Notre équipe va valider votre dossier sous peu.</p>
                <br/>
                <p>Cordialement,<br/>L'équipe LocationVoiture</p>";

                    // On attend l'envoi
                    await emailService.EnvoyerEmailAsync(client.Email, sujet, corps);
                }

                return RedirectToAction("Confirmation");
            }
            catch (System.Exception ex)
            {
                ViewBag.Erreur = "Erreur : " + ex.Message;
                return View("Reserver", location);
            }
        }

        // 3. PAGE DE CONFIRMATION
        public IActionResult Confirmation()
        {
            return View();
        }

        // 4. MES RESERVATIONS (Historique Client)
        public IActionResult MesReservations()
        {
            int? clientId = HttpContext.Session.GetInt32("ClientId");
            if (clientId == null) return RedirectToAction("Connexion", "Client");

            LocationRepository repo = new LocationRepository();
            // On filtre pour n'avoir que les locations de CE client
            var mesLocs = repo.GetAll().Where(l => l.ClientId == clientId.Value).ToList();

            return View(mesLocs);
        }
    }
}