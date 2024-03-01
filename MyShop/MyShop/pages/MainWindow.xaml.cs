using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace MyShop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string lastClosedPage = ConfigurationManager.AppSettings["LastClosedPage"];

            if(!string.IsNullOrEmpty(lastClosedPage))
            {
                SwitchToPage(lastClosedPage);
                return;
            }

            fContainer.Navigate(new System.Uri("pages/Home.xaml", UriKind.RelativeOrAbsolute));
        }

        private void SwitchToPage(string pagePath)
        {
            fContainer.Navigate(new System.Uri(pagePath, UriKind.Relative));
        }


        private void btnStatistic_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("pages/Statistic.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnProduct_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("pages/Product.xaml", UriKind.Relative));
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("pages/Home.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("pages/Setting.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnNotification_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnOrder_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("pages/Order.xaml", UriKind.RelativeOrAbsolute));
        }


        private void btnHome_MouseEnter(object sender, MouseEventArgs e)
        {
            if (btnToggle.IsChecked != true)
            {
                Popup.PlacementTarget = btnHome;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Home";
            }
        }

        private void btnHome_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnProduct_MouseEnter(object sender, MouseEventArgs e)
        {
            if (btnToggle.IsChecked != true)
            {
                Popup.PlacementTarget = btnProduct;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Product";
            }
        }



        private void btnProduct_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnStatistic_MouseEnter(object sender, MouseEventArgs e)
        {
            if (btnToggle.IsChecked != true)
            {
                Popup.PlacementTarget = btnStatistic;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Revenue Statistic";
            }

        }

        private void btnStatistic_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnOrder_MouseEnter(object sender, MouseEventArgs e)
        {
            if (btnToggle.IsChecked != true)
            {
                Popup.PlacementTarget = btnOrder;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Order";
            }
        }

        private void btnOrder_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnSetting_MouseEnter(object sender, MouseEventArgs e)
        {
            if (btnToggle.IsChecked != true)
            {
                Popup.PlacementTarget = btnSetting;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Setting";
            }

        }

        private void btnSetting_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }

        private void btnExit_MouseEnter(object sender, MouseEventArgs e)
        {
            if (btnToggle.IsChecked != true)
            {
                Popup.PlacementTarget = btnExit;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Exit";
            }

        }

        private void btnExit_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;
        }


        private void btnToggle_Click(object sender, RoutedEventArgs e)
        {

        }


        private void txtHome_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnHome_Click(sender, e);

        }

        private void txtStatistic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnStatistic_Click(sender, e);

        }

        private void txtProduct_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnProduct_Click(sender, e);

        }

        private void txtOrder_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnOrder_Click(sender, e);

        }

        private void txtSetting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnSetting_Click(sender, e);

        }

        private void txtExit_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnExit_Click(sender, e);
        }

        private void btnProductStatistic_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("pages/ProductStatistic.xaml", UriKind.RelativeOrAbsolute));
        }

        private void btnProductStatistic_MouseEnter(object sender, MouseEventArgs e)
        {

            if (btnToggle.IsChecked != true)
            {
                Popup.PlacementTarget = btnProductStatistic;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Product Statistic";
            }

        }

        private void btnProductStatistic_MouseLeave(object sender, MouseEventArgs e)
        {
            Popup.Visibility = Visibility.Collapsed;
            Popup.IsOpen = false;

        }

        private void txtProductStatistic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            btnProductStatistic_Click(sender, e);
        }

        private void btnCus_Click(object sender, RoutedEventArgs e)
        {
            fContainer.Navigate(new System.Uri("pages/CustomerManagement.xaml", UriKind.RelativeOrAbsolute));

        }

        private void btnCus_MouseEnter(object sender, MouseEventArgs e)
        {
            if (btnToggle.IsChecked != true)
            {
                Popup.PlacementTarget = btnProductStatistic;
                Popup.Placement = PlacementMode.Right;
                Popup.IsOpen = true;
                Header.PopupText.Text = "Product Statistic";
            }
        }

        private void btnCus_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}