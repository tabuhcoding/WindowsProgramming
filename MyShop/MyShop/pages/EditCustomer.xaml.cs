using Microsoft.Data.SqlClient;
using MyShop.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for EditCustomer.xaml
    /// </summary>
    public partial class EditCustomer : Window
    {
        string _phonenum;
        SqlDataReader _dataReader;
        public EditCustomer(string phonenum)
        {
            InitializeComponent();

            _phonenum = phonenum;
            Unloaded += Window_Unloaded;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastClosedPage"].Value = "pages/EditCustomer.xaml";
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }

        private void getDataFromDatabase()
        {
            string sql = $"select *, count(*) over() as Total from " + Database.Instance.tableCustomer + @" where phonenum = '{_phonenum}'";

            if (Database.Instance.Connection.State == ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            using (SqlCommand command = new SqlCommand(sql, Database.Instance.Connection))
            {

                MessageBox.Show(command.CommandText);
                // Mở DataReader và thực hiện truy vấn
                _dataReader = command.ExecuteReader();

                // Xử lý dữ liệu từ _dataReader ở đây
                if (_dataReader.Read())
                {
                    TxtName.Text = (string)_dataReader["name"];
                    TxtAddress.Text = (string)_dataReader["address"];
                    TxtPhone.Text = (string)_dataReader["phonenum"];
                    TxtGender.Text = (string)_dataReader["gender"];
                    TxtEmail.Text = (string)_dataReader["email"];
                }

                _dataReader.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getDataFromDatabase();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string sql;
                if (_phonenum == TxtPhone.Text)
                {
                    sql = $"update "+ Database.Instance.tableCustomer + @" set name = '{TxtName.Text}', address = '{TxtAddress.Text}', gender = '{TxtGender.Text}', Email = '{TxtEmail.Text}' where phonenum = '{_phonenum}'";
                    this.Close();
                }

                sql = $"select * from "+ Database.Instance.tableCustomer + @" where phonenum = '{TxtPhone.Text}'";

                using (SqlCommand command = new SqlCommand(sql, Database.Instance.Connection))
                {
                    _dataReader = command.ExecuteReader();

                    if (_dataReader.Read())
                    {
                        MessageBox.Show("Failed to update customer information: \nBecause the phone number is already registered", "Error", MessageBoxButton.OK);

                        _dataReader.Close();
                        return;
                    }

                    _dataReader.Close();
                }


                sql = $"update "+ Database.Instance.tableCustomer + @" set phonenum = '{TxtPhone.Text}', name = '{TxtName.Text}', address = '{TxtAddress.Text}', gender = '{TxtGender.Text}', Email = '{TxtEmail.Text}' where phonenum = '{_phonenum}'";

                using (SqlCommand command = new SqlCommand(sql, Database.Instance.Connection))
                {
                    command.ExecuteNonQuery();
                }

                sql = $"update "+ Database.Instance.tableCustomerOrder + @" set phonenum = '{TxtPhone.Text}' where phonenum = '{_phonenum}'";
                using (SqlCommand command = new SqlCommand(sql, Database.Instance.Connection))
                {
                    command.ExecuteNonQuery();
                }

                MessageBox.Show("Update successfully!", "Edit customer", MessageBoxButton.OK);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update customer information: \n" + ex.ToString(), "Error", MessageBoxButton.OK);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
