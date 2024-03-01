using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MyShop.DTO;

namespace MyShop.DAO
{
    public class CustomerDAO
    {
        public CustomerDTO getCustomerByPhone(string phoneNumber)
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            List<CustomerDTO> list = new List<CustomerDTO>();
            CustomerDTO result = new();

            var sql = @"SELECT * FROM " + Database.Instance.tableCustomer + @" WHERE PhoneNum = @PhoneNum";
            var command = new SqlCommand(sql, Database.Instance.Connection);
            command.Parameters.Add("@PhoneNum", System.Data.SqlDbType.Char).Value = phoneNumber;

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                CustomerDTO cus = new CustomerDTO()
                {
                    PhoneNum = (string)reader["PhoneNum"],
                    Address = (string)reader["Address"],
                    Name = (string)reader["Name"],
                    Email = (string)reader["Email"],
                    Gender = (string)reader["Gender"]
                };
                list.Add(cus);
            }

            reader.Close();
            result = list[0];

            return result;
        }

        public bool insertCustomer(CustomerDTO customer)
        {
            var sql = @"INSERT INTO CUSTOMER(PhoneNum, Address, Name, Email, Gender)"
                    + " VALUES(@PhoneNum, @Address, @Name, @Email, @Gender)";
            var command = new SqlCommand(sql, Database.Instance.Connection);

            command.Parameters.Add("@PhoneNum", System.Data.SqlDbType.Char).Value = customer.PhoneNum;
            command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar).Value = customer.Address;
            command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = customer.Name;
            command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar).Value = customer.Email;
            command.Parameters.Add("@Gender", System.Data.SqlDbType.NVarChar).Value = customer.Gender;

            int result = command.ExecuteNonQuery();

            return result > 0;
        }

        public bool deleteCustomerByPhone(string phonenumber)
        {
            try
            {
                var sql = @"DELETE FROM Customer WHERE PhoneNum = @PhoneNum";
                var command = new SqlCommand(sql, Database.Instance.Connection);

                command.Parameters.Add("@PhoneNum", System.Data.SqlDbType.Char).Value = phonenumber;

                int result = command.ExecuteNonQuery();
                return true;
            }
            catch (SqlException ex)
            {
                return false;
            }
        }
    }
}
