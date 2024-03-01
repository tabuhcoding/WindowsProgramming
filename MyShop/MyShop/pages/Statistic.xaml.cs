using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using MyShop.DAO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
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
    /// Interaction logic for Statistic.xaml
    /// </summary>
    public partial class Statistic : Page
    {
        private CartesianChart _chart;
        private int _currentYear;
        private DataTable _dataTable;
        List<int> uniqueYears;
        private Frame _pageNavigation;
        public Statistic()
        {

            InitializeComponent();
            _chart = chart;
            Unloaded += Page_Unloaded;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastClosedPage"].Value = "pages/Statistic.xaml";
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            _dataTable = getDataFromDatabase();
            YearMode();
        }

        private void setYearsComboBox()
        {
            List<string> years = uniqueYears.Select(year => year.ToString()).ToList();
            years.Add("Chọn năm");
            YearCombobox.ItemsSource = years;
            YearCombobox.SelectedItem = years[years.Count - 1];
        }

        private DataTable getDataFromDatabase()
        {
            DataTable dataTable = new DataTable();
            try
            {
                string sql = $"select createDate, totalCost, totalProfit from CUSTOMERORDER";
                using (SqlCommand command = new SqlCommand(sql, Database.Instance.Connection))
                {
                    Database.Instance.Connection.Open();
                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        if (dataReader != null && !dataReader.IsClosed)
                        {
                            dataTable.Load(dataReader);
                        }
                        else
                        {
                            MessageBox.Show("No data found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
            finally
            {
                Database.Instance.Connection.Close();
            }

            return dataTable;
        }

        private void YearMode()
        {
            List<Int32> totalCostsByYear = new List<Int32>();
            List<Int32> totalProfitByYear = new List<Int32>();

            uniqueYears = _dataTable.AsEnumerable()
                .Select(row => ((DateTime)row["createDate"]).Year)
                .Distinct()
                .ToList();
            setYearsComboBox();

            // Lặp qua từng năm và tính tổng totalCost
            foreach (int year in uniqueYears)
            {
                Int32 totalCostOfYear = _dataTable.AsEnumerable()
                    .Where(row => ((DateTime)row["createDate"]).Year == year)
                    .Sum(row => (Int32)row["totalCost"]);

                totalCostsByYear.Add(totalCostOfYear);
            }

            foreach (int year in uniqueYears)
            {
                Int32 totalProfitOfYear = _dataTable.AsEnumerable()
                    .Where(row => ((DateTime)row["createDate"]).Year == year)
                    .Sum(row => (Int32)row["totalProfit"]);

                totalProfitByYear.Add(totalProfitOfYear);
            }


            var valuesColChart = new ChartValues<Int32>(totalCostsByYear);
            var valuesLineChart = new ChartValues<Int32>(totalProfitByYear);

            /* if (_chart==null)
             {
                 _chart = new CartesianChart();
             }*/
            _chart.Series = new SeriesCollection();
            _chart.AxisX = new AxesCollection();

            _chart.Series.Add(new ColumnSeries()
            {
                Fill = Brushes.Firebrick,
                Title = "Doanh thu theo năm",
                Values = valuesColChart
            });

            _chart.Series.Add(new LineSeries()
            {
                Stroke = Brushes.DimGray,
                Title = "Lợi nhuận theo năm",
                Values = valuesLineChart
            });

            var currentYear = DateTime.Now.Year;
            string[] yearLabels = uniqueYears.Select(year => year.ToString()).ToArray();
            chart.AxisX.Add(new Axis()
            {
                Foreground = Brushes.Black,
                Title = "Năm",
                Labels = yearLabels
                /*Labels = new string[] {
                    $"{currentYear - 2}",
                    $"{currentYear - 1}",
                    $"{currentYear}",
                }*/
            });
            Title.Text = "Đang hiển thị chế độ xem theo năm";
        }

        private void MonthMode(int year)
        {


            List<Int32> totalCostsByMonth = new List<Int32>();
            List<Int32> totalProfitByMonth = new List<Int32>();

            // Lọc dữ liệu theo năm cụ thể
            var ordersInYear = _dataTable.AsEnumerable()
                .Where(row => ((DateTime)row["createDate"]).Year == year);

            // Lặp qua từng tháng và tính tổng totalCost và totalProfit
            for (int month = 1; month <= 12; month++)
            {
                Int32 totalCostOfMonth = ordersInYear
                    .Where(row => ((DateTime)row["createDate"]).Month == month)
                    .Sum(row => (Int32)row["totalCost"]);

                Int32 totalProfitOfMonth = ordersInYear
                    .Where(row => ((DateTime)row["createDate"]).Month == month)
                    .Sum(row => (Int32)row["totalProfit"]);

                totalCostsByMonth.Add(totalCostOfMonth);
                totalProfitByMonth.Add(totalProfitOfMonth);
            }

            // Lặp qua từng năm và tính tổng totalCost



            var valuesColChart = new ChartValues<Int32>(totalCostsByMonth);
            var valuesLineChart = new ChartValues<Int32>(totalProfitByMonth);

            _chart.Series = new SeriesCollection();
            _chart.AxisX = new AxesCollection();

            _chart.Series.Add(new ColumnSeries()
            {
                Fill = Brushes.Firebrick,
                Title = "Doanh thu theo tháng",
                Values = valuesColChart
            });

            _chart.Series.Add(new LineSeries()
            {
                Stroke = Brushes.DimGray,
                Title = "Lợi nhuận theo tháng",
                Values = valuesLineChart
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
        }

        private void WeekMode(int month, int year)
        {
            List<Int32> totalCostsByWeek = new List<Int32>();
            List<Int32> totalProfitByWeek = new List<Int32>();

            // Tạo một biến để lưu trữ ngày đầu tiên của tháng
            DateTime startDate = new DateTime(year, month, 1);

            // Xác định ngày cuối cùng của tháng
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DateTime endDate = new DateTime(year, month, daysInMonth);

            // Lặp qua từng tuần trong tháng
            DateTime currentWeekStart = startDate;
            while (currentWeekStart < endDate)
            {
                // Xác định ngày cuối cùng của tuần
                DateTime currentWeekEnd = currentWeekStart.AddDays(6); // 6 ngày sau để tới cuối tuần

                // Lọc dữ liệu theo tuần
                var ordersInWeek = _dataTable.AsEnumerable()
                    .Where(row => ((DateTime)row["createDate"]) >= currentWeekStart && ((DateTime)row["createDate"]) <= currentWeekEnd);

                // Tính tổng totalCost và totalProfit trong tuần và thêm vào danh sách
                Int32 totalCostOfWeek = ordersInWeek.Sum(row => (Int32)row["totalCost"]);
                Int32 totalProfitOfWeek = ordersInWeek.Sum(row => (Int32)row["totalProfit"]);

                totalCostsByWeek.Add(totalCostOfWeek);
                totalProfitByWeek.Add(totalProfitOfWeek);

                // Chuyển sang tuần tiếp theo
                currentWeekStart = currentWeekStart.AddDays(7); // Di chuyển tới ngày bắt đầu của tuần tiếp theo
            }
            var valuesColChart = new ChartValues<Int32>(totalCostsByWeek);
            var valuesLineChart = new ChartValues<Int32>(totalProfitByWeek);
            _chart.Series = new SeriesCollection();
            _chart.AxisX = new AxesCollection();

            _chart.Series.Add(new ColumnSeries()
            {
                Fill = Brushes.Firebrick,
                Title = "Doanh thu theo tuần",
                Values = valuesColChart
            });

            _chart.Series.Add(new LineSeries()
            {
                Stroke = Brushes.DimGray,
                Title = "Lợi nhuận theo tuần",
                Values = valuesLineChart
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
        }

        private void DateMode(DateTime startDate, DateTime endDate)
        {
            List<Int32> totalCostsByDay = new List<Int32>();
            List<Int32> totalProfitByDay = new List<Int32>();

            var ordersInRange = _dataTable.AsEnumerable()
                .Where(row => ((DateTime)row["createDate"]) >= startDate && ((DateTime)row["createDate"]) <= endDate);

            // Tạo một danh sách các ngày trong khoảng thời gian
            List<DateTime> daysInRange = new List<DateTime>();
            DateTime currentDate = startDate;
            while (currentDate <= endDate)
            {
                daysInRange.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            // Lặp qua từng ngày trong khoảng thời gian để tính tổng theo ngày
            foreach (DateTime day in daysInRange)
            {
                var ordersOnDay = ordersInRange
                    .Where(row => ((DateTime)row["createDate"]).Date == day.Date);

                Int32 totalCostOfDay = ordersOnDay.Sum(row => (Int32)row["totalCost"]);
                Int32 totalProfitOfDay = ordersOnDay.Sum(row => (Int32)row["totalProfit"]);

                totalCostsByDay.Add(totalCostOfDay);
                totalProfitByDay.Add(totalProfitOfDay);
            }

            var valuesColChart = new ChartValues<Int32>(totalCostsByDay);
            var valuesLineChart = new ChartValues<Int32>(totalProfitByDay);

            _chart.Series = new SeriesCollection();
            _chart.AxisX = new AxesCollection();

            _chart.Series.Add(new ColumnSeries()
            {
                Fill = Brushes.Chocolate,
                Title = "Doanh thu theo ngày",
                Values = valuesColChart
            });

            _chart.Series.Add(new LineSeries()
            {
                Stroke = Brushes.DeepSkyBlue,
                Title = "Lợi nhuận theo ngày",
                Values = valuesLineChart
            });
            List<string> labels = daysInRange.Select(day => day.ToString("dd/MM/yyyy")).ToList();
            chart.AxisX.Add(
                new Axis()
                {
                    Foreground = Brushes.Black,
                    Title = "Ngày",
                    Labels = labels.ToArray()
                }); ;
            Title.Text = "Đang hiển thị chế độ xem theo ngày";
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
                DateMode((DateTime)startDate, (DateTime)endDate);
            }
        }

        private void NextProductReport_Click(object sender, RoutedEventArgs e)
        {
            //_pageNavigation.NavigationService.Navigate(new ProductStatistic(_pageNavigation));
            ProductStatistic newPage = new ProductStatistic();
        }


        private void MonthCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = MonthCombobox.SelectedIndex;
            if (i == -1 || i == 0)
            {
                return;
            }
            else
            {
                WeekMode(i, _currentYear);
            }
        }

        private void YearCombobox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int _year;
            string year = YearCombobox.SelectedItem?.ToString();
            if (year.Equals("Chọn năm"))
            {
                YearMode();
                MonthCombobox.IsEnabled = false;
                MonthCombobox.SelectedIndex = 0;
            }
            else
            {
                _year = Convert.ToInt32(year);
                MonthMode(_year);
                _currentYear = _year;
            }
        }

    }
}