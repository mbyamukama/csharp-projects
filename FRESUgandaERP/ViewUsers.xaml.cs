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
            dataGridResults.ItemsSource = (dt = FRESUGDBHelper.GetUsers()).DefaultView;
        }

        private void btnAddNew_Click(object sender, RoutedEventArgs e)
        {
            if (FRESUGERP.AppUtilities.UtilityExtensions.currentSession.CurrentCLR > 2)
            {
                new Users().Show();
            }
            else
            {
                MessageBox.Show("You do not have enough permissions to perform this action.", "ERROR");
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (FRESUGERP.AppUtilities.UtilityExtensions.currentSession.CurrentCLR > 2)
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
