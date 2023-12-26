using SemaforoPorLotes.Models;
using SemaforoPorLotes.Repository;
using System.Windows;

namespace SemaforizacionPorLotes
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (DataIsValid())
            {
                string username = UserNameTextBox.Text;
                string password = PasswordTextBox.Password;
                User user = new User(username, password);
                UserRepositoryImpl userRepositoryImpl = new UserRepositoryImpl();
                bool access = userRepositoryImpl.login(user);
                if (access)
                {
                    LoadingData loadingData = new LoadingData(user);
                    this.Close();
                    loadingData.Show();

                }
                else
                {
                    MessageBox.Show("Usuario o contraseña incorrectos", "Error de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Asegurese de rellenar todos los campos", "Alerta de inicio de sesión", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private bool DataIsValid()
        {
            string username = UserNameTextBox.Text;
            string password = PasswordTextBox.Password;
            if (username.Equals("") || password.Equals(""))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
