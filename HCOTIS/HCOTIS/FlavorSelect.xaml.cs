using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Data;
using System.ComponentModel;

namespace HCOTIS
{
    /// <summary>
    /// Interaction logic for FlavorSelect.xaml
    /// </summary>

    public class StringValue
    {
        public StringValue(string s)
        {
            _value = s;
        }
        public string Value { get { return _value; } set { _value = value; } }
        string _value;
    }

 


public partial class FlavorSelect : Window
    {
        public String SelectedFlavor { get; set; }

        public FlavorSelect()
        {
            InitializeComponent();
            SelectedFlavor = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<StringValue> flavors = new List<StringValue>();
            StreamReader sr = new StreamReader("flavors.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                flavors.Add(new StringValue(line));
            }
            dataGridFlavor.ItemsSource= flavors;
        }

        private void dataGridFlavor_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                SelectedFlavor = (dataGridFlavor.SelectedItem as StringValue).Value;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
