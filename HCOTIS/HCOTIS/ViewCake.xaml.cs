using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HCOTIS
{
    /// <summary>
    /// Interaction logic for ViewCake.xaml
    /// </summary>
    public partial class ViewCake : Window
    {

        string fileName = "";
        public ViewCake(string selectedOrderId)
        {
            InitializeComponent();
            fileName = selectedOrderId;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                imgCake.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + @"\images\" + fileName + ".jpg"));
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
