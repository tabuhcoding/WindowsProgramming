using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MyShop.DTO;
using System;
using System.IO;

namespace MyShop.DAO
{
    public class PhoneDAO
    {
        public SqlDataAdapter getAll()
        {
            var sql = $"SELECT ID, Name, OS, Price, PriceOriginal, Quantity, Manufacturer, MemoryStorage, Details, Image FROM {Database.Instance.tablePhone}";
            var command = new SqlCommand(sql, Database.Instance.Connection);
            if (Database.Instance.Connection != null) { Database.Instance.Connection.Close(); }

            SqlDataAdapter adapter = new SqlDataAdapter(command);
            return adapter;
        }
        public bool insertPhone(PhoneDTO phone)
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            var sql = @"INSERT INTO " + Database.Instance.tablePhone + "(ID,Name,Quantity, OS, Manufacturer,PriceOriginal, Price, MemoryStorage,Image,Details)"
                    + " values (@ID,@Name,@Quantity, @OS, @Manufacturer,@PriceOriginal, @Price,@MemoryStorage,@Image,@Details)";
            var command = new SqlCommand(sql, Database.Instance.Connection);

            command.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = phone.id;
            command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = phone.name;
            command.Parameters.Add("@Quantity", System.Data.SqlDbType.Int).Value = phone.quantity;
            command.Parameters.Add("@OS", System.Data.SqlDbType.NVarChar).Value = phone.os;
            command.Parameters.Add("@Manufacturer", System.Data.SqlDbType.NVarChar).Value = phone.manufacturer;
            command.Parameters.Add("@PriceOriginal", System.Data.SqlDbType.Int).Value = phone.priceOriginal;
            command.Parameters.Add("@Price", System.Data.SqlDbType.Int).Value = phone.price;
            command.Parameters.Add("@MemoryStorage", System.Data.SqlDbType.NVarChar).Value = phone.memoryStorage;
            command.Parameters.Add("@Image", System.Data.SqlDbType.NVarChar).Value = phone.image;
            command.Parameters.Add("@Details", System.Data.SqlDbType.NVarChar).Value = phone.details;
            
            int result = command.ExecuteNonQuery();

            return result > 0;
        }
        public void deletePhoneByID(int phoneID)
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string backupFileName = Path.Combine(Path.GetDirectoryName(exePath), "backup.bak");
            string backupQuery = @"BACKUP DATABASE " + Database.Instance.Name + @" TO DISK = '" + backupFileName + "' WITH FORMAT, INIT, NAME = N'YourBackupName'";
            var command = new SqlCommand(backupQuery, Database.Instance.Connection);
            command.ExecuteNonQuery();

            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            var sql = @"DELETE FROM " + Database.Instance.tablePhone + " WHERE ID = @ID";
            command = new SqlCommand(sql, Database.Instance.Connection);

            command.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = phoneID;

            command.ExecuteNonQuery();
        }

        public bool restorePhone()
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            Database.Instance.Connection.Close();
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string backupFileName = Path.Combine(Path.GetDirectoryName(exePath), "backup.bak");
            if (File.Exists(backupFileName))
            {



                // Tập tin backup tồn tại, thực hiện restore
                string restoreQuery = $@"USE master; " + " Alter Database " + Database.Instance.Name + $@" Set Single_User With Rollback Immediate;" + $@" RESTORE DATABASE {Database.Instance.Name} FROM DISK = '{backupFileName}' WITH REPLACE, RECOVERY;";
                if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
                {
                    Database.Instance.Connection.Open();
                }
                var command = new SqlCommand(restoreQuery, Database.Instance.Connection);
                command.ExecuteNonQuery();
                return true;
            }
            else
            {
                return false;
            }

            

        }

        public void updatePhone(PhoneDTO phone)
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            var sql = @"UPDATE " + Database.Instance.tablePhone +
                " set Name=@name, OS=@os, Manufacturer=@manufacturer, Price=@price,MemoryStorage=@memoryStorage, Image=@image, Details=@details, Quantity=@quantity, PriceOriginal=@priceOriginal" +
                " where ID=@ID";
            var command = new SqlCommand(sql, Database.Instance.Connection);
            command.Parameters.Add("@name", System.Data.SqlDbType.Text)
                .Value = phone.name;
            command.Parameters.Add("@os", System.Data.SqlDbType.Text)
                .Value = phone.os;
            command.Parameters.Add("@price", System.Data.SqlDbType.Int)
                .Value = phone.price;
            command.Parameters.Add("@manufacturer", System.Data.SqlDbType.Text)
                .Value = phone.manufacturer;
            command.Parameters.Add("@memoryStorage", System.Data.SqlDbType.Text)
                .Value = phone.memoryStorage;
            command.Parameters.Add("@image", System.Data.SqlDbType.Text)
                .Value = phone.image;
            command.Parameters.Add("@ID", System.Data.SqlDbType.Int)
                .Value = phone.id;
            command.Parameters.Add("@details", System.Data.SqlDbType.Text)
                .Value = phone.details;
            command.Parameters.Add("@quantity", System.Data.SqlDbType.Int)
                .Value = phone.quantity;
            command.Parameters.Add("@priceOriginal", System.Data.SqlDbType.Int)
                .Value = phone.priceOriginal;

            command.ExecuteNonQuery();
        }
    }
}
