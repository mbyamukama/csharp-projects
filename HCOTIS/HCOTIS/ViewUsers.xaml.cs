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

namespace HCOTIS
{
    /// <summary>
    /// Interaction logic for ViewUsers.xaml
    /// </summary>
    public partial class ViewUsers : Window
    {
        SDataTable dt = null;
        public ViewUsers()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridResults.ItemsSource = (dt = FBDataHelper.GetUsers()).DefaultView;
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (Session.clrLevel > 1)
            {
                new AddUser().Show();                            
            }
            else
            {
                MessageBox.Show("You do not have enough permissions to perform this action.", "ERROR");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (Session.clrLevel > 1)
            {
                int i=0;
                if ((i = dt.UpdateSource()) > 0)
                {
                    MessageBox.Show("Updated " + i + " items", "SUCCESS");
                }
            }
            else
            {
                MessageBox.Show("You do not have enough permissions to perform this action.", "ERROR");
            }
        }
    }
}
