using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO.Ports;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for MonthWindow.xaml
    /// </summary>
    public partial class POSDisplaySettings : Window
    {
        public String SelectedPort = "";

        public POSDisplaySettings()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedPort = cbxPorts.SelectedItem.ToString();
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxPorts.ItemsSource = new ObservableCollection<string>(SerialPort.GetPortNames());
        }
    }
}
