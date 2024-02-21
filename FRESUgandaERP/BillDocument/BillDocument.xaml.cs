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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FRESUGERP.AppUtilities
{
    /// <summary>
    /// IGenerate the FRES UG Bill Document for a list of clients
    /// </summary>
    public partial class BillDocument : UserControl
    {
        public BillDocument(string clientId, string clientName, string billNo, string billDate,
            string billPeriod, string gracePeriod, int connFeeBal1, int servBBF1, int servCurr, int totOut,
            int connBilled, int connRec, int connfeeBal2, int serv1to30, int serv31to60,
            int serv61to90, int servOver90, int total)
        {
            InitializeComponent();
            lblClientId.Content +="\t"+ clientId;
            lblClientName.Content += "\t" + clientName;
            lblBillNo.Content += "\t\t" + billNo;
            lblBillDate.Content += "\t" + billDate;
            lblGracePeriod.Content += "\t" + gracePeriod;
            lblBillPeriod.Content += "\t" + billPeriod;

            lblConnBal1.Content += "\t" + connFeeBal1;
            lblServBBF1.Content += "\t" + servBBF1;
            lblServCurr.Content += "\t" + servCurr;
            lblTotOutStdg.Content += "\t" + totOut;
            lblConnBilled.Content = connBilled;
            lblConnRec.Content = connRec;
            lblConnBal2.Content = connfeeBal2;
            lblServ1to30.Content = serv1to30;
            lblServ31to60.Content = serv31to60;
            lblServ61to90.Content = serv61to90;
            lblServOver90.Content = servOver90;
            lblTotal.Content = total;

        }
    }
}
