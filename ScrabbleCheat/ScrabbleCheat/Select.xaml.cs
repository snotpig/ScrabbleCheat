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
    /// Interaction logic for Select.xaml
    /// </summary>
    public partial class Select : Window
    {
        public char letter = ' ';
        private Point pt;
        private int col, row;
        private char[,] letters = new char[,]
        {   {'A','B','C','D','E','F'},
            {'G','H','I','J','K','L'},
            {'M','N','O','P','Q','R'},
            {'S','T','U','V','W','X'},
            {' ',' ','Y','Z',' ',' '}  };
 
        public Select()
        {
            InitializeComponent();
        }

        private void grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            pt = Mouse.GetPosition(grid);
            col = (int) pt.X / 25;
            row = (int)pt.Y / 25;
            letter = char.ToLower(letters[row, col]);
            if(letter != ' ')
            {
                Close();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString().Length > 1) return;
            letter = e.Key.ToString()[0];
            Close();
        }


    }
}
