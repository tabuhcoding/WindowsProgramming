using MyShop.DAO;
using MyShop.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.BUS
{
    public class CustomerOrderBUS
    {
        private CustomerOrderDAO _odDAO;
        public CustomerOrderBUS()
        {
            _odDAO = new CustomerOrderDAO();
        }

        public BindingList<CustomerOrderDTO> GetAll()
        {
            return _odDAO.GetAll();
        }

        public BindingList<CustomerOrderDTO> getByOrderID(int orderID)
        {
            return _odDAO.GetByID(orderID);
        }
        public int deleteCustomerOrder(int orderID)
        {
            return _odDAO.DeleteCustomerOrder(orderID);
        }

    }
}
