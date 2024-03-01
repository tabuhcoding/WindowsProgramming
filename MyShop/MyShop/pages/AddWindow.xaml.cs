using Microsoft.Data.SqlClient;
using MyShop.BUS;
using MyShop.DAO;
using MyShop.DTO;
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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace MyShop.pages
{
    /// <summary>
    /// Interaction logic for AddWindow.xaml
    /// </summary>
    public partial class AddWindow : Window
    {
        private int ID;

        private readonly PhoneBUS phonebus;

        public PhoneDTO phoneDTO;
 
        public AddWindow(int Id)
        {
            InitializeComponent();
            ID = Id;
            phonebus = new PhoneBUS();
            Unloaded += Window_Unloaded;
        }

        private void Window_Unloaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["LastClosedPage"].Value = "pages/AddWindow.xaml";
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {

            phoneDTO = new PhoneDTO()
            {
                id = this.ID,
                name = NamePhone.Text,
                os = OSPhone.Text,
                manufacturer = ManufacturerPhone.Text,
                memoryStorage = MemoryPhone.Text,
                price = Convert.ToInt32(PricePhone.Text),
                image = ImagePhone.Text,
                quantity = Convert.ToInt32(QuantityPhone.Text),
                priceOriginal = Convert.ToInt32(PriceOriginalPhone.Text),
                details = DetailsPhone.Text,
            };

     /*       string sql = """           
                insert into ImportExcel(ID,Name,Quantity, OS, Manufacturer,PriceOriginal, Price, MemoryStorage,Image,Details )
                values (@ID,@Name,@Quantity, @OS, @Manufacturer,@PriceOriginal, @Price,@MemoryStorage,@Image,@Details)
                select ident_current('ImportExcel')
                """;
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            var command = new SqlCommand(sql, Database.Instance.Connection);
            command.Parameters.Add("@ID", System.Data.SqlDbType.Int)
                .Value = ID;
            command.Parameters.Add("@OS", System.Data.SqlDbType.Text)
                .Value = OS;
            command.Parameters.Add("@Manufacturer", System.Data.SqlDbType.Text)
                .Value = Manufacturer;
            command.Parameters.Add("@Name", System.Data.SqlDbType.Text)
                .Value = Name;
            command.Parameters.Add("@Price", System.Data.SqlDbType.Int)
                .Value = Price;
            command.Parameters.Add("@MemoryStorage", System.Data.SqlDbType.Text)
                .Value = MemoryStorage;
            command.Parameters.Add("@Image", System.Data.SqlDbType.Text)
                .Value = Image;
            command.Parameters.Add("@details", System.Data.SqlDbType.Text)
               .Value = Details;
            command.Parameters.Add("@quantity", System.Data.SqlDbType.Int)
                .Value = Quantity;
            command.Parameters.Add("@priceOriginal", System.Data.SqlDbType.Int)
                .Value = PriceOriginal;*/
            bool count = phonebus.insertPhone(phoneDTO);

            if (count)
            {
                MessageBox.Show($"Insert successfully new phone: {Name}");
                DialogResult = true;

            }
            else
            {
                MessageBox.Show("Insert failed");
            }

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
           this.Close();
        }
    }
}
