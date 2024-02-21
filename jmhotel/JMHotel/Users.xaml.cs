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
    public partial class Users : Window
    {
        SDataTable dt = null;
        public Users()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dt = JMFBDataHelper.GetUsers();
            dataGridItems.ItemsSource = dt.DefaultView;
            dataGridItems.Columns[1].Visibility = System.Windows.Visibility.Hidden;
            if (AppUtilities.Session.CLRLevel < 2)
            {
                dataGridItems.CanUserDeleteRows = false;
                dataGridItems.Columns[0].IsReadOnly = dataGridItems.Columns[1].IsReadOnly = true;
            }
        }

        private void dataGridItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
        }
 

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            int affRows = dt.UpdateSource();
            MessageBox.Show("The update affected " + affRows + " rows.", "UPDATE", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            new AddUser().Show();
        }
    }
}
