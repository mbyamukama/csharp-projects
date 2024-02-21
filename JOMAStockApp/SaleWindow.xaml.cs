using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using FirebirdSql.Data.FirebirdClient;
using StockApp.AppExtensions;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for SaleWindow.xaml
    /// </summary>
    public partial class SaleWindow : Window
    {
        DataTable dtStock = null, dtItemsBought = null, settings = null;
        string itemName = "", barcode = "";
        int qty = 0;
        int availQty = 0;
        int saleprice = 0;
        int totAmtDue = 0;
        int origAmtDue = 0;
        int itemAmtDue = 0;
        int amtPaid = 0;
        int balance = 0;
        int pointsEarned = 0;
        int discount = 0;
        double pointsPerShs = 0;
        DataRow customerData = null;

        int amtRedeemed = 0, pointsUsed = 0, redeemRate = 0, currentPoints = 0;
        bool redeemInSale = false;
        enum LoyaltyIntention { isNone=0, isRedeem, isEarn};
        LoyaltyIntention loyaltyIntention = LoyaltyIntention.isNone;

        public string paytype = "CASH", authcode = ""; //default

        public SaleWindow()
        {
            InitializeComponent();        
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                dtStock = FBDataHelper.GetStock(FBDataHelper.StockType.Basic, DateTime.Now, DateTime.Now);
                settings = FBDataHelper.GetSettings();
                GlobalSystemData.Settings = settings;

                redeemRate = Convert.ToInt32((from myRows in settings.AsEnumerable()  
                                              where myRows.Field<string>("ITEM") == "RedeemRatio"
                                              select myRows).ElementAt(0)["VALUE1"]);

                dtItemsBought = new DataTable();
                dtItemsBought.Columns.Add("BARCODE");
                dtItemsBought.Columns.Add("ITEMNAME");
                dtItemsBought.Columns.Add("QUANTITY");
                dtItemsBought.Columns.Add("SELLPPU");
                dtItemsBought.Columns.Add("AMOUNTDUE");
                dtItemsBought.Columns.Add("AVAILQTY");


                dtItemsBought.RowDeleted += dtItemsBought_RowDeleted;
                dtItemsBought.ColumnChanged += dtItemsBought_ColumnChanged;

                dtGridItemsBought.ItemsSource = dtItemsBought.AsDataView();


                // original code in Window Loaded is below: Above code was in constructor

                this.Title = "Logged on as: " + GlobalSystemData.Session.CurrentUser.UserName;

                if (GlobalSystemData.Session.CurrentUser.IsStandardUser())
                {
                    dtGridItemsBought.Columns[5].Visibility = fileMenu.Visibility = Visibility.Hidden;
                    GlobalSystemData.Session.OpenSaleWindows += 1;
                }

                dtGridItemsBought.FontSize = 12;
                txtItemName.Focus();
                this.WindowState = WindowState.Maximized;

                string ratio = (from myRows in settings.AsEnumerable()
                                where myRows.Field<string>("ITEM") == "PointCashRatio"
                                select myRows).ElementAt(0)["VALUE1"].ToString();

                pointsPerShs = 1.0 / Convert.ToDouble(ratio);

                foreach (DataGridColumn col in dtGridItemsBought.Columns)
                {
                    if (col.Header.ToString() != "QUANTITY")
                    {
                        col.IsReadOnly = true;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("The following error occurred:\n " + ex.ToString(), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            //EPOS Display has its own error handling
            if (EPOSDisplay.IsAlreadyInitialized)
            {
                EPOSDisplay.ShowWelcomeMessage();
            }
            else
            {
                EPOSDisplay.Initialize(Properties.Settings.Default.DisplayPortName);
            }

        }
        void dtItemsBought_ColumnChanged(object sender, DataColumnChangeEventArgs e)
        {
            if (e.Column.ColumnName == "QUANTITY")
            {
                e.Row.AcceptChanges();
                qty = Convert.ToInt32(e.Row["QUANTITY"]);
                availQty = Convert.ToInt32(e.Row["AVAILQTY"]);
                saleprice = Convert.ToInt32(e.Row["SELLPPU"]);
                totAmtDue = 0;

                if (qty <= availQty)
                {
                    e.Row["AMOUNTDUE"] = itemAmtDue = qty * saleprice;

                    foreach (DataRow r in dtItemsBought.Rows)
                    {
                        totAmtDue += (int)Convert.ToDouble(r["AMOUNTDUE"]);
                    }
                    origAmtDue = totAmtDue;
                    lblAmountDue.Content = (totAmtDue = GlobalSystemData.RoundUpToNearestNote(totAmtDue)).ToString("N0");

                    if (amtPaid > 0)  //pre-sale edit on row
                    {
                        lblBalance.Content = (balance = amtPaid - totAmtDue).ToString("N0");
                    }

                    EPOSDisplay.Clear();
                    EPOSDisplay.WriteToUpperLine(itemName.PadRight(100).Substring(0, 11) + " : " + GlobalSystemData.RoundUpToNearestNote(itemAmtDue).ToString("N0"));
                    EPOSDisplay.WriteToLowerLine("TOTAL : " + GlobalSystemData.RoundUpToNearestNote(totAmtDue).ToString("N0"));
                }
                else
                {
                    MessageBox.Show("The available quantity for this item is only " + availQty, "LOW QUANTITY ALERT", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Row["QUANTITY"] = 1;
                }
                txtItemName.Focus();
            }
        }


        void dtItemsBought_RowDeleted(object sender, DataRowChangeEventArgs e)
        {
            try
            {
                dtItemsBought.AcceptChanges();
                totAmtDue = 0;
                foreach (DataRow r in dtItemsBought.Rows)
                {
                    totAmtDue += (int)Convert.ToDouble(r["AMOUNTDUE"]);
                }
                origAmtDue = totAmtDue;
                lblAmountDue.Content = (totAmtDue = GlobalSystemData.RoundUpToNearestNote(totAmtDue)).ToString("N0");

                if (amtPaid > 0)  //pre-sale edit on row
                {
                    lblBalance.Content = (balance = amtPaid - totAmtDue).ToString("N0");
                }

                EPOSDisplay.Clear();
                DataRow lastRow = dtItemsBought.Rows[dtItemsBought.Rows.Count-1];

                EPOSDisplay.WriteToUpperLine(lastRow["ITEMNAME"].ToString().PadRight(100).Substring(0, 11) + " : " +
                    GlobalSystemData.RoundUpToNearestNote(Convert.ToInt32(lastRow["AMOUNTDUE"])).ToString("N0"));
                EPOSDisplay.WriteToLowerLine("TOTAL : " + totAmtDue.ToString("N0"));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + ex.Message);
            }
        }

        private void txtAmtPaid_TextChanged(object sender, TextChangedEventArgs e)
        {
            string amtPaidText = txtAmtPaid.Text.Trim();
            if (amtPaidText != "")
            {
                try
                {
                    amtPaid = Int32.Parse(amtPaidText);
                    balance = (amtPaid + amtRedeemed) - totAmtDue;

                    if (balance < 0)
                    {
                        lblBalance.Foreground = System.Windows.Media.Brushes.Red;
                    }
                    else lblBalance.Foreground = System.Windows.Media.Brushes.Black;

                    lblBalance.Content = balance.ToString("N0");
                }
                catch (FormatException ex)
                {
                    if (paytype != "VISA")           //hard coded. VISA payment throws exception when field is filled
                    {
                        MessageBox.Show("The Amount field only takes numeric characters.\n" + ex.Message, "ALERT", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void txtAmtPaid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (dtItemsBought.Rows.Count < 1)
                {
                    MessageBox.Show("No items have been entered. The sale cannot proceed.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }

              //  try
                {
                    if (txtAmtPaid.Text != "")
                    {
                        balance = (amtPaid + amtRedeemed) - totAmtDue;
                        lblBalance.Content = balance.ToString("N0");
                        if ((balance >= 0 && paytype == "CASH") || paytype == "VISA")
                        {
                            string saleid = "";
                            FbTransaction trans = null;

                            if (customerData != null)
                            {
                                pointsEarned = (int)(totAmtDue * pointsPerShs);
                            }
                            if (loyaltyIntention==LoyaltyIntention.isRedeem)
                            {
                                pointsEarned = 0;
                            }

                            bool success = FBDataHelper.AddSale(totAmtDue, amtPaid, balance,
                                GlobalSystemData.Session.CurrentUser.UserName, out saleid, customerData == null ? "" : customerData[0].ToString(),
                                paytype, authcode, pointsEarned, amtRedeemed, out trans);

                            //now we have a single transaction object to use for all detailed sale entries
                            if (success)
                            {
                                foreach (DataRow row in dtItemsBought.Rows)
                                {
                                    success &= FBDataHelper.AddDetailedSale(saleid, row["BARCODE"].ToString(), row["ITEMNAME"].ToString(),
                                        Convert.ToInt32(row["QUANTITY"]), Convert.ToInt32(row["AMOUNTDUE"]), trans);
                                    if (!success)
                                    {
                                        break;
                                    }

                                }
                            }
                            //everything is successful
                            if (success)
                            {
                                trans.Commit();
                                currentPoints = 0;
                                if (customerData != null)          //a card was scanned: intetion to get or use points
                                {
                                    bool result = false;
                                    if (loyaltyIntention==LoyaltyIntention.isEarn)         //no redeeming in this sale; points will be earned
                                    {
                                        pointsEarned = (int)(totAmtDue * pointsPerShs);
                                        result = FBDataHelper.SetPoints(customerData["CUSTOMERID"].ToString(), pointsEarned, saleid);
                                    }
                                    if (loyaltyIntention == LoyaltyIntention.isRedeem)               //customer is redeeming points
                                    {
                                        result = FBDataHelper.SetPoints(customerData["CUSTOMERID"].ToString(), -1 * pointsUsed, saleid);
                                        pointsEarned = 0;   
                                    }

                                    if (!result) MessageBox.Show("An error occurred updating the customer loyalty data.",
                                        "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

                                    customerData = FBDataHelper.GetCustomerData(GlobalSystemData.Session.CurrentCustomer);  //refresh
                                    currentPoints = Convert.ToInt32(customerData["POINTS"]);
                                    
                                }
                                else
                                {
                                    pointsEarned = currentPoints = 0;
                                }

                                ReceiptDataModel rDataModel = new ReceiptDataModel(dtItemsBought, paytype, amtPaid, amtRedeemed, totAmtDue, discount, balance,
                                    saleid, pointsEarned, pointsUsed, currentPoints,
                                    currentPoints * redeemRate, DateTime.Now.ToString("dd/MM/yyyy H:mm:ss"), GlobalSystemData.Session.CurrentUser.UserName);

                                FixedDocument fdoc = DocumentHelper.GenerateReceipt(rDataModel);
                                PrintDialog pd = new PrintDialog();
                                string printQueueName = pd.PrintQueue.FullName;

                                //cash drawer kicker

                                EPOSDisplay.Clear();
                                EPOSDisplay.WriteToUpperLine("CHANGE DUE:");
                                EPOSDisplay.WriteToLowerLine(GlobalSystemData.RoundUpToNearestNote(balance).ToString("N0"));

                                pd.PrintDocument(fdoc.DocumentPaginator, "Receipt for sale: " + saleid);
                                MessageBox.Show("The transaction completed successfully.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);

                                //attempt backup receipt
                                MemoryStream stream = new MemoryStream();
								BinaryFormatter bf = new BinaryFormatter();
                                bf.Serialize(stream, rDataModel);
								FBDataHelper.AddReceipt(saleid, stream);


                                dtItemsBought.Clear();
                                customerData = null; //clear loyalty information
                                GlobalSystemData.Session.CurrentCustomer = null;
                                discount = origAmtDue = totAmtDue = amtPaid = balance = pointsEarned = pointsUsed = currentPoints = amtRedeemed = 0;
                                txtDiscount.Clear();
                                txtAmtPaid.Clear();
                                txtAmtRedeemed.Clear();
                                lblAmountDue.Content = lblBalance.Content = 0;
                                txtItemName.Focus();
                                loyaltyIntention = LoyaltyIntention.isNone;
                                paytype = "CASH";  //default
                                txtDiscount.IsEnabled = false;
                                authcode = "";
                                this.Title = "LOGGED ON AS:  " + GlobalSystemData.Session.CurrentUser.UserName;

                                EPOSDisplay.Clear();
                                EPOSDisplay.WriteToUpperLine("WELCOME TO JOMA");
                                EPOSDisplay.WriteToLowerLine("SUPERMARKET");
                            }
                            else
                            {
                                trans.Rollback();
                                MessageBox.Show("The transaction failed and the sale was not added.", "SALE FAILURE");
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("The payment amount has not been entered.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                /*catch (Exception ex)
                {
                    MessageBox.Show("An error occurred during this sale.\nDETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }*/
            }
        }
            

        private void viewSalesMenu_Click(object sender, RoutedEventArgs e)
        {        
           new DataViewer("SALES").Show();
        }

        private void exitMenu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void txtItemName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && txtItemName.Text.Trim() != "")
            {
                try
                {
                    DataRow stockrow = (from myrows in dtStock.AsEnumerable()
                                   where myrows.Field<string>("BARCODE") == txtItemName.Text.Trim()
                                   select myrows).ElementAt(0);

                    barcode = stockrow["BARCODE"].ToString();
                    itemName = stockrow["ITEMNAME"].ToString();
                    saleprice = Convert.ToInt32(stockrow["SELLPPU"]);
                    availQty = Convert.ToInt32(stockrow["QUANTITY"]);

                    qty = 1;
                    if (qty <= availQty)
                    {
                        DataRow row = dtItemsBought.NewRow();
                       
                        row["BARCODE"] = barcode;
                        row["ITEMNAME"] = itemName;                    
                        row["SELLPPU"] = saleprice;
                        row["AMOUNTDUE"] = itemAmtDue = qty * saleprice;
                        row["AVAILQTY"] = availQty;
                        dtItemsBought.Rows.Add(row);

                        row["QUANTITY"] = qty;  //change qty after adding row

                        dtGridItemsBought.ScrollIntoView(dtGridItemsBought.Items[dtGridItemsBought.Items.Count - 1]);
                       
                        //get total in rows
                        totAmtDue = 0;
                        foreach (DataRow r in dtItemsBought.Rows)
                        {
                            totAmtDue += (int)Convert.ToDouble(r["AMOUNTDUE"]);
                        }
                        origAmtDue = totAmtDue;
                        lblAmountDue.Content = (totAmtDue = GlobalSystemData.RoundUpToNearestNote(totAmtDue)).ToString("N0");

                        EPOSDisplay.Clear();
                        EPOSDisplay.WriteToUpperLine(itemName.PadRight(100).Substring(0, 11) + " : " + GlobalSystemData.RoundUpToNearestNote(itemAmtDue).ToString("N0"));
                        EPOSDisplay.WriteToLowerLine("TOTAL : " + GlobalSystemData.RoundUpToNearestNote(totAmtDue).ToString("N0"));
                    }
                    else
                    {
                        MessageBox.Show("The available quantity for this item is " + availQty, "LOW QUANTITY", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    txtItemName.Text = "";                
                    txtItemName.Focus();

                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred.\nDETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
           
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.F1)
                {
                    //search items manually
                    ItemSearch window = new ItemSearch();
                    window.ShowDialog();
                    DataRow row = window.SelectedRow;
                    if (row != null)
                    {
                        txtItemName.Text = barcode = row["BARCODE"].ToString();
                        itemName = row["ITEMNAME"].ToString();
                        saleprice = Convert.ToInt32(row["SELLPPU"]);
                        availQty = Convert.ToInt32(row["QUANTITY"]);

                        //force key press

                        Key key = Key.Enter;                   // Key to send
                        var target = txtItemName;    // Target element
                        RoutedEvent routedEvent = Keyboard.KeyDownEvent; // Event to send

                        target.RaiseEvent(new KeyEventArgs(Keyboard.PrimaryDevice,
                            PresentationSource.FromVisual(target), 0, key)
                        { RoutedEvent = routedEvent }
                        );
                    }
                }

                if (e.Key == Key.F2)
                {
                    new SaleWindow().Show();
                }

                if (e.Key == Key.F3)
                {
                    //scan royalty
                    ScanWindow window = new ScanWindow();
                    if (window.ShowDialog() == true)
                    {
                        customerData = FBDataHelper.GetCustomerData(GlobalSystemData.Session.CurrentCustomer);
                        if (customerData != null)
                        {
                            this.Title += "               LOYALTY CUSTOMER: " + customerData["CNAME"] + "             " +
                                " POINTS: " + customerData[3];
                            EPOSDisplay.Clear();
                            EPOSDisplay.WriteToUpperLine("LOYALTY CUSTOMER");
                            EPOSDisplay.WriteToLowerLine(customerData["CNAME"].ToString().PadRight(50).Substring(0, 20));
                            loyaltyIntention = LoyaltyIntention.isEarn;
                        }
                    }
                }
                if (e.Key == Key.F4)
                {
                    //refresh stock
                    dtStock = FBDataHelper.GetStock(FBDataHelper.StockType.Basic, DateTime.Now, DateTime.Now);
                    foreach (DataRow row in dtItemsBought.Rows)
                    {
                        row["AVAILQTY"] = FBDataHelper.GetAvailableQty(row["BARCODE"].ToString());
                    }
                    MessageBox.Show("The database stock quantities have been updated. Please rescan item.", "UPDATE", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                if (e.Key == Key.F5)
                {
                    SimpleLogin simpleLogin = new SimpleLogin();
                    if (simpleLogin.ShowDialog() == true)
                    {
                        //get all credentials
                        DataTable admins = FBDataHelper.GetAdmins();
                        bool auth = false;
                        string user = "";
                        foreach (DataRow row in admins.Rows)
                        {
                            if (Hasher.ValidatePassword(simpleLogin.Password, row["HPASS"].ToString()))
                            {
                                auth = true;
                                user = row["USERNAME"].ToString();
                                break;
                            }
                        }

                        if (auth)
                        {
                            FBDataHelper.AddLog(DateTime.Now, "DISCOUNT GIVEN BY " + GlobalSystemData.Session.CurrentUser.UserName +
                                " USING PASSWORD OF " + user);
                            MessageBox.Show("Password Authenticated. Admin is " + user, "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                            txtDiscount.IsEnabled = true;
                            txtDiscount.Text = "0";
                        }
                        else
                            MessageBox.Show("Incorrect Password.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                //VISA PAYMENTS
                if (e.Key == Key.F6)
                {
                    txtAmtPaid.Clear();
                    VisaWindow vs = new VisaWindow(totAmtDue);
                    if (vs.ShowDialog() == true)
                    {
                        authcode = vs.AuthCode;
                        paytype = "VISA";
                        txtAmtPaid.Text = totAmtDue.ToString("N0");
                        KeyEventArgs ef = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Enter);
                        txtAmtPaid_KeyDown(this, ef);
                    }
                }
                if (e.Key == Key.F7)
                {
                    //redeem points
                    if (GlobalSystemData.Session.CurrentCustomer != null)
                    {
                        bool auth = false;
                        string user = "";
                        SimpleLogin simpleLogin = new SimpleLogin();
                        if (simpleLogin.ShowDialog() == true)
                        {
                            //get all credentials
                            DataTable admins = FBDataHelper.GetAdmins();
                            foreach (DataRow row in admins.Rows)
                            {
                                if (Hasher.ValidatePassword(simpleLogin.Password, row["HPASS"].ToString()))
                                {
                                    auth = true;
                                    user = row["USERNAME"].ToString();
                                    break;
                                }
                            }
                        }
                        if (auth)
                        {
                            FBDataHelper.AddLog(DateTime.Now, "REDEEM WINDOW ACTIVATED " + GlobalSystemData.Session.CurrentUser.UserName +
                                " USING PASSWORD OF " + user);
                            MessageBox.Show("Password Authenticated. Admin is "+user, "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                            DataRow dr = FBDataHelper.GetCustomerData(GlobalSystemData.Session.CurrentCustomer);
                            RedeemWindow rdw = new RedeemWindow(dr, redeemRate, totAmtDue);
                            if (rdw.ShowDialog() == true)
                            {
                                txtAmtRedeemed.Text = (amtRedeemed = rdw.AuthorizedAmount).ToString();
                                pointsUsed = rdw.PointsUsed;
                                balance = (amtPaid + amtRedeemed) - totAmtDue;
                                lblBalance.Content = balance.ToString("N0");
                                loyaltyIntention = LoyaltyIntention.isRedeem;
                                txtAmtPaid.Text = "0";
                                txtAmtPaid.Focus();
                            }
                        }
                        else
                            MessageBox.Show("Incorrect Password.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                        MessageBox.Show("No customer information was found.\n Please scan card using F3.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.\nDETAILS: " + ex.Message);
            }        
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to exit?","CONFIRM",MessageBoxButton.YesNo, MessageBoxImage.Question)== MessageBoxResult.Yes)
            {
                if (GlobalSystemData.Session.CurrentUser.IsStandardUser())
                {
                    if (GlobalSystemData.Session.OpenSaleWindows == 1)
                    {
                        GlobalSystemData.Session.Log += "\n Session End: " + DateTime.Now;
                        FBDataHelper.AddLog(DateTime.Now, GlobalSystemData.Session.Log);
                        FBDataHelper.CloseConnection();
                    }
                    else
                    {
                        GlobalSystemData.Session.OpenSaleWindows -= 1;
                    }
                }
                
            }
            else
                e.Cancel = true;
            
        }

        private void dtGridItemsBought_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyType == typeof(Int32) || e.PropertyType == typeof(Int64))
            {
                (e.Column as DataGridTextColumn).Binding.StringFormat = "N0";

            }
        }

        private void chkSalesMenu_Click(object sender, RoutedEventArgs e)
        {
            new DataViewer("CHECKSALES").Show();
        }

        private void txtDiscount_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (this.IsLoaded)
                {
                    if (txtDiscount.Text.Trim() == "") discount = 0;
                    else
                        discount = Int32.Parse(txtDiscount.Text.Trim());

                    if (totAmtDue > 0 && discount <= totAmtDue)
                    {
                        totAmtDue = origAmtDue - discount;
                        lblAmountDue.Content = totAmtDue.ToString("N0");

                        if (amtPaid > 0)
                        {
                            balance = amtPaid - totAmtDue;
                            if (balance < 0)
                            {
                                lblBalance.Foreground = System.Windows.Media.Brushes.Red;
                            }
                            else lblBalance.Foreground = System.Windows.Media.Brushes.Black;
                        }

                        lblBalance.Content = balance.ToString("N0");
                    }
                }
            }
            catch (FormatException ex)
            {
                MessageBox.Show("The discount field only takes numeric characters.\n" + ex.Message, "ALERT", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred.\n" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
      
    }
}
