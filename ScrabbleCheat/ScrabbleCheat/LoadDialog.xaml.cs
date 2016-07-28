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

namespace ScrabbleCheat
{
    /// <summary>
    /// Interaction logic for LoadDialog.xaml
    /// </summary>
    public partial class LoadDialog : Window
    {
        public SaveObject saveObject;
        public string name = "";
        private string appDataDir = "";
        string[] files;

        public LoadDialog()
        {
            InitializeComponent();
            appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + Constants.savePath;
//            Directory.CreateDirectory(@appDataDir);
            files = Directory.GetFiles(@appDataDir, "*.sav");

            for (int i = 0; i < files.Count(); i++)
            {
                string filename = files[i].Substring(appDataDir.Length, files[i].Length - appDataDir.Length - 4);
                listView.Items.Add(filename);
            }
        }

        public bool? ShowDlg()
        {
            if (files.Count() > 0)
            {
                Height = files.Count() * 22 + 40;
                if (Height > 390) Height = 390;
                Top = Top + 390 - Height;
                return ShowDialog();
            }
            else
            {
                MsgBox msgBox = new MsgBox();
                msgBox.Top = Top + 325;
                msgBox.Left = Left - 30;// 21;
                msgBox.ShowMessage("No saved games!");
                return false;
            }
        }

        private void Load()
        {
            if (listView.SelectedItems.Count == 1)
            {
                name = listView.SelectedItems[0].ToString();
                Serializer s = new Serializer();
                saveObject = s.DeSerializeObject(appDataDir + name + ".sav");
                DialogResult = true;
                Close();
            }
        }

        private void cmdLoad_Click(object sender, RoutedEventArgs e)
        {
            Load();
        }

        private void listView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Load();
        }

        private void cmdCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cmdDelete_Click(object sender, RoutedEventArgs e)
        {
            name = listView.SelectedItems[0].ToString();
            File.Delete(appDataDir + name + ".sav");
            listView.Items.Remove(name);
            listView.Items.Refresh();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

    }
}
