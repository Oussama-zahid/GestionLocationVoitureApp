namespace LocationVoiture.Core.Models
{
    public class Client : Personne
    {
        public string NumeroPermis { get; set; }
        public string Adresse { get; set; }
        public string Telephone { get; set; }

        // Constructeur qui utilise base()
        public Client(string nom, string prenom, string email, string permis)
            : base(nom, prenom, email)
        {
            NumeroPermis = permis;
        }

        public Client() { } // Constructeur vide requis pour la sérialisation/BDD
    }
}