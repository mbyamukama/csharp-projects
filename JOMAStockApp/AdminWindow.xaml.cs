using System.Windows;
using System.Windows.Input;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (GlobalSystemData.Session.CurrentUser.IsStandardUser())
            {
                btnReports.IsEnabled = false;
            }
    
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("USERS").Show();
        }

        private void btnLog_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("LOG").Show();  
        }

        private void btnRoyalty_Click(object sender, RoutedEventArgs e)
        {
            new EFRISWindow().Show();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("SETTINGS").Show();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                POSDisplaySettings pw = new POSDisplaySettings();
                if (pw.ShowDialog() == true)
                {
                    Properties.Settings.Default.DisplayPortName = pw.SelectedPort;
                    Properties.Settings.Default.Save();
                }

            }
            if (e.Key == Key.F4)
            {
                RawDataView pw = new RawDataView();
                pw.Show();

            }
            if (e.Key == Key.F5)
            {
                FBDataHelper.BackUp();
            }
        }
    }
}
