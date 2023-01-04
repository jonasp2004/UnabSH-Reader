using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace UnabSH_Reader {

    public partial class MainWindow : Window  {
        public string tempArtOverview;

        internal bool articlesLoaded = false;
        public int newsCount = 0;

        internal List<String> articles = new List<String>();
        internal List<String> authors = new List<String>();

        public MainWindow() {
            InitializeComponent();
            window_btnHints.Visibility = Visibility.Collapsed;
            blr_actionView.Radius = 0;
            grd_errorOnLoading.Visibility = Visibility.Collapsed;

            border.Cursor = ((TextBlock)this.Resources["mainCursor"]).Cursor;
            TopBtn_CloseWindow.Cursor = ((TextBlock)this.Resources["handCursor"]).Cursor;
            TopBtn_MaximizeWindow.Cursor = ((TextBlock)this.Resources["handCursor"]).Cursor;
            TopBtn_MinimizeWindow.Cursor = ((TextBlock)this.Resources["handCursor"]).Cursor;
        }

        private void window_Loaded(object sender, RoutedEventArgs e) {
            try {
                FetchTitles();
                GarbageCollector();
            } catch (Exception ex) {
                grd_errorOnLoading.Visibility = Visibility.Visible;
                blr_actionView.Radius = 25;
                txt_errDebug.Text = ex.Message.ToString();
            }
        }

        async void GarbageCollector()  {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public void FetchTitles() {
            articles.Clear();
            authors.Clear();
            int temp = 0;

            articleView.Items.Clear();
            // Benutzt den Webclient
            using (var client = new WebClient()) {
                // Deklariert das Zieldokument
                string html = client.DownloadString("https://unabsh.000webhostapp.com/articleList.php");
                // Erstellt ein HTMLDocument-Objekt
                HtmlDocument doc = new HtmlDocument();
                // Lädt das Zieldok runter
                doc.LoadHtml(html);
                // Wählt alle span-Knoten (HTML-Tags), welche als id 'app_newsCounter' enthalten, an.
                var nodes = doc.DocumentNode.SelectNodes("//span")
                .Where(d => d.Attributes.Contains("id"))
                .Where(d => d.Attributes["id"].Value.Contains("app_title"));
                // Liest Artikeltitel und Autorenname heraus
                foreach (HtmlNode node in nodes) {
                    articles.Add(Convert.ToString(node.InnerText));
                    temp++;
                }; 
                nodes = doc.DocumentNode.SelectNodes("//span")
                .Where(d => d.Attributes.Contains("id"))
                .Where(d => d.Attributes["id"].Value.Contains("app_author"));
                foreach (HtmlNode node in nodes) {
                    authors.Add(Convert.ToString(node.InnerText));
                };

                lastUpdatedIndicator.Text = "Am " + DateTime.Now + " gab es " + temp.ToString() + " Nachrichten";
                // Fügt die Ergebnisse ein, markiert die Titel entsprechend zu Lieblingsautoren
                string tempFavList = Properties.Settings.Default.lieblingsAutoren;
                for(int i = 0; articles.Count > i; i++) {
                    string tempAuthor = authors.ElementAt(i);
                    string tempTitle = articles.ElementAt(i);
                    if (tempFavList.Contains(tempAuthor)) {
                        articleView.Items.Add("♡ " + tempTitle);
                    } else {
                        articleView.Items.Add(tempTitle);
                    }
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
            } else {
                window.WindowState = WindowState.Normal;
            }
            GarbageCollector();
        }

        private void articleView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            Properties.Settings.Default.tempSelectedID = articleView.SelectedItems[0].ToString().Replace("♡ ", "");
            viewer view = new viewer();
            view.Show();
            GarbageCollector();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            Einstellungen settings = new();
            settings.ShowDialog();
        }

        private void TopBtn_MinimizeWindow_MouseEnter(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Visible;
            window_btnHints.Margin = new Thickness(23, 24, 0, 0);
            TopBtn_MinimizeWindow.Fill = Brushes.Orange;
            txt_windowBtnHint.Text = "Minimieren";
        }

        private void TopBtn_MinimizeWindow_MouseLeave(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Collapsed;
            TopBtn_MinimizeWindow.Fill = Brushes.Yellow;
            GarbageCollector(); }

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
            GarbageCollector(); }

        private void TopBtn_CloseWindow_MouseEnter(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Visible;
            window_btnHints.Margin = new Thickness(2, 24, 0, 0);
            TopBtn_CloseWindow.Fill = Brushes.DarkRed;
            txt_windowBtnHint.Text = "Schließen";
        }

        private void TopBtn_CloseWindow_MouseLeave(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Collapsed;
            TopBtn_CloseWindow.Fill = Brushes.Red;
            GarbageCollector(); }

        private void btn_closeProgram_Click(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }

        private void btn_reloadList_Click(object sender, RoutedEventArgs e) {
            blr_actionView.Radius = 0;
            grd_errorOnLoading.Visibility = Visibility.Collapsed;
            tempArtOverview = "Artikel werden abgerufen. Dies kann etwas dauern.";
            FetchTitles();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e) {
            Properties.Settings.Default.tempSelectedID = "Hiermit möchte ich das neue Update auf die Seitenversion 2.3 ankündigen!";
            viewer view = new viewer();
            view.Show();
            GarbageCollector();
        }
    }
}