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
using System.IO;
using Snotsoft.Controls;

namespace ScrabbleCheat
{
    /// <summary>
    /// Interaction logic for SaveDialog.xaml
    /// </summary>
    public partial class SaveDialog : Window
    {
        private const string NEWGAME = "New Game";
        public SaveObject saveObject;
        public string name = "";
        private string appDataDir = "";
        char[] invalidFileChars;

        public SaveDialog(string name)
        {
            InitializeComponent();
            appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Constants.savePath;
            txtName.Text = name;
            invalidFileChars = System.IO.Path.GetInvalidFileNameChars();
        }

        public void SetName(string name)
        {
            this.name = name;
            if (name == NEWGAME)
            {
                this.name = "";
            }
            else
            {
                this.name = name;
            }
            txtName.Text = this.name;
            FocusExtension.SetIsFocused(txtName, true);
            Keyboard.Focus(txtName);
            txtName.CaretIndex = txtName.Text.Length;
        }

        private bool IsValid(string name)
        {
            if (name.Length > 30) return false;
            foreach (char ch in invalidFileChars)
            {
                if (name.Contains(ch)) return false;
            }
            return true;
        }

        private void Save()
        {
            if (IsValid(txtName.Text))
            {
                name = txtName.Text;
                if (name == "")
                {
                    FocusExtension.SetIsFocused(txtName, true);
                    Keyboard.Focus(txtName);
                    return;
                }
                string fileName = appDataDir + name + ".sav";
                Serializer s = new Serializer();
                s.SerializeObject(fileName, saveObject);
                DialogResult = true;
            }
            else
            {
                MsgBox msgBox = new MsgBox();
                msgBox.SetPos(this.Left + 10, this.Top);
                msgBox.ShowMessage("Invalid name");
            }
        }

        private void cmdSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
//                cmdSave.Focus();
                Save();
            }
        }

    }
}
