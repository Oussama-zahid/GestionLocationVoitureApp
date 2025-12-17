using System.Collections.Generic;
using System.Data.SqlClient;
using LocationVoiture.Core.Models;

namespace LocationVoiture.Data
{
    public class ClientRepository
    {
        public List<Client> GetAll()
        {
            List<Client> liste = new List<Client>();
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, Nom, Prenom, Email FROM Clients", con);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liste.Add(new Client
                        {
                            Id = (int)reader["Id"],
                            Nom = (string)reader["Nom"],
                            Prenom = (string)reader["Prenom"],
                            Email = (string)reader["Email"]
                        });
                    }
                }
            }
            return liste;
        }
        public void Ajouter(Client c)
        {
            string query = "INSERT INTO Clients (Nom, Prenom, Email, MotDePasse, NumeroPermis, Adresse, Telephone) " +
                           "VALUES (@Nom, @Prenom, @Email, @Mdp, @Permis, @Adr, @Tel)";

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Nom", c.Nom);
                cmd.Parameters.AddWithValue("@Prenom", c.Prenom);
                cmd.Parameters.AddWithValue("@Email", c.Email);
                cmd.Parameters.AddWithValue("@Mdp", c.MotDePasse); // En clair pour ce projet
                cmd.Parameters.AddWithValue("@Permis", c.NumeroPermis);
                cmd.Parameters.AddWithValue("@Adr", c.Adresse);
                cmd.Parameters.AddWithValue("@Tel", c.Telephone);

                cmd.ExecuteNonQuery();
            }
        }
        public Client ValiderClient(string email, string mdp)
        {
            Client c = null;
            string query = "SELECT * FROM Clients WHERE Email = @Email AND MotDePasse = @Mdp";

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Mdp", mdp);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        c = new Client
                        {
                            Id = (int)reader["Id"],
                            Nom = (string)reader["Nom"],
                            Prenom = (string)reader["Prenom"],
                            Email = (string)reader["Email"],
                            NumeroPermis = (string)reader["NumeroPermis"]
                        };
                    }
                }
            }
            return c;
        }
    }
}