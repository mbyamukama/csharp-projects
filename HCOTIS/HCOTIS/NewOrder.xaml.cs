using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HCOTIS
{
    /// <summary>
    /// Interaction logic for NewOrder.xaml
    /// </summary>
    public partial class NewOrder : Window
    {
        DateTime eventDate = DateTime.Now.AddDays(4), pickUpDate = DateTime.Now.AddDays(4);
        int numofguests = 0, cost = 0, deposit = 0, invoiceNum = 0;

        string clientName = "", phoneno = "", eventType = "", theme = "", size = "", shape = "", flavor = "", icing = "",
            color = "", narration = "", accessories = "", location = "", teller = "";

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
          dtEventDate.SelectedDate = DateTime.Now.AddDays(5);
          dtPickUpDate.SelectedDate = DateTime.Now.AddDays(5);

            //populate sizes
            List<String> sizes = new List<String>();
            StreamReader sr = new StreamReader("sizes.txt");
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                sizes.Add(line);
            }
            ObservableCollection<string> obs = new ObservableCollection<string>(sizes);
            cbxSize.ItemsSource = obs;

        }

        Bitmap cakeImage = new Bitmap(200, 200);

        private void labelFlavor_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            flavor = "";
            labelFlavor.Content = "";
        }

        public NewOrder()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Filter = "Image Files | *.bmp; *.jpg; *.png; *.tiff";
                if (ofd.ShowDialog() == true)
                {
                    imgCake.Source = new BitmapImage(new Uri(ofd.FileName));
                    cakeImage = new Bitmap(ofd.FileName);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddFlavor_Click(object sender, RoutedEventArgs e)
        {
            FlavorSelect flavorSel = new FlavorSelect();
            if (flavorSel.ShowDialog() == true)
            {
                flavor += flavorSel.SelectedFlavor + " ";
            }
            labelFlavor.Content = flavor;
        }


        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            clientName = txtClientName.Text;
            phoneno = txtPhoneNo.Text;
            eventType = cbxEventType.Text;
            theme = txtTheme.Text;
            size = cbxSize.Text;
            shape = cbxShape.Text;
            icing = cbxIcing.Text;
            color = cbxColor.Text;
            narration = txtNarration.Text;
            accessories = txtAccessories.Text;
            location = txtLocation.Text;
            teller = txtTeller.Text;

            eventDate = dtEventDate.SelectedDate.Value;
            pickUpDate = dtPickUpDate.SelectedDate.Value;


            try
            {
                Int32.TryParse(txtNumOfGuests.Text, out numofguests);
                Int32.TryParse(txtCost.Text, out cost);
                Int32.TryParse(txtDeposit.Text, out deposit);
                Int32.TryParse(txtInvoiceNum.Text, out invoiceNum);
            }
            catch(Exception ex)
            {
                MessageBox.Show("An error occurred with one of the fields that expects a number. Please review.\n DETAILS: " 
                    + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            try
            {
                int balance = cost - deposit;

                string nextId = FBDataHelper.GetNextId("ORDERID", "ORDERS");

                int affRows = FBDataHelper.AddOrder(nextId, invoiceNum, clientName, phoneno, eventType,
                    eventDate, numofguests, pickUpDate, theme, shape, size, flavor,
                    icing, color, narration, accessories, location, DateTime.Now, cost, deposit, balance, teller);

                if (affRows > 0)
                {
                    MessageBox.Show("The order was added successfully", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    cakeImage.Save(System.AppDomain.CurrentDomain.BaseDirectory + @"\images\" + nextId + ".jpg", System.Drawing.Imaging.ImageFormat.Png);

                }
                else
                {
                    MessageBox.Show("The order was not added due to an error. More details may follow.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }


        }
    }
}
