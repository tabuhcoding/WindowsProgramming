using Microsoft.Data.SqlClient;
using MyShop.DAO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for CustomerManagement.xaml
    /// </summary>
    public partial class CustomerManagement : Page
    {
        SqlDataReader _dataReader;
        string searchVal = "";
        int _selected = -1;
        BindingList<object> _customerList = new BindingList<object>();
        List<string> phoneNum = new List<string>();
        public CustomerManagement()
        {
            InitializeComponent();
            Unloaded += Page_Unloaded;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastClosedPage"].Value = "pages/CustomerManagement.xaml";
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            searchVal = TxtSearch.Text;

            getDataFromDatabase();
        }

        private void getDataFromDatabase()
        {
            try
            {
                string sql = $"select *, count(*) over() as Total from " + Database.Instance.tableCustomer + @" where name like '%{searchVal}%'";

                if (Database.Instance.Connection.State == ConnectionState.Closed)
                {
                    Database.Instance.Connection.Open();
                }
                using (SqlCommand command = new SqlCommand(sql, Database.Instance.Connection))
                {
                    // Mở DataReader và thực hiện truy vấn
                    _dataReader = command.ExecuteReader();

                    // Xử lý dữ liệu từ _dataReader ở đây
                    int count = 1;
                    _customerList.Clear();
                    phoneNum.Clear();
                    while (_dataReader.Read())
                    {
                        phoneNum.Add((string)_dataReader["phonenum"]);
                        _customerList.Add(new
                        {
                            ID = count,
                            NAME = _dataReader["name"],
                            ADDRESS = _dataReader["address"],
                            PHONENUM = phoneNum[phoneNum.Count - 1],
                            EMAIL = _dataReader["email"],
                        });
                        count++;
                    }

                    _dataReader.Close();
                }

                CustomerList.ItemsSource = _customerList;
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot read datafrom database!\nPlease try again:\n " + e.ToString(), "Error", MessageBoxButton.OK);

                return;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            getDataFromDatabase();
        }

        private void AddBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            AddBtn.FontSize = 25;
        }

        private void AddBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            AddBtn.FontSize = 15;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            Window window = new NewCustomer();

            window.ShowDialog();

            getDataFromDatabase();
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selected < 0)
            {
                MessageBox.Show("Please select a customer!", "Error", MessageBoxButton.OK);

                return;
            }
            Window window = new EditCustomer(phoneNum[_selected]);

            window.ShowDialog();

            getDataFromDatabase();
        }

        private void EditBtn_MouseEnter(object sender, MouseEventArgs e)
        {
            EditBtn.FontSize = 25;
        }

        private void EditBtn_MouseLeave(object sender, MouseEventArgs e)
        {
            EditBtn.FontSize = 15;
        }

        private void CustomerList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selected = -1;
            if (CustomerList.SelectedIndex >= 0)
            {
                _selected = CustomerList.SelectedIndex;
            }
        }
    }
}