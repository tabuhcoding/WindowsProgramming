using MyShop.DAO;
using MyShop.DTO;

namespace MyShop.BUS
{
    public class AdminBUS
    {
        private AdminDAO _adminDAO;

        public AdminBUS()
        {
            _adminDAO = new AdminDAO();
        }

        public AdminDTO? GetAdmin(string username, string password)
        {
            return _adminDAO.GetAdmin(username, password);
        }

        public bool IsExistsID(string id)
        {
            return _adminDAO.IsExistsID(id);
        }

        public bool CreateAdmin(AdminDTO admin)
        {
            return _adminDAO.CreateAdmin(admin);
        }
    }
}
