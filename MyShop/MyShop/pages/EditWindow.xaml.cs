using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
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
using MyShop.DAO;
using MyShop.DTO;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        public PhoneDTO editPhone { get; set; }
        private readonly PhoneDAO phoneDAO;
        public EditWindow(PhoneDTO EditPhone)
        {
            InitializeComponent();
            editPhone = EditPhone;
            phoneDAO = new PhoneDAO();
            Unloaded += Window_Unloaded;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastClosedPage"].Value = "pages/EditWindow.xaml";
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = editPhone;
        }
        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            editPhone.name = NamePhone.Text;
            editPhone.os = OSPhone.Text;
            editPhone.manufacturer = ManufacturerPhone.Text;
            editPhone.memoryStorage = MemoryPhone.Text;
            editPhone.price =Convert.ToInt32(PricePhone.Text);
            editPhone.image= ImagePhone.Text;
            editPhone.priceOriginal = Convert.ToInt32(PriceOriginalPhone.Text);
            editPhone.quantity=Convert.ToInt32(QuantityPhone.Text);
            editPhone.details = DetailsPhone.Text;
            /*string sql = """           
                update ImportExcel
                set Name=@name, OS=@os, Manufacturer=@manufacturer, Price=@price,MemoryStorage=@memoryStorage, Image=@image, Details=@details, Quantity=@quantity, PriceOriginal=@priceOriginal
                where ID=@ID
                """;
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            var command = new SqlCommand(sql, Database.Instance.Connection);   
            command.Parameters.Add("@name", System.Data.SqlDbType.Text)
                .Value = editPhone.name;
            command.Parameters.Add("@os", System.Data.SqlDbType.Text)
                .Value = editPhone.os;
            command.Parameters.Add("@price", System.Data.SqlDbType.Int)
                .Value = editPhone.price;
            command.Parameters.Add("@manufacturer", System.Data.SqlDbType.Text)
                .Value = editPhone.manufacturer;
            command.Parameters.Add("@memoryStorage", System.Data.SqlDbType.Text)
                .Value = editPhone.memoryStorage;
            command.Parameters.Add("@image", System.Data.SqlDbType.Text)
                .Value = editPhone.image;
            command.Parameters.Add("@ID", System.Data.SqlDbType.Int)
                .Value = editPhone.id;
            command.Parameters.Add("@details", System.Data.SqlDbType.Text)
                .Value = editPhone.details;
            command.Parameters.Add("@quantity", System.Data.SqlDbType.Int)
                .Value = editPhone.quantity;
            command.Parameters.Add("@priceOriginal", System.Data.SqlDbType.Int)
                .Value = editPhone.priceOriginal;*/

            phoneDAO.updatePhone(editPhone);
            MessageBox.Show($"Insert successfully new phone: {Name}");
            DialogResult = true;
        }

    }
}
