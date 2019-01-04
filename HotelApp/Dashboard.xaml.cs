using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HotelApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Dashboard : Page
    {
        public Dashboard()
        {
            this.InitializeComponent();
            occupiedRooms = getOccupiedRooms();
            allRooms = getAllRooms();
            occupiedRoomsTextBlock.Text = "Currently occupied rooms: " + occupiedRooms.ToString();
            availableRoomsTextBlock.Text = "Available rooms: " + (allRooms - occupiedRooms).ToString();
            todayCheckIns.Text = "Pending check ins: " + getTodayPendingCheckIns().ToString();
            todayCheckOuts.Text = "Pending check outs: " + getTodayPendingCheckOuts().ToString();
            tomorrowCheckIns.Text = "Tomorrow check ins: " + getTomorrowyPendingCheckIns().ToString();
            tomorrowCheckOuts.Text = "Tomorrow check outs: " + getTomorrowPendingCheckOuts().ToString();
        }

        int occupiedRooms;
        int allRooms;

        public int getOccupiedRooms()
        {
            const string GetRoomsQuery = "SELECT count(DISTINCT (RoomID)) " +
                                          "from Reservations " +
                                          "where CheckInDate <= CAST(GETDATE() AS DATE) and CheckOutDate >= CAST(GETDATE() AS DATE) " +
                                          "and CheckIn = 1 and CheckOut = 0";
            int occupiedRooms = 0;
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
                                    occupiedRooms = reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }
                return occupiedRooms;
            }
            catch (Exception eSql)
            {
                var dialog = new MessageDialog("Exception: " + eSql.Message);
                dialog.ShowAsync();
            }
            return 0;
        }

        public int getAllRooms()
        {
            const string GetRoomsQuery = "select count(RoomID) from Rooms;";
            int rooms = 0;
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
                                    rooms = reader.GetInt32(0);
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
            return 0;
        }

        public int getTodayPendingCheckIns()
        {
            const string query = "select count(ReservationID) " +
                                         "from Reservations " +
                                         "where CheckInDate = CAST(GETDATE() AS DATE) " +
                                         "and CheckIn = 0; ";
            int pendingCheckIns = 0;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = query;
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    pendingCheckIns = reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }
                return pendingCheckIns;
            }
            catch (Exception eSql)
            {
                var dialog = new MessageDialog("Exception: " + eSql.Message);
                dialog.ShowAsync();
            }
            return 0;
        }

        public int getTodayPendingCheckOuts()
        {
            const string query = "select count(ReservationID) " +
                                         "from Reservations " +
                                         "where CheckOutDate = CAST(GETDATE() AS DATE) " +
                                         "and CheckOut = 0; ";
            int pendingCheckOuts = 0;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = query;
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    pendingCheckOuts = reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }
                return pendingCheckOuts;
            }
            catch (Exception eSql)
            {
                var dialog = new MessageDialog("Exception: " + eSql.Message);
                dialog.ShowAsync();
            }
            return 0;
        }

        public int getTomorrowyPendingCheckIns()
        {
            const string query = "select count(ReservationID) " +
                                         "from Reservations " +
                                         "where CheckInDate = CAST(GETDATE()+1 AS DATE) " +
                                         "and CheckIn = 0; ";
            int pendingCheckIns = 0;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = query;
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    pendingCheckIns = reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }
                return pendingCheckIns;
            }
            catch (Exception eSql)
            {
                var dialog = new MessageDialog("Exception: " + eSql.Message);
                dialog.ShowAsync();
            }
            return 0;
        }

        public int getTomorrowPendingCheckOuts()
        {
            const string query = "select count(ReservationID) " +
                                         "from Reservations " +
                                         "where CheckOutDate = CAST(GETDATE()+1 AS DATE) " +
                                         "and CheckOut = 0; ";
            int pendingCheckOuts = 0;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = query;
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    pendingCheckOuts = reader.GetInt32(0);
                                }
                            }
                        }
                    }
                }
                return pendingCheckOuts;
            }
            catch (Exception eSql)
            {
                var dialog = new MessageDialog("Exception: " + eSql.Message);
                dialog.ShowAsync();
            }
            return 0;
        }

    }
}
