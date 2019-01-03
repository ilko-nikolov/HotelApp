using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

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

        public static ObservableCollection<CRoom> GetRooms()
        {
            const string GetRoomsQuery = "select RoomID, RoomNumber, RoomDefaultPrice, RoomTypeName, SingleBeds, DoubleBeds, Capacity, Description"
            + " from Rooms join RoomType on Rooms.RoomTypeID = RoomType.RoomTypeID";

            var rooms = new ObservableCollection<CRoom>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = GetRoomsQuery;
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var room = new CRoom();
                                    room.RoomID = reader.GetInt32(0);
                                    room.RoomNumber = reader.GetString(1);
                                    room.RoomDefaultPrice = reader.GetDecimal(2);
                                    room.RoomType.RoomTypeName = reader.GetString(3);
                                    room.RoomType.SingleBeds = reader.GetInt32(4);
                                    room.RoomType.DoubleBeds = reader.GetInt32(5);
                                    room.RoomType.Capacity = reader.GetInt32(6);
                                    room.RoomType.Description = reader.GetString(7);
                                    rooms.Add(room);
                                }
                            }
                        }
                    }
                }
                return rooms;
            }
            catch (Exception eSql)
            {
                var dialog = new MessageDialog("Exception: " + eSql.Message);
                dialog.ShowAsync();
            }
            return null;
        }
    }
}
