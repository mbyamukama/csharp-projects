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
    public partial class ReceiptDocument : UserControl
    {
        public ReceiptDocument(string recNo, string clientId, string clientName, int amtPaid, string paydate, 
            string payfor, int bal, string paidby, string payType)
        {
            InitializeComponent();
            lblrecNo.Content += "\t" + recNo;
            lblClientId.Content +="\t"+ clientId;
            lblClientName.Content += "\t" + clientName;
            lblAmtPaid.Content += "\t" + amtPaid;
            lblPayDate.Content += "\t" + paydate;
            lblPayFor.Content += "\t" + payfor;
            lblServFeeBal.Content += "\t" + bal;
            lblPaidBy.Content += "\t\t" + paidby;
            lblPayType.Content += "\t" + payType;

        }
    }
}
