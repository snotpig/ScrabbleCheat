using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ScrabbleCheat
{
    /// <summary>
    /// Interaction logic for MsgBox.xaml
    /// </summary>
    public partial class MsgBox : Window
    {
        public MsgBox()
        {
            InitializeComponent();
        }

        public void ShowMessage(string message)
        {
            txt.Content = message;
            ShowDialog();
        }

        public void SetPos(double left, double top)
        {
            this.Left = left;
            this.Top = top;
        }

        private void cmdOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
