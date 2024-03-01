using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Configuration;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using Microsoft.Data.SqlClient;
using LiveCharts.Wpf.Charts.Base;
using System.Globalization;
using MyShop.DAO;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for ProductStatistic.xaml
    /// </summary>
    public partial class ProductStatistic : Page
    {
        private DataTable _dataTable;
        private CartesianChart _chart;
        private List<int> uniqueYears;
        //private Frame _pageNavigation;
        public ProductStatistic()
        {
            _chart = chart;

            //_pageNavigation = pageNavigation;
            InitializeComponent();
            Unloaded += Page_Unloaded;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastClosedPage"].Value = "pages/ProductStatistic.xaml";
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }

        private enum Mode
        {
            Year,
            Month,
            Week,
            Date
        }

        private int _currentYear;
        private string _currentProduct;
        Mode currentMode = Mode.Year;
        public DataTable GetDataFromDatabase()
        {
            DataTable dataTable = new DataTable();

            // Thay đổi connection string thành thông tin kết nối của bạn

            using (SqlConnection connection = new SqlConnection(Database.Instance.ConnectionString))
            {
                connection.Open();
                // Tạo câu truy vấn để kết hợp dữ liệu từ hai bảng
                string query = "SELECT od.OrderID, od.PhoneName, co.CreateDate,od.amount " +
                               "FROM OrderDetail od " +
                               "INNER JOIN CustomerOrder co ON od.OrderID = co.OrderID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable); // Đổ dữ liệu vào DataTable
                    }
                }
            }

            return dataTable;
        }
        private void setYearsComboBox()
        {
            List<string> years = uniqueYears.Select(year => year.ToString()).ToList();
            years.Add("Chọn năm");
            YearCombobox.ItemsSource = years;
            YearCombobox.SelectedItem = years[years.Count - 1];
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _dataTable = GetDataFromDatabase();
            chart.AxisY.Add(new Axis
            {
                Foreground = Brushes.Black,
                Title = "Số lượng đã bán",
                MinValue = 0,
                Separator = new LiveCharts.Wpf.Separator
                {
                    Step = 1 // Đặt khoảng cách giữa các giá trị là 1
                }
            });
            Title.Text = "Đang hiển thị chế độ xem theo năm";

            List<string> products = new List<string>();

            // Duyệt qua từng dòng trong DataTable và lấy giá trị của cột PhoneName
            foreach (DataRow row in _dataTable.Rows)
            {
                string phoneName = row["PhoneName"].ToString();
                products.Add(phoneName);
            }


            ProductsCombobox.ItemsSource = products;
            ProductsCombobox.SelectedIndex = 0;

            _currentProduct = ProductsCombobox.SelectedItem?.ToString();
            YearMode(_currentProduct);
        }
        private void YearMode(string product)
        {
            uniqueYears = _dataTable.AsEnumerable()
                .Select(row => ((DateTime)row["createDate"]).Year)
                .Distinct()
                .ToList();
            setYearsComboBox();

            List<int> productCountByYear = new List<int>();

            foreach (int year in uniqueYears)
            {
                int productCountForYear = _dataTable.AsEnumerable()
                    .Where(row => ((DateTime)row["CreateDate"]).Year == year &&
                                  row["PhoneName"].ToString().Equals(product, StringComparison.OrdinalIgnoreCase))
                    .Count();
                productCountByYear.Add(productCountForYear);
            }

            var valuesColChart = new ChartValues<int>(productCountByYear);

            chart.Series = new SeriesCollection();
            chart.AxisX = new AxesCollection();

            chart.Series.Add(new ColumnSeries()
            {
                Fill = Brushes.Firebrick,
                Title = "Số lượng đã bán theo năm",
                Values = valuesColChart
            });
            string[] yearLabels = uniqueYears.Select(year => year.ToString()).ToArray();
            var currentYear = DateTime.Now.Year;
            chart.AxisX.Add(
                new Axis()
                {
                    Foreground = Brushes.Black,
                    Title = "Năm",
                    Labels = yearLabels
                });
            Title.Text = "Đang hiển thị chế độ xem theo năm";
            currentMode = Mode.Year;
        }

        private void MonthMode(string product, int year)
        {
            List<int> productCountByMonth = new List<int>();

            for (int month = 1; month <= 12; month++)
            {
                int productCountForMonth = _dataTable.AsEnumerable()
                    .Where(row => ((DateTime)row["CreateDate"]).Year == year &&
                                  ((DateTime)row["CreateDate"]).Month == month &&
                                  row["PhoneName"].ToString().Equals(product, StringComparison.OrdinalIgnoreCase))
                    .Count();

                productCountByMonth.Add(productCountForMonth);
            }

            var valuesColChart = new ChartValues<int>(productCountByMonth);

            chart.Series = new SeriesCollection();
            chart.AxisX = new AxesCollection();

            chart.Series.Add(new ColumnSeries()
            {
                Fill = Brushes.Chocolate,
                Title = "Số lượng đã bán theo tháng",
                Values = valuesColChart
            });

            chart.AxisX.Add(
                new Axis()
                {
                    Foreground = Brushes.Black,
                    Title = "Tháng",
                    Labels = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12",
                    }
                });
            Title.Text = "Đang hiển thị chế độ xem theo tháng";
            MonthCombobox.IsEnabled = true;
            MonthCombobox.SelectedIndex = 0;
            currentMode = Mode.Month;
        }

        private void WeekMode(string product, int month, int year)
        {
            List<int> productCountByWeek = new List<int>();

            DateTime startDate = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime endDate = new DateTime(year, month, daysInMonth);

            // Lặp qua từng tuần trong tháng
            DateTime currentWeekStart = startDate;

            while (currentWeekStart < endDate)
            {
                DateTime currentWeekEnd = currentWeekStart.AddDays(7); // Ngày cuối cùng của tuần

                int productCountForWeek = _dataTable.AsEnumerable()
                    .Where(row => ((DateTime)row["CreateDate"]).Year == year &&
                                  ((DateTime)row["CreateDate"]).Month == month &&
                                  ((DateTime)row["CreateDate"]) >= currentWeekStart &&
                                  ((DateTime)row["CreateDate"]) < currentWeekEnd &&
                                  row["PhoneName"].ToString().Equals(product, StringComparison.OrdinalIgnoreCase))
                    .Count();

                productCountByWeek.Add(productCountForWeek);

                currentWeekStart = currentWeekEnd; // Di chuyển đến tuần tiếp theo
            }

            var valuesColChart = new ChartValues<int>(productCountByWeek);

            chart.Series = new SeriesCollection();
            chart.AxisX = new AxesCollection();

            chart.Series.Add(new ColumnSeries()
            {
                Fill = Brushes.Chocolate,
                Title = "Số lượng đã bán theo tuần",
                Values = valuesColChart
            });

            chart.AxisX.Add(
                new Axis()
                {
                    Foreground = Brushes.Black,
                    Title = "Tuần",
                    Labels = new string[] { "1", "2", "3", "4", "5",
                    }
                });
            Title.Text = "Đang hiển thị chế độ xem theo tuần";
            currentMode = Mode.Week;
        }

        private void DateMode(string product, DateTime startDate, DateTime endDate)
        {
            List<int> productCountByDay = new List<int>();

            DateTime currentDate = startDate;
            List<DateTime> daysInRange = new List<DateTime>();

            while (currentDate <= endDate)
            {
                int productCountForDay = _dataTable.AsEnumerable()
                    .Where(row => ((DateTime)row["CreateDate"]).Date == currentDate.Date &&
                                  row["PhoneName"].ToString().Equals(product, StringComparison.OrdinalIgnoreCase))
                    .Count();

                productCountByDay.Add(productCountForDay);

                daysInRange.Add(currentDate);
                currentDate = currentDate.AddDays(1); // Di chuyển đến ngày tiếp theo
            }

            var valuesColChart = new ChartValues<int>(productCountByDay);

            chart.Series = new SeriesCollection();
            chart.AxisX = new AxesCollection();

            chart.Series.Add(new ColumnSeries()
            {
                Fill = Brushes.Chocolate,
                Title = "Số lượng đã bán theo ngày",
                Values = valuesColChart
            });
            List<string> labels = daysInRange.Select(day => day.ToString("dd/MM/yyyy")).ToList();
            chart.AxisX.Add(
                new Axis()
                {
                    Foreground = Brushes.Black,
                    Title = "Date",
                    Labels = labels
                });
            Title.Text = "Đang hiển thị chế độ xem theo ngày";
            currentMode = Mode.Date;
        }

        private void YearCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _year;
            string year = YearCombobox.SelectedItem?.ToString();
            if (year.Equals("Chọn năm"))
            {
                YearMode(_currentProduct);
                MonthCombobox.IsEnabled = false;
                MonthCombobox.SelectedIndex = 0;
            }
            else
            {
                _year = Convert.ToInt32(year);
                MonthMode(_currentProduct, _year);
                _currentYear = _year;
            }
        }

        private void MonthCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = MonthCombobox.SelectedIndex;
            if (index > 0)
            {
                WeekMode(_currentProduct, index, _currentYear);
            }
        }

        private void ProductsCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _currentProduct = (string)ProductsCombobox.SelectedValue;
            if (currentMode == Mode.Year)
            {
                YearMode(_currentProduct);
            }
            else if (currentMode == Mode.Month)
            {
                int _year;
                string year = YearCombobox.SelectedItem?.ToString();
                if (year.Equals("Chọn năm"))
                {
                    YearMode(_currentProduct);
                    MonthCombobox.IsEnabled = false;
                    MonthCombobox.SelectedIndex = 0;
                }
                else
                {
                    _year = Convert.ToInt32(year);
                    MonthMode(_currentProduct, _year);
                    _currentYear = _year;
                }
            }
            else if (currentMode == Mode.Week)
            {
                int index = MonthCombobox.SelectedIndex;
                if (index > 0)
                {
                    WeekMode(_currentProduct, index, _currentYear);
                }
            }
            else if (currentMode == Mode.Date)
            {
                var startDate = StartDate.SelectedDate;
                var endDate = EndDate.SelectedDate;

                if (startDate == null || endDate == null)
                {
                    MessageBox.Show("Vui lòng chọn đủ ngày bắt đầu và kết thúc!", "Thông báo",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    YearCombobox.SelectedIndex = 0;
                    MonthCombobox.SelectedIndex = 0;
                    DateMode(_currentProduct, (DateTime)startDate, (DateTime)endDate);
                }
            }
        }



        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            //_pageNavigation.NavigationService.GoBack();
        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var startDate = StartDate.SelectedDate;
            var endDate = EndDate.SelectedDate;

            if (startDate == null || endDate == null)
            {
                MessageBox.Show("Vui lòng chọn đủ ngày bắt đầu và kết thúc!", "Thông báo",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                YearCombobox.SelectedIndex = 0;
                MonthCombobox.SelectedIndex = 0;
                DateMode(_currentProduct, (DateTime)startDate, (DateTime)endDate);
            }
        }

        private void btnListBestSeller_Click(object sender, RoutedEventArgs e)
        {
            /*Dictionary<string, int> phoneNameTotalAmount = new Dictionary<string, int>();

            // Lặp qua mỗi hàng trong DataTable để tính tổng amount cho mỗi PhoneName
            foreach (DataRow row in _dataTable.Rows)
            {
                string phoneName = row["PhoneName"].ToString();
                int amount = Convert.ToInt32(row["amount"]);

                // Nếu phoneName đã tồn tại trong Dictionary, cập nhật tổng amount
                if (phoneNameTotalAmount.ContainsKey(phoneName))
                {
                    phoneNameTotalAmount[phoneName] += amount;
                }
                else // Nếu chưa tồn tại, thêm vào Dictionary với giá trị amount ban đầu
                {
                    phoneNameTotalAmount.Add(phoneName, amount);
                }
            }

            // Sắp xếp theo giảm dần của tổng amount và chọn top 5
            var top5PhoneNames = phoneNameTotalAmount.OrderByDescending(x => x.Value)
                                                     .Take(5)
                                                     .ToList();

            List<TopSellerInfo> topSellers = new List<TopSellerInfo>();
            foreach (var item in top5PhoneNames)
            {
                topSellers.Add(new TopSellerInfo { PhoneName = item.Key, TotalAmount = item.Value });
            }*/

            TopSellersWindow topSellersWindow = new TopSellersWindow(_dataTable);
            //topSellersWindow.DisplayTopSellers(topSellers);
            topSellersWindow.Show();
        }
    }
}