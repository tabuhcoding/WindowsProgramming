using Microsoft.Data.SqlClient;
using MyShop.DAO;
using MyShop.DTO;
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
    /// Interaction logic for NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Window
    {
        BindingList<PhoneDTO> _product = new BindingList<PhoneDTO>();
        SqlDataReader _dataReader;
        int _orderPerPage = 5;
        int _totalOrders = -1;
        int _totalOrderItems = -1;
        int _currentOrderPage = 1;
        int _totalOrderCount = -1;
        int _productPerPage = 5;
        int _totalProduct = -1;
        int _totalProductItems = -1;
        int _currentProductPage = 1;
        int _totalProductCount = -1;
        int _selectedProduct = -1;
        int _selectedEdit = -1;
        Int32 _totalCost = 0;
        Int32 _totalProfit = 0;
        BindingList<OrderDetailDTO> _od = new BindingList<OrderDetailDTO>();
        public NewOrder()
        {
            InitializeComponent();
            Unloaded += Window_Unloaded;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastClosedPage"].Value = "pages/NewOrder.xaml";
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            getDataFromDatabase();
            ShipDate.SelectedDate = DateTime.Now.AddDays(3);
            ShipDate.DisplayDateStart = DateTime.Now.AddDays(3);
            ShipDate.DisplayDateEnd = DateTime.Now.AddYears(2);
            Gender.SelectedIndex = 0;
        }

        private void getDataFromDatabase()
        {
            string sql = $"select *, count(*) over() as TotalCount from mobile Order by name offset @Skip rows fetch next @Take rows only";

            SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);
            command.Parameters.Add("@Skip", SqlDbType.Int).Value = (_currentProductPage - 1) * _productPerPage;
            command.Parameters.Add("@Take", SqlDbType.Int).Value = _productPerPage;
            try
            {
                _dataReader = command.ExecuteReader();


                _product.Clear();

                while (_dataReader.Read())
                {
                    PhoneDTO phone = new PhoneDTO()
                    {
                        name = (string)_dataReader["name"],
                        image = (string)_dataReader["image"],
                        price = (Int32)_dataReader["price"],
                        amount = (Int32)_dataReader["amount"],
                        os = (string)_dataReader["os"],
                        manufacturer = (string)_dataReader["manufacturer"],
                        memoryStorage = (string)_dataReader["memoryStorage"],
                        priceOriginal = (Int32)_dataReader["priceOriginal"]
                    };
                    _totalProductCount = (int)_dataReader["TotalCount"];
                    _product.Add(phone);
                }
                if (_totalProductCount != _totalProductItems)
                {
                    _totalProductItems = _totalProductCount;
                    _totalProduct = (_totalProductItems / _productPerPage) +
                        (((_totalProductItems % _productPerPage) == 0) ? 0 : 1);
                }

                CurProductPage.Text = _currentProductPage.ToString();
                TotalProductPage.Text = _totalProduct.ToString();

                if (_totalProduct == -1 || _totalProduct == 0 || _product.Count == 0)
                {
                    CurProductPage.Text = "1";
                    TotalProductPage.Text = "1";
                }

                _dataReader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                _dataReader.Close();
            }

            ProductList.ItemsSource = _product;
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
        }

        private CustomerDTO GetCustomerClass(string phoneNum)
        {
            try
            {
                string sql = $"select * from CUSTOMERCLASS where phoneNum = '{phoneNum}'";
                SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);
                _dataReader = command.ExecuteReader();

                if (!_dataReader.Read())
                {
                    MessageBox.Show("Cannot find customer");
                    throw (new Exception("Cannot find customer"));
                }

                CustomerDTO customerClass = new CustomerDTO()
                {
                    PhoneNum = (string)_dataReader["phoneNum"],
                    Name = (string)_dataReader["Name"],
                    Address = (string)_dataReader["Address"],
                    Email = (string)_dataReader["Email"],
                    Gender = (string)_dataReader["Gender"]
                };

                _dataReader.Close();

                return customerClass;
            }
            catch (Exception e)
            {
                _dataReader.Close();

                MessageBox.Show(e.ToString());
            }

            return null;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (Customer_PhoneNum.Text.Length != 10)
            {
                MessageBox.Show("Invalid phonenum");

                return;
            }

            if (_od.Count == 0)
            {
                MessageBox.Show("Select at least 1 item");

                return;
            }

            string phonenum = Customer_PhoneNum.Text;

            foreach (var item in phonenum)
            {
                if (item < '0' || item > '9')
                {
                    MessageBox.Show("Invalid phonenum");
                }
            }

            try
            {
                int res = 0;
                string sql = $"select count(*) over() as total from CustomerCLass where PhoneNum = '{Customer_PhoneNum.Text}'";

                SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);
                _dataReader = command.ExecuteReader();

                if (!_dataReader.Read())
                {
                    _dataReader.Close();
                    sql = $"insert into CustomerClass(name, address, phonenum, email, gender)" +
                        $" values ('{Customer_name.Text}', '{Address.Text}', '{Customer_PhoneNum.Text}', '{Email.Text}', '{Gender.Text}')";
                    command = new SqlCommand(sql, Database.Instance.Connection);
                    res = command.ExecuteNonQuery();

                    if (res == 0)
                    {
                        MessageBox.Show("Fail to create order please recheck your information: Customer!");

                        return;
                    }
                }
                else
                {
                    _dataReader.Close();
                    CustomerDTO cus = GetCustomerClass(Customer_PhoneNum.Text);

                    do
                    {
                        if (cus.Name == Customer_name.Text && cus.Address == Address.Text && cus.Gender == Gender.Text && cus.Email == Email.Text)
                        {
                            break;
                        }
                        MessageBoxResult result = MessageBox.Show("This phone number already exists in the database!\n Would you like to update the information?", "Phone number exists", MessageBoxButton.YesNo);

                        if (result == MessageBoxResult.No)
                        {
                            return;
                        }

                        if (result == MessageBoxResult.Yes)
                        {
                            sql = $"delete from CustomerClass where phonenum = '{phonenum}'";

                            command = new SqlCommand(sql, Database.Instance.Connection);
                            res = command.ExecuteNonQuery();

                            if (res == 0)
                            {
                                MessageBox.Show("Fail to create order please recheck your information!");

                                return;
                            }

                            sql = $"insert into CustomerClass(name, address, phonenum, email , gender)" +
                            $" values ('{Customer_name.Text}', '{Address.Text}', '{Customer_PhoneNum.Text}', '{Email.Text}', '{Gender.Text}')";

                            command = new SqlCommand(sql, Database.Instance.Connection);
                            res = command.ExecuteNonQuery();

                            if (res == 0)
                            {
                                MessageBox.Show("Fail to edit order please recheck your information!");

                                return;
                            }




                        }
                    } while (false);
                }

                sql = $"insert into CustomerOrder(phoneNum, createDate, shipmentDate, status, totalCost, totalProfit)" +
                    $" values ('{phonenum}', '{DateTime.Now.ToString()}', '{ShipDate.SelectedDate.ToString()}', 'pending', {_totalCost},{_totalProfit})";

                command = new SqlCommand(sql, Database.Instance.Connection);
                res = command.ExecuteNonQuery();

                if (res == 0)
                {
                    MessageBox.Show("Fail to create order please recheck your information!");

                    return;
                }

                sql = $"select max(OrderId) as max from CustomerOrder";
                command = new SqlCommand(sql, Database.Instance.Connection);
                _dataReader = command.ExecuteReader();
                int orderId = 0;

                if (!_dataReader.Read())
                {
                    MessageBox.Show("Fail to create order please recheck your information!");
                    _dataReader.Close();
                    return;
                }
                orderId = (int)_dataReader["max"];
                MessageBox.Show(orderId.ToString());
                _dataReader.Close();


                foreach (var item in _od)
                {
                    sql = $"insert into OrderDetail( OrderId, amount, Phonename, total, image)" +
                        $" values ({orderId}, {item.amount}, '{item.phone}', {item.total}, '{item.image}')";
                    command = new SqlCommand(sql, Database.Instance.Connection);
                    res = command.ExecuteNonQuery();
                    MessageBox.Show(sql + $" {res} rows affected");
                    if (res <= 0)
                    {
                        MessageBox.Show("Fail to complete order please recheck your information!");

                        return;
                    }
                }

                foreach (var item in _od)
                {
                    sql = $"update Mobile set amount = amount - {item.amount} where name = '{item.phone}'";
                    command = new SqlCommand(sql, Database.Instance.Connection);
                    res = command.ExecuteNonQuery();
                    MessageBox.Show(sql + $" {res} rows affected");
                    if (res <= 0)
                    {
                        MessageBox.Show("Fail to complete order please recheck your information!");


                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Fail to create order please recheck your information!" + ex.ToString());

                return;
            }

            MessageBox.Show("Successfully created Order!");

        }

        private void PreviousProductPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentProductPage == 1)
            {
                return;
            }

            _currentProductPage--;
            getDataFromDatabase();
        }

        private void NextProductPage_Click(object sender, RoutedEventArgs e)
        {
            if (_currentProductPage == _totalProduct || TotalProductPage.Text == "1")
            {
                return;
            }

            _currentProductPage++;
            getDataFromDatabase();
        }
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedProduct < 0)
            {
                MessageBox.Show("No product choosed", "Error", MessageBoxButton.OK);
                return;
            }
            ProductList.SelectedIndex = _selectedProduct;

            try
            {
                int amount = int.Parse(Amount.Text);

                if (amount <= 0)
                {
                    MessageBox.Show("Invalid amount", "Error", MessageBoxButton.OK);

                    return;
                }

                if (amount > _product[_selectedProduct].amount)
                {
                    MessageBox.Show("Out of stock", "Error", MessageBoxButton.OK);

                    return;
                }

                foreach (var item in _od)
                {
                    if (item.phone == _product[_selectedProduct].name)
                    {
                        if (item.amount != amount)
                        {
                            item.amount = amount;
                            item.total = amount * _product[_selectedProduct].price;
                            item.totalProfit = amount * _product[_selectedProduct].priceOriginal;
                        }
                        return;
                    }
                }

                OrderDetailDTO od = new OrderDetailDTO()
                {
                    phone = _product[_selectedProduct].name,
                    amount = amount,
                    image = _product[_selectedProduct].image,
                    total = amount * _product[_selectedProduct].price,
                    totalProfit = amount * _product[_selectedProduct].priceOriginal
                };

                _od.Add(od);

                _totalCost += od.total;
                _totalProfit += od.totalProfit;
                OrderDetailList.ItemsSource = _od;
                TotalCost.Text = _totalCost.ToString();
            }
            catch
            {
                MessageBox.Show("Invalid amount", "Error", MessageBoxButton.OK);
            }
        }

        private void ProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductList.SelectedIndex < 0)
            {
                return;
            }

            _selectedProduct = ProductList.SelectedIndex;

            selectedItem.Text = _product[_selectedProduct].name;
            BitmapImage bitmap = new BitmapImage(
                new Uri(_product[_selectedProduct].image, UriKind.Absolute)
            );
            selectedImage.Source = bitmap;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            _selectedEdit = OrderDetailList.SelectedIndex;
            if (_selectedEdit < 0 || _od.Count == 0 || _selectedEdit >= _od.Count)
            {
                MessageBox.Show("No product choosed");
                return;
            }
            _totalCost -= _od[_selectedEdit].total;
            _od.RemoveAt(OrderDetailList.SelectedIndex);

            AmountEdit.Text = "";

            BitmapImage bitmap = new BitmapImage();
            selectedImageOrder.Source = bitmap;

            TotalCost.Text = _totalCost.ToString();
        }

        private void OrderDetailList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedEdit = OrderDetailList.SelectedIndex;
            if (_selectedEdit < 0 || _od.Count == 0 || _selectedEdit >= _od.Count)
            {
                return;
            }

            AmountEdit.Text = _od[_selectedEdit].amount.ToString();
            BitmapImage bitmap = new BitmapImage(
                new Uri(_product[_selectedProduct].image, UriKind.Absolute)
            );
            selectedImageOrder.Source = bitmap;
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            _selectedEdit = OrderDetailList.SelectedIndex;
            if (_selectedEdit < 0 || _od.Count == 0 || _selectedEdit >= _od.Count)
            {
                return;
            }

            try
            {
                int amount = int.Parse(AmountEdit.Text);

                if (amount <= 0)
                {
                    MessageBox.Show("Invalid amount", "Error", MessageBoxButton.OK);

                    return;
                }


                foreach (var item in _product)
                {
                    if (item.name == _od[_selectedEdit].phone)
                    {
                        if (amount != _od[_selectedEdit].amount)
                        {
                            _totalCost += (amount - _od[_selectedEdit].amount) * item.price;
                            _totalProfit += (amount - _od[_selectedEdit].amount) * item.priceOriginal;

                            _od[_selectedEdit].amount = amount;

                            _od[_selectedEdit].total = amount * item.price;

                            TotalCost.Text = _totalCost.ToString();

                            OrderDetailList.ItemsSource = new BindingList<OrderDetailDTO>();

                            OrderDetailList.ItemsSource = _od;

                            return;
                        }
                    }
                }

                MessageBox.Show("Invalid selection", "Error", MessageBoxButton.OK);
            }
            catch
            {
                MessageBox.Show("Invalid amount", "Error", MessageBoxButton.OK);
            }
        }

        private void SearchBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TxtSearch.Text == "Search" || TxtSearch.Text == "")
            {
                getDataFromDatabase();

                return;
            }
            try
            {
                string sql = $"select *, count(*) over() as Total from Mobile where name like '%{TxtSearch.Text}%'" + @" Order by name offset @Skip rows fetch next @Take rows only";

                _product.Clear();

                _currentProductPage = 1;

                SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);
                command.Parameters.Add("@Skip", SqlDbType.Int).Value = (_currentProductPage - 1) * _productPerPage;
                command.Parameters.Add("@Take", SqlDbType.Int).Value = _productPerPage;
                _dataReader = command.ExecuteReader();

                while (_dataReader.Read())
                {
                    PhoneDTO phone = new PhoneDTO()
                    {
                        name = (string)_dataReader["name"],
                        image = (string)_dataReader["image"],
                        price = (Int32)_dataReader["price"],
                        amount = (Int32)_dataReader["amount"],
                        os = (string)_dataReader["os"],
                        manufacturer = (string)_dataReader["manufacturer"],
                        memoryStorage = (string)_dataReader["memoryStorage"]
                    };
                    _totalProductCount = (int)_dataReader["TotalCount"];
                    _product.Add(phone);
                }
                if (_totalProductCount != _totalProductItems)
                {
                    _totalProductItems = _totalProductCount;
                    _totalProduct = (_totalProductItems / _productPerPage) +
                        (((_totalProductItems % _productPerPage) == 0) ? 0 : 1);
                }

                CurProductPage.Text = _currentProductPage.ToString();
                TotalProductPage.Text = _totalProduct.ToString();

                if (_totalProduct == -1 || _totalProduct == 0 || _product.Count == 0)
                {
                    CurProductPage.Text = "1";
                    TotalProductPage.Text = "1";
                }

                ProductList.ItemsSource = _product;

                _dataReader.Close();
            }
            catch
            {
                _dataReader.Close();
            }
        }
    }
}