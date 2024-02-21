using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JMHotel
{
    /// <summary>
    /// Interaction logic for Items.xaml
    /// </summary>
    public partial class Items : Window
    {
        SDataTable sdt = null;
        public Items()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sdt = JMFBDataHelper.GetItems();
            dataGridItems.ItemsSource = sdt.DefaultView;
            this.WindowState = System.Windows.WindowState.Maximized;

            if (AppUtilities.Session.CLRLevel < 2)
            {
                dataGridItems.CanUserDeleteRows = false;
                dataGridItems.Columns[0].IsReadOnly = dataGridItems.Columns[1].IsReadOnly = true;
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int affRows = sdt.UpdateSource();
            MessageBox.Show("The update affected " + affRows + " rows.", "UPDATE", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            string dName = txtSearch.Text.Trim().ToUpper();
            try
            {
                EnumerableRowCollection<DataRow> qresult = from rtndRows in sdt.AsEnumerable()
                                                           where
                                                           rtndRows[0].ToString().ToUpper().Contains(dName)
                                                           || rtndRows[1].ToString().ToUpper().Contains(dName)
                                                           select rtndRows;
                if (qresult.Count() > 0)
                {
                    dataGridItems.ItemsSource = qresult.AsDataView<DataRow>();
                }
                else
                {
                    MessageBox.Show("The search has not returned any results", "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
