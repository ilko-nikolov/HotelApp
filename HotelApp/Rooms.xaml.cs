using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
    public sealed partial class Rooms : Page
    {
        public Rooms()
        {
            this.InitializeComponent();
        }

        string connectionString = @"Data Source=DESKTOP-B3VTSNV\SQLEXPRESS;Initial Catalog=HoteAppDB;Integrated Security=SSPI";

        string tempRoomNumber = "";
        string tempRoomDefaultPrice = "";
        

        private async void AddRoomButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            roomNumberTextBox.Text = tempRoomNumber;
            roomDefaultPriceTextBox.Text = tempRoomDefaultPrice;
            ContentDialogResult result = await addRoomContentDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // New Room added
            }
            else
            {
                // User pressed Cancel, ESC, or the back arrow.
                // New Room not added
            }
        }

        private async void AddRoomTypeButton_ClickAsync(object sender, RoutedEventArgs e)
        {
            tempRoomNumber = roomNumberTextBox.Text;
            tempRoomDefaultPrice = roomDefaultPriceTextBox.Text;
            addRoomContentDialog.Hide();
            ContentDialogResult result = await addRoomTypeContentDialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                // New Room Type added
                string roomTypeName = roomTypeTextBox.Text;
                int singleBeds = Convert.ToInt32(singleBedsTextBox.Text);
                int doubleBeds = Convert.ToInt32(doubleBedsTextBox.Text);
                int capacity = Convert.ToInt32(capacityTextBox.Text);
                string description = descriptionTextBox.Text;

                using(SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string sqlQuery = "INSERT INTO RoomType(RoomTypeName, SingleBeds, DoubleBeds, Capacity, Description) " +
                         "VALUES(@RoomTypeName, @SingleBeds, @DoubleBeds, @Capacity, @Description)";
                    using(SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@RoomTypeName", roomTypeName);
                        sqlCommand.Parameters.AddWithValue("@SingleBeds", singleBeds);
                        sqlCommand.Parameters.AddWithValue("@DoubleBeds", doubleBeds);
                        sqlCommand.Parameters.AddWithValue("@Capacity", capacity);
                        sqlCommand.Parameters.AddWithValue("@Description", description);

                        sqlConnection.Open();

                        int sqlResult = sqlCommand.ExecuteNonQuery();
                        
                        if(sqlResult < 0)
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
            AddRoomButton_ClickAsync(null, null);
        }
    }
}
