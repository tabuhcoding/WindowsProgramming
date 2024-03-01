using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MyShop.BUS;
using MyShop.DAO;
using MyShop.DTO;
using MyShop.Helpers;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        Server server = Server.Instance;
        public SignIn()
        {
            InitializeComponent();
        }

        // sign in button
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtUsername.Text) || string.IsNullOrEmpty(passwordBox.Password))
            {
                MessageBox.Show("Please enter username and password!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string username = txtUsername.Text;
            string password = passwordBox.Password;

            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = Server.Instance.Name;
            builder.InitialCatalog = Database.Instance.Name;
            builder.IntegratedSecurity = true;
            builder.TrustServerCertificate = true;

            string connectionString = builder.ConnectionString;
            Database.Instance.ConnectionString = connectionString;

            var connection = new SqlConnection(connectionString);

            connection = await Task.Run(() =>
            {
                var _connection = new SqlConnection(connectionString);
                try
                {
                    _connection.Open();
                }
                catch (Exception ex)
                {
                    _connection = null;
                    MessageBox.Show(ex.Message);
                }
                return _connection;
            });

            AdminBUS adminBUS = new AdminBUS();
            AdminDTO? adminDTO = adminBUS.GetAdmin(username, password);

            if (adminDTO != null && connection != null)
            {
                if (rememberMe.IsChecked == true)
                {
                    string passwordIn64 = Encryption.Encrypt(password, "1234567890123456");


                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["Username"].Value = username;
                    config.AppSettings.Settings["Password"].Value = passwordIn64;
                    /*config.AppSettings.Settings["Entropy"].Value = entropyIn64;*/
                    config.Save(ConfigurationSaveMode.Minimal);

                    ConfigurationManager.RefreshSection("appSettings");
                }
                MessageBox.Show("Successfully signed in!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                Window window = new MainWindow();
                window.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to in database! Please sign up!", "Fail!", MessageBoxButton.OK, MessageBoxImage.Error);
                Window window = new SignIn();
                window.Show();
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sourceImage = new SourceImage()
            {
                closeImage = "res/asset/close.png",
                lockImage = "res/asset/Lock.png",
                emailImage = "res/asset/Email.png",
                eyeImage = "res/asset/eye.png",
            };
            this.DataContext = sourceImage;
            var passwordIn64 = ConfigurationManager.AppSettings["Password"];

            if (passwordIn64.Length != 0)
            {
                string password = Encryption.Decrypt(passwordIn64, "1234567890123456");
                passwordBox.Password = password;

                txtUsername.Text = ConfigurationManager.AppSettings["Username"];
            }
        }

        private void Border_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(passwordBox.Password) && passwordBox.Password.Length > 0)
                textPassword.Visibility = Visibility.Collapsed;
            else
                textPassword.Visibility = Visibility.Visible;
        }

        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            passwordBox.Focus();
        }

        private void txtUsername_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtUsername.Text) && txtUsername.Text.Length > 0)
                textUsername.Visibility = Visibility.Collapsed;
            else
                textUsername.Visibility = Visibility.Visible;
        }

        private void textUsername_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtUsername.Focus();
        }

        private void signUpButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new SignUp();
            window.Show();
            this.Close();
        }

        private void toggleShowPassword_Click(object sender, RoutedEventArgs e)
        {
            if (toggleShowPassword.IsChecked == true)
            {
                txtVisiblePassword.Text = passwordBox.Password;
                txtVisiblePassword.Visibility = Visibility.Visible;
                passwordBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                txtVisiblePassword.Visibility = Visibility.Collapsed;
                passwordBox.Visibility = Visibility.Visible;
                passwordBox.Focus();
            }
        }
    }
}
