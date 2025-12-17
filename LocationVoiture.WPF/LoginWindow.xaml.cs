using System.Windows;
using LocationVoiture.Data;

namespace LocationVoiture.WPF
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            string email = txtEmail.Text;
            string password = txtPass.Password;

            UtilisateurRepository repo = new UtilisateurRepository();

            if (repo.ValiderConnexion(email, password))
            {
                // Succès : On ouvre la fenêtre principale
                MainWindow main = new MainWindow();
                main.Show();

                // On ferme la fenêtre de login
                this.Close();
            }
            else
            {
                MessageBox.Show("Email ou mot de passe incorrect.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnQuitter_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}