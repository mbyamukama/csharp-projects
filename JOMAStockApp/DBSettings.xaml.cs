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

namespace StockApp
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class DBSettings : Window
    {
        public DBSettings()
        {
            InitializeComponent();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (txtServerIP.Text.Trim() != string.Empty && txtDirectory.Text.Trim() != string.Empty)
            {
                Properties.Settings.Default.ServerIP = txtServerIP.Text.Trim();
                Properties.Settings.Default.DBDirectory = txtDirectory.Text.Trim();
                if (txtSysDbaPass.Text != "")
                {
                    MessageBox.Show("SYSDBA password will be set.", "SYSDBA PASS CHANGE");
                    Properties.Settings.Default.SysDBAPass = txtSysDbaPass.Text;
                }
                Properties.Settings.Default.Save();
                MessageBox.Show("Settings saved", "SUCCESS");
                this.Close();
            }
            else
            {
                MessageBox.Show("One or more fields are empty. Settings cannot be saved.", "ERROR"); 
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtServerIP.Text= Properties.Settings.Default.ServerIP;
            txtDirectory.Text = Properties.Settings.Default.DBDirectory;
        }
    }
}
