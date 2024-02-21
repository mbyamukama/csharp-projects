using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;


namespace StockApp
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (chkDisablePrinting.IsChecked.Value)
            {
                AppUtilities.session.IsPrintingEnabled = false;
            }
            else
            {
                AppUtilities.session.IsPrintingEnabled = true;
            }
            this.Close();
        }
    }
}
