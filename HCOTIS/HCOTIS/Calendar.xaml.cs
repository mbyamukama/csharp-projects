using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Collections.ObjectModel;
using System.Data;

namespace HCOTIS
{
   
    public partial class Calendar: Window
    {
        public Calendar()
        {
            InitializeComponent();
        }

 

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DataTable dt = FBDataHelper.GetUpcomingEvents();
                dt.Columns.Add("UPCOMINGON");
                System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();

                foreach (DataRow row in dt.Rows)
                {
                    DateTime pastdate = DateTime.Parse(row["EVENTDATE"].ToString());
                    row["UPCOMINGON"] = mfi.GetMonthName(pastdate.Month) + " " + pastdate.Day + " (" + pastdate.AddYears(1).DayOfWeek + " )";
                }
                dataGridCalendar.ItemsSource = dt.DefaultView;
                dataGridCalendar.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void dataGridCalendar_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "{0:dd/MM/yyyy}";
            }
        }
    }
}
