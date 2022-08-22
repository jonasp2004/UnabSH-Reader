using HtmlAgilityPack;
using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UnabSH_Reader
{
    public partial class viewer : Window {

        public string appID;

        public viewer() {
            InitializeComponent();
            viewerAppereanceOptionsWindow.Visibility = Visibility.Collapsed;
            LoadSettings();
            GarbageCollector();
        }

        public void LoadSettings() {
            fontSizeSlider.Value = Convert.ToDouble(Properties.Settings.Default.Schriftgröße);
            backgroundblurSlider.Value = Properties.Settings.Default.Hintergrundunschärfe;
            backgroundopacitySlider.Value = Properties.Settings.Default.Bildsichtbarkeit;
            if (Properties.Settings.Default.stdSchriftart == "arial") {
                fontSelector_Arial.IsChecked = true;
            }
            if (Properties.Settings.Default.stdSchriftart == "courier") {
                fontSelector_Courier.IsChecked = true;
            }
            if (Properties.Settings.Default.stdSchriftart == "comic") {
                fontSelector_Comic.IsChecked = true;
            }
            if (Properties.Settings.Default.stdSchriftart == "segoe") {
                fontSelector_Segoe.IsChecked = true;
            }
            if (Properties.Settings.Default.stdAussehen == "std") {
                themeChooser_white.IsChecked = true;
            }
            if (Properties.Settings.Default.stdAussehen == "black") {
                themeChooser_black.IsChecked = true;
            }
            if (Properties.Settings.Default.stdAussehen == "sepia") {
                themeChooser_sepia.IsChecked = true;
            }
            if (Properties.Settings.Default.stdAussehen == "night") {
                themeChooser_night.IsChecked = true;
            }
        }

        void GarbageCollector() {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void titleText_MouseDown(object sender, MouseButtonEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed)  {
                window.DragMove();
            }
            GarbageCollector();
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
            Clipboard.SetText("https://unabsh.000webhostapp.com/news/view.php?id=" + appID);
            GarbageCollector();
        }

        private void btn_openWebViewer_Click(object sender, RoutedEventArgs e) {
            try {
                string tempUrl = @"https://unabsh.000webhostapp.com/news/view.php?id=" + appID;
                Process.Start(tempUrl);
            } catch (System.ComponentModel.Win32Exception noBrowser) {
                Properties.Settings.Default.tempErrMsg = "Sie haben möglicherweise keinen gültigen Browser installiert.";
                err_dialog error = new();
                error.ShowDialog();
            } catch (System.Exception ex) {
                Properties.Settings.Default.tempErrMsg = "Konnte eine Webseite nicht starten. \n" + ex.Message;
                err_dialog error = new();
                error.ShowDialog();
            }
            GarbageCollector();
        }

        public void FetchWebData() {
            try { 
            using (var client = new WebClient()) {
                string html = client.DownloadString("https://unabsh.000webhostapp.com/news/appviewData.php?title=" + Properties.Settings.Default.tempSelectedID);
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(html);
                var nodes = doc.DocumentNode.SelectNodes("//span")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value == "app_content");
                foreach (HtmlNode node in nodes) {
                    articleContent.Text = node.InnerText;
                }
                
                nodes = doc.DocumentNode.SelectNodes("//span")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value == "app_authorName");
                foreach (HtmlNode node in nodes) {
                    authorName.Text = node.InnerText;
                }

                nodes = doc.DocumentNode.SelectNodes("//span")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value == "app_timestamp");
                foreach (HtmlNode node in nodes)
                {
                    timestamp.Text = node.InnerText;
                }
                nodes = doc.DocumentNode.SelectNodes("//span")
                    .Where(d => d.Attributes.Contains("id"))
                    .Where(d => d.Attributes["id"].Value == "app_title");
                foreach (HtmlNode node in nodes)
                {
                    articleTitle.Text = node.InnerText;
                }
                    nodes = doc.DocumentNode.SelectNodes("//span")
                        .Where(d => d.Attributes.Contains("id"))
                        .Where(d => d.Attributes["id"].Value == "app_id");
                    foreach (HtmlNode node in nodes) {
                        appID = node.InnerText;
                    }
                authorProfilepic.Source = new BitmapImage(new Uri("pack://application:,,,/Images/profilepic.png"));
                if (authorName.Text == "Jonas") {
                    authorProfilepic.Source = new BitmapImage(new Uri("pack://application:,,,/Images/j_noPicture.png"));
                } else if (authorName.Text == "Walter Scheel") {
                    authorProfilepic.Source = new BitmapImage(new Uri("pack://application:,,,/Images/walterscheel.png"));
                }


                    var backgroundURI = @"https://unabsh.000webhostapp.com/news/images/" + appID + ".png";
                    BitmapImage bitmap2 = new BitmapImage();
                    bitmap2.BeginInit();
                    bitmap2.UriSource = new Uri(backgroundURI, UriKind.Absolute);
                    bitmap2.EndInit();
                    backgroundImage.Source = bitmap2;
                    GarbageCollector();
                }
            } catch (Exception ex){
                Properties.Settings.Default.tempErrMsg = ex.Message.ToString();
                err_dialog error = new();
                error.ShowDialog();
                GarbageCollector();
                Close();
            }
            GarbageCollector();
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

        private void themeChooser_white_Checked(object sender, RoutedEventArgs e) {
            border.Background = Brushes.White;
            articleContent.Foreground = Brushes.Black;
            articleTitle.Foreground = Brushes.Black;
            GarbageCollector();

        }

        private void themeChooser_black_Checked(object sender, RoutedEventArgs e) {
            border.Background = new SolidColorBrush(Color.FromRgb(14, 14, 14));
            articleContent.Foreground = Brushes.White;
            articleTitle.Foreground = Brushes.White;
            GarbageCollector();
        }

        private void themeChooser_sepia_Checked(object sender, RoutedEventArgs e) {
            border.Background = new SolidColorBrush(Color.FromRgb(236, 194, 151));
            articleContent.Foreground = Brushes.Black;
            articleTitle.Foreground = Brushes.Black;
            GarbageCollector();
        }

        private void themeChooser_night_Checked(object sender, RoutedEventArgs e) {
            border.Background = Brushes.Black;
            articleContent.Foreground = Brushes.White;
            articleTitle.Foreground = Brushes.White;
            GarbageCollector();
        }
    }
}
