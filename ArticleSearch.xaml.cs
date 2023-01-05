using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UnabSH_Reader {

    public partial class ArticleSearch : Window {

        public string[] metadataArticles;
        public string[] contentArticles;


        public ArticleSearch() {
            InitializeComponent();
            window_btnHints.Visibility = Visibility.Collapsed;
            brdr_waiting.Visibility= Visibility.Collapsed;
        }

        public void GarbageCollector() {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void titleText_MouseDown(object sender, MouseButtonEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                window.DragMove();
            }
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



        private void TopBtn_CloseWindow_MouseEnter(object sender, MouseEventArgs e) {
            window_btnHints.Visibility = Visibility.Visible;
            window_btnHints.Margin = new Thickness(14, 34, 0, 0);
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
            window_btnHints.Margin = new Thickness(35, 34, 0, 0);
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
            window_btnHints.Margin = new Thickness(55, 34, 0, 0);
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

        private async void btn_search_Click(object sender, RoutedEventArgs e) {
            brdr_waiting.Visibility= Visibility.Visible;
            await MetaSearch();
            txt_query.Focus();
            brdr_waiting.Visibility= Visibility.Collapsed;
        }

        internal async Task MetaSearch() {
            await Task.Run(() => {
                Thread.Sleep(1000);
            });
        }
    }
}