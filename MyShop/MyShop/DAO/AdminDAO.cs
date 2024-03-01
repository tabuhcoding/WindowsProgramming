using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MyShop.DTO;
using MyShop.Helpers;

namespace MyShop.DAO
{
    public class AdminDAO
    {
        public AdminDTO? GetAdmin(string username, string password)
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            var sql = "SELECT * FROM [Admin] WHERE Email = @Username";

            string hashedPassword = Encryption.Encrypt(password, "1234567890123456");

            var command = new SqlCommand(sql, Database.Instance.Connection);
            command.Parameters.Add("@Username", System.Data.SqlDbType.Char)
                    .Value = username;
            /*            command.Parameters.Add("@Password", System.Data.SqlDbType.Text)
                                .Value = hashedPassword;*/

            var reader = command.ExecuteReader();

            AdminDTO? admin = null;
            if (reader.Read())
            {
                string passwordResult = (string)reader["Password"];
                if (passwordResult == hashedPassword)
                {
                    admin = new AdminDTO()
                    {
                        ID = (string)reader["ID"],
                        FirstName = (string)reader["FirstName"],
                        LastName = (string)reader["LastName"],
                        Email = (string)reader["Email"],
                        Phone = (string)reader["Phone"],
                        Address = (string)reader["Address"],
                        Gender = (string)reader["Gender"],
                        Age = (int)reader["Age"],
                        Password = (string)reader["Password"],
                    };
                }
                else
                {
                    reader.Close();
                    return null;
                }
            }
            reader.Close();

            return admin;
        }

        public bool IsExistsID(string id)
        {
            var sql = @"SELECT * FROM Admin WHERE ID = @ID";
            var command = new SqlCommand(sql, Database.Instance.Connection);
            command.Parameters.Add("@ID", System.Data.SqlDbType.Char).Value = id;

            var reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                reader.Close();
                return true;
            }
            reader.Close();
            return false;
        }

        public bool CreateAdmin(AdminDTO admin)
        {
            // add to database
            var sql = @"insert into Admin(id, firstName, lastName, gender, email, address, phone, age, password) 
                                values(@ID, @FirstName, @LastName, @Gender, @Email,
                                @Address, @Phone, @Age, @Password)";
            var command = new SqlCommand(sql, Database.Instance.Connection);
            command.Parameters.Add("@ID", System.Data.SqlDbType.Char)
                        .Value = admin.ID;
            command.Parameters.Add("@FirstName", System.Data.SqlDbType.Text)
                .Value = admin.FirstName;
            command.Parameters.Add("@LastName", System.Data.SqlDbType.Text)
                .Value = admin.LastName;
            command.Parameters.Add("@Gender", System.Data.SqlDbType.Text)
                .Value = admin.Gender;
            command.Parameters.Add("@Email", System.Data.SqlDbType.Text)
                .Value = admin.Email;
            command.Parameters.Add("@Address", System.Data.SqlDbType.Text)
                .Value = admin.Address;
            command.Parameters.Add("@Phone", System.Data.SqlDbType.Text)
                .Value = admin.Phone;
            command.Parameters.Add("@Age", System.Data.SqlDbType.Int)
                .Value = admin.Age;
            command.Parameters.Add("@Password", System.Data.SqlDbType.Text)
                .Value = admin.Password;

            int rowsAffected = command.ExecuteNonQuery();

            return rowsAffected > 0;
        }
    }
}
