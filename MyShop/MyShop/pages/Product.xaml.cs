using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
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
using System.Windows.Controls.Primitives;
using static System.Net.Mime.MediaTypeNames;
using MyShop.DAO;
using MyShop.DTO;
using MyShop.BUS;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class Product : Page
    {
        private readonly PhoneBUS _phoneBUS;

        public string[] nameOS;

        public DataTable MobileData { get; set; } = new DataTable();
        public DataTable filteredData { get; set; } = new DataTable();
        // public string recentOS { get; set; }
        private int itemsPerPage = 10; // Số dòng trên mỗi trang
        private DataView filteredDataView;
        private int currentPage = 1;

        public Product()
        {
            InitializeComponent();
            _phoneBUS = new PhoneBUS();

            Unloaded += Page_Unloaded;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastClosedPage"].Value = "pages/Product.xaml";
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }

        private async void PageOpened(object sender, RoutedEventArgs e)
        {
            await Database.Instance.ImportDataToSQLAsync();
            LoadDataIntoGrid();
        }
        private void LoadDataIntoGrid()
        {
            /*var sql = $"SELECT ID, Name, OS, Price, PriceOriginal, Quantity, Manufacturer, MemoryStorage, Details, Image FROM {Database.Instance.tableName}";
            var command = new SqlCommand(sql, Database.Instance.Connection);
            if (Database.Instance.Connection != null) { Database.Instance.Connection.Close(); }*/

            /*            SqlDataAdapter adapter = new SqlDataAdapter(command);
            */
            try
            {
                SqlDataAdapter adapter = _phoneBUS.getAll();
                adapter.Fill(MobileData);
                SetPageComboBox();
                SetPriceRangeComboBox();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi ở đây, ví dụ:
                MessageBox.Show("Error occurred: " + ex.Message);
            }

        }

        private void SetPageComboBox()
        {
            nameOS = MobileData.AsEnumerable().Select(row => row.Field<string>("os")).Distinct().ToArray();
            List<string> nameOSList = nameOS.ToList();
            nameOSList.Add("All");

            nameOS = nameOSList.ToArray();
            comboBox.ItemsSource = nameOS;
            comboBox.SelectedItem = nameOS[nameOS.Length - 1];
        }

        private void SetPriceRangeComboBox()
        {
            // Tạo 5 khoảng giá
            string[] priceRanges = new string[] { "< 2tr", "2tr - 5tr", "5tr - 10tr", "10tr - 20tr", "> 20tr", "All" };

         
            priceComboBox.ItemsSource = priceRanges;
            priceComboBox.SelectedItem = priceRanges[priceRanges.Length-1];
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListPhone.SelectedItem != null && ListPhone.SelectedItems.Count == 1)
            {
                if (ListPhone.SelectedItem is DataRowView selectedRow)
                {
                    PhoneDTO phone = new PhoneDTO
                    {
                        name = selectedRow["Name"].ToString(),
                        os = selectedRow["OS"].ToString(),
                        image = selectedRow["Image"].ToString(),
                        manufacturer = selectedRow["Manufacturer"].ToString(),
                        memoryStorage = selectedRow["MemoryStorage"].ToString(),
                        id = Convert.ToInt32(selectedRow["ID"].ToString()),
                        quantity = Convert.ToInt32(selectedRow["Quantity"].ToString()),
                        priceOriginal = Convert.ToInt32(selectedRow["PriceOriginal"].ToString()),
                        details = selectedRow["Details"].ToString(),
                    };
                    /* _phone.Add(phone);*/
                    if (int.TryParse(selectedRow["Price"].ToString(), out int priceValue))
                    {
                        phone.price = priceValue;
                    }
                    else
                    {
                        phone.price = 0;
                    }

                    SetDetails(phone);
                }
            }

        }

        private void SetDetails(PhoneDTO phone)
        {
            this.DataContext = phone;
        }
        
        private void comboBox_Selected(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();          
        }

        private void priceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
            
        }
        private int totalPages = 0;
        private void ApplyFilters()
        {
            string searchText = searchTextBox.Text;
            string selectedType = comboBox.SelectedItem?.ToString();
            string selectedPrice = priceComboBox.SelectedItem?.ToString();

            DataView dv = MobileData.DefaultView;

            // Filter based on Name
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                dv.RowFilter = $"name LIKE '%{searchText}%'";
            }
            else
            {
                dv.RowFilter = "";
            }

            // Filter based on selected product type
            if (!string.IsNullOrEmpty(selectedType))
            {
                if (selectedType != "All")
                {
                    if (!string.IsNullOrEmpty(dv.RowFilter))
                    {
                        dv.RowFilter += $" AND os = '{selectedType}'";
                    }
                    else
                    {
                        dv.RowFilter = $"os = '{selectedType}'";
                    }
                }
            }


            // Filter based on selected price range
            if (!string.IsNullOrEmpty(selectedPrice))
            {
                if (selectedPrice != "All")
                {
                    // Assuming the price range is a column named 'Price' in your DataTable
                    if (!string.IsNullOrEmpty(dv.RowFilter))
                    {
                        if (selectedPrice == "< 2tr") // Example: Low price range
                        {
                            dv.RowFilter += " AND price < 2000000"; // Set your low-price threshold as needed
                        }
                        else if (selectedPrice == "2tr - 5tr") // Example: Medium price range
                        {
                            dv.RowFilter += " AND price >= 2000000 AND price <= 5000000"; // Set your medium-price threshold as needed
                        }
                        else if (selectedPrice == "5tr - 10tr") // Example: High price range
                        {
                            dv.RowFilter += " AND price >= 5000000 AND price <=10000000"; // Set your high-price threshold as needed

                        }
                        else if (selectedPrice == "10tr - 20tr") // Example: High price range
                        {
                            dv.RowFilter += " AND price >= 10000000 AND price <=20000000"; // Set your high-price threshold as needed
                        }
                        else if (selectedPrice == "> 20tr") // Example: High price range
                        {
                            dv.RowFilter += " AND price > 20000000"; // Set your high-price threshold as needed
                        }
                    }
                    else
                    {
                        // Adjust these conditions according to your price range criteria
                        if (selectedPrice == "< 2tr") // Example: Low price range
                        {
                            dv.RowFilter = "  price < 2000000"; // Set your low-price threshold as needed
                        }
                        else if (selectedPrice == "2tr - 5tr") // Example: Medium price range
                        {
                            dv.RowFilter = "  price >= 2000000 AND price <= 5000000"; // Set your medium-price threshold as needed
                        }
                        else if (selectedPrice == "5tr - 10tr") // Example: High price range
                        {
                            dv.RowFilter = "  price >= 5000000 AND price <=10000000"; // Set your high-price threshold as needed

                        }
                        else if (selectedPrice == "10tr - 20tr") // Example: High price range
                        {
                            dv.RowFilter = "  price >= 10000000 AND price <=20000000"; // Set your high-price threshold as needed
                        }
                        else if (selectedPrice == "> 20tr") // Example: High price range
                        {
                            dv.RowFilter = "  price > 20000000"; // Set your high-price threshold as needed
                        }
                        else
                        {
                            dv.RowFilter = "";
                        }
                    }
                }
            }
            //ListPhone.ItemsSource = dv;
            filteredData = dv.ToTable();
            filteredDataView = new DataView(filteredData);
            filteredDataView.RowFilter = dv.RowFilter;

            if (filteredDataView != null && filteredDataView.Count == 0)
            {
                ListPhone.ItemsSource = null; // Xóa dữ liệu hiện tại trong DataGrid
                return; // Không gán dữ liệu mới vào DataGrid
            }
            int totalItems = filteredDataView.Count;
            totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            // Hiển thị trang đầu tiên
            currentPage = 1;
            ShowCurrentPage();

            // Cập nhật ComboBox với danh sách các trang và hiển thị trang 1
            UpdateComboBoxCurrentPageInfo();
            //comboPage.SelectedItem = currentPage;
        }

        private void ShowCurrentPage()
        {
            int startIndex = (currentPage - 1) * itemsPerPage;
            int endIndex = startIndex + itemsPerPage - 1;

            // Create a DataTable for the current page
            DataTable currentPageData = filteredDataView.ToTable().AsEnumerable()
                                            .Skip(startIndex).Take(itemsPerPage).CopyToDataTable();

            // Bind the DataTable to the DataGrid
            ListPhone.ItemsSource = currentPageData.DefaultView;
        }

        // Xử lý khi chuyển đến trang trước đó
        private void btnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                ShowCurrentPage();
                UpdateComboBoxCurrentPage();
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            int totalItems = filteredDataView.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / itemsPerPage);

            if (currentPage < totalPages)
            {
                currentPage++;
                ShowCurrentPage();
                UpdateComboBoxCurrentPage();
            }
        }

        private void UpdateComboBoxCurrentPage()
        {
            comboPage.SelectedItem = currentPage;
            comboPage.SelectedIndex = currentPage-1; // Cập nhật ComboBox với trang hiện tại
        }

        private void UpdateComboBoxCurrentPageInfo()
        {
            List<string> pagesInfo = new List<string>();

            for (int i = 1; i <= totalPages; i++)
            {
                string pageInfo = $"{i} / {totalPages}";
                pagesInfo.Add(pageInfo);
            }

            comboPage.ItemsSource = pagesInfo; // Cập nhật ComboBox với thông tin trang hiện tại / tổng số trang
            comboPage.SelectedIndex = currentPage - 1; // Chọn trang hiện tại
        }

        private void comboPage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboPage.SelectedItem != null)
            {
                string[] pageInfo = comboPage.SelectedItem.ToString().Split('/');
                currentPage = int.Parse(pageInfo[0].Trim());
                ShowCurrentPage();
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            // Lấy hàng được chọn từ DataGrid
            
            if (ListPhone.SelectedItem is DataRowView selectedRow)
            {
                PhoneDTO phone = new PhoneDTO
                {
                    name = selectedRow["Name"].ToString(),
                    os = selectedRow["OS"].ToString(),
                    image = selectedRow["Image"].ToString(),
                    manufacturer = selectedRow["Manufacturer"].ToString(),
                    memoryStorage = selectedRow["MemoryStorage"].ToString(),
                    id = Convert.ToInt32(selectedRow["ID"].ToString()),
                    details = selectedRow["Details"].ToString(),
                    quantity =Convert.ToInt32(selectedRow["Quantity"].ToString()),
                    priceOriginal = Convert.ToInt32(selectedRow["PriceOriginal"].ToString())
                    

                };
                /* _phone.Add(phone);*/
                if (int.TryParse(selectedRow["Price"].ToString(), out int priceValue))
                {
                    phone.price = priceValue;
                }
                else
                {
                    phone.price = 0;
                }
                var screen=new EditWindow(phone);
                if (screen.ShowDialog() == true)
                {
                    // Nếu cập nhật thành công, cập nhật lại dữ liệu trong DataGrid
                    DataRowView editedRow = ListPhone.SelectedItem as DataRowView;
                    // Cập nhật các giá trị trong hàng được chọn từ cửa sổ chỉnh sửa
                    editedRow["Name"] = phone.name;
                    editedRow["OS"] = phone.os;
                    editedRow["Image"] = phone.image;
                    editedRow["Manufacturer"] = phone.manufacturer;
                    editedRow["MemoryStorage"] = phone.memoryStorage;
                    editedRow["Quantity"] = phone.quantity;
                    editedRow["PriceOriginal"] = phone.priceOriginal;
                    editedRow["Details"] = phone.details;
                    // Gọi Refresh để cập nhật hiển thị trong DataGrid
                    ListPhone.Items.Refresh();
                    SetPageComboBox();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ListPhone.SelectedItem is DataRowView selectedRow)
            {
                // Lấy thông tin cần thiết từ hàng được chọn trong DataGrid
                int idToDelete = Convert.ToInt32(selectedRow["ID"]); // ID của hàng cần xóa

                // Xóa hàng từ cơ sở dữ liệu
                bool deletedFromDatabase = DeleteFromDatabase(idToDelete); // Thay thế bằng logic xóa hàng từ cơ sở dữ liệu thực tế của bạn
                DataRow rowToDelete = MobileData.AsEnumerable().FirstOrDefault(row => row.Field<int>("ID") == idToDelete);

                if (deletedFromDatabase)
                {
                    // Xóa hàng từ DataGrid
                    //(ListPhone.ItemsSource as DataView)?.Table.Rows.Remove(selectedRow.Row);
                    MobileData.Rows.Remove(rowToDelete);

                    // Tạo DataView mới từ DataTable đã cập nhật
                    DataView newView = new DataView(MobileData);

                    // Gán DataView mới làm ItemsSource cho DataGrid
                    ListPhone.ItemsSource = newView;

                    // Cập nhật lại hiển thị của DataGrid
                    ListPhone.Items.Refresh();
                    SetPageComboBox();
                    ApplyFilters();
                    MessageBox.Show($"Delete successfully phone ID: {idToDelete}");
                }
                else
                {
                    // Xử lý trường hợp không xóa được từ cơ sở dữ liệu
                    MessageBox.Show("Delete failed");
                }
            }
        }

        private bool DeleteFromDatabase(int idToDelete)
        {
            _phoneBUS.deletePhone(idToDelete);
            return true;
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            int maxID = MobileData.AsEnumerable()
                .Select(row => row.Field<int>("ID"))
                .DefaultIfEmpty()
                .Max();
            
            var screen = new AddWindow(maxID+1);
            if (screen.ShowDialog() == true)
            {
                // Nếu cập nhật thành công, cập nhật lại dữ liệu trong DataGrid
                DataRow newRow = MobileData.NewRow();
                newRow["ID"] = maxID+1;
                newRow["Name"] =screen.phoneDTO.name;
                newRow["OS"] = screen.phoneDTO.os;
                newRow["Manufacturer"] = screen.phoneDTO.manufacturer;
                newRow["Price"] = screen.phoneDTO.price;
                newRow["PriceOriginal"] = screen.phoneDTO.priceOriginal;
                newRow["MemoryStorage"] = screen.phoneDTO.memoryStorage;
                newRow["Image"] = screen.phoneDTO.image;
                newRow["Quantity"] = screen.phoneDTO.quantity;
                newRow["Details"] = screen.phoneDTO.details;
 
                MobileData.Rows.Add(newRow);

                ListPhone.ItemsSource = MobileData.DefaultView;
                SetPageComboBox();
                ApplyFilters();
            }
        }

        private void restoreDB(object sender, RoutedEventArgs e)
        {
            if (_phoneBUS.restorePhone())
            {
                MessageBox.Show("Restore success");
            }
            else
            {
                MessageBox.Show(".bak not exits");
            }
        }
    }

}
