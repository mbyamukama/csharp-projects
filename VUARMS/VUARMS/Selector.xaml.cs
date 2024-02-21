using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data;

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for Selector.xaml
    /// </summary>
    public partial class Selector : Window
    {
        public List<String> SelectedItems { get; set; }
        SDataTable courses = null;

        public Selector()
        {
            InitializeComponent();
            courses = VUARMSFBDataHelper.GetCourses();
            SelectedItems = new List<string>();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string sQuery = txtSearch.Text.Trim();
            try
            {
                EnumerableRowCollection<DataRow> qresult = from rtndRows in courses.AsEnumerable()
                                                           where rtndRows[0].ToString().ToUpper().Contains(sQuery.ToUpper()) ||
                                                           rtndRows[1].ToString().ToUpper().Contains(sQuery.ToUpper())
                                                           || rtndRows[2].ToString().ToUpper().Contains(sQuery.ToUpper())
                                                           select rtndRows;
                if (qresult.Count() > 0)
                {
                    dataGridResults.ItemsSource = qresult.AsDataView<DataRow>();
                }
                else
                {
                    MessageBox.Show("The search has not returned any results", "ERROR");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred. Searching in this information may not be enabled yet.\n DETAILS:" + ex.Message, "ERROR");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridResults.ItemsSource = courses.DefaultView;
            this.Title = "Select Module Name(s)";
        }

        private void dataGridResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedItems.Add((dataGridResults.SelectedItem as DataRowView).Row["COURSECODE"].ToString());
            this.DialogResult = true;
        }

        private void btnSelectOK_Click(object sender, RoutedEventArgs e)
        {
            var selection = dataGridResults.SelectedItems;
            foreach(DataRowView item in selection)
            {
                SelectedItems.Add(item.Row["COURSECODE"].ToString());
            }
            this.DialogResult = true;
        }
    }
}
