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
using System.Windows.Media.Animation;

namespace ScrabbleCheat
{
    /// <summary>
    /// Interaction logic for AboutBox.xaml
    /// </summary>
    public partial class AboutBox : Window
    {
        private TimeSpan DURATION = TimeSpan.FromMilliseconds(200);
        DoubleAnimation animationLeft = new DoubleAnimation();
        DoubleAnimation animationTop = new DoubleAnimation();
        DoubleAnimation animationWidth = new DoubleAnimation();
        DoubleAnimation animationHeight = new DoubleAnimation();
        bool isOpening = true;

        public AboutBox()
        {
            InitializeComponent();
            animationLeft.Duration = DURATION;
            animationTop.Duration = DURATION;
            animationWidth.Duration = DURATION;
            animationHeight.Duration = DURATION;
            animationLeft.From = 250;
            animationTop.From = 413;
            animationWidth.From = 0;
            animationHeight.From = 0;
            animationLeft.To = 260;
            animationTop.To = 150;
            animationWidth.To = 180;
            animationHeight.To = 210;

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            txtVersion.Text = "Version " + assembly.GetName().Version; 
            txtCopyright.Text = "© Adam Sands";

            animationLeft.Completed += new EventHandler(animationLeft_Completed);

            disc.BeginAnimation(LeftProperty, animationLeft);
            disc.BeginAnimation(TopProperty, animationTop);
            disc.BeginAnimation(WidthProperty, animationWidth);
            disc.BeginAnimation(HeightProperty, animationHeight);
        }

        void animationLeft_Completed(object sender, EventArgs e)
        {
            if (isOpening)
            {
                txtProgram.Visibility = System.Windows.Visibility.Visible;
                txtVersion.Visibility = System.Windows.Visibility.Visible;
                txtCopyright.Visibility = System.Windows.Visibility.Visible;
            }
            else Close();
        }

        private void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isOpening = false;
            animationLeft.From = 260;
            animationTop.From = 150;
            animationWidth.From = 180;
            animationHeight.From = 210;
            animationLeft.To = 250;
            animationTop.To = 413;
            animationWidth.To = 0;
            animationHeight.To = 0;

            txtProgram.Visibility = System.Windows.Visibility.Hidden;
            txtVersion.Visibility = System.Windows.Visibility.Hidden;
            txtCopyright.Visibility = System.Windows.Visibility.Hidden;

            disc.BeginAnimation(LeftProperty, animationLeft);
            disc.BeginAnimation(TopProperty, animationTop);
            disc.BeginAnimation(WidthProperty, animationWidth);
            disc.BeginAnimation(HeightProperty, animationHeight);
        }
    }
}
