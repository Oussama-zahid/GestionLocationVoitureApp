using System;

namespace LocationVoiture.Core.Models
{
    public class Location
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int VoitureId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime DateFin { get; set; }
        public decimal PrixTotal { get; set; }
        public string Statut { get; set; } // En attente, Active, Terminée

        // Propriétés d'affichage (Peuplées via JOIN SQL)
        public string NomClient { get; set; }     // Pour afficher "Youssef El Amrani" au lieu de "1"
        public string VoitureModele { get; set; } // Pour afficher "Dacia Logan" au lieu de "4"
    }
}