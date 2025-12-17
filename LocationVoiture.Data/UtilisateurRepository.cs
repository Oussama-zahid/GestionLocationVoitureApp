using System.Data.SqlClient;

namespace LocationVoiture.Data
{
    public class UtilisateurRepository
    {
        // Vérifie si l'utilisateur existe et retourne son ID (ou 0 si échec)
        public bool ValiderConnexion(string email, string motDePasse)
        {
            // Note: Dans un vrai projet pro, on hacherait le mot de passe (BCrypt/SHA256).
            // Pour ce projet scolaire, on compare en texte clair comme souvent demandé au début.
            string query = "SELECT COUNT(*) FROM Utilisateurs WHERE Email = @Email AND MotDePasse = @Mdp AND Role = 'Admin'";

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Mdp", motDePasse);

                int count = (int)cmd.ExecuteScalar(); // Retourne le nombre de lignes trouvées
                return count > 0;
            }
        }
    }
}