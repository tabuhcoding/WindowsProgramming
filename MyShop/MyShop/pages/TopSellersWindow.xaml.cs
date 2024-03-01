using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Configuration;
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

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for TopSellersWindow.xaml
    /// </summary>
    public partial class TopSellersWindow : Window
    {
        private DataTable _dataTable;
        private List<int> uniqueYears;
        private int _currentYear;
        public TopSellersWindow(DataTable data)
        {
            InitializeComponent();
            _dataTable = data;

            uniqueYears = _dataTable.AsEnumerable()
               .Select(row => ((DateTime)row["createDate"]).Year)
               .Distinct()
               .ToList();
            setYearsComboBox();

            Unloaded += Window_Unloaded;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastClosedPage"].Value = "pages/TopSellersWindow.xaml";
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }

        // Phương thức để nhận dữ liệu từ cửa sổ gọi và hiển thị nó trong DataGrid
        public void DisplayTopSellers(List<TopSellerInfo> topSellers)
        {
            topSellersGrid.ItemsSource = topSellers;
        }
        private void setYearsComboBox()
        {
            List<string> years = uniqueYears.Select(year => year.ToString()).ToList();
            years.Add("Chọn năm");
            YearCombobox.ItemsSource = years;
            YearCombobox.SelectedItem = years[years.Count - 1];
        }


        private void MonthCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearCombobox.SelectedItem != null && YearCombobox.SelectedItem.ToString() != "Chọn năm")
            {
                int selectedYear = Convert.ToInt32(YearCombobox.SelectedItem);

                // Kiểm tra xem đã chọn tháng từ MonthCombobox chưa
                if (MonthCombobox.SelectedItem != null && MonthCombobox.SelectedItem.ToString() != "Chọn tháng")
                {
                    ComboBoxItem selectedMonthItem = (ComboBoxItem)MonthCombobox.SelectedItem;
                    string selectedMonthText = selectedMonthItem.Content.ToString();

                    if (selectedMonthText.StartsWith("Tháng "))
                    {
                        string monthNumberText = selectedMonthText.Replace("Tháng ", "");
                        int selectedMonth = Convert.ToInt32(monthNumberText);

                        // Gọi hàm để lấy top 5 sản phẩm của tháng trong năm đã chọn
                        List<TopSellerInfo> topProducts = GetTopProductOfMonth(selectedYear, selectedMonth);

                        // Hiển thị danh sách top sản phẩm lên DataGrid
                        topSellersGrid.ItemsSource = topProducts;
                    }
                }
            }
        }

        private void YearCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YearCombobox.SelectedItem != null && YearCombobox.SelectedItem.ToString() != "Chọn năm")
            {
                int selectedYear = Convert.ToInt32(YearCombobox.SelectedItem);
                // Lấy danh sách top 5 sản phẩm có số lượng nhiều nhất trong năm được chọn
                List<TopSellerInfo> topProductsOfYear = GetTopProductsOfYear(selectedYear);
                // Hiển thị danh sách top sản phẩm lên DataGrid
                topSellersGrid.ItemsSource = topProductsOfYear;
                MonthCombobox.IsEnabled = true;
            }

        }

        private List<TopSellerInfo> GetTopProductsOfYear(int year)
        {
            List<TopSellerInfo> topProductsOfYear = new List<TopSellerInfo>();

            var productsOfYear = _dataTable.AsEnumerable()
                .Where(row => ((DateTime)row["createDate"]).Year == year)
                .GroupBy(row => row["PhoneName"])
                .Select(group => new
                {
                    ProductName = group.Key.ToString(),
                    TotalQuantity = group.Sum(row => Convert.ToInt32(row["amount"]))
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(5)
                .ToList();

            foreach (var productInfo in productsOfYear)
            {
                topProductsOfYear.Add(new TopSellerInfo
                {
                    PhoneName = productInfo.ProductName,
                    TotalAmount = productInfo.TotalQuantity
                });
            }

            return topProductsOfYear;
        }

        private List<TopSellerInfo> GetTopProductOfMonth(int year, int month)
        {
            // Lọc dữ liệu từ DataTable theo năm và tháng đã chọn
            var filteredData = _dataTable.AsEnumerable()
                .Where(row => ((DateTime)row["createDate"]).Year == year && ((DateTime)row["createDate"]).Month == month);

            // Tính toán tổng amount cho mỗi PhoneName
            var phoneNameTotalAmount = filteredData
                .GroupBy(row => row["PhoneName"])
                .Select(group => new
                {
                    PhoneName = group.Key.ToString(),
                    TotalAmount = group.Sum(row => Convert.ToDecimal(row["amount"]))
                })
                .OrderByDescending(x => x.TotalAmount)
                .Take(5)
                .ToList();

            // Chuyển kết quả sang danh sách TopSellerInfo (tùy thuộc vào cấu trúc dữ liệu của bạn)
            List<TopSellerInfo> topSellers = phoneNameTotalAmount.Select(item => new TopSellerInfo
            {
                PhoneName = item.PhoneName,
                TotalAmount = item.TotalAmount
            }).ToList();

            return topSellers;
        }
    }
}