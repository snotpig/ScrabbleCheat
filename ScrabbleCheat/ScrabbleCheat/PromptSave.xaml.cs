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
    /// Interaction logic for PromptSave.xaml
    /// </summary>
    public partial class PromptSave : Window
    {
        public bool confirmed = false;

        public PromptSave()
        {
            InitializeComponent();
        }

        public PromptSave(string message) : this()
        {
            lblMsg.Content = message;
        }

        private void cmdYes_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
