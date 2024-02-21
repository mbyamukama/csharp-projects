using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using GenericUtilities;
using System.Collections.ObjectModel;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for InventoryWindow.xaml
    /// </summary>
    public partial class DataViewer : Window
    {
        string param = "";
        DataTable dt = new DataTable();
        public DataRowView SelectedItem = null;

        public DataViewer(string param)
        {
            InitializeComponent();
            this.param = param;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (param == "COURSES")
            {
                dt = VUARMSFBDataHelper.GetCourses();
            }

            if (param == "DEAN")
            {
                dt.Columns.Add("VUREFNO");
                dt.Columns.Add("FULLNAME");
                dt.Columns.Add("COURSE");
                dt.Columns.Add("GPA");

                DataTable students = VUARMSFBDataHelper.GetAllStudents();
                foreach (DataRow row in students.Rows)
                {
                    string refNo = row["VUREFNO"].ToString();
                    DataTable refResults = VUARMSFBDataHelper.GetResults(refNo, true, "ALL");
                    float CGPA = AppUtilities.GetCGPA(refResults);
                    if (CGPA >= 4.40)
                    {
                        dt.Rows.Add(refNo, row["FULLNAME"], row["PROGRAMME"], CGPA);
                    }
                }
                DataView dv = dt.DefaultView;
                dv.Sort = "CGPA DESC";
                dt = dv.ToTable();
            }

            dataGridResults.ItemsSource = dt.DefaultView;
            dataGridResults.IsReadOnly = true;
        }
        private void txtItemName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string sQuery = txtSearch.Text.Trim();
            try
            {
                EnumerableRowCollection<DataRow> qresult = from rtndRows in dt.AsEnumerable()
                                                           where rtndRows[0].ToString().ToUpper().Contains(sQuery.ToUpper()) ||
                                                           rtndRows[1].ToString().ToUpper().Contains(sQuery.ToUpper())
                                                           || rtndRows[2].ToString().ToUpper().Contains(sQuery.ToUpper())
                                                           select rtndRows;
                if (qresult.Count() > 0)
                {
                    dataGridResults.ItemsSource = qresult.AsDataView<DataRow>();
                    lblCount.Content = qresult.Count() + " items returned";
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



        private void dataGridResults_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(DateTime))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "{0:dd/MM/yyyy}";
            }
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void menuPrint_Click(object sender, RoutedEventArgs e)    
        {
            if (param == "DEAN")
            {
                FixedPage page = new FixedPage();
                Grid grid = Utilities.LabelTable(dt, new Thickness(1), 12, "Times New Roman", 30, new int[] { 100, 300, 50, 50 }, new string[] { "L", "L", "L", "L"});
                grid.Margin = new Thickness(30, 150, 0, 0);
                page.Children.Add(grid);

                PageContent pc = new PageContent();
                pc.Child = page;
                FixedDocument doc = new FixedDocument();
                doc.Pages.Add(pc);

                PrintDialog pd = new PrintDialog();
                if (pd.ShowDialog() == true)
                {
                    pd.PrintDocument(doc.DocumentPaginator, "Deans List");
                }
            }
        }

        private void dataGridResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            SelectedItem = dataGridResults.SelectedItem as DataRowView;
            this.DialogResult = true;
        }

    }
}