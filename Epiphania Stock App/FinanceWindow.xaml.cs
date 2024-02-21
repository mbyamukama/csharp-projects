using System.Windows;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for FinanceWindow.xaml
    /// </summary>
    public partial class FinanceWindow : Window
    {
        public FinanceWindow()
        {
            InitializeComponent();
        }

        private void btnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("SUPPLIERS").ShowDialog();
        }

        private void btnReports_Click(object sender, RoutedEventArgs e)
        {          
            new Reports().Show();
        }

        private void btnExpenses_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("EXPENSES").Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppUtilities.session.User.CLRlevel < 2)
            {
                btnReports.IsEnabled = false;
            }
        }

        private void btnPayments_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("PAYMENTS").Show();
        }

        private void btnPurchases_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("PURCHASES").Show();
        }
    }
}
