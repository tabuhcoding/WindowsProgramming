using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DTO
{
    public class CustomerOrderDTO : INotifyPropertyChanged
    {
        public int OrderId { get; set; }
        public string PhoneNum { get; set; }
        public int Amount { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ShipDate { get; set; }
        public string Status { get; set; }
        public string StatusImgPath { get; set; }
        public long TotalCost { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
