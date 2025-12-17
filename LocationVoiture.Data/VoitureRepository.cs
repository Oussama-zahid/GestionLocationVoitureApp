using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using LocationVoiture.Core.Models; // On utilise les modèles du Core

namespace LocationVoiture.Data
{
    public class VoitureRepository
    {
        // LECTURE (READ)
        // Dans LocationVoiture.Data -> VoitureRepository.cs

        public List<Voiture> GetAll()
        {
            List<Voiture> liste = new List<Voiture>();
            string query = "SELECT * FROM Voitures";

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // ⚠️ L'erreur vient souvent d'ici :
                    // Ne jamais essayer de lire reader["..."] AVANT cette ligne while !

                    while (reader.Read()) // Tant qu'il y a des lignes à lire
                    {
                        // C'est SEULEMENT ICI qu'on peut lire les données
                        Voiture v = new Voiture
                        {
                            Id = (int)reader["Id"],
                            Marque = (string)reader["Marque"],
                            Modele = (string)reader["Modele"],
                            Annee = (int)reader["Annee"],
                            Carburant = (string)reader["Carburant"],
                            PrixParJour = (decimal)reader["PrixParJour"],
                            Immatriculation = (string)reader["Immatriculation"],
                            Statut = (string)reader["Statut"],
                            Transmission = (string)reader["Transmission"],
                            // Gestion sécurisée de l'image (si null dans la BDD)
                            ImageChemin = reader["ImageChemin"] == DBNull.Value ? null : (string)reader["ImageChemin"]
                        };
                        liste.Add(v);
                    }
                }
            }
            return liste;
        }

        // AJOUT (CREATE)
        public void Ajouter(Voiture v)
        {
            string query = "INSERT INTO Voitures (Marque, Modele, Annee, Carburant, PrixParJour, Immatriculation, Statut, Transmission,ImageChemin) " +
                           "VALUES (@Marque, @Modele, @Annee, @Carburant, @Prix, @Immat, @Statut, @Trans, @Img)";

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);

                
                cmd.Parameters.AddWithValue("@Marque", v.Marque);
                cmd.Parameters.AddWithValue("@Modele", v.Modele);
                cmd.Parameters.AddWithValue("@Annee", v.Annee);
                cmd.Parameters.AddWithValue("@Carburant", v.Carburant);
                cmd.Parameters.AddWithValue("@Prix", v.PrixParJour);
                cmd.Parameters.AddWithValue("@Immat", v.Immatriculation);
                cmd.Parameters.AddWithValue("@Statut", "Disponible");
                cmd.Parameters.AddWithValue("@Trans", v.Transmission);
                cmd.Parameters.AddWithValue("@Img", (object)v.ImageChemin ?? DBNull.Value);
                cmd.ExecuteNonQuery(); // [cite: 681] Pour Insert/Update/Delete
            }
        }

        // SUPPRIMER UNE VOITURE (DELETE)
        public void Supprimer(int id)
        {
            string query = "DELETE FROM Voitures WHERE Id = @Id";
            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        // Mettre à jour le statut de la voiture (Ex: "Louée", "Disponible")
        public void UpdateStatut(int id, string nouveauStatut)
        {
            string query = "UPDATE Voitures SET Statut = @Statut WHERE Id = @Id";

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Statut", nouveauStatut);
                cmd.Parameters.AddWithValue("@Id", id);

                cmd.ExecuteNonQuery();
            }
        }
    }
}