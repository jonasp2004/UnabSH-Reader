using System;
using System.Windows;
using System.Windows.Input;

using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Windows.Controls;
using HtmlAgilityPack;
using System.Linq;
using System.Net;
using System.Reflection;

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

        void FetchData() {
            /* try {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("Select id,title,author from headlines ORDER BY id DESC", conn);
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adp.Fill(ds, "LoadDataBinding");
                articleDataGrid.DataContext = ds;
                conn.Close();
                lastUpdatedIndicator.Text = "Sie sind auf den neuesten Stand seit: " + DateTime.Now.ToString("HH:mm") + ". Viel Spaß beim Entdecken!";
            }
            catch (MySqlException ex) {
                Clipboard.SetText(ex.Message);
                if (ex.Message.Contains("Unable to connect to any of the specified MySQL hosts.")) {
                    MessageBox.Show("Server konnte nicht gefunden werden. Bitte aktualisieren Sie die Anwendung oder versuchen Sie es erneut.", "Fehler beim Verbindungsaufbau", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                } else {
                    MessageBox.Show("Ein Fehler bei der Verbindung ist aufgetreten! Im nächsten Dialogfenster wird näheres erläutert.", "Fehler bei der Verbindung", MessageBoxButton.OK, MessageBoxImage.Error);
                    MessageBox.Show(ex.ToString());
                }
            }*/
            using (var client = new WebClient())
            {
                string html = client.DownloadString("https://unabsh.000webhostapp.com/news/news.php");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                //Selecting all the nodes with tagname `span` having "id=ctl00_ContentBody_CacheName".

                var nodes = doc.DocumentNode.SelectNodes("//span")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value == "app_getID")
                    .Where(e => e.Attributes.Contains("id"))
                    .Where(e => e.Attributes["id"].Value == "app_getArticleTitle");
                foreach (HtmlNode node in nodes) {
                    
                }
            }
        }

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
