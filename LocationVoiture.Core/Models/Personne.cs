namespace LocationVoiture.Core.Models
{
    // Classe de base
    public class Personne
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Email { get; set; }
        public string MotDePasse { get; set; } // Ajouté pour la connexion

        // Constructeur
        public Personne() { }

        public Personne(string nom, string prenom, string email)
        {
            Nom = nom;
            Prenom = prenom;
            Email = email;
        }

        public override string ToString()
        {
            return $"{Nom} {Prenom}"; // [cite: 469] Redéfinition de ToString
        }
    }
}