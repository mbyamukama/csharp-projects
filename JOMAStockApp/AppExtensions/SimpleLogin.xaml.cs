using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Reflection;

namespace StockApp.AppExtensions
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class SimpleLogin : Window
    {
        public String Password;
        public SimpleLogin()
        {
            InitializeComponent();
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
            Password = "";

        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Password = this.txtPassword.Password;
                this.DialogResult = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtPassword.Focus();
        }

    }
}
