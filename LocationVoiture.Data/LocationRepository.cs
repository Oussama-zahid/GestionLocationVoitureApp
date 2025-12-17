using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using LocationVoiture.Core.Models;

namespace LocationVoiture.Data
{
    public class LocationRepository
    {
        // 1. Récupérer les locations avec les Noms Clients et Modèles Voitures
        public List<Location> GetAll()
        {
            List<Location> liste = new List<Location>();
            // La Jointure (JOIN) permet de lier les tables entre elles
            string query = @"
                SELECT L.Id, L.ClientId, L.VoitureId, L.DateDebut, L.DateFin, L.PrixTotal, L.Statut, 
                C.Nom, C.Prenom, 
                V.Marque, V.Modele 
                FROM Locations L
                INNER JOIN Clients C ON L.ClientId = C.Id
                INNER JOIN Voitures V ON L.VoitureId = V.Id
                ORDER BY L.Id DESC";// Les plus récentes en premier

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liste.Add(new Location
                        {
                            Id = (int)reader["Id"],
                            ClientId = (int)reader["ClientId"],
                            VoitureId = (int)reader["VoitureId"],
                            DateDebut = (DateTime)reader["DateDebut"],
                            DateFin = (DateTime)reader["DateFin"],
                            PrixTotal = (decimal)reader["PrixTotal"],
                            Statut = (string)reader["Statut"],
                            // On remplit les propriétés d'affichage
                            NomClient = $"{reader["Nom"]} {reader["Prenom"]}",
                            VoitureModele = $"{reader["Marque"]} {reader["Modele"]}"
                        });
                    }
                }
            }
            return liste;
        }

        // 2. Créer une location
        // Mettre à jour le statut (Valider ou Refuser)
        public void UpdateStatut(int locationId, string nouveauStatut)
        {
            string query = "UPDATE Locations SET Statut = @Statut WHERE Id = @Id";

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@Statut", nouveauStatut);
                cmd.Parameters.AddWithValue("@Id", locationId);

                cmd.ExecuteNonQuery();
            }
        }

        // MÉTHODE NÉCESSAIRE POUR LE SITE WEB (CLIENT)
        public void Ajouter(Location loc)
        {
            string query = @"INSERT INTO Locations (ClientId, VoitureId, DateDebut, DateFin, PrixTotal, Statut) 
                     VALUES (@CId, @VId, @Debut, @Fin, @Prix, @Statut)";

            using (SqlConnection con = Database.GetConnection())
            {
                con.Open();
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@CId", loc.ClientId);
                cmd.Parameters.AddWithValue("@VId", loc.VoitureId);
                cmd.Parameters.AddWithValue("@Debut", loc.DateDebut);
                cmd.Parameters.AddWithValue("@Fin", loc.DateFin);
                cmd.Parameters.AddWithValue("@Prix", loc.PrixTotal);

                // Le site Web enverra toujours "En attente"
                cmd.Parameters.AddWithValue("@Statut", loc.Statut);

                cmd.ExecuteNonQuery();
            }
        }
    }
}