using HtmlAgilityPack;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows.Networking.NetworkOperators;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;

namespace UnabSH_Reader {
    public partial class viewer : Window {

        public string appID;

        public string title;
        public string text;
        public string author;
        public string releaseDate;

        internal FontFamily varela = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Varela Round");
        internal FontFamily arial = new FontFamily("Arial");
        internal FontFamily courier = new FontFamily("Courier New");
        internal FontFamily comic = new FontFamily("Comic Sans MS");
        internal FontFamily segoe = new FontFamily("Segoe UI");

        public BitmapImage bitmapImage = new BitmapImage();

        public viewer() {
            InitializeComponent();
            window_btnHints.Visibility = Visibility.Collapsed;
            grd_authorInfo.Visibility = Visibility.Collapsed;

            border.Cursor = ((TextBlock)this.Resources["mainCursor"]).Cursor;
            TopBtn_CloseWindow.Cursor = ((TextBlock)this.Resources["handCursor"]).Cursor;
            TopBtn_MaximizeWindow.Cursor = ((TextBlock)this.Resources["handCursor"]).Cursor;
            TopBtn_MinimizeWindow.Cursor = ((TextBlock)this.Resources["handCursor"]).Cursor;
            grd_containerPageSettings.Cursor = ((TextBlock)this.Resources["mainCursor"]).Cursor;

            LoadSettings();
            GarbageCollector();
        }

        public void LoadSettings() {
            fontSizeSlider.Value = Convert.ToDouble(Properties.Settings.Default.Schriftgröße);
            backgroundblurSlider.Value = Properties.Settings.Default.Hintergrundunschärfe;
            backgroundopacitySlider.Value = Properties.Settings.Default.Bildsichtbarkeit;
            if (Properties.Settings.Default.stdSchriftart == "arial") {
                fontSelector_Arial.IsChecked = true;
                ApplyFont(arial);
            } else if (Properties.Settings.Default.stdSchriftart == "courier") {
                fontSelector_Courier.IsChecked = true;
                ApplyFont(courier);
            } else if (Properties.Settings.Default.stdSchriftart == "comic") {
                fontSelector_Comic.IsChecked = true;
                ApplyFont(comic);
            } else if (Properties.Settings.Default.stdSchriftart == "segoe") {
                fontSelector_Segoe.IsChecked = true;
                ApplyFont(segoe);
            }
            if (Properties.Settings.Default.stdAussehen == "std") {
                themeChooser_white.IsChecked = true;
            } else if (Properties.Settings.Default.stdAussehen == "black") {
                themeChooser_black.IsChecked = true;
            } else if (Properties.Settings.Default.stdAussehen == "sepia") {
                themeChooser_sepia.IsChecked = true;
            } else if (Properties.Settings.Default.stdAussehen == "night") {
                themeChooser_night.IsChecked = true;
            }
        }

        internal void ApplyFont(FontFamily font) {
            articleContent.FontFamily = font;
            authorName.FontFamily = font;
            timestamp.FontFamily = font;
            articleTitle.FontFamily = font;
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
                txt_windowBtnHint.Text = "Wiederherstellen";
            } else {
                window.WindowState = WindowState.Normal;
                txt_windowBtnHint.Text = "Maximieren";
            }
            GarbageCollector();
        }

