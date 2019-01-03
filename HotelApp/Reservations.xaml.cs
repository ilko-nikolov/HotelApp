using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
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
    public sealed partial class Reservations : Page
    {

        public Reservations()
        {
            this.InitializeComponent();
            //------ReservationsList.ItemsSource = GetReservations((App.Current as App).ConnectionString);
            

        }

        private ObservableCollection<CRoom> roomsCollection;

        private DateTime checkInDateTimeTemp = DateTime.Today;
        private DateTime checkOutDateTimeTemp = DateTime.Today;

        public ObservableCollection<CReservation> GetReservations(string connectionString)
        {
            const string GetReservationsQuery = "select ReservationID, RoomID, Price from Reservations;";

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
                            cmd.CommandText = GetReservationsQuery;
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

        private async void NewReservationButton_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            checkInDate.MinDate = DateTime.Today;
            checkInDate.Date = DateTime.Today;
            checkOutDate.MinDate = DateTime.Today;
            checkOutDate.Date = DateTime.Today;
            roomsCollection = CRoom.GetRooms();
            roomsComboBox.ItemsSource = roomsCollection;
            ContentDialogResult result = await newReservationDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var checkInDateTime = checkInDate.Date.Value.Date;
                var checkOutDateTime = checkOutDate.Date.Value.DateTime;
                var selectedRoomID = roomsComboBox.SelectedValue;
                var firstName = firstNameTextBox.Text;
                if (firstName == "")
                {
                    var dialog = new MessageDialog("Client first name cannot be empty", "Error");
                    await dialog.ShowAsync();
                    return;
                }
                var lastName = familyNameTextBox.Text;
                if (lastName == "")
                {
                    var dialog = new MessageDialog("Client family name cannot be empty", "Error");
                    await dialog.ShowAsync();
                    return;
                }
                var phoneNumber = phoneNumberTextBox.Text;
                if (phoneNumber == "")
                {
                    var dialog = new MessageDialog("Client phone number cannot be empty", "Error");
                    await dialog.ShowAsync();
                    return;
                }
                else if (phoneNumber == "")
                {
                    var dialog = new MessageDialog("Client phone number is not valid", "Error");
                    await dialog.ShowAsync();
                    return;
                }
                var email = emailTextBox.Text;

                if (email != "")
                {
                    var emailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                             + "@"
                             + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";
                    if (!Regex.Match(email, emailPattern).Success || email == "")
                    {
                        var dialog = new MessageDialog("Client email address is not valid", "Error");
                        await dialog.ShowAsync();
                        return;
                    }
                }

                decimal price;
                if (!Decimal.TryParse(priceTextBox.Text, out price))
                {
                    var dialog = new MessageDialog("Invalid price format", "Error");
                    await dialog.ShowAsync();
                    return;
                }
                decimal paidAmount;
                if (!Decimal.TryParse(paidAmountTextBox.Text, out paidAmount))
                {
                    var dialog = new MessageDialog("Invalid paid amount format", "Error");
                    await dialog.ShowAsync();
                    return;
                }

                int clientID;

                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    string sqlQuery = "INSERT INTO Clients(ClientFirstName, ClientLastName, ClientPhoneNumber, ClientEmail) " +
                    "output inserted.ClientID VALUES(@ClientFirstName, @ClientLastName, @ClientPhoneNumber, @ClientEmail)";
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@ClientFirstName", firstName);
                        sqlCommand.Parameters.AddWithValue("@ClientLastName", lastName);
                        sqlCommand.Parameters.AddWithValue("@ClientPhoneNumber", phoneNumber);
                        sqlCommand.Parameters.AddWithValue("@ClientEmail", email);
                        sqlConnection.Open();

                        clientID = Convert.ToInt32(sqlCommand.ExecuteScalar());

                        if (sqlConnection.State == System.Data.ConnectionState.Open)
                        {
                            sqlConnection.Close();
                        }


                    }
                }

                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    string sqlQuery = "insert into Reservations(ClientID, RoomID, CheckInDate, CheckOutDate, Price, Paid, CheckIn, CheckOut) " + 
                        "values (@ClientID, @RoomID, @CheckInDate, @CheckOutDate, @Price, @Paid, @CheckIn, @CheckOut)";
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@ClientID", clientID);
                        sqlCommand.Parameters.AddWithValue("@RoomID", selectedRoomID);
                        sqlCommand.Parameters.AddWithValue("@CheckInDate", checkInDateTime);
                        sqlCommand.Parameters.AddWithValue("@CheckOutDate", checkOutDateTime);
                        sqlCommand.Parameters.AddWithValue("@Price", price);
                        sqlCommand.Parameters.AddWithValue("@Paid", paidAmount);
                        sqlCommand.Parameters.AddWithValue("@CheckIn", checkInCheckBox.IsChecked);
                        sqlCommand.Parameters.AddWithValue("@CheckOut", false);

                        sqlConnection.Open();

                        int sqlResult = sqlCommand.ExecuteNonQuery();

                        if (sqlResult < 0)
                        {
                            Console.WriteLine("Error");
                        }


                    }
                }

            }
            else
            {
                checkInDate.Date = DateTime.Today;
                checkOutDate.Date = DateTime.Today;
                roomsComboBox.SelectedIndex = -1;
                checkInCheckBox.IsChecked = false;
                firstNameTextBox.Text = string.Empty;
                familyNameTextBox.Text = string.Empty;
                phoneNumberTextBox.Text = string.Empty;
                emailTextBox.Text = string.Empty;
                priceTextBox.Text = string.Empty;
                priceTextBox.PlaceholderText = string.Empty;
                paidAmountTextBox.Text = string.Empty;
                infoTextBlock.Text = string.Empty;
            }
        }

        private void CheckInDate_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if(checkInDate.Date == null)
            {
                checkInDate.Date = checkInDateTimeTemp;
                checkInDate.IsCalendarOpen = false;
            }
            checkInDateTimeTemp = checkInDate.Date.Value.Date;
            if(checkInDate.Date > checkOutDate.Date)
            {
                checkOutDate.Date = checkInDate.Date;
            }
            DateTime checkInDateTime = checkInDate.Date.Value.DateTime;
            checkOutDate.MinDate = checkInDateTime;
            if(checkInDate.Date == DateTime.Today)
            {
                checkInCheckBox.IsEnabled = true;
            }
            else
            {
                checkInCheckBox.IsChecked = false;
                checkInCheckBox.IsEnabled = false;
            }
        }

        private void CheckOutDate_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (checkOutDate.Date == null)
            {
                checkOutDate.Date = checkOutDateTimeTemp;
                checkOutDate.IsCalendarOpen = false;
            }
            checkOutDateTimeTemp = checkOutDate.Date.Value.Date;

        }

        private void RoomsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(roomsComboBox.SelectedIndex < 0)
            {
                return;
            }
            var room = roomsCollection[roomsComboBox.SelectedIndex];
            var nights = (checkOutDate.Date - checkInDate.Date).Value.Days;
            var calculatedPrice = room.RoomDefaultPrice * nights;
            if(nights == 0)
            {
                calculatedPrice = room.RoomDefaultPrice;
            }
            infoTextBlock.Text = "Room type: " + room.RoomType.RoomTypeName +
                "\nSingle beds: " + room.RoomType.SingleBeds +
                "\nDouble beds: " + room.RoomType.DoubleBeds +
                "\nNights: " + nights + 
                "\nPrice per night: " + room.RoomDefaultPrice +
                "\nCalculated Total: " + room.RoomDefaultPrice * nights;
            priceTextBox.PlaceholderText = calculatedPrice.ToString("#.##");
        }


    }
}
