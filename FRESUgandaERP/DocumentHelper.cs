using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Windows.Controls;
using System.Data;
using CEDAT.MathLab;
using System.Windows.Documents;
using FRESUGERP.AppUtilities;

namespace FRESUgandaERP
{
    static class DocumentHelper
    {
        public static FixedDocument GenerateReceipt(string recNo, string clientId, string clientName, 
            int amtPaid, string paydate, string payfor, int bal,string paidby, string payType)
        {
            FixedDocument fdoc = new FixedDocument();
            FixedPage fp = new FixedPage();

            ReceiptDocument rdoc = new ReceiptDocument(recNo, clientId, clientName, amtPaid, paydate,
            payfor, bal, paidby, payType);

            fp.Children.Add(rdoc);
            PageContent pc = new PageContent();
            pc.Child = fp;
            fdoc.Pages.Add(pc);
            return fdoc;
        }

        public static FixedDocument GenerateBills(List<DataRow> billList)
        {
            FixedDocument fdoc = new FixedDocument();
            foreach (DataRow row in billList)
            {
            FixedPage fp = new FixedPage();

            int servbbf = Convert.ToInt32(row["BBF"]);
            DataRow connFeeDetails =  FRESUGDBHelper.GetConnFeeDetails(row["CLIENTID"].ToString()).Rows[0];
            int connbal = Convert.ToInt32(connFeeDetails["BALANCE"]);
                string gracePeriod = Convert.ToDateTime(connFeeDetails["CONNDATE"]).ToShortDateString() + " to " + 
                    Convert.ToDateTime(connFeeDetails["CONNDATE"]).AddDays(30).ToShortDateString();
            int servCurr = Convert.ToInt32(row["AMTDUE"]);
            int totOut = servbbf + servCurr + connbal;
            int[] ages = UtilityExtensions.Decompose(servbbf + servCurr, servCurr);

            BillDocument bills = new BillDocument
            (
            row["CLIENTID"].ToString(), FRESUGDBHelper.GetClientName(row["CLIENTID"].ToString()),
            row["BILLNO"].ToString(), Convert.ToDateTime(row["BILLDATE"]).ToShortDateString(), row["BILLPERIOD"].ToString(),gracePeriod,
            connbal, servbbf, servCurr,totOut,
            Convert.ToInt32(connFeeDetails["CONNFEES"]), 
            Convert.ToInt32(connFeeDetails["AMTPAID"]), connbal, ages[0], ages[1], ages[2], ages[3], totOut);

            fp.Children.Add(bills);
            fp.Height = 400;

            PageContent pc = new PageContent();
            pc.Child = fp;
            fdoc.Pages.Add(pc);
            }

            return fdoc;
            
        }
    }
}
