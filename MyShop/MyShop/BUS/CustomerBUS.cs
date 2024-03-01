using MyShop.DAO;
using MyShop.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.BUS
{
    public class CustomerBUS
    {
        private CustomerDAO customerDAO;
        public CustomerBUS()
        {
            customerDAO = new CustomerDAO();
        }

        public CustomerDTO getCusByPhone(string phone) => customerDAO.getCustomerByPhone(phone);

        public bool insertCustomer(CustomerDTO customerDTO)
        {
            return customerDAO.insertCustomer(customerDTO);
        }

        public bool deleteCustomer(string phonenumber)
        {
            return customerDAO.deleteCustomerByPhone(phonenumber);
        }
    }
}
