using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FRESUGERP.AppUtilities;

namespace FRESUgandaERP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AddClient : Window
    {
        string clientId, name, phone, village, gps, subcounty, district;
        int serviceLevel;
        DateTime conndate;
        List<int> energyLevels;
        Dictionary<int, string> stores = null;

        public AddClient()
        {
            InitializeComponent();
            energyLevels = new List<int>();
            stores = FRESUGDBHelper.GetEnergyStores();
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            //this is a direct-to-database addition: to be changed to use SDataTable
            int storekey = stores.Single(k => k.Value == (string)cbxEnergyStore.SelectedValue).Key;
            name = txtFullName.Text.Trim();

            string sLevel = cbxServLev.Text.Trim();
            if (sLevel == "S1") serviceLevel = 1; else if (sLevel == "S2") serviceLevel = 2;
            else if (sLevel == "S3") serviceLevel = 3; else if (sLevel == "S4") serviceLevel = 4;
            else if (sLevel == "S4+") serviceLevel = 5;

            phone = txtPhoneNum.Text.Trim();
            village = cbxVillage.Text.Trim();
            gps = txtGPSCoord.Text.Trim();
            conndate = (DateTime)dtPickerConnDate.SelectedDate;
            subcounty = cbxSubCounty.Text.Trim();
            district = cbxDistrict.Text.Trim();


            clientId = UtilityExtensions.As2Digits(storekey) + "." + serviceLevel + UtilityExtensions.As4Digits(FRESUGDBHelper.GetMaxClientID(" WHERE CLIENTID like '%" + storekey + "." + serviceLevel + "%'") + 1);
            if (name == "" || phone == "")
            {
                MessageBox.Show("Some fields are missing!", "ERROR");
            }
            else
            {
                bool success = FRESUGDBHelper.AddNewClient(clientId, name, phone, village, subcounty, district, gps, storekey, serviceLevel, conndate);           
                if (success)
                {
                    FRESUGDBHelper.AddLog(this.Title, UtilityExtensions.currentSession.USER, "New Client," + clientId + " added at " + DateTime.Now, UtilityExtensions.currentSession.STATION);
                    MessageBox.Show("Client Infomation Added.", "SUCCESS");
                }

                //clear all items
                txtFullName.Text = txtGPSCoord.Text = txtPhoneNum.Text = cbxVillage.Text = "";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbxEnergyStore.ItemsSource = new ObservableCollection<string>(stores.Values);
            cbxEnergyStore.SelectedValue = cbxEnergyStore.Items[0];
            dtPickerConnDate.SelectedDate = DateTime.Now;

            cbxDistrict.ItemsSource = new ObservableCollection<string>(FRESUGDBHelper.GetDistricts());
            cbxDistrict.SelectedIndex = 0;

        }

        private void cbxDistrict_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbxSubCounty.ItemsSource = new ObservableCollection<string>(FRESUGDBHelper.GetSubcounties(cbxDistrict.Text.Trim()));
            cbxSubCounty.SelectedIndex = 0;
        }

        private void cbxSubCounty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cbxVillage.ItemsSource = new ObservableCollection<string>(FRESUGDBHelper.GetVillages(cbxSubCounty.Text.Trim()));
            cbxVillage.SelectedIndex = 0;
        }
    }
}
