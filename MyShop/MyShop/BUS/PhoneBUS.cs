using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using MyShop.DAO;
using MyShop.DTO;

namespace MyShop.BUS
{
    public class PhoneBUS
    {
        private PhoneDAO _phoneDAO;
        public PhoneBUS()
        {
            _phoneDAO = new PhoneDAO();
        }

        public SqlDataAdapter getAll()
        {
            return _phoneDAO.getAll();
        }

        public bool insertPhone(PhoneDTO phone)
        {
            return _phoneDAO.insertPhone(phone);
        }

        public void deletePhone(int phoneID)
        {
            _phoneDAO.deletePhoneByID(phoneID);
        }

        public bool restorePhone()
        {
            return _phoneDAO.restorePhone();
        }
    }
}
