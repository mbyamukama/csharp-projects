using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Data;
using System.Globalization;
using FirebirdSql.Data.FirebirdClient;
using FRESUGERP.AppUtilities;

namespace FRESUgandaERP
{
    
    public static class FRESUGDBHelper
    {
        static string connstr = @"User=SYSDBA;Password="+Properties.Settings.Default.SysdbaPass+";Database="+Properties.Settings.Default.ServerIP+":"
            + Properties.Settings.Default.DBdirectory+ "/fresugdb.fdb";
        static FbConnection conn = null;
        public static FbCommand cmd = null;

        public static bool OpenConnection()
        {
            conn = new FbConnection(connstr);
            bool result = false;
            try
            {
                conn.Open();  
                result = true;
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred. Make sure the database file exists on"+
                    " the server and the path is correct.\n The current properties are:\n"+
                "DB="+conn.DataSource+"\n DIR="+conn.Database+"\n\n TECHNICAL DETAILS: " + e.Message, "ERROR");
                new Settings().ShowDialog();
            }
            return result;
        }
       /* public static SDataTable GetSchema(string dbTableName)
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT * FROM " + dbTableName, conn);
                adp.FillSchema(dt, SchemaType.Source);
                dt.Adapter = adp;
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return dt;
        }*/
        public static Dictionary<int, string> GetEnergyStores()
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            try
            {
                cmd = new FbCommand("SELECT * FROM ENSTORES", conn);
                FbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    dict.Add(reader.GetInt32(0), reader.GetString(1));

                }
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return dict;
        }
        public static int GetRowCount(string dbTableName, string condition=" WHERE COL = VALUE")
        {
            int rowCount = 0;
            try
            {
                cmd = new FbCommand("SELECT COUNT(*) FROM " + dbTableName + condition, conn);
                FbDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                reader.Read();
                rowCount = reader.GetInt32(0);
                reader.Close();
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return rowCount;
        }
        public static int GetMaxClientID(string condition = " WHERE COL = VALUE")
        {
            int maxVal = 0;
            try
            {
                cmd = new FbCommand("SELECT MAX(CLIENTID) FROM CLIENTINFO " + condition, conn);
                FbDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        maxVal = Int32.Parse(reader.GetString(0).Substring(5));
                    }
                    else maxVal = 0;
                }
                else maxVal = 0;
                reader.Close();
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return maxVal;
        }

        public static DataTable GetNewClients(DateTime from, DateTime to, string centre, string level)
        {
            DataTable dt = new DataTable();

            string qAppend = "";
            if (centre != "ALL") qAppend = " AND r.ENSTORE=" + centre;
            if (level != "ALL") qAppend += " AND r.SLEVEL=" + level;

            try
            {
            cmd = new FbCommand("SELECT r.CLIENTID, r.FULLNAME, s.STORENAME,r.SLEVEL, r.CONNDATE FROM CLIENTINFO r"+
                                " INNER JOIN ENSTORES s on r.ENSTORE=s.STOREID "+ qAppend+" WHERE r.CONNDATE>=@FROM AND r.CONNDATE <=@TO ORDER BY r.CONNDATE",conn);
            cmd.Parameters.Add("FROM", from.AsFBDateTime());
            cmd.Parameters.Add("TO", to.AsFBDateTime());
            FbDataAdapter adp = new FbDataAdapter(cmd);
            adp.Fill(dt);
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + e.Message, "ERROR");
            }
            return dt;
        }
        
        public static List<string> GetDistricts()
        {
            FbDataAdapter adp = null;
            SDataTable dt = null;
            try
            {
                adp = new FbDataAdapter("SELECT DISTINCT DISTRICT FROM CLIENTINFO ORDER BY DISTRICT ASC", conn);
                dt = new SDataTable();
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            List<string> districts = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "")
                {
                    districts.Add(row[0].ToString());
                }
            }
            return districts;
        }
        public static List<string> GetSubcounties(string district)
        {
            FbDataAdapter adp = null;
            SDataTable dt = null;
            try
            {
                adp = new FbDataAdapter("SELECT DISTINCT SUBCOUNTY FROM CLIENTINFO WHERE DISTRICT = '" + district + "' ORDER BY SUBCOUNTY ASC", conn);
                dt = new SDataTable();
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            List<string> sub = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "")
                {
                    sub.Add(row[0].ToString());
                }
            }
            return sub;
        }
        public static List<string> GetVillages(string subcounty)
        {
            FbDataAdapter adp = null;
            SDataTable dt = null;
            try
            {
                adp = new FbDataAdapter("SELECT DISTINCT VILLAGE FROM CLIENTINFO WHERE SUBCOUNTY = '" + subcounty + "' ORDER BY VILLAGE ASC", conn);
                dt = new SDataTable();
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            List<string> vill = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (row[0].ToString() != "")
                {
                    vill.Add(row[0].ToString());
                }
            }
            return vill;
        }
        public static SDataTable GetClients(string colNames="col1,col2,col3...etc")
        {
            FbDataAdapter adp = null;
            SDataTable dt = null;
            try
            {
                if (colNames == "*")
                {
                    adp = new FbDataAdapter("SELECT * FROM CLIENTINFO", conn);
                }
                else
                {
                    adp = new FbDataAdapter("SELECT "+colNames+" FROM CLIENTINFO", conn);
                }
                dt = new SDataTable();
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return dt;
        }
        public static bool AddNewClient(string clientId, string fullName, string phone, string village, string county, string district,
            string gps, int energyStore, int serviceLevel, DateTime connDate)
        {
            int connFee = Convert.ToInt32((from myRow in GetFees().AsEnumerable()
                                           where Convert.ToInt32(myRow["SLEVEL"]) == serviceLevel
                                           select myRow).ElementAt(0)["CONNFEE"]);

            FbTransaction trans = conn.BeginTransaction();
            bool result = false;
            try
            {
                cmd = new FbCommand("INSERT INTO CLIENTINFO VALUES(@ID, @NAME,@TEL,@VILLAGE,@COUNTY,@DIST,@GPS,@ESTORE,@SLEVEL,@DATE,@USER)", conn, trans);
                cmd.Parameters.Add("ID", clientId);
                cmd.Parameters.Add("NAME", fullName);
                cmd.Parameters.Add("TEL", phone);
                cmd.Parameters.Add("VILLAGE", village);
                cmd.Parameters.Add("COUNTY", county);
                cmd.Parameters.Add("DIST", district);
                cmd.Parameters.Add("GPS", gps);
                cmd.Parameters.Add("ESTORE", energyStore);
                cmd.Parameters.Add("SLEVEL", serviceLevel);
                cmd.Parameters.Add("DATE", connDate.ToShortDateString());
                cmd.Parameters.Add("USER", UtilityExtensions.currentSession.USER);
                cmd.ExecuteNonQuery();


                cmd = new FbCommand("INSERT INTO CONNFEES VALUES(@CLIENTID,@CONNFEES,0,@CONNFEES,@PAYDATE)", conn, trans);
                cmd.Parameters.Add("CLIENTID", clientId);
                cmd.Parameters.Add("CONNFEES", connFee);
                cmd.Parameters.Add("PAYDATE", null);
                cmd.ExecuteNonQuery();

                //generate maintenance schedules
                for (int i = 1; i <= 10; i++)
                {
                    cmd = new FbCommand("INSERT INTO MAINTENANCE(CLIENTID,DUEDATE,MTNTYPE) VALUES(@CLIENTID,@DUEDATE,@MTNTYPE)", conn, trans);
                    if (i <= 7)
                    {
                        cmd.Parameters.Add("CLIENTID", clientId);
                        cmd.Parameters.Add("DUEDATE", connDate.AddDays(180 * i).AsFBDateTime());
                        cmd.Parameters.Add("MTNTYPE", "MAINTENANCE");
                    }
                    else
                    {
                        cmd.Parameters.Add("CLIENTID", clientId);
                        cmd.Parameters.Add("DUEDATE", connDate.AddDays(720 * (i - 7)).AsFBDateTime());
                        cmd.Parameters.Add("MTNTYPE", "BATTERY REPLACEMENT");
                    }
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                trans.Commit();
                result = true;
            }
            catch (FbException ex)
            {
                trans.Rollback();
                result = false;
                MessageBox.Show("An Error Occurred.\n " + ex.Message, "ERROR");
            }
            return result;
        }
       
        public static bool CheckThisMonthAlreadyProcessed(DateTime dateTime)
        {
            bool thisMonthAlreadyProcessed = false;
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT DISTINCT BILLDATE FROM BILLING", conn);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        DateTime date = Convert.ToDateTime(r["BILLDATE"]);
                        if (date.Month == dateTime.Month && date.Year == dateTime.Year)
                        {
                            thisMonthAlreadyProcessed = true;
                            break;
                        }
                    }
                }
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return thisMonthAlreadyProcessed;
        }
       public static int GetCurrentBalance(string clientId)
        {
            int currentBillAmt = 0, totalPayments = 0;
            DateTime lastBillDate = new DateTime();
                try
                {
                    //now get sum of service payments and sum of bills and subtract
                    cmd = new FbCommand("select * from BILLING b  where b.CLIENTID=@CLIENTID " +
                        " and b.BILLNO=(select max(BILLNO) from BILLING where CLIENTID=@CLIENTID)", conn);
                    cmd.Parameters.Add("CLIENTID", clientId);
                    FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    if (reader.Read())
                    {                        
                        currentBillAmt = reader.IsDBNull(6) ? 0 : reader.GetInt32(6);
                        lastBillDate = reader.IsDBNull(1) ? new DateTime(1900, 1, 1) : reader.GetDateTime(1);
                    }

                    cmd = new FbCommand("select sum(p.AMTPAID) from PAYMENTS p  where p.CLIENTID=@CLIENTID and p.PAYDATE >= @DATE", conn);
                    cmd.Parameters.Add("CLIENTID", clientId);
                    cmd.Parameters.Add("DATE", lastBillDate.AsFBDateTime());
                    reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    if (reader.Read())
                    {
                        totalPayments = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                    }               
                }
                catch (FbException ex)
                {
                    MessageBox.Show(
                     "An error occurred.\nBilling or Payments information for this client may be unavailable.\n"+
                     "DETAILS:"+ex.Message, 
                     "ERROR");
                }
                return (currentBillAmt - totalPayments); 
        }
        public static DataTable GetFees()
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM SERVICES", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show(
                 "An error occurred at the database level.\nDETAILS:" + ex.Message,"ERROR");
            }
            return dt;
        }

        public static DataTable GetClientBills(string clientId, out string nextBillNo)
        {
            DataTable dt = new DataTable();
            nextBillNo = "";
            try
            {
                FbCommand cmd = new FbCommand("select c.FULLNAME, b.*  from billing b inner join" +
                    " CLIENTINFO c on b.CLIENTID= c.CLIENTID where b.CLIENTID=@CLIENTID", conn);
                cmd.Parameters.Add("CLIENTID", clientId);

                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                int numOfBills = dt.Rows.Count;
                nextBillNo = clientId + "." + UtilityExtensions.As3Digits(numOfBills);
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return dt;

        }
        public static DataTable GetBills(DateTime from, DateTime to)
        {
            DataTable dt = new DataTable();
            try
            {
                FbCommand cmd = new FbCommand("select  c.FULLNAME, e.STORENAME, c.SLEVEL, s.SERVICEFEE, b.*  from billing b"+
                    " inner join CLIENTINFO c on b.CLIENTID= c.CLIENTID inner join ENSTORES e on c.ENSTORE=e.STOREID"+
                    "  inner join SERVICES s on s.SLEVEL=c.SLEVEL where b.BILLDATE>=@FROM  and b.BILLDATE<=@TO ", conn);
                cmd.Parameters.Add("FROM", from.AsFBDateTime());
                cmd.Parameters.Add("TO", to.AsFBDateTime());
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return dt;
        }
        public static DataTable GetConnFeeTable()
        {
            DataTable dt = new DataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT  r.CLIENTID, p.FULLNAME, r.CONNFEES, r.AMTPAID, r.BALANCE, r.PAYDATE" +
                    " FROM CONNFEES r INNER JOIN CLIENTINFO p on p.CLIENTID=r.CLIENTID", conn);     
                adp.Fill(dt);
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return dt;
            
        }
        public static DataTable GetConnFeeDetails(string clientId)
        {
            DataTable dt = new DataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT  r.CONNFEES, r.AMTPAID, r.BALANCE, r.PAYDATE, p.CONNDATE" +
                    " FROM CONNFEES r INNER JOIN CLIENTINFO p on p.CLIENTID=r.CLIENTID WHERE r.CLIENTID='" + clientId + "'", conn);
                adp.Fill(dt);
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return dt;
            
        }
        public static DataRow GetMostRecentBill(string clientId)
        {
            DataTable dt = new DataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT * FROM BILLING WHERE BILLNO=(SELECT MAX(BILLNO) FROM BILLING WHERE CLIENTID='" + clientId + "')", conn);
                adp.Fill(dt);
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return dt.Rows[0];
        }
        public static bool DeletePayment(string receiptNo, string clientId, DateTime paydate)
        {
            bool res = false;
            FbTransaction trans = conn.BeginTransaction();
            DateTime mostRecentBillDate=new DateTime();
            try
            {
                //get the maximum bill date
                cmd = new FbCommand("select max(r.BILLDATE)  from BILLING r where r.CLIENTID=@CLIENTID", conn, trans);
                cmd.Parameters.Add("CLIENTID", clientId);

                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                reader.Read();
                if (!reader.IsDBNull(0))
                {
                    mostRecentBillDate = Convert.ToDateTime(reader.GetDateTime(0));
                }

                if (paydate <= mostRecentBillDate)
                {
                    MessageBox.Show("This payment has already been used to generate the Bal B/F in a bill.\n" +
                        "It can not be deleted.", "ERROR");
                    trans.Dispose();
                }
                else
                {
                    cmd = new FbCommand("DELETE FROM PAYMENTS WHERE RECEIPTNO=@RECNO", conn, trans);
                    cmd.Parameters.Add("RECNO", receiptNo);
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    res = true;
                }
            }
            catch (FbException e)
            {
                trans.Rollback();
                res = false;
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return res;
        }
        public static bool MakeServicePayment(int amtPaid, string paidBy, string payDate, string clientId, 
            string payType, string depositor, out string receiptNo, out int newBal)
        {      
            bool result = false;
            receiptNo = clientId + "." + GetRowCount("PAYMENTS", " WHERE CLIENTID='" + clientId + "'");
            newBal = 0;

            FbTransaction trans = conn.BeginTransaction();
            try
            {
                cmd = new FbCommand("INSERT INTO PAYMENTS VALUES(@RECNO,@CLIENTID,@AMTPAID,@PAYDATE,@TOWARDS,@PAYTYPE,@PAIDBY,@USER)", conn, trans);
                    cmd.Parameters.Add("RECNO", receiptNo);
                    cmd.Parameters.Add("CLIENTID", clientId);
                    cmd.Parameters.Add("AMTPAID", amtPaid);
                    cmd.Parameters.Add("PAYDATE", DateTime.Now.AsFBDateTime());
                    cmd.Parameters.Add("TOWARDS", "SERV");
                    cmd.Parameters.Add("USER", UtilityExtensions.currentSession.USER);
                    cmd.Parameters.Add("PAYTYPE", payType);
                    cmd.Parameters.Add("PAIDBY", depositor);
                    cmd.ExecuteNonQuery();
                    trans.Commit();
                    newBal = GetCurrentBalance(clientId);
                result = true;
            }
            catch (FbException ex)
            {
                trans.Rollback();
                MessageBox.Show("An error occurred. This payment cannot be completed.\n TECHNICAL DETAILS:" + ex.Message);
            }

            return result;
        }
        public static bool UpdateConnFee(int amtPaid, string paidBy, string payDate, string clientId, string payType, string depositor,
            out string receiptNo, out int newBal)
        {
            bool result = false;
            newBal = 0;
            receiptNo = clientId + "." + GetRowCount("PAYMENTS", " WHERE CLIENTID='" + clientId + "'");
            FbTransaction trans = conn.BeginTransaction();
            try
            {
                    cmd = new FbCommand("UPDATE CONNFEES SET AMTPAID = (AMTPAID+" + amtPaid + ") , BALANCE= (BALANCE-" + amtPaid + "), PAYDATE='" + payDate +
                        "' WHERE CLIENTID='" + clientId + "'", conn, trans);
                    cmd.ExecuteNonQuery();
  
                    cmd = new FbCommand("SELECT BALANCE FROM CONNFEES WHERE CLIENTID='" + clientId + "'", conn, trans);
                    FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    reader.Read();
                    if (!reader.IsDBNull(0)) newBal = reader.GetInt32(0);
                    reader.Close();

                    cmd = new FbCommand("INSERT INTO PAYMENTS VALUES(@RECNO,@CLIENTID,@AMTPAID,@PAYDATE,@TOWARDS,@PAYTYPE,@PAIDBY,@USER)", conn, trans);
                    cmd.Parameters.Add("RECNO", receiptNo);
                    cmd.Parameters.Add("CLIENTID", clientId);
                    cmd.Parameters.Add("AMTPAID", amtPaid);
                    cmd.Parameters.Add("PAYDATE", DateTime.Now.AsFBDateTime());
                    cmd.Parameters.Add("TOWARDS", "CONN");
                    cmd.Parameters.Add("USER", UtilityExtensions.currentSession.USER);
                    cmd.Parameters.Add("PAYTYPE", payType);
                    cmd.Parameters.Add("PAIDBY", depositor);
                    cmd.ExecuteNonQuery();

                    trans.Commit();
                    result = true;             
            }
            catch (FbException ex)
            {
                trans.Rollback();
                MessageBox.Show("An error occurred. This payment cannot be completed.\n TECHNICAL DETAILS:" + ex.Message);
            }

            return result;
        }
        public static string GetClientName(string clientId)
        {
            cmd = new FbCommand("SELECT FULLNAME FROM CLIENTINFO WHERE CLIENTID='" + clientId + "'", conn);
            FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            reader.Read();
            if (reader.IsDBNull(0)) return "";
            else return reader.GetString(0);
        }

        public static void AddBill(string billNo, string billDate, string billPeriod,  string clientId, int amtDue)
        {

            int bbf = GetCurrentBalance(clientId);
            cmd = new FbCommand("INSERT INTO BILLING VALUES (@BILLNO,@BILLDATE,@BILLPERIOD,@CLIENTID,@BBF,@AMTDUE,@TOTAL)", conn);
            cmd.Parameters.Add("BILLNO", billNo);
            cmd.Parameters.Add("BILLDATE", billDate);
            cmd.Parameters.Add("BILLPERIOD", billPeriod);
            cmd.Parameters.Add("CLIENTID", clientId);
            cmd.Parameters.Add("BBF", bbf);
            cmd.Parameters.Add("AMTDUE", amtDue);
            cmd.Parameters.Add("TOTAL", bbf + amtDue);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }
        public static DataTable GetStatement(string clientId)
        {
            DataTable statement = new DataTable();
            try
            {
             
             statement.Columns.Add("DATE");
             statement.Columns.Add("TRANSACTION");
             statement.Columns.Add("AMOUNT");

             string useless = String.Empty;
             DataTable bills = GetClientBills(clientId, out useless);
             DataTable connFees = GetConnFeeDetails(clientId);
             DataTable payments = GetPayments(clientId);

             foreach (DataRow row in bills.Rows)
             {
                 statement.Rows.Add(row["BILLDATE"], "SERVICE BILL", row["AMTDUE"]);
             }
             foreach (DataRow row in connFees.Rows)
             {
                 statement.Rows.Add(row["PAYDATE"], "CONN FEES", row["BALANCE"]);
             }
             foreach (DataRow row in  payments.Rows)
             {
                 statement.Rows.Add(row["PAYDATE"], "PAYMENT", row["AMTPAID"]);
             }

             statement.DefaultView.Sort = "DATE DESC";

             int owed = 0;
             foreach (DataRow row in statement.Rows)
             {
                 if (row["TRANSACTION"].ToString() == "SERVICE BILL")
                 {
                     owed += Convert.ToInt32(row["AMOUNT"]);
                 }
                 if (row["TRANSACTION"].ToString() == "CONN FEES")
                 {
                     owed += Convert.ToInt32(row["AMOUNT"]);
                 }
                 if (row["TRANSACTION"].ToString() == "PAYMENT")
                 {
                     owed += -1 * Convert.ToInt32(row["AMOUNT"]);
                 }
             }

             statement.Rows.Add();
             statement.Rows.Add("", "OWED", owed);

            }
             
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return statement;
        }
        public static bool AddUser(string username, string pass, int clr, string station)
        {
            string hash = CEDAT.MathLab.Hasher.CreateHash(pass);
            bool result = false;
            try
            {
                cmd = new FbCommand("INSERT INTO USERS VALUES (@USERNAME,@HASH,@CLR,@STATION)", conn);
                cmd.Parameters.Add("USERNAME", username);
                cmd.Parameters.Add("HASH", hash);
                cmd.Parameters.Add("CLR", clr);
                cmd.Parameters.Add("STATION", station);
                if (cmd.ExecuteNonQuery() > 0)  result = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while adding the user into the database.\n" + ex.Message, "ERROR");
            }
            return result;
        }
        public static bool DeleteClient(string clientId)
        {
            bool res = false;
            FbTransaction trans = conn.BeginTransaction(/*IsolationLevel.RepeatableRead*/);
            try
            {
                cmd = new FbCommand("INSERT INTO DEACTIVATED SELECT * FROM CLIENTINFO r WHERE r.CLIENTID=@CLIENTID",conn, trans);
                cmd.Parameters.Add("CLIENTID", clientId);
                cmd.ExecuteNonQuery();
                cmd = new FbCommand("DELETE FROM CLIENTINFO WHERE CLIENTID=@CLIENTID", conn,trans);
                cmd.Parameters.Add("CLIENTID", clientId);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    trans.Commit();
                    res = true;
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while deleting this client from the database.\n" + ex.Message, "ERROR");
            }
            return res;
        }
        public static bool AddOtherIncome(string item, int amount, DateTime date)
        {
            string entryID =  UtilityExtensions.As4Digits(GetRowCount("INDINCOME", "") + 1);
            bool res = false;
            try
            {
                cmd = new FbCommand("INSERT INTO INDINCOME VALUES (@ENTRYID, @ITEM,@AMT,@DATE)", conn);
                cmd.Parameters.Add("ENTRYID", entryID);
                cmd.Parameters.Add("ITEM", item);
                cmd.Parameters.Add("AMT", amount);
                cmd.Parameters.Add("DATE", date.AsFBDateTime());
                if (cmd.ExecuteNonQuery() > 0) res= true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while inserting data into the database.\n" + ex.Message, "ERROR");
            }
            return res;
        }
        public static bool AddExpense(string item, int amount, DateTime date)
        {
            string entryID = UtilityExtensions.As4Digits(GetRowCount("EXPENSES", "") + 1);
            bool res = false;
            try
            {
                cmd = new FbCommand("INSERT INTO EXPENSES VALUES (@ENTRYID, @ITEM,@AMT,@DATE)", conn);
                cmd.Parameters.Add("ENTRYID", entryID);
                cmd.Parameters.Add("ITEM", item);
                cmd.Parameters.Add("AMT", amount);
                cmd.Parameters.Add("DATE", date.AsFBDateTime());
                if (cmd.ExecuteNonQuery() > 0) res = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while inserting data into the database.\n" + ex.Message, "ERROR");
            }
            return res;
        }
        public static object[] GetCredentials(string username, out bool result)
        {
            object[] cred = new object[3];
            result = false;
            try
            {
                cmd = new FbCommand("SELECT PASSHASH, CLRLEVEL, STATION FROM USERS WHERE USERNAME ='" + username + "'", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    cred[0] = dt.Rows[0]["PASSHASH"].ToString();
                    cred[1] = Convert.ToInt32(dt.Rows[0]["CLRLEVEL"]);
                    cred[2] = dt.Rows[0]["STATION"].ToString();
                    result = true;
                }
                else
                {
                    MessageBox.Show("An error occurred during authentication.\n The log on credentials provided cannot be verified.\n" +
                    "The username provided may not be valid.", "ERROR");
                }
            }
           catch (FbException ex)
            {
                MessageBox.Show("An error occurred during authentication.\n DETAILS: " + ex.Message,"ERROR");
            }
            return cred;
        }
        public static int GetTotalPayments(DateTime from, DateTime to)
        {
            int amt = 0;
            cmd = new FbCommand("SELECT SUM(R.AMTPAID) FROM PAYMENTS R WHERE R.PAYDATE>=@FROM AND R.PAYDATE <=@TO", conn);
            cmd.Parameters.Add("FROM", from.AsFBDateTime());
            cmd.Parameters.Add("TO", to.AsFBDateTime());
            FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            reader.Read();
            if (reader.IsDBNull(0)) amt = 0;
            else amt = reader.GetInt32(0);
            reader.Close();
            return amt;
        }
        public static int GetTotalConnFeePayments(DateTime from, DateTime to)
        {
            int amt = 0;
            cmd = new FbCommand("SELECT SUM(R.AMTPAID) FROM CONNFEES R WHERE R.PAYDATE>=@FROM AND R.PAYDATE <=@TO", conn);
            cmd.Parameters.Add("FROM", from.AsFBDateTime());
            cmd.Parameters.Add("TO", to.AsFBDateTime());
            FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
            reader.Read();
            if (reader.IsDBNull(0)) amt = 0;
            else amt = reader.GetInt32(0);
            reader.Close();
            return amt;
        }
        public static DataTable GetTotalIndirectIncomes(DateTime from, DateTime to)
        {
            cmd = new FbCommand("SELECT  r.ITEM , SUM(r.AMTPAID) FROM INDINCOME r  where r.TDATE >=@FROM AND r.TDATE <=@TO GROUP BY r.ITEM", conn);
            cmd.Parameters.Add("FROM", from.AsFBDateTime());
            cmd.Parameters.Add("TO", to.AsFBDateTime());
            FbDataAdapter adp = new FbDataAdapter(cmd);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }
        public static DataTable GetTotalExpenses(DateTime from, DateTime to)
        {
            cmd = new FbCommand("SELECT  r.ITEM , -1 * SUM(r.AMTPAID) AS TSUM FROM EXPENSES r  where r.TDATE >=@FROM AND r.TDATE <=@TO GROUP BY r.ITEM", conn);
            cmd.Parameters.Add("FROM", from.AsFBDateTime());
            cmd.Parameters.Add("TO", to.AsFBDateTime());
            FbDataAdapter adp = new FbDataAdapter(cmd);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }
        public static int GetConnFeeBalance(string clientId)
        {
            int amt = 0;
            try
            {
                cmd = new FbCommand("SELECT R.BALANCE FROM CONNFEES R WHERE R.CLIENTID='" + clientId + "'", conn);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                reader.Read();
                if (reader.IsDBNull(0)) amt = 0;
                else amt = reader.GetInt32(0);
                reader.Close();
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\n" + e.Message, "ERROR");
            }
            return amt;
        }

        public static DataTable GetPayments(string clientId)
        {
            FbDataAdapter adp = new FbDataAdapter("SELECT * FROM PAYMENTS WHERE CLIENTID='"+clientId+"'", conn);
            SDataTable dt = new SDataTable();
            adp.Fill(dt);
            return dt;
        }
        public static SDataTable GetPayments(DateTime from, DateTime to, string payType)
        {
            string qAppend = "";
            if (payType != "ALL") qAppend = " AND r.PAYTYPE LIKE '%" + payType + "%'";

            FbCommand cmd = new FbCommand("SELECT  * FROM PAYMENTS r  WHERE (r.PAYDATE>=@FROM AND r.PAYDATE <=@TO)"+ qAppend, conn);
            cmd.Parameters.Add("FROM", from.AsFBDateTime());
            cmd.Parameters.Add("TO", to.AsFBDateTime());
            SDataTable dt = new SDataTable();
            FbDataAdapter adp = new FbDataAdapter(cmd);
            adp.Fill(dt);
            dt.Adapter = adp;
            return dt;
        }
        public static DataTable GetBillExpectations(DateTime from, DateTime to, string centre, string level)
        {
            DataTable dt = new DataTable();

            string qAppend = "";
            if (centre != "ALL") qAppend = " AND f.ENSTORE=" + centre;
            if (level != "ALL") qAppend += " AND f.SLEVEL=" + level;

            try
            {
                cmd = new FbCommand("SELECT r.BILLNO, f.FULLNAME, r.BILLDATE, r.TOTAL FROM BILLING r" +
                                    " INNER JOIN CLIENTINFO f on f.CLIENTID=r.CLIENTID" +
                                    " WHERE r.BILLDATE>=@FROM AND r.BILLDATE<=@TO"+qAppend, conn);
                cmd.Parameters.Add("FROM", from.AsFBDateTime());
                cmd.Parameters.Add("TO", to.AsFBDateTime());
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while getting bill report.\n DETAILS: " + ex.Message, "ERROR");
            }
            return dt;
        }

        

        public static DataTable GetDebtors(string centre, string level)
        {
            DataTable dt = new DataTable();

            string qAppend = "";
            if (centre != "ALL") qAppend = " AND f.ENSTORE=" + centre;
            if (level != "ALL") qAppend += " AND f.SLEVEL=" + level;

            try
            {
                cmd = new FbCommand(" select  f.FULLNAME, f.ENSTORE, f.SLEVEL, x.* from CLIENTINFO f " +
                    " inner join ( Select g.*, h.BALANCE as CONNFEEBAL, i.TOTALPAID from (select b.CLIENTID," +
                    "  max(b.TOTAL) as TOTALSERV from BILLING b  group by b.CLIENTID) g inner join (select c.CLIENTID," +
                    " c.BALANCE from CONNFEES c ) h on h.CLIENTID=g.CLIENTID left outer join (select p.CLIENTID," +
                    " sum(p.AMTPAID) as TOTALPAID from PAYMENTS p group by p.CLIENTID) i on h.CLIENTID = i.CLIENTID) x " +
                    " on x.CLIENTID=f.CLIENTID " + qAppend +" where TOTALPAID is null or (TOTALSERV+CONNFEEBAL-TOTALPAID)>0" , conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while collecting debtors data from the database.\n DETAILS: " + ex.Message, "ERROR");
            }
            return dt;
        }

        public static SDataTable GetUsers()
        {
             SDataTable dt = new SDataTable();
             try
             {
                 FbDataAdapter adp = new FbDataAdapter("SELECT r.USERNAME, r.CLRLEVEL FROM USERS r WHERE r.USERNAME !='superadmin'", conn);
                 dt.Adapter = adp;
                 adp.Fill(dt);
             }
             catch (FbException ex)
             {
                 MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR");
             }
             return dt;
        }
        public static bool AddLog(string window, string username, string details, string station)
        {
            bool result = false;
            try
            {
                string id = UtilityExtensions.As6Digits(GetRowCount("LOG", null));
                cmd = new FbCommand("INSERT INTO LOG VALUES(@ID,@WINDOW,@USER,@DETAILS,@STATION)", conn);
                cmd.Parameters.Add("ID", id);
                cmd.Parameters.Add("WINDOW", window);
                cmd.Parameters.Add("USER", username);
                cmd.Parameters.Add("DETAILS", details);
                cmd.Parameters.Add("STATION", station);

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0) result = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while adding log to the database.\n DETAILS: " + ex.Message, "ERROR");
            }
            catch (Exception ex)
            {
               //do nothing
            }
            return result;
        }

        public static DataTable GetLogTable()
        {   
            DataTable dt = new DataTable();
            try
            {
            cmd = new FbCommand("SELECT * FROM LOG",conn);
            FbDataAdapter adp = new FbDataAdapter(cmd);
            adp.Fill(dt);
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + e.Message, "ERROR");
            }
            return dt;
        }
        public static List<string> GetDBTables()
        {
            List<string> list = new List<string>();
            try
            {
                cmd = new FbCommand("select r.RDB$RELATION_NAME from RDB$RELATIONS r where r.RDB$FLAGS=1", conn);
                FbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader.GetString(0));
                }
                reader.Close();
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + e.Message, "ERROR");
            }
            return list;
        }
        /*public static DataTable GetMaintenanceSchedule(DateTime from, DateTime to)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("SELECT r.CLIENTID, f.FULLNAME, f.PHONENUM, r.DUEDATE," +
                    " r.MTNTYPE as MAINTENANCE_TYPE,r.MTNDATE, r.MTNLOG FROM MAINTENANCE r INNER JOIN CLIENTINFO f on f.CLIENTID=r.CLIENTID" +
                    " WHERE r.DUEDATE>=@FROM AND r.DUEDATE>=@TO", conn);
                cmd.Parameters.Add("FROM", from.AsFBDateTime());
                cmd.Parameters.Add("TO", to.AsFBDateTime());
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + e.Message, "ERROR");
            }
            catch (Exception e)
            {
                MessageBox.Show("An unknown error occurred.\nDETAILS: " + e.Message, "ERROR");
            }
            return dt;
        }*/
        public static DataTable GetMaintenanceSchedule(DateTime from, DateTime to, string centre, string level)
        {
            DataTable dt = new DataTable();

            string qAppend = "";
            if (centre != "ALL") qAppend = " AND f.ENSTORE=" + centre;
            if (level != "ALL") qAppend += " AND f.SLEVEL=" + level;

            try
            {
                cmd = new FbCommand("SELECT  f.CLIENTID,f.FULLNAME, f.PHONENUM, d.STORENAME,f.SLEVEL, m.DUEDATE, m.MTNTYPE,m.MTNDATE,m.MTNLOG as LOG,"+ 
                   " (current_date-m.DUEDATE) as AGE from CLIENTINFO f INNER JOIN ENSTORES d ON d.STOREID=f.ENSTORE"+
                   " inner join MAINTENANCE m on m.CLIENTID=f.CLIENTID where m.DUEDATE>=@FROM and m.DUEDATE<=@TO"
                   + qAppend + " order BY AGE desc", conn);

                cmd.Parameters.Add("FROM", from.AsFBDateTime());
                cmd.Parameters.Add("TO", to.AsFBDateTime());
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while getting maintenance report.\n DETAILS: " + ex.Message, "ERROR");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred while getting maintenance report.\n DETAILS: " + ex.Message, "ERROR");
            }
            return dt;
        }
        public static DataTable GetUnMaintainedClients(DateTime upto, string centre, string level)
        {
            DataTable dt = new DataTable();
            string qAppend = "";
            if (centre != "ALL") qAppend = " AND f.ENSTORE=" + centre;
            if (level != "ALL") qAppend += " AND f.SLEVEL=" + level;

            try
            {
                cmd = new FbCommand("SELECT * FROM ( SELECT  f.CLIENTID,f.FULLNAME, f.PHONENUM, d.STORENAME,f.SLEVEL,"+
                " m.DUEDATE, m.MTNTYPE, (current_date-m.DUEDATE) as AGE  from CLIENTINFO f INNER JOIN ENSTORES d ON d.STOREID=f.ENSTORE" +
                " inner join MAINTENANCE m on m.CLIENTID=f.CLIENTID where m.MTNDATE is null"+ qAppend+
                " order by AGE desc) g where g.AGE<1000 and g.DUEDATE<=@TODAY", conn);

                cmd.Parameters.Add("TODAY", upto.AsFBDateTime());
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while getting maintenance details.\n DETAILS: " + ex.Message, "ERROR");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred while getting maintenance details.\n DETAILS: " + ex.Message, "ERROR");
            }
            return dt;
        }
        public static bool UpdateMaintenance(string clientId, string log, DateTime dueDate,DateTime mtnDate)
        {
            bool res = false;
            try
            {
                cmd = new FbCommand("UPDATE MAINTENANCE r SET r.MTNDATE=@MTNDATE, r.MTNLOG=@LOG" +
                                    " WHERE r.CLIENTID=@CLIENTID and r.DUEDATE=@DUEDATE", conn);
                cmd.Parameters.Add("MTNDATE", mtnDate.AsFBDateTime());
                cmd.Parameters.Add("LOG", log);
                cmd.Parameters.Add("CLIENTID", clientId);
                cmd.Parameters.Add("DUEDATE", dueDate.AsFBDateTime());

                if (cmd.ExecuteNonQuery() > 0) res = true;
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + e.Message, "ERROR");
            }
            return res;
        }
       
          public static void Import()
          {

              DataTable dt = CEDAT.MathLab.Utilities.ReadCSVFile(@"D:\Dropbox\Work\business\fres uganda\list.csv");
              DataTable errors = dt.Clone();
              int count = 0;

              foreach (DataRow row in dt.Rows)
              {
                  int amtDue = 0;
                  string clientId = '0'+row["CLIENTID"].ToString();
                  if (clientId.Split(new char[] { '.' })[1].Length == 4) 
                  {
                      clientId = clientId + "0";
                  }

                  if (Int32.TryParse(row["BALANCE"].ToString(), out amtDue))
                  {
                      cmd = new FbCommand("UPDATE BILLING SET AMTDUE=@AMT WHERE CLIENTID=@ID", conn);
                      cmd.Parameters.Add("ID", clientId);
                      cmd.Parameters.Add("AMT", amtDue);
                     count+= cmd.ExecuteNonQuery();
                  }
                  else
                  {
                      errors.ImportRow(row);
                  }
              }
                  
              CEDAT.MathLab.Utilities.WriteCSVFile(errors, @"D:\Dropbox\Work\business\fres uganda\listErrors.csv");
              MessageBox.Show("Import Complete.\n" + count + " rows imported", "SUCCESS");
          }


        internal static DataTable BillAges(string estore, string slevel, string age)
        {
            DataTable dt = new DataTable();
            string qAppend = "";
            if (age == "ALL") qAppend = " WHERE AGE > 0";
            else qAppend = " WHERE AGE = " + age;

            if (estore != "ALL") qAppend += " AND ENSTORE=" + estore;
            if (slevel != "ALL") qAppend += " AND SLEVEL=" + slevel;
            try
            {
                cmd = new FbCommand("SELECT * FROM (SELECT f.*, d.BILLNO, d.BILLDATE, d.BILLPERIOD, d.LATESTBILL,"+
                    " (d.LATESTBILL/f.SERVICEFEE * 30) as AGE from ( SELECT c.CLIENTID, c.FULLNAME, c.ENSTORE, c.PHONENUM," +
                    " p.SLEVEL, p.SERVICEFEE FROM CLIENTINFO c INNER JOIN SERVICES p"+
                    " on c.SLEVEL = p.SLEVEL INNER JOIN ENSTORES e on c.ENSTORE = e.STOREID) f INNER join "+ 
                    "( SELECT z.CLIENTID, s.BILLNO, s.BILLDATE, s.BILLPERIOD, s.BALANCE as LATESTBILL FROM BILLING s"+
                    " INNER JOIN ( SELECT CLIENTID,  max(BALANCE) as LATESTBILL FROM BILLING GROUP BY CLIENTID) z"+ 
                    " on s.CLIENTID = z.CLIENTID  and s.BALANCE=z.LATESTBILL ) d " +
                    " on f.CLIENTID = d.CLIENTID )" + qAppend, conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while collecting bill ages data from the database.\n DETAILS: " + ex.Message, "ERROR");
            }
            return dt;
        }


        internal static SDataTable GetTableData(string tableName)
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT * FROM " + tableName, conn);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + e.Message, "ERROR");
            }
            return dt;
        }
    }
}
