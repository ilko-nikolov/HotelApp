using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelApp
{
    public class CReservation : INotifyPropertyChanged
    {
        public int ReservationID { get; set; }
        public CClient Client = new CClient();
        public CRoom Room = new CRoom();
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public double Price { get; set; }
        public double Paid { get; set; }
        public bool CheckIn { get; set; }
        public bool CheckOut { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
