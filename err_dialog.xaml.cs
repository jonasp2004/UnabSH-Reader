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

namespace UnabSH_Reader {
    /// <summary>
    /// Interaktionslogik für err_dialog.xaml
    /// </summary>
    public partial class err_dialog : Window {
        public err_dialog() {
            InitializeComponent();
            errorDesc.Text = Properties.Settings.Default.tempErrMsg;
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
