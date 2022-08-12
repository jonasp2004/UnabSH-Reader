using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace UnabSH_Reader
{

    public partial class Einstellungen : Window {
        public Einstellungen() {
            InitializeComponent();
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
            prefimgblur.Radius = backgroundblurSlider.Value / 2;
            Properties.Settings.Default.Hintergrundunschärfe = backgroundblurSlider.Value;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void backgroundopacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            prev_image.Opacity = backgroundopacitySlider.Value;
            Properties.Settings.Default.Bildsichtbarkeit = backgroundopacitySlider.Value;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void textSizeSlinder_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            previewText.FontSize = textSizeSlinder.Value;
            Properties.Settings.Default.Bildsichtbarkeit = textSizeSlinder.Value;
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void fontSelector_Varela_Checked(object sender, RoutedEventArgs e) {
            //Macht Probleme
            /*try {
                previewText.FontFamily = FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Varela Round");
                previewTitle.FontFamily = FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#Varela Round");
                errorPrompt.Visibility = Visibility.Collapsed;
            } catch {
                errorPrompt.Visibility = Visibility.Visible;
            }*/
            Properties.Settings.Default.stdSchriftart = "varela";
            Properties.Settings.Default.Save();
            GarbageCollector();
        }

        private void fontSelector_Arial_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.stdSchriftart = "arial";
            Properties.Settings.Default.Save();
            previewText.FontFamily = new FontFamily("Arial");
            previewTitle.FontFamily = new FontFamily("Arial");
            GarbageCollector();
        }

        private void fontSelector_Courier_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.stdSchriftart = "courier";
            Properties.Settings.Default.Save();
            previewText.FontFamily = new FontFamily("Courier New");
            previewTitle.FontFamily = new FontFamily("Courier New");
            GarbageCollector();
        }

        private void fontSelector_Comic_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.stdSchriftart = "comic";
            Properties.Settings.Default.Save();
            previewText.FontFamily = new FontFamily("Comic Sans MS");
            previewTitle.FontFamily = new FontFamily("Comic Sans MS");
            GarbageCollector();
        }

        private void fontSelector_Segoe_Checked(object sender, RoutedEventArgs e) {
            Properties.Settings.Default.stdSchriftart = "segoe";
            Properties.Settings.Default.Save();
            previewText.FontFamily = new FontFamily("Segoe UI");
            previewTitle.FontFamily = new FontFamily("Segoe UI");
            GarbageCollector();
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
    }
}
