using MyShop.DAO;
using MyShop.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.ComponentModel;

namespace MyShop.BUS
{
    public class OrderDetailBUS
    {
        private OrderDetailDAO _odDAO;
        public OrderDetailBUS()
        {
            _odDAO = new OrderDetailDAO();
        }

        public BindingList<OrderDetailDTO> GetAll()
        {
            return _odDAO.GetAll();
        }

        public BindingList<OrderDetailDTO> GetByOrderID(int ID)
        {
            return _odDAO.GetByOrderID(ID);
        }
        public int DeleteOrder(int ID)
        {
            return _odDAO.DeleteOrder(ID);
        }
    }
}
