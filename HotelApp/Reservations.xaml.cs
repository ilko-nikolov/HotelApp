using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class Reservations : Page
    {

        public Reservations()
        {
            this.InitializeComponent();
            //------ReservationsList.ItemsSource = GetReservations((App.Current as App).ConnectionString);

        }

        public ObservableCollection<CReservation> GetReservations(string connectionString)
        {
            const string GetProductsQuery = "select ReservationID, RoomID, Price from Reservations;";

            var reservations = new ObservableCollection<CReservation>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetProductsQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var reservation = new CReservation();
                                    reservation.ReservationID = reader.GetInt32(0);
                                    reservation.Room.RoomNumber = reader.GetString(1);
                                    reservation.Price = reader.GetInt32(5);
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
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }
    }
}
