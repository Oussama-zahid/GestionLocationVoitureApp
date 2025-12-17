using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32; // Nécessaire pour OpenFileDialog
using LocationVoiture.Core.Models;
using LocationVoiture.Data;

namespace LocationVoiture.WPF
{
    public partial class AjoutVoitureWindow : Window
    {
        private string _cheminImageSelectionne = null;

        public AjoutVoitureWindow()
        {
            InitializeComponent();
        }

        // 1. Gérer le clic sur "Choisir une image"
        private void btnImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png;*.bmp;*.webp;*.gif|Tous les fichiers (*.*)|*.*"; // Filtre les fichiers images

            if (openFileDialog.ShowDialog() == true)
            {
                _cheminImageSelectionne = openFileDialog.FileName; // Garde le chemin complet
                txtImageChemin.Text = System.IO.Path.GetFileName(_cheminImageSelectionne); // Affiche juste le nom du fichier
                txtImageChemin.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        // 2. Gérer l'enregistrement
        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            // Validation basique
            if (string.IsNullOrWhiteSpace(txtMarque.Text) || string.IsNullOrWhiteSpace(txtPrix.Text))
            {
                MessageBox.Show("Veuillez remplir au moins la marque et le prix.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Récupération des valeurs des ComboBox
                string carburantSelectionne = (cbCarburant.SelectedItem as ComboBoxItem).Content.ToString();
                string transmissionSelectionnee = (cbTransmission.SelectedItem as ComboBoxItem).Content.ToString();

                // Création de l'objet complet
                Voiture v = new Voiture
                {
                    Marque = txtMarque.Text,
                    Modele = txtModele.Text,
                    Immatriculation = txtImmat.Text,
                    Annee = int.Parse(txtAnnee.Text), // Conversion du texte en entier
                    PrixParJour = decimal.Parse(txtPrix.Text), // Conversion du texte en décimal
                    Carburant = carburantSelectionne,
                    Transmission = transmissionSelectionnee,
                    Statut = "Disponible", // Toujours dispo à la création
                    ImageChemin = _cheminImageSelectionne // Le chemin de l'image
                };

                // Sauvegarde via le Repository
                VoitureRepository repo = new VoitureRepository();
                repo.Ajouter(v);

                MessageBox.Show("Voiture ajoutée avec succès !");
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("Veuillez vérifier que l'année et le prix sont des chiffres valides.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur critique : " + ex.Message);
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}