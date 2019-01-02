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
            this.InitializeComponent();
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
    }
}
