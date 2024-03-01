using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;

namespace MyShop.DAO
{
    public class Database
    {
        private static Database? _instance = null;
        private SqlConnection _connection = null;

        public string ConnectionString { get; set; }
        public SqlConnection? Connection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SqlConnection(ConnectionString); ;
                    _connection.Open();
                }
                /*if (_connection.State != ConnectionState.Open)
                {
                    _connection = new SqlConnection(ConnectionString); ;
                    _connection.Open();
                }*/
                return _connection;
            }
        }

        public static Database Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Database();
                }
                return _instance;
            }
        }

        public string tablePhone = "Mobile";

        public string tableOrder = "OrderDetail";

        public string tableCustomerOrder = "CustomerOrder";

        public string tableAdmin = "Admin";

        public string tableCustomer = "CustomerClass";

        public string Name { get; set; }
        public bool tableExist { get; set; }
        public async Task ImportDataToSQLAsync()
        {
            try
            {
                // Mở kết nối đến SQL Server
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    tableExist = await TableExistsAsync();
                    if (!tableExist)
                    {
                        // Mở hộp thoại chọn file Excel
                        OpenFileDialog openFileDialog = new OpenFileDialog
                        {
                            Filter = "Excel Files|*.xls;*.xlsx"
                        };

                        if (openFileDialog.ShowDialog() == true)
                        {
                            string excelFilePath = openFileDialog.FileName;
                            // Đọc dữ liệu từ file Excel
                            using (var stream = System.IO.File.Open(excelFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                            {
                                using (var reader = ExcelReaderFactory.CreateReader(stream))
                                {
                                    var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration()
                                    {
                                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = true }
                                    });
                                    DataTable dataTable = dataSet.Tables[0];

                                    // Loại bỏ các cột không có header
                                    var columnsToRemove = new List<DataColumn>();
                                    foreach (DataColumn column in dataTable.Columns)
                                    {
                                        if (column.ColumnName.StartsWith("Column") || string.IsNullOrWhiteSpace(column.ColumnName))
                                        {
                                            columnsToRemove.Add(column);
                                        }
                                    }

                                    // Xóa các cột không có header
                                    foreach (var columnToRemove in columnsToRemove)
                                    {
                                        dataTable.Columns.Remove(columnToRemove);
                                    }

                                    foreach (DataColumn col in dataTable.Columns)
                                    {
                                        col.ColumnName = ConvertToNoSpaceNoAccent(col.ColumnName);
                                    }

                                    // Tạo bảng mới trong cơ sở dữ liệu nếu bảng không tồn tại
                                    CreateNewTable(connection, tablePhone, dataTable);

                                    // Import dữ liệu vào cơ sở dữ liệu
                                    ImportData(connection, tablePhone, dataTable);

                                }
                            }
                        }
                        MessageBox.Show("Data imported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }

        public async Task<bool> TableExistsAsync()
        {
            bool result = false;
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    await connection.OpenAsync(); // Mở kết nối cơ sở dữ liệu không đồng bộ
                    using (SqlCommand command = new SqlCommand($"IF OBJECT_ID('{tablePhone}', 'U') IS NOT NULL SELECT 1 ELSE SELECT 0", connection))
                    {
                        object queryResult = await command.ExecuteScalarAsync(); // Thực thi truy vấn không đồng bộ
                        result = (int)queryResult == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking table existence: {ex.Message}");
            }

            return result;
        }
        private void CreateNewTable(SqlConnection connection, string tableName, DataTable dataTable)
        {
            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (SqlCommand command = new SqlCommand($"CREATE TABLE {tableName} (", connection))
                {
                    string outputData = "";
                    foreach (DataColumn col in dataTable.Columns)
                    {
                        string columnType = GetSqlDbType(col.DataType);
                        command.CommandText += $"{col.ColumnName} {columnType}, ";
                    }
                    command.CommandText = command.CommandText.TrimEnd(',', ' ') + ");";
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ khi thực hiện truy vấn
                Console.WriteLine($"Error creating table: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Đóng kết nối sau khi sử dụng
                }
            }
        }
        private void ImportData(SqlConnection connection, string tableName, DataTable dataTable)
        {

            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = tableName;

                    foreach (DataColumn col in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.WriteToServer(dataTable);
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi ghi dữ liệu vào cơ sở dữ liệu
                Console.WriteLine($"Error during data import: {ex.Message}");
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close(); // Đóng kết nối sau khi sử dụng
                }
            }


        }

        private string ConvertToNoSpaceNoAccent(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            // Loại bỏ khoảng trắng và chuyển về chữ thường
            string noSpaceNoAccent = input.Replace(" ", "").ToLowerInvariant();

            // Dùng thư viện System.Globalization để loại bỏ dấu
            var normalizedString = noSpaceNoAccent.Normalize(NormalizationForm.FormKD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString();
        }

        // Hàm import dữ liệu vào bảng


        // Hàm chuyển đổi kiểu dữ liệu .NET sang kiểu dữ liệu SQL
        private string GetSqlDbType(Type dataType)
        {
            if (dataType == typeof(int) || dataType == typeof(double) || dataType == typeof(float))
            {
                return "INT";

            }
            else if (dataType == typeof(string))
            {
                return "NVARCHAR(300)";

            }
            else if (dataType == typeof(object))
            {
                return "NVARCHAR(300)"; // hoặc kiểu dữ liệu mặc định khác

            }
            else
                throw new NotSupportedException($"Unsupported data type: {dataType}");
        }
    }
}
