using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using MyShop.DAO;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public DataTable _dataMobile { get; set; } = new DataTable();
        public DataTable _dataOrder { get; set; } = new DataTable();

        public Home()
        {
            InitializeComponent();
            _dataMobile = LoadDataMobileIntoGrid();
            _dataOrder = LoadDataOrderIntoGrid();
            setTotalProductAndBrand();
            setDataGrid();
            UpdateChartWithOrdersByMonth();
            UpdateChartWithOrdersByWeek();

        }
        private void setDataGrid()
        {
            topSellersGrid.ItemsSource = GetTopProductsOfYear();
        }
        private void UpdateChartWithOrdersByWeek()
        {
            int currentYear = DateTime.Now.Year;
            int currentMonth = DateTime.Now.Month;

            // Lọc dữ liệu theo tháng và năm hiện tại
            var ordersThisMonth = _dataOrder.AsEnumerable()
                .Where(row => row.Field<DateTime>("CreateDate").Year == currentYear &&
                               row.Field<DateTime>("CreateDate").Month == currentMonth)
                .GroupBy(
                    row => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(row.Field<DateTime>("CreateDate"), CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday),
                    (key, group) => new
                    {
                        Week = key,
                        OrderCount = group.Count()
                    })
                .OrderBy(item => item.Week)
                .ToList();

            // Chuẩn bị dữ liệu cho biểu đồ
            SeriesCollection seriesCollection = new SeriesCollection();

            // Tạo series cho biểu đồ
            ColumnSeries columnSeries = new ColumnSeries
            {
                Title = "Orders",
                Values = new ChartValues<int>(ordersThisMonth.Select(x => x.OrderCount)),
                DataLabels = true,
                LabelPoint = point => point.Y.ToString() // Hiển thị số lượng trên cột
            };

            // Thêm series vào SeriesCollection
            seriesCollection.Add(columnSeries);

            // Tạo các nhãn cho trục X (các tuần)
            string[] labels = ordersThisMonth.Select(x => $"Week {x.Week}").ToArray();

            // Cập nhật dữ liệu vào biểu đồ
            chartWeek.Series = seriesCollection;
            chartWeek.AxisX.Clear();
            chartWeek.AxisX.Add(new Axis { Labels = labels });
        }
        private void UpdateChartWithOrdersByMonth()
        {
            int currentYear = DateTime.Now.Year;

            // Lọc dữ liệu theo năm hiện tại
            var ordersThisYear = _dataOrder.AsEnumerable()
                .Where(row => row.Field<DateTime>("CreateDate").Year == currentYear)
                .GroupBy(
                    row => row.Field<DateTime>("CreateDate").Month,
                    (key, group) => new
                    {
                        Month = key,
                        OrderCount = group.Count()
                    })
                .OrderBy(item => item.Month)
                .ToList();

            // Chuẩn bị dữ liệu cho biểu đồ
            SeriesCollection seriesCollection = new SeriesCollection();

            // Tạo series cho biểu đồ
            ColumnSeries columnSeries = new ColumnSeries
            {
                Title = "Orders",
                Values = new ChartValues<int>(ordersThisYear.Select(x => x.OrderCount)),
                DataLabels = true,
                LabelPoint = point => point.Y.ToString() // Hiển thị số lượng trên cột
            };

            // Thêm series vào SeriesCollection
            seriesCollection.Add(columnSeries);

            // Thiết lập trục X (các tháng)
            string[] labels = ordersThisYear.Select(x => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(x.Month)).ToArray();

            // Cập nhật dữ liệu vào biểu đồ
            chartMonth.Series = seriesCollection;
            chartMonth.AxisX.Clear();
            chartMonth.AxisX.Add(new Axis { Labels = labels });
        }
        private void setTotalProductAndBrand()
        {
            int totalAmount = 0;

            foreach (DataRow row in _dataMobile.Rows)
            {
                if (row["amount"] != DBNull.Value)
                {
                    totalAmount += Convert.ToInt32(row["amount"]);
                }
            }
            var uniqueManufacturers = _dataMobile.AsEnumerable()
                .Select(row => row.Field<string>("manufacturer"))
                .Where(manufacturer => !string.IsNullOrEmpty(manufacturer))
                .Distinct();

            int uniqueManufacturerCount = uniqueManufacturers.Count();

            txtTotalBrand.Text = uniqueManufacturerCount.ToString();
            txtTotalPhone.Text = totalAmount.ToString();
        }

        private DataTable LoadDataMobileIntoGrid()
        {

            string query = "SELECT * FROM Mobile"; // Thay đổi truy vấn cho phù hợp với bảng của bạn

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(Database.Instance.ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                try
                {
                    connection.Open();
                    adapter.Fill(dataTable); // Lấy dữ liệu từ SQL vào DataTable
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    Console.WriteLine("Lỗi: " + ex.Message);
                }
            }

            return dataTable;
        }
        private DataTable LoadDataOrderIntoGrid()
        {

            string query = "SELECT * FROM CustomerOrder"; // Thay đổi truy vấn cho phù hợp với bảng của bạn

            DataTable dataTable = new DataTable();

            using (SqlConnection connection = new SqlConnection(Database.Instance.ConnectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataAdapter adapter = new SqlDataAdapter(command);
                try
                {
                    connection.Open();
                    adapter.Fill(dataTable); // Lấy dữ liệu từ SQL vào DataTable
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi nếu có
                    Console.WriteLine("Lỗi: " + ex.Message);
                }
            }

            return dataTable;
        }

        private List<TopSellerInfo> GetTopProductsOfYear()
        {
            var productsQuantity = _dataMobile.AsEnumerable()
                .GroupBy(
                    row => row.Field<string>("name"),
                    (key, group) => new TopSellerInfo
                    {
                        PhoneName = key,
                        TotalAmount = group.Sum(row => row.Field<int>("amount"))
                    })
                .OrderBy(item => item.TotalAmount)
                .Take(5)
                .ToList();
            return productsQuantity;
        }


    }

}