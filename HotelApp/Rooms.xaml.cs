using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
using Microsoft.Toolkit.Uwp.UI.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HotelApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Rooms : Page
    {
        public Rooms()
        {
            this.InitializeComponent();
            roomsCollection = CRoom.GetRooms();
            RoomsListView.ItemsSource = roomsCollection;
            removeRoomButton.IsEnabled = false;
        }

        string connectionString = @"Data Source=DESKTOP-B3VTSNV\SQLEXPRESS;Initial Catalog=HoteAppDB;Integrated Security=SSPI";

        public ObservableCollection<CRoom> roomsCollection = new ObservableCollection<CRoom>();
        public ObservableCollection<CRoomType> roomTypes = new ObservableCollection<CRoomType>();

        string tempRoomNumber = "";
        string tempRoomDefaultPrice = "";


        private async void AddRoomButton_Tapped(object sender, RoutedEventArgs e) //from frame
        {
            roomNumberTextBox.Text = tempRoomNumber;
            roomDefaultPriceTextBox.Text = tempRoomDefaultPrice;
            roomTypes = GetRoomTypes();
            roomTypeListBox.ItemsSource = roomTypes;
            roomTypeListBox.SelectedValuePath = "RoomTypeID";
            roomTypeListBox.DisplayMemberPath = "RoomTypeName";

            ContentDialogResult result = await addRoomContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                bool roomExist = roomsCollection.Any(p => p.RoomNumber == roomNumberTextBox.Text);
                if (roomExist)
                {
                    //room already exist
                    var dialog = new MessageDialog("Room with number " + roomNumberTextBox.Text + " already exist", "Error");
                    await dialog.ShowAsync();
                    return;
                }
                // New Room added
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "INSERT INTO Rooms(RoomNumber, RoomDefaultPrice, RoomTypeID) " +
                    "VALUES(@RoomNumber, @RoomDefaultPrice, @RoomTypeID)";
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@RoomNumber", roomNumberTextBox.Text);
                        sqlCommand.Parameters.AddWithValue("@RoomDefaultPrice", roomDefaultPriceTextBox.Text);
                        sqlCommand.Parameters.AddWithValue("@RoomTypeID", roomTypeListBox.SelectedValue);
                        sqlConnection.Open();

                        int sqlResult = sqlCommand.ExecuteNonQuery();

                        if (sqlResult < 0)
                        {
                            Console.WriteLine("Error");
                        }

                        roomsCollection = CRoom.GetRooms();
                        RoomsListView.ItemsSource = roomsCollection;
                    }
                }
            }
            else
            {
                // User pressed Cancel, ESC, or the back arrow.
                // New Room not added
            }
        }

        private async void AddRoomTypeButton_ClickAsync(object sender, RoutedEventArgs e) //content dialog
        {
            tempRoomNumber = roomNumberTextBox.Text;
            tempRoomDefaultPrice = roomDefaultPriceTextBox.Text;
            addRoomContentDialog.Hide();
            ContentDialogResult result = await addRoomTypeContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // New Room Type added
                string roomTypeName = roomTypeTextBox.Text;
                int singleBeds = Convert.ToInt32(singleBedsTextBox.Text);
                int doubleBeds = Convert.ToInt32(doubleBedsTextBox.Text);
                int capacity = Convert.ToInt32(capacityTextBox.Text);
                string description = descriptionTextBox.Text;

                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "INSERT INTO RoomType(RoomTypeName, SingleBeds, DoubleBeds, Capacity, Description) " +
                    "VALUES(@RoomTypeName, @SingleBeds, @DoubleBeds, @Capacity, @Description)";
                    using (SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@RoomTypeName", roomTypeName);
                        sqlCommand.Parameters.AddWithValue("@SingleBeds", singleBeds);
                        sqlCommand.Parameters.AddWithValue("@DoubleBeds", doubleBeds);
                        sqlCommand.Parameters.AddWithValue("@Capacity", capacity);
                        sqlCommand.Parameters.AddWithValue("@Description", description);

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
                // User pressed Cancel, ESC, or the back arrow.
                // New Room type not added

            }
            AddRoomButton_Tapped(null, null);
        }

        public ObservableCollection<CRoomType> GetRoomTypes()
        {
            const string GetRoomTypesQuery = "select RoomTypeID, RoomTypeName, SingleBeds, DoubleBeds, Capacity, Description from RoomType";

            var roomTypes = new ObservableCollection<CRoomType>();
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = GetRoomTypesQuery;
                            using (SqlDataReader reader = sqlCommand.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var roomType = new CRoomType();
                                    roomType.RoomTypeID = reader.GetInt32(0);
                                    roomType.RoomTypeName = reader.GetString(1);
                                    roomType.SingleBeds = reader.GetInt32(2);
                                    roomType.DoubleBeds = reader.GetInt32(3);
                                    roomType.Capacity = reader.GetInt32(4);
                                    roomType.Description = reader.GetString(5);
                                    roomTypes.Add(roomType);
                                }
                            }
                        }
                    }
                }
                return roomTypes;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }


        private void RoomsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(RoomsListView.SelectedIndex < 0)
            {
                removeRoomButton.IsEnabled = false;
            }
            else
            {
                removeRoomButton.IsEnabled = true;
            }
        }

        private void RemoveRoomButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string selectedRoomID = RoomsListView.SelectedValue.ToString();
            string deleteRoomQuery = "DELETE FROM Rooms WHERE RoomID = " + selectedRoomID;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = deleteRoomQuery;
                            SqlDataReader reader = sqlCommand.ExecuteReader();
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return;
            }
            roomsCollection = CRoom.GetRooms();
            RoomsListView.ItemsSource = roomsCollection;
            removeRoomButtonFlyout.Hide();
        }

        private void TextBox_DigitsOnly(TextBox sender,
                                          TextBoxBeforeTextChangingEventArgs args)
        {
            args.Cancel = args.NewText.Any(c => !char.IsDigit(c));
        }

        private void RoomTypeListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(roomTypeListBox.SelectedIndex < 0)
            {
                removeRoomTypeButton.IsEnabled = false;
                return;
            }
            else
            {
                removeRoomTypeButton.IsEnabled = true;
            }
            var selectedRoomType = roomTypes[roomTypeListBox.SelectedIndex];
            roomTypeInfoTextBlock.Text = "Capacity: " + selectedRoomType.Capacity + "\nSingle beds: " + selectedRoomType.SingleBeds + "\nDouble beds: " + selectedRoomType.DoubleBeds;
        }

        private void removeRoomTypeButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            string selectedRoomTypeID = roomTypeListBox.SelectedValue.ToString();
            
            string deleteRoomQuery = "DELETE FROM RoomType WHERE RoomTypeID = " + selectedRoomTypeID;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    sqlConnection.Open();
                    if (sqlConnection.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                        {
                            sqlCommand.CommandText = deleteRoomQuery;
                            SqlDataReader reader = sqlCommand.ExecuteReader();
                        }
                    }
                }
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
                return;
            }
            roomTypes = GetRoomTypes();
            roomTypeListBox.ItemsSource = roomTypes;
            removeRoomTypeButtonFlyout.Hide();
            removeRoomTypeButton.IsEnabled = false;
        }
    }
}
