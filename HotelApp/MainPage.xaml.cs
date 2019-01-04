using System;
using System.Collections.Generic;
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

using System.Data.SqlClient;
using Windows.ApplicationModel.Core;
using System.Collections.ObjectModel;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HotelApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            //this.InitializeComponent();
            LogInAsync();
        }

        private void Dashboard_Tapped(object sender, TappedRoutedEventArgs e)
        {
            navigationView.Header = "Dashboard";
            MainFrame.Navigate(typeof(Dashboard));
        }

        private void Reservations_Tapped(object sender, TappedRoutedEventArgs e)
        {
            navigationView.Header = "Reservations";
            MainFrame.Navigate(typeof(Reservations));
        }

        private void Rooms_Tapped(object sender, TappedRoutedEventArgs e)
        {
            navigationView.Header = "Rooms";
            MainFrame.Navigate(typeof(Rooms));
        }

        private async void LogInAsync()
        {
            StackPanel content = new StackPanel();
            TextBox username = new TextBox();
            username.Header = "Username";
            PasswordBox password = new PasswordBox();
            password.Header = "Password:";

            content.Children.Add(username);
            content.Children.Add(password);

            ContentDialog dialog = new ContentDialog();

            dialog.Content = content;

            dialog.Title = "Login";
            dialog.IsSecondaryButtonEnabled = true;
            dialog.PrimaryButtonText = "Enter";
            dialog.SecondaryButtonText = "Exit";
            if(await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                const string query = "select EmployeeID " +
                                     "from Employees " +
                                     "where EmployeeUserName = @username and EmployeeUserPassword = @password; ";
                int employee = 0;

                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(App.ConnectionString))
                    {
                        sqlConnection.Open();
                        if (sqlConnection.State == System.Data.ConnectionState.Open)
                        {
                            using (SqlCommand sqlCommand = sqlConnection.CreateCommand())
                            {
                                sqlCommand.CommandText = query; ;
                                sqlCommand.Parameters.AddWithValue("@username", username.Text);
                                sqlCommand.Parameters.AddWithValue("@password", username.Text);
                                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        employee = reader.GetInt32(0);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception eSql)
                {
                    var exDialog = new MessageDialog("Exception: " + eSql.Message);
                    await exDialog.ShowAsync();
                }
                if(employee > 0)
                {
                    InitializeComponent();
                    MainFrame.Navigate(typeof(Dashboard));
                }
                else
                {
                    var message = new MessageDialog("Wrong username or password");
                    await message.ShowAsync();
                    LogInAsync();
                }
            }
            else
            {
                CoreApplication.Exit();
            }
            
        }
    }
}
