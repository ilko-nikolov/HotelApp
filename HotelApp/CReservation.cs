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
    public class CReservation : INotifyPropertyChanged
    {
        public int ReservationID { get; set; }
        public CClient Client = new CClient();
        public CRoom Room = new CRoom();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Price { get; set; }
        public decimal Paid { get; set; }
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

        public string getStatus()
        {
            string status = "status: ";
            if(CheckOut)
            {
                status += "checked out";
            }
            else if(CheckIn)
            {
                status += "checked in";
            }
            else
            {
                status += "not checked in";
            }

            return status;
        }

        public string getClientNames()
        {
            return Client.FirstName + " " + Client.LastName;
        }

        public string getReservationDates()
        {
            return "from " + StartDate.ToString("dd.MM.yyyy") + " to " + EndDate.ToString("dd.MM.yyyy");
        }

        public static ObservableCollection<CReservation> GetAllReservations()
        {
            const string GetReservationsQuery = "SELECT RE.ReservationID, RE.ClientID, RE.RoomID, RE.CheckInDate, RE.CheckOutDate, RE.Price, RE.Paid, RE.CheckIn, RE.CheckOut, " +
                "CL.ClientFirstName, CL.ClientLastName, CL.ClientPhoneNumber, ClientEmail, " + 
                "RO.RoomNumber, RO.RoomDefaultPrice, RO.RoomTypeID, " + 
                "RT.RoomTypeName, RT.SingleBeds, RT.DoubleBeds, RT.Capacity, RT.Description " +
                "FROM Reservations AS RE " +
                "LEFT JOIN Clients AS CL ON CL.ClientID = RE.ClientID " +
                "LEFT JOIN Rooms AS RO ON RO.RoomID = RE.RoomID " +
                "LEFT JOIN RoomType AS RT ON RT.RoomTypeID = RO.RoomTypeID; ";

            var reservations = new ObservableCollection<CReservation>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = GetReservationsQuery;
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var reservation = new CReservation();
                                    reservation.ReservationID = reader.GetInt32(0);
                                    reservation.Client.ClientID = reader.GetInt32(1);
                                    reservation.Room.RoomID = reader.GetInt32(2);
                                    reservation.StartDate = reader.GetDateTime(3);
                                    reservation.EndDate = reader.GetDateTime(4);
                                    reservation.Price = reader.GetDecimal(5);
                                    reservation.Paid = reader.GetDecimal(6);
                                    reservation.CheckIn = reader.GetBoolean(7);
                                    reservation.CheckOut = reader.GetBoolean(8);
                                    reservation.Client.FirstName = reader.GetString(9);
                                    reservation.Client.LastName = reader.GetString(10);
                                    reservation.Client.PhoneNumber = reader.GetString(11);
                                    reservation.Client.Email = reader.GetString(12);
                                    reservation.Room.RoomNumber = reader.GetString(13);
                                    reservation.Room.RoomDefaultPrice = reader.GetDecimal(14);
                                    reservation.Room.RoomType.RoomTypeID = reader.GetInt32(15);
                                    reservation.Room.RoomType.RoomTypeName = reader.GetString(16);
                                    reservation.Room.RoomType.SingleBeds = reader.GetInt32(17);
                                    reservation.Room.RoomType.DoubleBeds = reader.GetInt32(18);
                                    reservation.Room.RoomType.Capacity = reader.GetInt32(19);
                                    reservation.Room.RoomType.Description = reader.GetString(20);
                                    reservations.Add(reservation);
                                }
                            }
                        }
                    }
                }
                return reservations;
            }
            catch (Exception eSql)
            {
                var dialog = new MessageDialog("Exception: " + eSql.Message);
                dialog.ShowAsync();
            }
            return null;
        }

        public static ObservableCollection<CReservation> GetFilteredReservations(bool? checkIn, bool? checkOut, DateTime dateTime)
        {
            const string GetReservationsQuery = "SELECT RE.ReservationID, RE.ClientID, RE.RoomID, RE.CheckInDate, RE.CheckOutDate, RE.Price, RE.Paid, RE.CheckIn, RE.CheckOut, " +
                                                "CL.ClientFirstName, CL.ClientLastName, CL.ClientPhoneNumber, ClientEmail, " +
                                                "RO.RoomNumber, RO.RoomDefaultPrice, RO.RoomTypeID, " +
                                                "RT.RoomTypeName, RT.SingleBeds, RT.DoubleBeds, RT.Capacity, RT.Description " +
                                                "FROM Reservations AS RE " +
                                                "LEFT JOIN Clients AS CL ON CL.ClientID = RE.ClientID " +
                                                "LEFT JOIN Rooms AS RO ON RO.RoomID = RE.RoomID " +
                                                "LEFT JOIN RoomType AS RT ON RT.RoomTypeID = RO.RoomTypeID " +
                                                "where ((MONTH(RE.CheckInDate) = @month and YEAR(RE.CheckInDate) = @year) or (MONTH(RE.CheckOutDate) = @month and YEAR(RE.CheckOutDate) = @year))" +
                                                "and CheckIn = @checkIn and CheckOut = @checkOut;";

            var reservations = new ObservableCollection<CReservation>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = GetReservationsQuery;
                            sqlCommand.Parameters.AddWithValue("@month", dateTime.Month);
                            sqlCommand.Parameters.AddWithValue("@year", dateTime.Year);
                            sqlCommand.Parameters.AddWithValue("@checkIn", checkIn);
                            sqlCommand.Parameters.AddWithValue("@checkOut", checkOut);
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var reservation = new CReservation();
                                    reservation.ReservationID = reader.GetInt32(0);
                                    reservation.Client.ClientID = reader.GetInt32(1);
                                    reservation.Room.RoomID = reader.GetInt32(2);
                                    reservation.StartDate = reader.GetDateTime(3);
                                    reservation.EndDate = reader.GetDateTime(4);
                                    reservation.Price = reader.GetDecimal(5);
                                    reservation.Paid = reader.GetDecimal(6);
                                    reservation.CheckIn = reader.GetBoolean(7);
                                    reservation.CheckOut = reader.GetBoolean(8);
                                    reservation.Client.FirstName = reader.GetString(9);
                                    reservation.Client.LastName = reader.GetString(10);
                                    reservation.Client.PhoneNumber = reader.GetString(11);
                                    reservation.Client.Email = reader.GetString(12);
                                    reservation.Room.RoomNumber = reader.GetString(13);
                                    reservation.Room.RoomDefaultPrice = reader.GetDecimal(14);
                                    reservation.Room.RoomType.RoomTypeID = reader.GetInt32(15);
                                    reservation.Room.RoomType.RoomTypeName = reader.GetString(16);
                                    reservation.Room.RoomType.SingleBeds = reader.GetInt32(17);
                                    reservation.Room.RoomType.DoubleBeds = reader.GetInt32(18);
                                    reservation.Room.RoomType.Capacity = reader.GetInt32(19);
                                    reservation.Room.RoomType.Description = reader.GetString(20);
                                    reservations.Add(reservation);
                                }
                            }
                        }
                    }
                }
                return reservations;
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
