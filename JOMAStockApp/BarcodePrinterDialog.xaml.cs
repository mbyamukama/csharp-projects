using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.IO.Ports;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for MonthWindow.xaml
    /// </summary>
    public partial class BarcodePrinterDialog : Window
    {
        public Int32 Copies = 1;
        public Int32 LeftMargin, Spacing;
        public String PaperType = "45x55mm";
        public Boolean ViewPreview = false, CompressImages = false;

        public BarcodePrinterDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Copies = Int32.Parse(txtCopies.Text);
                LeftMargin = (int)sliderLeft.Value;
                Spacing = (int)sliderSpacing.Value;
                PaperType = (cbxPaperSize.SelectedItem as ComboBoxItem).Content.ToString();
                if (chkViewPreview.IsChecked==true) ViewPreview = true;
                if (chkCompressImages.IsChecked == true) CompressImages = true;
                this.DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            sliderLeft.Value = 10;
            sliderSpacing.Value = 80;
        }
    }
}
