using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
using Microsoft.Data.SqlClient;
using MyShop.DAO;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for InputDatabase.xaml
    /// </summary>
    public partial class InputDatabase : Window
    {
        Server server = Server.Instance;
        public InputDatabase()
        {
            InitializeComponent();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void connectDatabaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtDatabase.Text))
            {
                MessageBox.Show("Please enter database name!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            string database = txtDatabase.Text;

            loadingBar.Visibility = Visibility.Visible;
            loadingBar.IsIndeterminate = true;
            
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = server.Name;
            builder.InitialCatalog = database;
            builder.IntegratedSecurity = true;
            builder.TrustServerCertificate = true;
            builder.MultipleActiveResultSets = true;

            string connectionString = builder.ConnectionString;

            // DESKTOP-MQMBQC9
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
                    if(ex.Number == 4060)
                    {
                        MessageBoxResult result = MessageBox.Show($"Database {database} is not exist. " +
                            "Do you want to create new database?", "!!!", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if(result == MessageBoxResult.Yes)
                        {
                            try
                            {

                                string connectionStr = "Data Source=.;Integrated Security=True;TrustServerCertificate = true;";
                                SqlConnection connection1 = new SqlConnection(connectionStr);
                                SqlCommand cmd;
                                string sql = $"CREATE DATABASE {database}";
                                connection1.Open();
                                cmd = connection1.CreateCommand();
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                                connection1.Close();

                                _connection = connection1;

                                MessageBox.Show($"Database {database} has been created successfully.", "!!!", MessageBoxButton.OK, MessageBoxImage.Information);
                            } catch(SqlException ex2)
                            {
                                _connection = null;
                                MessageBox.Show($"Cannot create database:\n{ex.Message}", "Fail!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            _connection = null;
                        }

                    }
                    else
                    {
                        _connection = null;
                        MessageBox.Show($"Cannot create database:\n{ex.Message}", "Fail!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                return _connection;
            });

            loadingBar.Visibility = Visibility.Collapsed;
            loadingBar.IsIndeterminate = false;

            if (connection != null)
            {
                MessageBox.Show($"Connected to database {database} successfully.", "!!!", MessageBoxButton.OK, MessageBoxImage.Information);
                Database.Instance.Name = database;

                string databaseStr = database;
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["Database"].Value = databaseStr;
                config.Save(ConfigurationSaveMode.Minimal);
                ConfigurationManager.RefreshSection("appSettings");

                Window window = new SignIn();
                window.Show();
                this.Close();
            }
            else
            {
                Window window = new InputDatabase();
                window.Show();
                this.Close();
            }
        }

        private void textDatabase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtDatabase.Focus();
        }

        private void txtDatabase_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDatabase.Text) && txtDatabase.Text.Length > 0)
                textDatabase.Visibility = Visibility.Collapsed;
            else
                textDatabase.Visibility = Visibility.Visible;
        }

        private void borderDatabase_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var sourceImage = new SourceImage()
            {
                closeImage = "res/asset/close.png",
                dbImage = "res/asset/database.png",
            };

            var database = ConfigurationManager.AppSettings["Database"];

            if (database.Length != 0)
            {
                txtDatabase.Text = database;
            }

            this.DataContext = sourceImage;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new SignIn();
            window.Show();
            this.Close();
        }
    }
}
