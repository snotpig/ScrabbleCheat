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
using Microsoft.Win32;

namespace Snotsoft.Validation
{
    /// <summary>
    /// Interaction logic for Validate.xaml
    /// </summary>
    public partial class Validate : Window
    {
        private const string alphaNum = "ABCDEFGHJKLMNPRSTUVWXY0123456789";
        private const int MAGIC_NUMBER = 418392823;
        private string regKeyName = "HKEY_CURRENT_USER\\Software\\Snotsoft\\";
        private string appName, user, machineName;
        private int regCode = 0;
        private string publicSeed = "";
        private string publicCode = "";
        private string date = "";
        private bool isValid = false;

        public Validate(string appName)
        {
            InitializeComponent();
            this.appName = appName;
            regKeyName = regKeyName + appName;
            user = Environment.UserName;
            if (user.Length > 6)
            {
                user = user.Substring(0, 6);
            }
            machineName = Environment.MachineName;
            if (machineName.Length > 6)
            {
                machineName = machineName.Substring(0, 6);
            }
            GetDate();
            GenerateRegCode();
            GeneratePublicSeed();
            GeneratePublicCode();
        }

        private void GetDate()
        {
            StringBuilder sb = new StringBuilder(DateTime.Now.ToShortDateString());
            sb.Remove(2, 1);
            sb.Remove(4, 3);
            date = sb.ToString();
        }

        private void GenerateRegCode()
        {
            regCode = StringToInt(appName + user + machineName);
        }

        private void GeneratePublicSeed()
        {
            publicSeed = IntToAlpha(StringToInt(user + machineName + date));
        }

        private void GeneratePublicCode()
        {
            publicCode = Convert(publicSeed);
        }

        private bool CheckRegCode()
        {
            int? value = (int?)Registry.GetValue(regKeyName, "reg", 0);
            if (value != regCode) return false;
            return true;
        }

        private bool CheckUserCode()
        {
            txtMsg1.Text = "Enter code for " + publicSeed + ".";
            ShowDialog();
            return isValid;
        }

        private string IntToAlpha(int n)
        {
            n = n & 0x3fffffff;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < 6; i++)
            {
                sb.Append(alphaNum[n & 31]);
                n = n >> 5;
            }

            return sb.ToString();
        }

        private int AlphaToInt(string str)
        {
            if (str.Length < 6) throw new Exception("Code too short");
            int n = 0;
            for (int i = 0; i < 6; i++)
            {
                n = n << 5;
                n = n + alphaNum.IndexOf(str[5 - i]);
            }
            return n;
        }

        private int StringToInt(string str)
        {
            int n = 0;
            for (int i = 0; i < str.Length; i++)
            {
                n = n << 1;
                n += (int)str[i];
            }
            return n;
        }

        private string Convert(string alpha)
        {
            int i = AlphaToInt(alpha);
            i = i ^ MAGIC_NUMBER;
            return IntToAlpha(i);
        }

        public bool CheckValid()
        {
            if (CheckRegCode()) return true;
            else return CheckUserCode();
        }

        private void CodeEntered()
        {
            if (txtCode.Text == publicCode)
            {
                txtCode.Visibility = Visibility.Collapsed;
                button.Visibility = Visibility.Collapsed;
                txtMsg2.Visibility = Visibility.Visible;
                txtMsg1.Text = "Code Correct!";
                txtMsg2.Text = "Close Window to Continue.";
                Registry.SetValue(regKeyName, "reg", regCode);
                isValid = true;
            }
            else
            {
                txtCode.Text = "";
                isValid = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CodeEntered();
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return) CodeEntered();
        }
    }
}
