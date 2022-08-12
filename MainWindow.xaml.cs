using HtmlAgilityPack;
using MySql.Data.MySqlClient;
using MySql.Data.MySqlClient.Memcached;
using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;

namespace UnabSH_Reader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly MySqlConnection conn = new
            MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        public MainWindow() {
            InitializeComponent();
        }

#pragma warning disable CS1998
        private async void FetchData() {
            try {
                
                } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.

        void timer_Tick(object sender, EventArgs e) {
            FetchData();
        }

        private void titleText_MouseDown(object sender, MouseButtonEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                window.DragMove();
            }
        }

        private void TopBtn_CloseWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            Close();
        }

        private void TopBtn_MinimizeWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            window.WindowState = WindowState.Minimized;
        }

        private void TopBtn_MaximizeWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (window.WindowState == WindowState.Normal) {
                window.WindowState = WindowState.Maximized;
                TopBtn_MaximizeWindow.ToolTip = "Wiederherstellen";
            } else {
                window.WindowState = WindowState.Normal;
                TopBtn_MaximizeWindow.ToolTip = "Maximieren";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            lastUpdatedIndicator.Text = "Bitte warten... Daten werden abgerufen...";
            FetchData();
        }

        private void articleDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            DataRowView row = articleDataGrid.SelectedItem as DataRowView;
            Properties.Settings.Default.tempSelectedID = row.Row.ItemArray[0].ToString();
            viewer view = new viewer();
            view.Show();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e) {
            Properties.Settings.Default.tempSelectedID = "20220714";
            viewer view = new viewer();
            view.Show();
        }
    }
}
