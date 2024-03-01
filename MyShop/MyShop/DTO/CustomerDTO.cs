using System.ComponentModel;

namespace MyShop.DTO
{
    public class CustomerDTO : INotifyPropertyChanged
    {
        public string PhoneNum { get; set; }
        public string Address { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
