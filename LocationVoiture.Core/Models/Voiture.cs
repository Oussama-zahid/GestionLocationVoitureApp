namespace LocationVoiture.Core.Models
{
    public class Voiture
    {
        public int Id { get; set; }
        public string Marque { get; set; }
        public string Modele { get; set; }
        public int Annee { get; set; }
        public string Carburant { get; set; } // Essence/Diesel
        public decimal PrixParJour { get; set; }
        public string Immatriculation { get; set; }
        public string Statut { get; set; } // Disponible, Louée, Entretien
        public string Transmission { get; set; } // Manuelle/Automatique
        public string ImageChemin { get; set; } // Pour l'affichage WPF

        public override string ToString()
        {
            return $"{Marque} {Modele} - {Immatriculation}";
        }
    }
}