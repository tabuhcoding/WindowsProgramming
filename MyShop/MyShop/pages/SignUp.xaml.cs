using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using MyShop.Helpers;
using MyShop.DAO;
using MyShop.DTO;
using MyShop.BUS;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        Server server = Server.Instance;
        public string gender = "";
        public SignUp()
        {
            InitializeComponent();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new SignIn();
            window.Show();
            this.Close();
        }

        private async void signUpSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(firstNameTextBox.Text) || string.IsNullOrEmpty(lastNameTextBox.Text) || string.IsNullOrEmpty(emailTextBox.Text) || string.IsNullOrEmpty(addressTextBox.Text) || string.IsNullOrEmpty(phoneTextBox.Text) || string.IsNullOrEmpty(ageTextBox.Text) || string.IsNullOrEmpty(passwordTextBox.Text))
            {
                MessageBox.Show("Please enter all information!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // connect to SQL Server
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = server.Name;
            builder.InitialCatalog = Database.Instance.Name;
            builder.IntegratedSecurity = true;
            builder.TrustServerCertificate = true;

            string connectionString = builder.ConnectionString;
            /*Database.Instance.ConnectionString = connectionString;*/

            var connection = new SqlConnection(connectionString);

            connection = await Task.Run(() =>
            {
                var _connection = new SqlConnection(connectionString);
                try
                {
                    _connection.Open();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                return _connection;
            });

            if (connection != null)
            {
                /*MessageBox.Show("Successfully connected to SQL Server");*/
                Database.Instance.ConnectionString = connectionString;

                string idString = "";
                bool isExist = true;
                while (isExist)
                {
                    Random random = new Random();
                    int id = random.Next(0, 999);
                    idString = id.ToString();
                    if (id < 10)
                    {
                        idString = "00" + idString;
                    }
                    else if (id < 100)
                    {
                        idString = "0" + idString;
                    }

                    AdminBUS adminBus = new AdminBUS();
                    bool checkExists = adminBus.IsExistsID(idString);
                    if (!checkExists)
                    {
                        isExist = false;
                    }
                }

                AdminDTO adminDTO = new AdminDTO()
                {
                    ID = idString,
                    FirstName = firstNameTextBox.Text,
                    LastName = lastNameTextBox.Text,
                    Email = emailTextBox.Text,
                    Address = addressTextBox.Text,
                    Phone = phoneTextBox.Text,
                    Gender = gender,
                    Age = int.Parse(ageTextBox.Text),
                    Password = Encryption.Encrypt(passwordTextBox.Text, "1234567890123456"),
                };

                AdminBUS adminBUS = new AdminBUS();
                bool signUpSuccess = adminBUS.CreateAdmin(adminDTO);

                if (signUpSuccess)
                {
                    MessageBox.Show("Successfully signed up!", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
                    Window newWindow = new SignIn();
                    newWindow.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to sign up!", "Fail!", MessageBoxButton.OK, MessageBoxImage.Error);
                    Window window1 = new SignUp();
                    window1.Show();
                }
            }
        }

        private void genderButton_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender == maleButton)
            {
                MessageBox.Show("You choose Male", "!!!", MessageBoxButton.OK, MessageBoxImage.Information);
                gender = "Male";
            }
            else if (sender == femaleButton)
            {
                MessageBox.Show("You choose Female", "!!!", MessageBoxButton.OK, MessageBoxImage.Information);
                gender = "Female";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sourceImage = new SourceImage()
            {
                closeImage = "res/asset/close.png",
                personImage = "res/asset/img.png"
            };
            this.DataContext = sourceImage;
        }
    }
}
