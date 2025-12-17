using System.Linq;
using System.Windows;
using LocationVoiture.Data;
using LocationVoiture.Core.Models;

namespace LocationVoiture.WPF
{
    public partial class EntretienWindow : Window
    {
        private VoitureRepository _repo = new VoitureRepository();

        public EntretienWindow()
        {
            InitializeComponent();
            ChargerListes();
        }

        private void ChargerListes()
        {
            var toutesLesVoitures = _repo.GetAll();

            // Remplir la liste de gauche (Seulement les Disponibles)
            dgDisponibles.ItemsSource = toutesLesVoitures.Where(v => v.Statut == "Disponible").ToList();

            // Remplir la liste de droite (Seulement celles en Entretien)
            dgEntretien.ItemsSource = toutesLesVoitures.Where(v => v.Statut == "Entretien").ToList();
        }

        // Action : Mettre en panne
        private void btnEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            if (dgDisponibles.SelectedItem is Voiture v)
            {
                _repo.UpdateStatut(v.Id, "Entretien");
                ChargerListes(); // Rafraichir
            }
            else
            {
                MessageBox.Show("Sélectionnez une voiture disponible.");
            }
        }

        // Action : Réparer
        private void btnTerminer_Click(object sender, RoutedEventArgs e)
        {
            if (dgEntretien.SelectedItem is Voiture v)
            {
                _repo.UpdateStatut(v.Id, "Disponible");
                ChargerListes(); // Rafraichir
            }
            else
            {
                MessageBox.Show("Sélectionnez une voiture en entretien.");
            }
        }
    }
}