        private async void window_Loaded(object sender, RoutedEventArgs e) {
            try {
                await FetchWebData();

                authorName.Text = author;
                timestamp.Text = releaseDate;
                articleTitle.Text = title;
                articleContent.Text = text;

                if (authorName.Text == "Hans Schwürloh") {
                    authorProfilepic.Source = new BitmapImage(new Uri("pack://application:,,,/Images/profilepic.png"));
                    img_authorImageXXL.Source = new BitmapImage(new Uri("pack://application:,,,/Images/profilepic.png"));
                    txt_authorAttr.Text = "Leitung unserer freien Presse";
                } else if (authorName.Text == "Jonas") {
                    authorProfilepic.Source = new BitmapImage(new Uri("pack://application:,,,/Images/j_noPicture.png"));
                    img_authorImageXXL.Source = new BitmapImage(new Uri("pack://application:,,,/Images/j_noPicture.png"));
                    txt_authorAttr.Text = "Head of IT; Technik-News";
                } else if (authorName.Text == "Walter Scheel") {
                    authorProfilepic.Source = new BitmapImage(new Uri("pack://application:,,,/Images/walterscheel.png"));
                    img_authorImageXXL.Source = new BitmapImage(new Uri("pack://application:,,,/Images/walterscheel.png"));
                    txt_authorAttr.Text = "Chef- & Außenreporter";
                }
                txt_authorName.Text = authorName.Text;

                var backgroundURI = @"https://unabsh.000webhostapp.com/news/images/" + appID + ".png";
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(backgroundURI, UriKind.Absolute);
                bitmapImage.EndInit();
                backgroundImage.Source = bitmapImage;
                Bitmap bmpImg = colorConv.BitmapImage2Bitmap(bitmapImage);
                brdr_header.Background = colorConv.GetDominantColor(bmpImg);

                if (Properties.Settings.Default.lieblingsAutoren.Contains(txt_authorName.Text)) {
                    btn_addFavouriteAuthor.Content = "Lieblingsautor";
                    btn_addFavouriteAuthor.Foreground = Brushes.White;
                    btn_addFavouriteAuthor.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/UIAssets/btn_blue.png")));
                }
                
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn_copyLink_Click(object sender, RoutedEventArgs e) {
            Clipboard.SetText("https://unabsh.000webhostapp.com/news/view.php?id=" + appID);
            GarbageCollector();
        }

        private void btn_openWebViewer_Click(object sender, RoutedEventArgs e) {
            try {
                var uri = @"https://unabsh.000webhostapp.com/news/view.php?id=" + appID;
                var psi = new System.Diagnostics.ProcessStartInfo();
                psi.UseShellExecute = true;
                psi.FileName = uri;
                System.Diagnostics.Process.Start(psi);
            } catch (System.ComponentModel.Win32Exception noBrowser) {
                err_dialog error = new("Sie haben möglicherweise keinen gültigen Browser installiert.");
                error.ShowDialog();
            } catch (System.Exception ex) {
                err_dialog error = new("Konnte eine Webseite nicht starten. \n" + ex.Message);
                error.ShowDialog();
            }
            GarbageCollector();
        }

        private async Task FetchWebData() {
            await Task.Run(() => {
                using (var client = new WebClient()) {
                    string html = client.DownloadString("https://unabsh.000webhostapp.com/news/appviewData.php?title=" + Properties.Settings.Default.tempSelectedID);
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(html);
                    var nodes = doc.DocumentNode.SelectNodes("//span")
                        .Where(d => d.Attributes.Contains("id"))
                        .Where(d => d.Attributes["id"].Value == "app_content");
                    foreach (HtmlNode node in nodes) {
                        text = node.InnerText;
                    }

                    nodes = doc.DocumentNode.SelectNodes("//span")
                        .Where(d => d.Attributes.Contains("id"))
                        .Where(d => d.Attributes["id"].Value == "app_authorName");
                    foreach (HtmlNode node in nodes) {
                        author = node.InnerText;
                    }

                    nodes = doc.DocumentNode.SelectNodes("//span")
                        .Where(d => d.Attributes.Contains("id"))
                        .Where(d => d.Attributes["id"].Value == "app_timestamp");
                    foreach (HtmlNode node in nodes)
                    {
                        releaseDate = node.InnerText;
                    }
                    nodes = doc.DocumentNode.SelectNodes("//span")
                        .Where(d => d.Attributes.Contains("id"))
                        .Where(d => d.Attributes["id"].Value == "app_title");
                    foreach (HtmlNode node in nodes) {
                        title = node.InnerText;
                    }
                    nodes = doc.DocumentNode.SelectNodes("//span")
                        .Where(d => d.Attributes.Contains("id"))
                        .Where(d => d.Attributes["id"].Value == "app_id");
                    foreach (HtmlNode node in nodes) {
                        appID = node.InnerText;
                    }
                    
                }
            });
            GarbageCollector();
        }

        private void btn_showReaderAppereance_Click(object sender, RoutedEventArgs e) {
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
            window.FontFamily = FontFamily = varela;
            GarbageCollector();
        }

        private void fontSelector_Arial_Checked(object sender, RoutedEventArgs e) {
            articleContent.FontFamily = arial;
            authorName.FontFamily = arial;
            timestamp.FontFamily = arial;
            articleTitle.FontFamily = arial;
            GarbageCollector();
        }

        private void fontSelector_Courier_Checked(object sender, RoutedEventArgs e) {
            articleContent.FontFamily = courier;
            authorName.FontFamily = courier;
            timestamp.FontFamily = courier;
            articleTitle.FontFamily = courier;
            GarbageCollector();
        }

        private void fontSelector_Comic_Checked(object sender, RoutedEventArgs e) {
            articleContent.FontFamily = comic;
            authorName.FontFamily = comic;
            timestamp.FontFamily = comic;
            articleTitle.FontFamily = comic;
            GarbageCollector();
        }

        private void fontSelector_Segoe_Checked(object sender, RoutedEventArgs e) {
            articleContent.FontFamily = segoe;
            authorName.FontFamily = segoe;
            timestamp.FontFamily = segoe;
            articleTitle.FontFamily = segoe;
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

        private void TopBtn_CloseWindow_MouseEnter(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Visible;
            window_btnHints.Margin = new Thickness(4, 24, 0, 0);
            TopBtn_CloseWindow.Fill = Brushes.DarkRed;
            txt_windowBtnHint.Text = "Schließen";
        }

        private void TopBtn_CloseWindow_MouseLeave(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Collapsed;
            TopBtn_CloseWindow.Fill = Brushes.Red;
            GarbageCollector();
        }

        private void TopBtn_MinimizeWindow_MouseEnter(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Visible;
            window_btnHints.Margin = new Thickness(25, 24, 0, 0);
            TopBtn_MinimizeWindow.Fill = Brushes.Orange;
            txt_windowBtnHint.Text = "Minimieren";
        }

        private void TopBtn_MinimizeWindow_MouseLeave(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Collapsed;
            TopBtn_MinimizeWindow.Fill = Brushes.Yellow;
            GarbageCollector();
        }

        private void TopBtn_MaximizeWindow_MouseEnter(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Visible;
            window_btnHints.Margin = new Thickness(45, 24, 0, 0);
            TopBtn_MaximizeWindow.Fill = Brushes.DarkGreen;
            if (window.WindowState == WindowState.Maximized) {
                txt_windowBtnHint.Text = "Wiederherstellen";
            } else {
                txt_windowBtnHint.Text = "Maximieren";
            }
        }

        private void TopBtn_MaximizeWindow_MouseLeave(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Collapsed;
            TopBtn_MaximizeWindow.Fill = Brushes.Green;
            GarbageCollector();
        }

        private void btn_saveDesignGlobally_Click(object sender, RoutedEventArgs e) {
            // Schriftart speichern
            if(fontSelector_Varela.IsChecked == true) {
                Properties.Settings.Default.stdSchriftart = "varela";
            } else if (fontSelector_Segoe.IsChecked == true) {
                Properties.Settings.Default.stdSchriftart = "segoe";
            } else if (fontSelector_Courier.IsChecked == true) {
                Properties.Settings.Default.stdSchriftart = "courier";
            } else if (fontSelector_Arial.IsChecked == true) {
                Properties.Settings.Default.stdSchriftart = "arial";
            } else if (fontSelector_Comic.IsChecked == true) {
                Properties.Settings.Default.stdSchriftart = "comic";
            }
            
            if(themeChooser_white.IsChecked == true) {
                Properties.Settings.Default.stdAussehen = "std";
            } else if (themeChooser_black.IsChecked == true) {
                Properties.Settings.Default.stdSchriftart = "black";
            } else if (themeChooser_sepia.IsChecked == true) {
                Properties.Settings.Default.stdSchriftart = "sepia";
            } else if (themeChooser_night.IsChecked == true) {
                Properties.Settings.Default.stdSchriftart = "night";
            }

            // Die Zahlenwerte speichern
            Properties.Settings.Default.Schriftgröße = (decimal)fontSizeSlider.Value;
            Properties.Settings.Default.Bildsichtbarkeit = (float)backgroundopacitySlider.Value;
            Properties.Settings.Default.Hintergrundunschärfe = backgroundblurSlider.Value;
            Properties.Settings.Default.Save();
        }

        private void authorProfilepic_MouseEnter(object sender, MouseEventArgs e) {
            grd_authorInfo.Visibility = Visibility.Visible;
        }

        private void authorProfilepic_MouseLeave(object sender, MouseEventArgs e) {
            grd_authorInfo.Visibility = Visibility.Collapsed;
        }

        private void btn_addFavouriteAuthor_MouseDown(object sender, MouseButtonEventArgs e) {
            if (btn_addFavouriteAuthor.Content == "Lieblingsautor") {
                Properties.Settings.Default.lieblingsAutoren = Properties.Settings.Default.lieblingsAutoren.Replace(txt_authorName.Text + ";", "");
                Properties.Settings.Default.Save();
                btn_addFavouriteAuthor.Content = "Lieblingsautor setzen";
                btn_addFavouriteAuthor.Foreground = Brushes.Black;
                btn_addFavouriteAuthor.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/UIAssets/btn_base.png")));
            } else {
                Properties.Settings.Default.lieblingsAutoren = Properties.Settings.Default.lieblingsAutoren + txt_authorName.Text + ";";
                Properties.Settings.Default.Save();
                btn_addFavouriteAuthor.Content = "Lieblingsautor";
                btn_addFavouriteAuthor.Foreground = Brushes.White;
                btn_addFavouriteAuthor.Background = new ImageBrush(new BitmapImage(new Uri(@"pack://application:,,,/UIAssets/btn_blue.png")));
            }
        }
    }
}
