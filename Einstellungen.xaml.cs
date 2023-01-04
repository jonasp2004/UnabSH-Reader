using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UnabSH_Reader {

    public partial class Einstellungen : Window {

        public FontFamily tempSegoe = new FontFamily("Segoe UI");
        public FontFamily tempComicSans = new FontFamily("Comic Sans MS");
        public FontFamily tempCourier = new FontFamily("Courier New");
        public FontFamily tempArial = new FontFamily("Arial");
        public FontFamily tempVarela = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Varela Round");

        internal double imageBlurValue;
        internal double fontSizeValue;
        internal float backgroundVisibilityValue;

        public Einstellungen() {
            InitializeComponent();
            window_btnHints.Visibility = Visibility.Collapsed;
        }


        private void window_Loaded(object sender, RoutedEventArgs e) {
            RestoreSettings();
        }

        void RestoreSettings() {
            textSizeSlinder.Value = Convert.ToDouble(Properties.Settings.Default.Schriftgröße);
            backgroundblurSlider.Value = Convert.ToDouble(Properties.Settings.Default.Hintergrundunschärfe);
            backgroundopacitySlider.Value = Properties.Settings.Default.Bildsichtbarkeit;
        }

        async void GarbageCollector() {
            GC.Collect();
            GC.WaitForPendingFinalizers();
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

        private void backgroundblurSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            imageBlurValue = backgroundblurSlider.Value;
            txt_blurVal.Text = imageBlurValue.ToString();
            prefimgblur.Radius = imageBlurValue;
        }

        private void backgroundopacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            backgroundVisibilityValue = (float)backgroundopacitySlider.Value;
            txt_visibilityVal.Text = backgroundVisibilityValue.ToString();
            prev_image.Opacity = backgroundVisibilityValue;
        }

        private void textSizeSlinder_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            fontSizeValue = textSizeSlinder.Value;
            previewText.FontSize = fontSizeValue;
        }

        private void fontSelector_Varela_Checked(object sender, RoutedEventArgs e) {
            try {
                previewText.FontFamily = tempVarela;
                previewTitle.FontFamily = tempVarela;
            } catch { }
            Properties.Settings.Default.stdSchriftart = "varela";
            Properties.Settings.Default.Save();
        }

        private void fontSelector_Arial_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.stdSchriftart = "arial";
            Properties.Settings.Default.Save();
            previewText.FontFamily = tempArial;
            previewTitle.FontFamily = tempArial;
        }

        private void fontSelector_Courier_Checked(object sender, RoutedEventArgs e) {
            previewText.FontFamily = tempCourier;
            previewTitle.FontFamily = tempCourier;
            Properties.Settings.Default.stdSchriftart = "courier";
            Properties.Settings.Default.Save();
        }

        private void fontSelector_Comic_Checked(object sender, RoutedEventArgs e) {
            previewText.FontFamily = tempComicSans;
            previewTitle.FontFamily = tempComicSans;
            Properties.Settings.Default.stdSchriftart = "comic";
            Properties.Settings.Default.Save();
        }

        private void fontSelector_Segoe_Checked(object sender, RoutedEventArgs e) {
            previewText.FontFamily = tempSegoe;
            previewTitle.FontFamily = tempSegoe;
            Properties.Settings.Default.stdSchriftart = "segoe";
            Properties.Settings.Default.Save();
        }

        private void themeChooser_white_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.stdAussehen = "std";
            Properties.Settings.Default.Save();
            PreviewWindow.Background = new SolidColorBrush(Color.FromRgb(232, 232, 232));
            previewText.Foreground = Brushes.Black;
            previewTitle.Foreground = Brushes.Black;
            GarbageCollector();
        }

        private void themeChooser_black_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.stdAussehen = "black";
            Properties.Settings.Default.Save();
            PreviewWindow.Background = new SolidColorBrush(Color.FromRgb(14, 14, 14));
            previewText.Foreground = Brushes.White;
            previewTitle.Foreground = Brushes.White;
            GarbageCollector();
        }

        private void themeChooser_sepia_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.stdAussehen = "sepia";
            Properties.Settings.Default.Save();
            PreviewWindow.Background = new SolidColorBrush(Color.FromRgb(236, 194, 151));
            previewText.Foreground = Brushes.Black;
            previewTitle.Foreground = Brushes.Black;
            GarbageCollector();
        }

        private void themeChooser_night_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.stdAussehen = "night";
            Properties.Settings.Default.Save();
            PreviewWindow.Background = Brushes.Black;
            previewText.Foreground = Brushes.White;
            previewTitle.Foreground = Brushes.White;
            GarbageCollector();
        }

        private void btn_save_Click(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.Bildsichtbarkeit = (float)fontSizeValue;
            Properties.Settings.Default.Bildsichtbarkeit = backgroundVisibilityValue;
            Properties.Settings.Default.Hintergrundunschärfe = imageBlurValue;
            Properties.Settings.Default.Save();
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
    }
}