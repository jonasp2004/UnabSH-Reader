using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;
using MySql.Data.MySqlClient;
using System.Configuration;
using static System.Net.Mime.MediaTypeNames;
using HtmlAgilityPack;
using System.Net;

namespace UnabSH_Reader {
    public partial class viewer : Window {

        readonly MySqlConnection conn = new
            MySqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);

        public viewer() {
            InitializeComponent();
            viewerAppereanceOptionsWindow.Visibility = Visibility.Collapsed;
            fontSizeSlider.Value = 12;
            backgroundblurSlider.Value = 7;
            backgroundopacitySlider.Value = 0.2;
            GarbageCollector();
        }

        void GarbageCollector() {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void titleText_MouseDown(object sender, MouseButtonEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed)  {
                window.DragMove();
                GarbageCollector();
            }
        }

        private void TopBtn_CloseWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            Close();
            GarbageCollector();
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

        private void window_Loaded(object sender, RoutedEventArgs e) {
            FetchWebData();
        }

        private void btn_copyLink_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText("https://unabsh.000webhostapp.com/news/view.php?id=" + Properties.Settings.Default.tempSelectedID);
            GarbageCollector();
        }

        private void btn_openWebViewer_Click(object sender, RoutedEventArgs e) {
            webPresenterWindow.Visibility = Visibility.Visible;
            windowBlur.Radius = 10;
            webView.Navigate("https://unabsh.000webhostapp.com/news/view.php?id=" + Properties.Settings.Default.tempSelectedID);
        }

        public async void FetchWebData() {
            try { 
            using (var client = new WebClient()) {
                string html = client.DownloadString("https://unabsh.000webhostapp.com/news/view.php?id=" + Properties.Settings.Default.tempSelectedID);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var nodes = doc.DocumentNode.SelectNodes("//span")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value == "app_content");
                foreach (HtmlNode node in nodes) {
                    articleContent.Text = node.InnerText;
                }
                
                nodes = doc.DocumentNode.SelectNodes("//p")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value == "app_authorName");
                foreach (HtmlNode node in nodes) {
                    authorName.Text = node.InnerText;
                }

                nodes = doc.DocumentNode.SelectNodes("//p")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value == "app_timestamp");
                foreach (HtmlNode node in nodes)
                {
                    timestamp.Text = node.InnerText;
                }
                nodes = doc.DocumentNode.SelectNodes("//h2")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value == "app_title");
                foreach (HtmlNode node in nodes)
                {
                    articleTitle.Text = node.InnerText;
                }

                    var fullFilePath = @"https://unabsh.000webhostapp.com/assets/profilepic.png";
                    if (authorName.Text == "Jonas")
                    {
                        fullFilePath = @"https://unabsh.000webhostapp.com/assets/j_noPicture.jpg";
                    }
                    else if (authorName.Text == "Walter Scheel")
                    {
                        fullFilePath = @"https://unabsh.000webhostapp.com/assets/walterscheel.jpg";
                    }
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(fullFilePath, UriKind.Absolute);
                    bitmap.EndInit();
                    authorProfilepic.Source = bitmap;


                    var backgroundURI = @"https://unabsh.000webhostapp.com/news/images/" + Properties.Settings.Default.tempSelectedID + ".png";
                    BitmapImage bitmap2 = new BitmapImage();
                    bitmap2.BeginInit();
                    bitmap2.UriSource = new Uri(backgroundURI, UriKind.Absolute);
                    bitmap2.EndInit();
                    backgroundImage.Source = bitmap2;
                    GarbageCollector();
                }
            } catch {
                MessageBox.Show("Fehler beim abrufen der Daten. Das Fenster wird nun geschlossen!", "Info", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                GarbageCollector();
                Close();
            }
        }

        private void closeAppereanceWindow_MouseUp(object sender, MouseButtonEventArgs e) {
            viewerAppereanceOptionsWindow.Visibility = Visibility.Collapsed;
            GarbageCollector();
        }

        private void btn_showReaderAppereance_Click(object sender, RoutedEventArgs e) {
            viewerAppereanceOptionsWindow.Visibility = Visibility.Visible;
            GarbageCollector();
        }

        private void backgroundblurSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            bkgblur.Radius = backgroundblurSlider.Value;
            GarbageCollector();
        }

        private void backgroundopacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            backgroundImage.Opacity = backgroundopacitySlider.Value;
            GarbageCollector();
        }

        private void fontSelector_Varela_Checked(object sender, RoutedEventArgs e) {
            articleContent.FontFamily = FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Varela Round");
            authorName.FontFamily = FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Varela Round");
            timestamp.FontFamily = FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Varela Round");
            articleTitle.FontFamily = FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Varela Round");
            GarbageCollector();
        }

        private void fontSelector_Arial_Checked(object sender, RoutedEventArgs e) {
            articleContent.FontFamily = new FontFamily("Arial");
            authorName.FontFamily = new FontFamily("Arial");
            timestamp.FontFamily = new FontFamily("Arial");
            articleTitle.FontFamily = new FontFamily("Arial");
            GarbageCollector();
        }

        private void fontSelector_Courier_Checked(object sender, RoutedEventArgs e) {
            articleContent.FontFamily = new FontFamily("Courier New");
            authorName.FontFamily = new FontFamily("Courier New");
            timestamp.FontFamily = new FontFamily("Courier New");
            articleTitle.FontFamily = new FontFamily("Courier New");
            GarbageCollector();
        }

        private void fontSelector_Comic_Checked(object sender, RoutedEventArgs e) {
            articleContent.FontFamily = new FontFamily("Comic Sans MS");
            authorName.FontFamily = new FontFamily("Comic Sans MS");
            timestamp.FontFamily = new FontFamily("Comic Sans MS");
            articleTitle.FontFamily = new FontFamily("Comic Sans MS");
            GarbageCollector();
        }

        private void fontSelector_Segoe_Checked(object sender, RoutedEventArgs e) {
            articleContent.FontFamily = new FontFamily("Segoe UI");
            authorName.FontFamily = new FontFamily("Segoe UI");
            timestamp.FontFamily = new FontFamily("Segoe UI");
            articleTitle.FontFamily = new FontFamily("Segoe UI");
            GarbageCollector();
        }

        private void fontSizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            articleContent.FontSize = fontSizeSlider.Value;
            GarbageCollector();
        }

        private void btn_closeWebViewer_MouseUp(object sender, MouseButtonEventArgs e) {
            webPresenterWindow.Visibility = Visibility.Collapsed;
            windowBlur.Radius = 0;
            GarbageCollector();
        }

        private void webView_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e){
            webView.Visibility = Visibility.Visible;
            GarbageCollector();
        }
    }
}
