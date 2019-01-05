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
            initializeFrame();

        }

        private void initializeFrame()
        {
            reservations = CReservation.GetAllReservations();
            reservationsListView.ItemsSource = reservations;
            filterToggle.IsOn = false;
            filterDatePicker.Date = DateTime.Today;
            filterDatePicker.IsEnabled = false;
            filterCheckIn.IsEnabled = false;
            filterCheckOut.IsEnabled = false;
            openReservationButton.IsEnabled = false;
        }

        private ObservableCollection<CRoom> roomsCollection;
        private ObservableCollection<CReservation> reservations;

        private DateTime checkInDateTimeTemp = DateTime.Today;
        private DateTime checkOutDateTimeTemp = DateTime.Today;

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
                var checkInDateTime = checkInDate.Date.Value.DateTime;
                var checkOutDateTime = checkOutDate.Date.Value.DateTime;
                var selectedRoomID = roomsComboBox.SelectedValue;
                var firstName = firstNameTextBox.Text;
				if(roomsComboBox.SelectedIndex < 0)
				{
					var dialog = new MessageDialog("Please select a room, before confirming reservation", "Error");
					await dialog.ShowAsync();
					return;
				}
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
                initializeFrame();
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
            RoomsComboBox_SelectionChanged(null, null);
        }

        private void CheckOutDate_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (checkOutDate.Date == null)
            {
                checkOutDate.Date = checkOutDateTimeTemp;
                checkOutDate.IsCalendarOpen = false;
            }
            checkOutDateTimeTemp = checkOutDate.Date.Value.Date;
            RoomsComboBox_SelectionChanged(null, null);
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

        private async void OpenReservationButton_TappedAsync(object sender, TappedRoutedEventArgs e)
        {
            CReservation selectedReservation = reservations[reservationsListView.SelectedIndex];
            editCheckInDate.Date = selectedReservation.StartDate;
            editCheckOutDate.Date = selectedReservation.EndDate;
            editCheckIn.IsChecked = selectedReservation.CheckIn;
            editCheckOut.IsChecked = selectedReservation.CheckOut;
            editPrice.Text = selectedReservation.Price.ToString("#.##");
            editPaidAmount.Text = selectedReservation.Paid.ToString("#.##");
            editInfo.Text = selectedReservation.Client.FirstName + "\n" + selectedReservation.Client.LastName + "\n" +
                "Phone number:\n" + selectedReservation.Client.PhoneNumber + "\nEmail:\n" + selectedReservation.Client.Email + "\n" +
                "Room: " + selectedReservation.Room.RoomNumber + "\n" +
                "Room price: " + selectedReservation.Room.RoomDefaultPrice.ToString("#.##");
            if(selectedReservation.CheckIn)
            {
                editCheckInDate.IsEnabled = false;
                editCheckIn.IsEnabled = false;
            } else
            {
                editCheckInDate.IsEnabled = true;
                editCheckIn.IsEnabled = true;
            }
            if(editCheckInDate.Date.Value.Date == DateTime.Today)
            {
                editCheckOut.IsEnabled = true;
            }
            else
            {
                editCheckOut.IsEnabled = false;
            }
            ContentDialogResult result = await selectedReservationDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                {
                    string sqlQuery = "update Reservations " +
                                      "set CheckInDate = @CheckInDate, CheckOutDate = @CheckOutDate, " +
                                      "Price = @Price, Paid = @Paid, CheckIn = @CheckIn, CheckOut = @CheckOut " +
                                      "where ReservationID = @ReservationID";
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@CheckInDate", editCheckInDate.Date.Value.DateTime);
                        sqlCommand.Parameters.AddWithValue("@CheckOutDate", editCheckOutDate.Date.Value.DateTime);
                        sqlCommand.Parameters.AddWithValue("@Price", editPrice.Text);
                        sqlCommand.Parameters.AddWithValue("@Paid", editPaidAmount.Text);
                        sqlCommand.Parameters.AddWithValue("@CheckIn", editCheckIn.IsChecked);
                        sqlCommand.Parameters.AddWithValue("@CheckOut", editCheckOut.IsChecked);
                        sqlCommand.Parameters.AddWithValue("@ReservationID", reservationsListView.SelectedValue);


                        sqlConnection.Open();

                        int sqlResult = sqlCommand.ExecuteNonQuery();

                        if (sqlResult < 0)
                        {
                            Console.WriteLine("Error");
                        }
                    }
                }
                initializeFrame();
            }
        }

        private void EditCheckInDate_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (editCheckInDate.Date == null)
            {
                editCheckInDate.Date = checkInDateTimeTemp;
                editCheckInDate.IsCalendarOpen = false;
            }
            checkInDateTimeTemp = editCheckInDate.Date.Value.Date;
            if(editCheckInDate.Date > editCheckOutDate.Date)
            {
                editCheckOutDate.Date = editCheckInDate.Date;
            }
            DateTime editCheckInDateTime = editCheckInDate.Date.Value.DateTime;
            editCheckOutDate.MinDate = editCheckInDateTime;
            if(editCheckIn.IsChecked == false && editCheckInDate.Date >= DateTime.Today)
            {
                editCheckIn.IsEnabled = true;
            }
            else
            {
                editCheckIn.IsEnabled = false;
            }
        }

        private void EditCheckOutDate_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if (editCheckOutDate.Date == null)
            {
                editCheckOutDate.Date = checkOutDateTimeTemp;
                editCheckOutDate.IsCalendarOpen = false;
            }
            checkOutDateTimeTemp = editCheckOutDate.Date.Value.Date;

            if(editCheckOutDate.Date <= DateTime.Today.Date)
            {
                editCheckOut.IsEnabled = true;
            }
            else
            {
                editCheckOut.IsChecked = false;
                editCheckOut.IsEnabled = false;
            }
        }

        private void FilterToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (filterToggle.IsOn)
            {
                filterCheckIn.IsEnabled = true;
                filterCheckOut.IsEnabled = true;
                filterDatePicker.IsEnabled = true;
                filterReservations();
            }
            else
            {
                filterCheckIn.IsEnabled = false;
                filterCheckOut.IsEnabled = false;
                filterDatePicker.IsEnabled = false;
                reservations = CReservation.GetAllReservations();
                reservationsListView.ItemsSource = reservations;

            }
        }

        private void filterReservations()
        {
            reservations = CReservation.GetFilteredReservations(filterCheckIn.IsChecked, filterCheckOut.IsChecked, filterDatePicker.Date.DateTime);
            reservationsListView.ItemsSource = reservations;
        }

        private void FilterCheckIn_Click(object sender, RoutedEventArgs e)
        {
            if(filterCheckIn.IsChecked.Value)
            {
                filterCheckOut.IsEnabled = true;
            }
            else
            {
                filterCheckOut.IsChecked = false;
                filterCheckOut.IsEnabled = false;
            }
            filterReservations();
        }

        private void FilterCheckOut_Click(object sender, RoutedEventArgs e)
        {
            filterReservations();
            if(filterCheckOut.IsChecked.Value)
            {
                filterCheckIn.IsEnabled = false;
            }
            else
            {
                filterCheckIn.IsEnabled = true;
            }
        }

        private void FilterDatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            filterReservations();
        }

        private void ReservationsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(reservationsListView.SelectedIndex < 0)
            {
                openReservationButton.IsEnabled = false;
            }
            else
            {
                openReservationButton.IsEnabled = true;
            }
        }
    }
}
