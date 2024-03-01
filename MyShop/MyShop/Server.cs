using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ExcelDataReader;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;

namespace MyShop
{
    public class Server
    {
        private static Server? _instance = null;

        public static Server Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Server();
                }
                return _instance;
            }
        }
        public string Name { get; set; }
    }
}