using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using GenericUtilities;
using FirebirdSql.Data.FirebirdClient;
using System.IO;

namespace GroupMarkAssigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DataTable dt = null;
        FileInfo[] csvFiles = null;
        FileInfo currFile = null;

        public MainWindow()
        {
            InitializeComponent();
            dt = new DataTable();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                DirectoryInfo dinfo = new DirectoryInfo(Directory.GetCurrentDirectory());
                csvFiles = dinfo.GetFiles("*.csv");
                List<string> fnames = (from files in csvFiles select files.Name).ToList();
             
                cbxModule.ItemsSource =
                new System.Collections.ObjectModel.ObservableCollection<string>(fnames);

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR");
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                dt.AcceptChanges();
                Utilities.WriteCSVFile(dt, currFile.FullName); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error. " + ex.Message);
            }
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string[] items = txtFilter.Text.ToUpper().Split(new char[] { ',' });
                var results = from myRows in dt.AsEnumerable()
                              where myRows["REGNO"].ToString().ToUpper().ContainsAnyOf(items) ||
                               myRows["FULLNAME"].ToString().ToUpper().ContainsAnyOf(items)
                              select myRows;
                int count = 0;
                if ((count = results.Count()) > 0)
                {
                    dtGridResults.ItemsSource = results.AsDataView<DataRow>();
                    label1.Content = "Student Count:\t" + count;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR");
            }

        }

        private void cbxModule_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dt = new DataTable();
            string selection = "";
            try
            {
                selection = cbxModule.SelectedValue.ToString();
                foreach (FileInfo f in csvFiles)
                {
                    if(f.FullName.Contains(selection))
                    {
                        dt = Utilities.ReadCSVFile(f.FullName);
                        currFile = f;
                        break;
                    }
                }
                dtGridResults.ItemsSource = dt.DefaultView;
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR");
            }           

            
            label1.Content = "Student Count:\t" + dt.Rows.Count;
            dtGridResults.ItemsSource = dt.DefaultView;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.Filter = "CSV File|*.csv";
            if (sfd.ShowDialog(this) == true)
            {
                try
                {
                    if (dt.Rows.Count > 0)
                    {
                        Utilities.WriteCSVFile(dt, sfd.FileName + ".csv");
                    }
                    else
                        MessageBox.Show("There are no rows to export.", "ERROR");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occured.\n DETAILS: " + ex.Message, "ERROR");
                }
            }
        }

       
    }
}
