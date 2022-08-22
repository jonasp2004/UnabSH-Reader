using HtmlAgilityPack;
using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UnabSH_Reader {

    public partial class MainWindow : Window  {
        public int tempNewsCounter = 0;
        public string tempArticleID;
        public string tempArticleName;
        public string tempArticleAuthor;

        public string tempArtOverview;
        public MainWindow() {
            InitializeComponent();
        }

        async void GarbageCollector()  {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private async void FetchData() {
            try {
                await Task.Run(FangeWebseiteAb);
                lastUpdatedIndicator.Text = tempArtOverview;
                FetchTitles();
            } catch (Exception ex) {
                Properties.Settings.Default.tempErrMsg = ex.Message.ToString();
                err_dialog error = new();
                error.ShowDialog();
            }
            GarbageCollector();
        }

        internal async Task FangeWebseiteAb() {
            using (var client = new WebClient()) {
                string html = client.DownloadString("https://unabsh.000webhostapp.com/articleList.php");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var nodes = doc.DocumentNode.SelectNodes("//span")
                        .Where(d => d.Attributes.Contains("id"))
                        .Where(d => d.Attributes["id"].Value == "app_newsCounter");
                foreach (HtmlNode node in nodes) {
                    tempNewsCounter = Convert.ToInt32(node.InnerText);
                    tempArtOverview = "Am " + DateTime.Now + " gab es " + tempNewsCounter.ToString() + " Nachrichten";
                }
            }
        }

        internal async Task FetchTitles() {
            using (var client = new WebClient()) {
                //Hier fängt das eigentliche Abrufen der Daten an
                string html = client.DownloadString("https://unabsh.000webhostapp.com/articleList.php");
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                for (int i = 1; i < tempNewsCounter + 1; i++)  {
                    var nodes = doc.DocumentNode.SelectNodes("//span")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value.Contains("app_title" + i.ToString()));
                    foreach (HtmlNode node in nodes) {
                        this.articleView.Items.Add(Convert.ToString(node.InnerText));
                        Console.WriteLine();
                    };
                }
            }
            GarbageCollector();
        }

        private void titleText_MouseDown(object sender, MouseButtonEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                window.DragMove();
            }
            GarbageCollector();
        }

        private void TopBtn_CloseWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            GarbageCollector();
            Close();
        }

        private void TopBtn_MinimizeWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            window.WindowState = WindowState.Minimized;
            GarbageCollector();
        }

        private void TopBtn_MaximizeWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (window.WindowState == WindowState.Normal) {
                window.WindowState = WindowState.Maximized;
                TopBtn_MaximizeWindow.ToolTip = "Wiederherstellen";
            } else {
                window.WindowState = WindowState.Normal;
                TopBtn_MaximizeWindow.ToolTip = "Maximieren";
            }
            GarbageCollector();
        }

        private void articleView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Properties.Settings.Default.tempSelectedID = articleView.SelectedItems[0].ToString();
            viewer view = new viewer();
            view.Show();
            GarbageCollector();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Einstellungen settings = new();
            settings.ShowDialog();
        }

        private void window_Initialized(object sender, EventArgs e) {
            FetchData();
            lastUpdatedIndicator.Text = "Am " + DateTime.Now + " gab es " + tempNewsCounter.ToString() + " Nachrichten";
            GarbageCollector();
        }
    }
}
