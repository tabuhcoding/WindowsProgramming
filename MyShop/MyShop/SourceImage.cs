using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop
{
    class SourceImage : INotifyPropertyChanged
    {
        public string closeImage { get; set; }
        public string personImage { get; set; }
        public string lockImage { get; set; }
        public string emailImage { get; set; }
        public string eyeImage { get; set; }
        public string dbImage { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private string _source;
        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Source"));
            }
        }
    }
}
