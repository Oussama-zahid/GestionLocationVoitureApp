using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using LocationVoiture.Core.Models;
using LocationVoiture.Data;
using LocationVoiture.WPF.Services;// On appelle notre couche Data

namespace LocationVoiture.WPF
{
    public partial class MainWindow : Window
    {
        // Instance de notre Repository pour parler à SQL Server
        private VoitureRepository _repo = new VoitureRepository();

        public MainWindow()
        {
            InitializeComponent();
            ChargerDonnees(); // Charger la liste au démarrage
        }

        // Fonction pour récupérer les données et les mettre dans la Grille
        private void ChargerDonnees()
        {
            try
            {
                List<Voiture> voitures = _repo.GetAll();
                dgVoitures.ItemsSource = voitures; // Liaison de données (Data Binding)
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors du chargement : " + ex.Message);
            }
        }

        // Événement Clic Bouton "Rafraîchir"
        private void btnCharger_Click(object sender, RoutedEventArgs e)
        {
            ChargerDonnees();
        }

        // Événement Clic Bouton "Ajouter"
        // Action : REFUSER
        private void btnRefuser_Click(object sender, RoutedEventArgs e)
        {
            // 1. Vérifier si une ligne est sélectionnée
            if (dgLocations.SelectedItem is Location locSelectionnee)
            {
                if (locSelectionnee.Statut != "En attente")
                {
                    MessageBox.Show($"Cette location est déjà '{locSelectionnee.Statut}'.");
                    return;
                }

                try
                {
                    // --- ETAPE 1 : Mettre la location en "Annulée" ---
                    LocationRepository repo = new LocationRepository();
                    repo.UpdateStatut(locSelectionnee.Id, "Annulee");

                    // --- ETAPE 2 (AJOUT) : S'assurer que la voiture est bien "Disponible" ---
                    // C'est ici que vous ajoutez votre ligne !
                    VoitureRepository repoVoiture = new VoitureRepository();
                    repoVoiture.UpdateStatut(locSelectionnee.VoitureId, "Disponible");

                    MessageBox.Show("Demande refusée. La voiture est rendue disponible.", "Succès");

                    // 4. Rafraîchir les deux listes (Locations et Voitures)
                    ChargerLocations();
                    ChargerDonnees(); // Important pour voir le statut changer dans l'onglet Voitures
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur : " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une demande à refuser.");
            }
        }

        // Événement Clic Bouton "Supprimer"
        private void btnSupprimer_Click(object sender, RoutedEventArgs e)
        {
            // Vérifier si une ligne est sélectionnée
            if (dgVoitures.SelectedItem is Voiture voitureSelectionnee)
            {
                MessageBoxResult resultat = MessageBox.Show($"Voulez-vous supprimer la {voitureSelectionnee.Marque} ?",
                    "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (resultat == MessageBoxResult.Yes)
                {
                    _repo.Supprimer(voitureSelectionnee.Id);
                    ChargerDonnees();
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une voiture à supprimer.");
            }
        }
        // --- LOGIQUE LOCATIONS ---

        private void btnRefreshLoc_Click(object sender, RoutedEventArgs e)
        {
            ChargerLocations();
        }

        private void ChargerLocations()
        {
            try
            {
                LocationRepository repo = new LocationRepository();
                List<Location> toutesLocations = repo.GetAll();

                // Petit filtre simple : Si la case est cochée, on ne montre que les "En attente"
                if (chkFiltre.IsChecked == true)
                {
                    // Utilisation de LINQ pour filtrer la liste en mémoire
                    dgLocations.ItemsSource = toutesLocations.Where(l => l.Statut == "En attente").ToList();
                }
                else
                {
                    dgLocations.ItemsSource = toutesLocations;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur chargement locations : " + ex.Message);
            }
        }
        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            if (dgLocations.SelectedItem is Location locSelectionnee)
            {
                if (locSelectionnee.Statut != "En attente")
                {
                    MessageBox.Show("Cette demande est déjà traitée.");
                    return;
                }

                try
                {
                    // 1. Mettre à jour la LOCATION (Passe à "Active")
                    LocationRepository repoLoc = new LocationRepository();
                    repoLoc.UpdateStatut(locSelectionnee.Id, "Active");

                    // 2. Mettre à jour la VOITURE (Passe à "Louée") <--- AJOUT ICI
                    VoitureRepository repoVoiture = new VoitureRepository();
                    repoVoiture.UpdateStatut(locSelectionnee.VoitureId, "Louee"); // Ou "Indisponible"

                    // 3. Générer le PDF (votre code existant)
                    PdfService pdfService = new PdfService();
                    pdfService.GenererBonReservation(locSelectionnee);

                    MessageBox.Show("Location validée et Voiture marquée comme 'Louée' !");

                    // 4. Rafraîchir les listes
                    ChargerLocations();
                    ChargerDonnees(); // Rafraîchit aussi l'onglet Voitures pour voir le changement de statut
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur : " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une demande.");
            }
        }
        private void TraiterLocation(string nouveauStatut)
        {
            // 1. Vérifier si une ligne est sélectionnée
            if (dgLocations.SelectedItem is Location locSelectionnee)
            {
                // 2. Vérifier si elle est déjà traitée
                if (locSelectionnee.Statut != "En attente")
                {
                    MessageBox.Show($"Cette location est déjà '{locSelectionnee.Statut}'. Impossible de modifier.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                try
                {
                    // 3. Appel au Repository pour mise à jour SQL
                    LocationRepository repo = new LocationRepository();
                    repo.UpdateStatut(locSelectionnee.Id, nouveauStatut);

                    MessageBox.Show($"Location passée en statut : {nouveauStatut}", "Succès");

                    // 4. Rafraîchir la liste
                    ChargerLocations();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erreur : " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Veuillez sélectionner une demande dans la liste.");
            }
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            AjoutVoitureWindow fenetreAjout = new AjoutVoitureWindow();
            fenetreAjout.ShowDialog();
            ChargerDonnees();
        }
        private void btnOpenDashboard_Click(object sender, RoutedEventArgs e)
        {
            DashboardWindow dashboard = new DashboardWindow();
            dashboard.Show();
        }
        private void btnOpenEntretien_Click(object sender, RoutedEventArgs e)
        {
            EntretienWindow entretien = new EntretienWindow();
            entretien.ShowDialog();

            // Quand on revient de l'entretien, on met à jour les listes
            ChargerDonnees();
        }
    }
}