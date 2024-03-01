using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Reflection;
using MyShop.DTO;
using System.Data;
using System.ComponentModel;

namespace MyShop.DAO
{
    public class OrderDetailDAO
    {
        public BindingList<OrderDetailDTO> GetAll()
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }

            string sql = @"select * from " + Database.Instance.tableOrder;

            SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);

            SqlDataReader _dataReader = command.ExecuteReader();

            BindingList<OrderDetailDTO> _list = new BindingList<OrderDetailDTO>();

            while (_dataReader.Read())
            {
                OrderDetailDTO phone = new OrderDetailDTO()
                {
                    phone = (string)_dataReader["PhoneName"],
                    image = (string)_dataReader["image"],
                    total = (int)_dataReader["total"],
                    amount = (int)_dataReader["amount"]
                };
                _list.Add(phone);
            }
            _dataReader.Close();

            return _list;
        }

        public BindingList<OrderDetailDTO> GetByOrderID(int orderID)
        {
            if (Database.Instance.Connection.State == System.Data.ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            string sql = $"select * from " + Database.Instance.tableOrder + @" where OrderId = " + orderID + @" Order by phonename";

            SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);

            SqlDataReader _dataReader = command.ExecuteReader();

            BindingList<OrderDetailDTO> _list = new BindingList<OrderDetailDTO>();

            while (_dataReader.Read())
            {
                OrderDetailDTO phone = new OrderDetailDTO()
                {
                    phone = (string)_dataReader["PhoneName"],
                    image = (string)_dataReader["image"],
                    total = (int)_dataReader["total"],
                    amount = (int)_dataReader["amount"]
                };
                _list.Add(phone);
            }
            _dataReader.Close();

            return _list;
        }

        public int DeleteOrder(int orderID)
        {
            if (Database.Instance.Connection.State == ConnectionState.Closed)
            {
                Database.Instance.Connection.Open();
            }
            string sql = $"delete from " + Database.Instance.tableOrder + @" where OrderId = " + orderID;
            SqlCommand command = new SqlCommand(sql, Database.Instance.Connection);
            int recordChanged = command.ExecuteNonQuery();
            return recordChanged;
        }
    }
}
