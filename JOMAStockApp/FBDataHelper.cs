using System;
using System.Collections.Generic;
using System.Windows;
using System.Data;
using FirebirdSql.Data.FirebirdClient;
using System.IO;
using StockApp.AppExtensions;

namespace StockApp
{
    public static class FBDataHelper
    {
       public static string connstr2 = @"User=SYSDBA;Password=" + Properties.Settings.Default.SysDBAPass +
                                        ";Database=" + Properties.Settings.Default.ServerIP + ":"
                                              + Properties.Settings.Default.DBDirectory + "/stockdb.fdb";

        static FbConnection conn = null;
        public static FbCommand cmd = null;
        public enum StockType { Basic, Detailed };

        public static bool OpenConnection(out string server)
        {
            bool result = false;
            server = "";
            try
            {
                conn = new FbConnection(connstr2);
                conn.Open();
                server = conn.DataSource;
                result = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("A connection to could not be opened.\n " +
                    "Make sure the database file exists on the server and the path is correct.\n " +
                    "The current properties will be shown in the settings window following:\n DETAILS: "
                + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                new DBSettings().ShowDialog();
            }
            return result;
        }

        internal static bool ReturnItem(string itemName, int qty, int itemValue, string notes, out string message)
        {
            //1. get how many items there are with this name
            bool pass1 = false, pass2 = false;
            message = "";
            try
            {
                FbTransaction trans = conn.BeginTransaction();
                cmd = new FbCommand("SELECT * FROM STOCK WHERE ITEMNAME=@ITEMNAME", conn, trans);
                cmd.Parameters.Add("ITEMNAME", itemName);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                DataTable dt = new DataTable();
                adp.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows.Count > 1)
                    {
                        message += "Multiple items with this name were found. Only the 1st instance will be updated.\n";
                    }

                    string barcode = dt.Rows[0]["BARCODE"].ToString();
                    cmd = new FbCommand("UPDATE STOCK SET QUANTITY=QUANTITY+@QTY WHERE BARCODE=@BARCODE", conn, trans);
                    cmd.Parameters.Add("QTY", qty);
                    cmd.Parameters.Add("BARCODE", barcode);
                    int affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        pass1 = true;
                        message += "The number of affected Rows was: " + affectedRows;
                    }

                    cmd = new FbCommand("INSERT INTO EXPENSES VALUES(@ITEM, @ITEMVALUE, @TDATE, @TTIME,@NOTES)", conn, trans);
                    cmd.Parameters.Add("ITEM", itemName);
                    cmd.Parameters.Add("ITEMVALUE", itemValue);
                    cmd.Parameters.Add("TDATE", DateTime.Now.AsFBDateTime());
                    cmd.Parameters.Add("TTIME", DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                    cmd.Parameters.Add("NOTES", notes);

                    affectedRows = cmd.ExecuteNonQuery();
                    if (affectedRows > 0)
                    {
                        pass2 = true;
                    }
                    if (pass1 & pass2)
                        trans.Commit();
                    else
                        trans.Rollback();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.\nDETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return (pass1 & pass2);
        }

        public static DataTable GetDailyTotals(DateTime fromDate, DateTime toDate)
        {
            FbDataAdapter adp = new FbDataAdapter("SELECT r.TDATE, sum(r.AMOUNTDUE) as TOTAL FROM SALES r where r.TDATE >= @FROM and r.TDATE <= @TO group by r.TDATE", conn);
            adp.SelectCommand.Parameters.Add("FROM", fromDate);
            adp.SelectCommand.Parameters.Add("TO", toDate);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }
        public static DataTable GetDailyTotals(DateTime fromDate, DateTime toDate, string itemName)
        {
            FbDataAdapter adp = new FbDataAdapter("SELECT t.TDATE, sum(t.AMOUNTDUE) FROM (SELECT s.TDATE, r.AMOUNTDUE FROM DETSALES r"+
                                                  " INNER JOIN SALES s on s.SALEID = r.SALEID where r.ITEMNAME = @ITEMNAME order by s.TDATE) t GROUP BY t.TDATE", conn);
            adp.SelectCommand.Parameters.Add("FROM", fromDate);
            adp.SelectCommand.Parameters.Add("TO", toDate);
            adp.SelectCommand.Parameters.Add("ITEMNAME", itemName);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }

        internal static DataTable GetExpenses(DateTime fromDate, DateTime toDate)
        {
            FbDataAdapter adp = new FbDataAdapter("SELECT * FROM EXPENSES s WHERE s.TDATE >= @FROM and s.TDATE <= @TO ", conn);
            adp.SelectCommand.Parameters.Add("FROM", fromDate);
            adp.SelectCommand.Parameters.Add("TO", toDate);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            return dt;
        }

        internal static bool AddExpense(string itemName, int qty, int itemValue, string notes)
        {
            bool pass = false;
            try
            {
                cmd = new FbCommand("INSERT INTO EXPENSES VALUES(@ITEM, @ITEMVALUE, @TDATE, @TTIME,@NOTES)", conn);
                cmd.Parameters.Add("ITEM", itemName);
                cmd.Parameters.Add("ITEMVALUE", itemValue);
                cmd.Parameters.Add("TDATE", DateTime.Now.AsFBDateTime());
                cmd.Parameters.Add("TTIME", DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                cmd.Parameters.Add("NOTES", notes);

                int affectedRows = cmd.ExecuteNonQuery();
                if (affectedRows > 0)
                {
                    pass = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.\nDETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return pass;
        }

        internal static DataTable GetSaleHistory(string itemName, DateTime from, DateTime to)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("SELECT r.*, s.TDATE FROM DETSALES r inner join SALES s on r.SALEID = s.SALEID " +
                    " where r.ITEMNAME = @ITEM  and s.TDATE >= @FROM and s.TDATE <= @TO  ", conn);
                cmd.Parameters.Add("ITEM", itemName);
                cmd.Parameters.Add("FROM", from);
                cmd.Parameters.Add("TO", to);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return dt;
        }

        internal static List<string> GetDBTables()
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

        public static void CloseConnection()
        {
            try
            {
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured.\nDETAILS: "+ ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

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

        public static bool IsConnectionOpen()
        {
            bool result = false;
            if (conn != null && conn.State == ConnectionState.Open)
            {
                result = true;
            }
            return result;
        }
        public static FbConnection GetActiveConnection()
        {
            if (conn != null && conn.State == ConnectionState.Open)
                return conn;
            else
                return null;
        }
        public static Int64  GetStockValue()
        {
            Int64 values = 0; ;
            try
            {
                    cmd = new FbCommand("SELECT sum(r.COSTPPU*r.QUANTITY) FROM STOCK r", conn);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    values = (Int64)(reader.GetDouble(0));
                  //  values[1] = (Int64)(reader.GetDouble(1));
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return values;
        }
       
        public static Int64 GetTotalSales(DateTime from, DateTime to)
        {
            Int64 values = 0;
            try
            {
                cmd = new FbCommand("SELECT sum(r.AMOUNTDUE) FROM SALES r WHERE r.TDATE>=@FROM AND r.TDATE<=@TO", conn);
                cmd.Parameters.Add("FROM", from);
                cmd.Parameters.Add("TO", to);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    values = (Int64)(reader.GetDouble(0));
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return values;

        }
        public static Int64 GetTotalProfit(DateTime from, DateTime to)
        {
            Int64 values = 0;
            try
            {
                cmd = new FbCommand("SELECT sum(g.PROFIT) FROM (SELECT r.SALEID, r.ITEMNAME, r.QUANTITY, r.AMOUNTDUE, s.FACTOR,"+
                    " (r.AMOUNTDUE-r.AMOUNTDUE/s.FACTOR) as PROFIT FROM DETSALES r INNER JOIN STOCK s on s.ITEMNAME=r.ITEMNAME) g"+
                    " INNER JOIN SALES t ON t.SALEID = g.SALEID WHERE t.TDATE>=@FROM AND t.TDATE<=@TO", conn);
                cmd.Parameters.Add("FROM", from);
                cmd.Parameters.Add("TO", to);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    values = (Int64)(reader.GetDouble(0));
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message);
            }
            return values;

        }
        public static SDataTable GetStock(StockType stockType, DateTime? from, DateTime? to)
        {
            SDataTable dt = new SDataTable();
            FbDataAdapter adp = null;
          //  FbTransaction trans = conn.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                if (stockType == StockType.Basic)
                {
                    cmd = new FbCommand("SELECT * FROM STOCK", conn);
                }
                else if (stockType == StockType.Detailed)
                {
                    cmd = new FbCommand("SELECT * FROM DETSTOCK A WHERE A.TDATE>=@from AND A.TDATE<=@TO ", conn);
                    cmd.Parameters.Add("FROM", from);
                    cmd.Parameters.Add("TO", to);
                }

                adp = new FbDataAdapter(cmd);
                dt.Adapter = adp;
                adp.Fill(dt);  
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return dt;
        }
        public static SDataTable GetEmployees()
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT FNAME, LNAME, PHONENUM, USERNAME FROM EMPLOYEES WHERE USERNAME != 'SUPERADMIN'", conn);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return dt;
        }
        public static SDataTable GetSuppliers()
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT * FROM SUPPLIERS", conn);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return dt;
        }

        
        public static bool AddStock(string entryId, string barcode,  string itemName,
            string supplier, string invoiceNum, int qty, float costppu, float factor, string category, DateTime dateofEntry)
        {
            bool result = false;
            FbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction(IsolationLevel.Serializable);
                int sellppu = GlobalSystemData.RoundUpToNearestNote(factor * costppu);
                cmd = new FbCommand("UPDATE STOCK SET COSTPPU=@COSTPPU, FACTOR = @FACTOR, SELLPPU = @SELLPPU, " +
                    "QUANTITY=QUANTITY+@QUANTITY,  CATEGORY = @CATEGORY WHERE BARCODE=@BARCODE", conn, trans);
                cmd.Parameters.Add("BARCODE", barcode);
                cmd.Parameters.Add("COSTPPU", costppu);
                cmd.Parameters.Add("FACTOR", factor);
                cmd.Parameters.Add("SELLPPU", sellppu);
                cmd.Parameters.Add("CATEGORY", category);
                cmd.Parameters.Add("QUANTITY", qty);    

                cmd.ExecuteNonQuery();


                cmd = new FbCommand("INSERT INTO DETSTOCK VALUES (@ENTRYID, @ITEMNAME," +
                     "@SUPPLIER, @INVOICENUM, @QTY, @TOTAMT, @DATEOFENTRY)", conn, trans);
                int totAmt = (int)(qty*costppu);

                cmd.Parameters.Add("ENTRYID", entryId);
                cmd.Parameters.Add("ITEMNAME", itemName);
                cmd.Parameters.Add("SUPPLIER", supplier);
                cmd.Parameters.Add("INVOICENUM", invoiceNum);
                cmd.Parameters.Add("QTY", qty); 
                cmd.Parameters.Add("TOTAMT", totAmt);
                cmd.Parameters.Add("DATEOFENTRY", DateTime.Now);
           
                cmd.ExecuteNonQuery();

                trans.Commit();
                result = true;
            }
            catch (FbException ex)
            {
                trans.Rollback();
                MessageBox.Show("An error occurred while adding stock.\n" + ex.Message,"ERROR");
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return result;
        }
        public static Int32 GetMaxEntryID(string tableName, string pkeyColName, FbTransaction trans)
        {
            int id = 10000;
            try
            {
                cmd = new FbCommand("SELECT MAX (" + pkeyColName + ") FROM " + tableName, conn, trans);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                reader.Read();
                if (!reader.IsDBNull(0))
                {
                    id = reader.GetInt32(0);
                }
                reader.Close();
            }
            catch (FbException ex)
            {
                trans.Rollback();
                MessageBox.Show("An error occurred getting the entry ID.\n" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return id;
        }
        public static Int32 GetMaxEntryID(string tableName, string pkeyColName)
        {
            int id = 10000;
            try
            {
                cmd = new FbCommand("SELECT MAX (" + pkeyColName + ") FROM " + tableName, conn);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                reader.Read();

                if (!reader.IsDBNull(0))
                {
                    id = reader.GetInt32(0);
                }
                reader.Close();
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return id;
        }

        internal static MemoryStream GetReceipt(string saleid)
        {
            cmd = new FbCommand("SELECT * FROM RECBACKUP r WHERE r.SALEID='" + saleid + "'", conn);
            FbDataReader reader = cmd.ExecuteReader();
            reader.Read();  //always 1 row
            int bytecount = (int)reader["BYTECOUNT"];
            byte[] buff = new byte[bytecount];
            reader.GetBytes(1, 0, buff, 0, bytecount);
            return new MemoryStream(buff);
        }
        internal static void AddReceipt(string saleid, MemoryStream recMemStream)
        {
            cmd = new FbCommand("INSERT INTO RECBACKUP VALUES (@SALEID, @OBJECT,@COUNT)", conn);
            cmd.Parameters.Add("SALEID", saleid);
            cmd.Parameters.Add("OBJECT", recMemStream.GetBuffer());
            cmd.Parameters.Add("COUNT", recMemStream.Length);
            cmd.ExecuteNonQuery();
        }

        public static bool DeleteSale(string saleid)
        {
            bool res = false;
            try
            {
                cmd = new FbCommand
                 ("DELETE FROM SALES WHERE SALEID=@ENTID", conn);
                cmd.Parameters.Add("ENTID", saleid);
                if (cmd.ExecuteNonQuery() > 0) res = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred.\n" + ex.Message, "ERROR");
            }
            return res;
        }

        public static bool AddSale(int amtDue, int amtPaid, int bal, string teller,out string saleid, string customerID, string saletype, string authcode, int earned, int redeemed, out FbTransaction trans)
        {
            bool result = false;
            trans = conn.BeginTransaction(IsolationLevel.Serializable);
            saleid = "";
            try
            {              
                int entryID = GetMaxEntryID("SALES", "SALEID", trans);
                cmd = new FbCommand
             ("INSERT INTO SALES VALUES (@ENTID, @AMTDUE,@AMTPAID, @BAL, CURRENT_DATE, CURRENT_TIME, @TELLER,@CUSTOMERID,@SALETYPE,@AUTHCODE,@POINTSEARNED,@AMTREDEEMED)", conn, trans);
                cmd.Parameters.Add("ENTID", ++entryID);
                saleid = entryID.ToString(); ;  //assign for receipt

                cmd.Parameters.Add("AMTPAID", amtPaid);
                cmd.Parameters.Add("AMTDUE", amtDue);
                cmd.Parameters.Add("BAL", bal);
                //cmd.Parameters.Add("DATE", DateTime.Now);
                //cmd.Parameters.Add("TIME", DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                cmd.Parameters.Add("TELLER", teller);
                cmd.Parameters.Add("CUSTOMERID", customerID);
                cmd.Parameters.Add("SALETYPE", saletype);
                cmd.Parameters.Add("AUTHCODE", authcode);
                cmd.Parameters.Add("POINTSEARNED", earned);
                cmd.Parameters.Add("AMTREDEEMED", redeemed);
                

                if (cmd.ExecuteNonQuery() > 0)
                {
                    result = true;
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while adding the sale.\n" + ex.Message, "ERROR");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred.\n" + ex.Message, "ERROR");
            }
            return result;
        }
        public static bool AddDetailedSale(string saleid, string itemId, string itemName, int qty, int amtDue, FbTransaction trans)
        {
            bool res = false;
            try
            {
                int entryID = GetMaxEntryID("DETSALES", "ENTRYID", trans);
                cmd = new FbCommand
             ("INSERT INTO DETSALES VALUES (@ENTID, @SALEID, @ITEMNAME, @QTY, @AMTDUE)", conn, trans);
                cmd.Parameters.Add("ENTID", ++entryID);
                cmd.Parameters.Add("SALEID", saleid);
                cmd.Parameters.Add("ITEMNAME", itemName);
                cmd.Parameters.Add("QTY", qty);
                cmd.Parameters.Add("AMTDUE", amtDue);

                cmd.ExecuteNonQuery();

                cmd = new FbCommand("UPDATE STOCK SET QUANTITY=QUANTITY-@QTY WHERE BARCODE=@ITEMID", conn, trans);
                cmd.Parameters.Add("ITEMID", itemId);
                cmd.Parameters.Add("QTY", qty);

                cmd.ExecuteNonQuery();
                res = true;
            }
            catch (FbException ex)
            {
                if (ex.ErrorCode == 335544558)
                {
                    MessageBox.Show("An error has occurred. The database refused to accept the sale of:\n" +
                        itemName + "\nIt may be that:\n1. You are trying to sale more than what is available of this item twice in one sale.\n" +
                                                "2. This item has just been depleted at another till while your sale was pending.\n", "ERROR",
                                                MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (ex.ErrorCode == 335544345)
                {
                    MessageBox.Show("An error has occurred. The database has refused to accept the sale of\n\t" +
                        itemName + "\n because this item is currently locked in a pending transaction.  ", "ERROR");
                }
                else
                    MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");

            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown  error occurred.\nDETAILS: " + ex.Message, "ERROR");
            }
            return res;
        }

        public static DataTable GetDetailedSales(DateTime from, DateTime to)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("SELECT r.SALEID, r.ITEMNAME, r.QUANTITY, r.AMOUNTDUE, cast(round(r.AMOUNTDUE*(1-1/s.FACTOR)) as integer)"
                    + " as PROFIT, (r.AMOUNTDUE * s.VAT/118.0) as VAT, a.TDATE FROM DETSALES r INNER JOIN SALES a ON r.SALEID=a.SALEID INNER JOIN STOCK s on s.ITEMNAME = r.ITEMNAME" +
                                     " where a.TDATE>=@FROM and a.TDATE<=@TO ORDER BY SALEID ASC", conn);
                cmd.Parameters.Add("FROM", from);
                cmd.Parameters.Add("TO", to);
                FbDataAdapter adp = new FbDataAdapter(cmd);

                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return dt;
        }

        

        public static SDataTable GetDetailedSales(string saleid)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM DETSALES r WHERE r.SALEID=@SALEID", conn);
                cmd.Parameters.Add("SALEID", saleid);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return dt;
        }
        public static DataTable GetDetailedLoyalty(string customerid, DateTime from, DateTime to)
        {
            SDataTable dt = new SDataTable();
            try
            {
                string query = "SELECT * FROM DETLOYALTY r WHERE r.TRANSDATE>=@FROM and r.TRANSDATE<=@TO";
                if (customerid.Trim() != "") query = query + " AND r.CUSTOMERID = @CUSTOMERID";
                cmd = new FbCommand(query, conn);
                if (query.Contains("CUSTOMERID"))
                {
                    cmd.Parameters.Add("CUSTOMERID", customerid);
                }
                cmd.Parameters.Add("FROM", from.AsFBDateTime());
                cmd.Parameters.Add("TO", to.AsFBDateTime());
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred retrieving Loyalty data.\n" + ex.Message, "ERROR");
            }
            return dt;
        }
        public static SDataTable GetSales(DateTime from, DateTime to, GlobalSystemData.SaleViewType viewType, string customerIDTin)
        {
            SDataTable dt = new SDataTable();
            try
            {
                if (viewType== GlobalSystemData.SaleViewType.All)
                {
                    cmd = new FbCommand("SELECT * FROM SALES p where p.TDATE>=@FROM and p.TDATE<=@TO", conn);
                }
                if(viewType == GlobalSystemData.SaleViewType.AllShowingVAT)
                {
                    cmd = new FbCommand("SELECT x.*, p.AMOUNTPAID, p.BALANCE, p.CUSTOMERID, p.TDATE, p.TTIME,p.TELLER FROM" +
                                       " (SELECT  r.SALEID, sum(r.AMOUNTDUE) as TOTAL, sum((r.AMOUNTDUE*s.VAT/118)) as VATAMT" +
                                       " FROM DETSALES r inner join STOCK s on s.ITEMNAME=r.ITEMNAME group by r.SALEID) x" +
                                       " inner join SALES p  on p.SALEID = x.SALEID where p.TDATE>=@FROM and p.TDATE<=@TO", conn);
                }
                if (viewType == GlobalSystemData.SaleViewType.TinCustomersOnly)
                {
                    cmd = new FbCommand("SELECT x.*, p.AMOUNTPAID, p.BALANCE, p.CUSTOMERID, p.TDATE, p.TTIME,p.TELLER, z.TIN FROM" +
                                       " (SELECT  r.SALEID, sum(r.AMOUNTDUE) as TOTAL, sum((r.AMOUNTDUE*s.VAT/118)) as VATAMT" +
                                       " FROM DETSALES r inner join STOCK s on s.ITEMNAME=r.ITEMNAME group by r.SALEID) x" +
                                       " inner join SALES p  on p.SALEID = x.SALEID inner join ROYALTY z on z.CUSTOMERID=p.CUSTOMERID"+
                                       " where p.TDATE>=@FROM and p.TDATE<=@TO and z.TIN is not null", conn);
                }
                if (viewType == GlobalSystemData.SaleViewType.SingleCustomerWithTIN)
                {
                    cmd = new FbCommand("SELECT x.*, p.AMOUNTPAID, p.BALANCE, p.CUSTOMERID, p.TDATE, p.TTIME,p.TELLER,z.TIN FROM" +
                                      " (SELECT r.SALEID, sum(r.AMOUNTDUE) as TOTAL, sum((r.AMOUNTDUE * s.VAT / 118)) as VATAMT" +
                                      " FROM DETSALES r inner join STOCK s on s.ITEMNAME = r.ITEMNAME group by r.SALEID) x" +
                                      " inner join SALES p on p.SALEID = x.SALEID inner join ROYALTY z on z.CUSTOMERID = p.CUSTOMERID" +
                                      " where (z.CUSTOMERID = @CUSTOMERID or z.TIN = @TIN) and  (p.TDATE>=@FROM and p.TDATE<=@TO)", conn);
                }

                cmd.Parameters.Add("FROM", from.AsFBDateTime());
                cmd.Parameters.Add("TO", to.AsFBDateTime());

                if(viewType == GlobalSystemData.SaleViewType.SingleCustomerWithTIN)
                {
                    cmd.Parameters.Add("CUSTOMERID", customerIDTin);
                    cmd.Parameters.Add("TIN", customerIDTin);
                }
                    FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return dt;
       }

        public static bool AddSupplier(string name)
        {
            FbTransaction trans = conn.BeginTransaction();
            bool res = false;
            try
            {
                cmd = new FbCommand("INSERT INTO SUPPLIERS VALUES (@NAME)", conn, trans);
                cmd.Parameters.Add("NAME", name);
                cmd.ExecuteNonQuery();

                trans.Commit();
                res = true;
            }
            catch (FbException ex)
            {
                trans.Rollback();
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return res;
        }
        public static bool AddLog(DateTime logDate, string details)
        {
            bool res=false;
            try
            {
                cmd = new FbCommand("INSERT INTO LOG VALUES (@DATE, @DETAIL)", conn);
                cmd.Parameters.Add("DATE", logDate.AsFBDateTime());
                cmd.Parameters.Add("DETAIL", details);
                if (cmd.ExecuteNonQuery() > 0)
                    res = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return res;
        }
        public static bool AddEmployee(string fname, string lname, string phone, string username, string password, int clearence)
        {
            bool res=false;
            try
            {
                cmd = new FbCommand("INSERT INTO EMPLOYEES VALUES (@FNAME, @LNAME, @PHONE, @USERNAME, @PASS, @CLEARENCE)", conn);
                cmd.Parameters.Add("FNAME", fname);
                cmd.Parameters.Add("LNAME", lname);
                cmd.Parameters.Add("PHONE", phone);
                cmd.Parameters.Add("USERNAME", username);
                cmd.Parameters.Add("PASS", password);
                cmd.Parameters.Add("CLEARENCE", clearence);
                if (cmd.ExecuteNonQuery() > 0) res = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return res;
        }

        internal static DataTable GetAdmins()
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM EMPLOYEES where CLEARENCE > 1", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return dt;
        }

        public static bool UpdateEmployee(string username, string phone, string password, int clearence)
        {
            bool res = false;
            try
            {
                cmd = new FbCommand("UPDATE EMPLOYEES SET PHONENUM= @PHONE, HPASS= @PASS,"+ 
                "CLEARENCE = @CLEARENCE WHERE USERNAME=@USERNAME", conn);
                cmd.Parameters.Add("PHONE", phone);
                cmd.Parameters.Add("USERNAME", username);
                cmd.Parameters.Add("PASS", password);
                cmd.Parameters.Add("CLEARENCE", clearence);
                if (cmd.ExecuteNonQuery() > 0) res = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return res;
        }
        public static string[] GetCredentials(string username)
        {
            string[] cred = new string[2];

                    cmd = new FbCommand("SELECT HPASS, CLEARENCE FROM EMPLOYEES WHERE USERNAME ='" + username + "'", conn);
                    FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    if (reader.Read())
                    {
                        cred[0] = reader.GetString(0);
                        cred[1] = reader.GetString(1);
                        reader.Close();
                    }

            return cred;

        }
        public static SDataTable GetAlerts(int expirywarn)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT r.ITEMNAME, r.QUANTITY, r.EXPIRYDATE, datediff(day from current_date to r.EXPIRYDATE)"+
                    " as DAYSTOEXPIRE FROM STOCK r where datediff(day from current_date to r.EXPIRYDATE)"+
                    " < "+ expirywarn + " and datediff(day from current_date to r.EXPIRYDATE) > -365 order by DAYSTOEXPIRE desc", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return dt;
        }
        public static List<DataRow> GetSearchResult(string query)
        {
            List<DataRow> searchResults = new List<DataRow>();
            try
            {
                List<DataTable> list = new List<DataTable>();
                List<string> tableNames = new List<string>();
                cmd = new FbCommand("select rdb$relation_name from rdb$relations where rdb$view_blr is null " +
                    "and (rdb$system_flag is null or rdb$system_flag = 0)", conn);
                FbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tableNames.Add(reader.GetString(0));
                }
                reader.Close();
                foreach (string tName in tableNames)
                {
                    string cmdText = "SELECT * FROM " + tName;
                    if (tName == "EMPLOYEES")
                    {
                        cmdText = "SELECT r.FNAME, r.LNAME, r.PHONENUM, r.USERNAME, r.CLEARENCE" +
                                                       " FROM EMPLOYEES r ";
                    }
                    FbDataAdapter adp = new FbDataAdapter(cmdText, conn);
                    DataTable table = new DataTable(tName);
                    adp.Fill(table);
                    if (table.Rows.Count > 0)
                    {
                        list.Add(table);
                    }
                }

                foreach (DataTable table in list)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        foreach (DataColumn col in table.Columns)
                        {
                            if (row[col].ToString().ToUpper().Contains(query.ToUpper()))
                            {
                                searchResults.Add(row);
                            }
                        }
                    }
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("A database error occurred. .\n" + ex.Message, "ERROR");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unkown error occurred.\n" + ex.Message, "ERROR");
            }
            return searchResults;
        }
        public static SDataTable GetLog(DateTime from, DateTime to)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM LOG a where a.LOGDATE>=@FROM and a.LOGDATE<=@TO", conn);
                cmd.Parameters.Add("FROM", from.AsFBDateTime());
                cmd.Parameters.Add("TO", to.AsFBDateTime());
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter= adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return dt;
            
        }
        public static int RunQuery(string query, out DataTable dt)
        {
            int affectedRows = 0;
            dt = new DataTable();
            try
            {
                cmd = new FbCommand(query, conn);
                if (query.Contains("SELECT"))
                {
                    FbDataAdapter adp = new FbDataAdapter(cmd);
                    adp.Fill(dt);
                }
                else
                {
                    affectedRows = cmd.ExecuteNonQuery();
                    if(query.Contains("CREATE"))
                    {
                        MessageBox.Show("CREATE TABLE Query was detected. The table will show the current list of tables.", 
                            "ATTENTION", MessageBoxButton.OK, MessageBoxImage.Information);
                        List<string> tableList = FBDataHelper.GetDBTables();
                        dt.Columns.Add("TABLE_LIST");
                        foreach(string item in tableList)
                        {
                            dt.Rows.Add(item);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return affectedRows;
        }

        internal static IEnumerable<string> GetCategories()
        {
            cmd = new FbCommand("SELECT distinct r.CATEGORY FROM STOCK r where r.CATEGORY is not null", conn);
            FbDataReader reader = cmd.ExecuteReader();
            List <string> categories = new List<string>();
            while( reader.Read())
            {
                categories.Add(reader.GetString(0));
            }
            return categories;
        }

        internal static DataTable GetPurchaseHistory(string itemName)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("select * FROM DETSTOCK WHERE ITEMNAME = @ITEM", conn);
                cmd.Parameters.Add("ITEM", itemName);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Columns.Add("COSTPPU");
                foreach (DataRow row in dt.Rows)
                {
                    row["COSTPPU"] = Math.Round(Convert.ToDouble(row["TOTAMT"]) / Convert.ToDouble(row["QUANTITY"]), 0);
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return dt;
        }
        internal static SDataTable GetRoyalty()
        {
            SDataTable dt = new SDataTable();
            try
            {
               cmd = new FbCommand("SELECT * FROM ROYALTY", conn);          
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return dt;

        }

        internal static DataRow GetCustomerData(string customerId)
        {
            SDataTable dt = new SDataTable();
            DataRow row = null;
            try
            {
                cmd = new FbCommand("SELECT * FROM ROYALTY where CUSTOMERID=@ID OR TIN=@ID ", conn);
                cmd.Parameters.Add("ID", customerId);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
                row = dt.Rows[0];
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred. The user may not exist in the database.\n" + ex.Message, "ERROR");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred. The scanned barcode may not exist in the database.\n"  + 
                  "Please consult admin \n"  + ex.Message, "ERROR");
            }
            return row;
        }
        public static bool SetPoints(string customerId, int points,  string saleid)
        {
            bool result = false;
            try
            {
                FbTransaction trans = conn.BeginTransaction();
                cmd = new FbCommand("UPDATE ROYALTY SET POINTS=POINTS+@POINTS where CUSTOMERID=@ID", conn, trans);
                cmd.Parameters.Add("ID", customerId);
                cmd.Parameters.Add("POINTS", points);
                if (cmd.ExecuteNonQuery() > 0)
                {

                    int balance = 0;
                    cmd = new FbCommand("SELECT * FROM ROYALTY WHERE CUSTOMERID=@CUSTOMERID", conn, trans);
                    cmd.Parameters.Add("CUSTOMERID", customerId);
                    FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                    if (reader.Read())
                    {
                        balance = Convert.ToInt32(reader["POINTS"].ToString());
                    }

                    int entryID = GetMaxEntryID("DETLOYALTY", "ENTRYID", trans);

                    cmd = new FbCommand("INSERT INTO DETLOYALTY VALUES(@ENTRYID, @CUSTOMERID, @EARNED, @SPENT, @BALANCE, @SALEID, @TRANSDATE,@TRANSTIME)", conn, trans);
                    cmd.Parameters.Add("ENTRYID", ++entryID);
                    cmd.Parameters.Add("CUSTOMERID", customerId);
                    if (points > 0) cmd.Parameters.Add("EARNED", points); else cmd.Parameters.Add("EARNED", DBNull.Value);
                    if (points <= 0) cmd.Parameters.Add("SPENT", -1 * points); else cmd.Parameters.Add("SPENT", DBNull.Value);
                    cmd.Parameters.Add("BALANCE", balance);
                    cmd.Parameters.Add("SALEID", saleid);
                    cmd.Parameters.Add("TRANSDATE", DateTime.Now.AsFBDateTime());
                    cmd.Parameters.Add("TRANSTIME", DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
                    if (cmd.ExecuteNonQuery() > 0)
                    {
                        trans.Commit();
                        result = true;
                    }
                    else trans.Rollback();
                }
                else
                    trans.Rollback();
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while updating the customers points.\n" + ex.Message, "ERROR");
            }
            return result;
        }

        public static SDataTable GetSettings()
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT * FROM GENERAL", conn);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
            return dt;
        }

        static string backUpMessage = "";
        public static void BackUp()
        {
            try
            {
                backUpMessage = "";
                FirebirdSql.Data.Services.FbBackup svc = new FirebirdSql.Data.Services.FbBackup();
            svc.ConnectionString = connstr2;
            svc.BackupFiles.Add(new FirebirdSql.Data.Services.FbBackupFile("bak." +DateTime.Now.AsFBDateTime() + ".fbk", 2048));
            svc.Verbose =true;
            svc.Options = FirebirdSql.Data.Services.FbBackupFlags.IgnoreLimbo;
                svc.ServiceOutput += Svc_ServiceOutput;
            svc.Execute();
            MessageBox.Show("Back up Complete", "BACKUP", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        private static void Svc_ServiceOutput(object sender, FirebirdSql.Data.Services.ServiceOutputEventArgs e)
        {
            backUpMessage += e.Message + "\n";
        }

        public static SDataTable CrossCheckSales(DateTime from, DateTime to)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT z.*, (Z.TOTALINDETAILED-Z.TOTALINSUMMARY) as DIFF" +
                    " FROM (SELECT x.*, s.AMOUNTDUE as TOTALINSUMMARY, s.TDATE, s.TTIME, s.TELLER" +
                    " FROM (SELECT r.SALEID, sum(r.AMOUNTDUE) as TOTALINDETAILED FROM DETSALES r" +
                    " group by r.SALEID) x inner join SALES s on s.SALEID=x.SALEID) z WHERE z.TDATE>=@FROM and z.TDATE<=@TO", conn);
                cmd.Parameters.Add("FROM", from.AsFBDateTime());
                cmd.Parameters.Add("TO", to.AsFBDateTime());

                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);        
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
            return dt;
        }

        internal static int GetAvailableQty(string barcode)
        {
            int qty = 0;
            try
            {
                cmd = new FbCommand("SELECT QUANTITY FROM STOCK WHERE BARCODE=@BARCODE", conn);
                cmd.Parameters.Add("BARCODE", barcode);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    qty = reader.GetInt32(0);
                }
            }
            catch (Exception ex)
            {
                qty = 0;
            }
            return qty;
        }        
    }
}