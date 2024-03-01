using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MyShop.DTO;

namespace MyShop.DAO
{
    public class CustomerOrderDAO
    {

        public BindingList<CustomerOrderDTO> GetAll()
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            string sql = @"select * from " + Database.Instance.tableCustomerOrder + @" Order by OrderID";

            SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);

            SqlDataReader _dataReader = command.ExecuteReader();

            BindingList<CustomerOrderDTO> _list = new BindingList<CustomerOrderDTO>();


            while (_dataReader.Read())
            {
                CustomerOrderDTO order = new CustomerOrderDTO()
                {
                    OrderId = (int)_dataReader["OrderID"],
                    PhoneNum = (string)_dataReader["PhoneNum"],
                    Status = (string)_dataReader["Status"],
                    CreateDate = (DateTime)_dataReader["CreateDate"],
                    ShipDate = (DateTime)_dataReader["ShipmentDate"],
                    TotalCost = (int)_dataReader["totalcost"]
                };
                order.StatusImgPath = "res\\icon\\" + order.Status + ".png";
                _list.Add(order);
            }
            _dataReader.Close();
            return _list;
        }

        public BindingList<CustomerOrderDTO> GetByID(int OrderID)
        {
            string sql = @"select * from " + Database.Instance.tableCustomerOrder + " where OrderId = " + OrderID + @" Order by OrderID";

            SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);

            SqlDataReader _dataReader = command.ExecuteReader();

            BindingList<CustomerOrderDTO> _list = new BindingList<CustomerOrderDTO>();


            while (_dataReader.Read())
            {
                CustomerOrderDTO order = new CustomerOrderDTO()
                {
                    OrderId = (int)_dataReader["OrderID"],
                    PhoneNum = (string)_dataReader["PhoneNum"],
                    Status = (string)_dataReader["Status"],
                    CreateDate = (DateTime)_dataReader["CreateDate"],
                    ShipDate = (DateTime)_dataReader["ShipmentDate"],
                    TotalCost = (int)_dataReader["totalcost"]
                };
                order.StatusImgPath = "res\\icon\\" + order.Status + ".png";
                _list.Add(order);
            }
            _dataReader.Close();

            return _list;
        }
        public int DeleteCustomerOrder(int orderID)
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            string sql = $"delete from " + Database.Instance.tableCustomerOrder + @" where OrderId = " + orderID;
            SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);
            int recordChanged = command.ExecuteNonQuery();
            return recordChanged;
        }
    }
}
