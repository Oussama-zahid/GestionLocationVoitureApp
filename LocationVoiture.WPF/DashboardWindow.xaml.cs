using System;
using System.Data.SqlClient;
using System.Windows;
using LocationVoiture.Data;

namespace LocationVoiture.WPF
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
            ChargerStatistiques();
        }

        private void ChargerStatistiques()
        {
            try
            {
                using (SqlConnection con = Database.GetConnection())
                {
                    con.Open();

                    // 1. Total Voitures
                    txtNbVoitures.Text = GetCount(con, "SELECT COUNT(*) FROM Voitures").ToString();

                    // 2. Voitures Disponibles
                    txtDisponibles.Text = GetCount(con, "SELECT COUNT(*) FROM Voitures WHERE Statut = 'Disponible'").ToString();

                    // 3. Voitures Louées (Gère 'Louee' et 'Louée')
                    txtLouees.Text = GetCount(con, "SELECT COUNT(*) FROM Voitures WHERE Statut = 'Louee' OR Statut = 'Louée'").ToString();

                    // 4. Voitures en Entretien
                    txtEntretien.Text = GetCount(con, "SELECT COUNT(*) FROM Voitures WHERE Statut = 'Entretien'").ToString();

                    // 5. Demandes en Attente
                    txtAttente.Text = GetCount(con, "SELECT COUNT(*) FROM Locations WHERE Statut = 'En attente'").ToString();

                    // 6. Chiffre d'Affaires
                    SqlCommand cmdCA = new SqlCommand("SELECT ISNULL(SUM(PrixTotal), 0) FROM Locations WHERE Statut = 'Active' OR Statut = 'Terminee'", con);
                    decimal ca = (decimal)cmdCA.ExecuteScalar();
                    txtChiffreAffaires.Text = ca.ToString("N0") + " DH";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur chargement stats : " + ex.Message);
            }
        }

        // Fonction utilitaire pour éviter de répéter le code
        private int GetCount(SqlConnection con, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}