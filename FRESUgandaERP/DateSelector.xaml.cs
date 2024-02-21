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

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for DateSelector.xaml
    /// </summary>
    public partial class DateSelector : Window
    {
       public  DateTime startDate = DateTime.Now, endDate = DateTime.Now;
        public DateSelector()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
          //  this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            startDatePicker.SelectedDate = endDatePicker.SelectedDate = DateTime.Now;
        }

        private void startDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate = startDatePicker.SelectedDate.Value;
        }

        private void endDatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate = endDatePicker.SelectedDate.Value;
        }
    }
}
