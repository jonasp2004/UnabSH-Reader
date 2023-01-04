using System.Windows;
using System.Windows.Input;

namespace UnabSH_Reader {

    public partial class err_dialog : Window {
        public err_dialog(string message) {
            InitializeComponent();
            errorDesc.Text = message;
        }

        private void titleText_MouseDown(object sender, MouseButtonEventArgs e) {
            if (Mouse.LeftButton == MouseButtonState.Pressed) {
                window.DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
