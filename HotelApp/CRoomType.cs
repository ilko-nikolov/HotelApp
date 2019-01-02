using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp
{
    public class CRoomType : INotifyPropertyChanged
    {
        public int RoomTypeID { get; set; }
        public string RoomTypeName { get; set; }
        public int SingleBeds { get; set; }
        public int DoubleBeds { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
