using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp
{
    public class CRoom : INotifyPropertyChanged
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; }
        public decimal RoomDefaultPrice { get; set; }
        public CRoomType RoomType = new CRoomType();

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public string PriceToString()
        {
            return RoomDefaultPrice.ToString("0.00");
        }
    }
}
