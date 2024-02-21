using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Data;
using FirebirdSql.Data.FirebirdClient;

namespace StockApp
{
    static class FBDataHelper
    {
        static string connstr = "User=SYSDBA;Password=peps1c0;Database=127.0.0.1:" + System.AppDomain.CurrentDomain.BaseDirectory + @"stock.fdb";
        static FbConnection conn = null;
        public static FbCommand cmd = null;
        public  enum StockType {Basic, Detailed};


        public static bool OpenConnection()
        {
            bool result = false;
            try
            {
                conn = new FbConnection(connstr);
                conn.Open();
                result = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return result;
        }
        public static bool CloseConnection()
        {
            bool result = false;
            try
            {
                if (!conn.State.Equals(ConnectionState.Closed))
                {
                    conn.Close();
                    result = true;
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return result;
        }
        public static Int64 [] GetStockValue()
        {
            Int64[] values = new Int64[2];
            try
            {
                    cmd = new FbCommand("SELECT sum(r.COSTPPU*r.QUANTITY), sum(r.COSTPPU*r.QUANTITY*r.FACTOR) FROM STOCK r", conn);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    values[0] = (Int64)(reader.GetDouble(0));
                    values[1] = (Int64)(reader.GetDouble(1));
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message);
            }
            return values;
        }
      /*  public static Int64 GetTotalCredit()
        {
            Int64 values = 0;
            try
            {
                cmd = new FbCommand("SELECT sum(r.BALANCE) FROM DETSTOCK r", conn);
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

        }*/
        public static Int64 GetTotalExpenses(DateTime from, DateTime to)
        {
            Int64 values = 0;
            try
            {
                cmd = new FbCommand("SELECT sum(r.AMTPAID) FROM EXPENSES r WHERE r.TDATE>=@FROM AND r.TDATE<=@TO", conn);
                cmd.Parameters.Add("FROM", from);
                cmd.Parameters.Add("TO", to);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        values = (Int64)(reader.GetDouble(0));
                    }
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message);
            }
            return values;

        }
        public static SDataTable GetExpenses(DateTime from, DateTime to)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM EXPENSES r WHERE r.TDATE>=@FROM AND r.TDATE<=@TO", conn);
                cmd.Parameters.Add("FROM", from);
                cmd.Parameters.Add("TO", to);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message);
            }
            return dt;

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
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message);
            }
            return values;

        }
        public static Int64 GetTotalProfit(DateTime from, DateTime to)
        {
            Int64 values = 0;
            try
            {
                cmd = new FbCommand("SELECT sum(g.PROFIT) FROM (SELECT r.SALEID, r.DRUGNAME, r.QUANTITY, r.AMOUNTDUE, s.FACTOR,"+
                    " (r.AMOUNTDUE-r.AMOUNTDUE/s.FACTOR) as PROFIT FROM DETSALES r INNER JOIN STOCK s on s.DRUGNAME=r.DRUGNAME) g"+
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
        public static SDataTable GetStock(StockType stockType, DateTime from, DateTime to)
        {
            SDataTable dt = new SDataTable();
            FbDataAdapter adp = null;
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
                FbDataAdapter adp = new FbDataAdapter("SELECT FNAME, LNAME, PHONENUM, USERNAME FROM EMPLOYEES WHERE USERNAME != 'superadmin'", conn);
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
        public static string GetGroup(string drugName)
        {
           string result = "";
            try
            {
                cmd = new FbCommand("SELECT DGROUP FROM STOCK WHERE DRUGNAME LIKE '%"+drugName+"%'", conn);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                reader.Read();
                if (!reader.IsDBNull(0))
                {
                    result = reader.GetString(0);
                }
                reader.Close();
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return result;
        }
        public static int UpdateTable(DataTable dt, FbDataAdapter adp)
        {
            int affRows = 0;
            try
            {
                FbCommandBuilder cmdb = new FbCommandBuilder(adp);
                affRows= adp.Update(dt);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unknown error occurred.\n + DETAIL: " + ex.Message, " ERROR");
            }
            return affRows;
        }
        
        public static bool AddStock(string entryId, string drugName, string batchNo, 
            string supplier, string invoiceNum, int qty, float costppu, float factor, DateTime expDate,
            DateTime dateOfEntry, string pType, bool drugExists)
        {
            bool result = false;
            FbTransaction trans = null;
            try
            {
                trans = conn.BeginTransaction(IsolationLevel.Serializable);
                if (drugExists)
                {
                    cmd = new FbCommand("UPDATE STOCK SET COSTPPU=@COSTPPU, FACTOR = @FACTOR," +
                        "QUANTITY=QUANTITY+@QUANTITY,EXPIRYDATE=@EXPIRYDATE WHERE DRUGNAME=@DRUGNAME", conn, trans);
                }
                else
                {
                    cmd = new FbCommand("INSERT INTO STOCK (DRUGNAME, COSTPPU, FACTOR,QUANTITY,EXPIRYDATE) " +
                        "VALUES (@DRUGNAME, @COSTPPU, @FACTOR,@QUANTITY,@EXPIRYDATE)", conn, trans);
                }

                cmd.Parameters.Add("DRUGNAME", drugName);
                cmd.Parameters.Add("COSTPPU", costppu);
                cmd.Parameters.Add("FACTOR", factor);
                cmd.Parameters.Add("QUANTITY", qty);
                cmd.Parameters.Add("EXPIRYDATE", expDate.AsFBDateTime());

                cmd.ExecuteNonQuery();


                cmd = new FbCommand("INSERT INTO DETSTOCK VALUES (@ENTRYID, @DRUGNAME, @BATCHNO," +
                     "@SUPPLIER, @INVOICENUM, @QTY, @TOTAMT, @PURCHASE,  @DATEOFENTRY)", conn, trans);
                int totAmt = (int)(qty*costppu);

                cmd.Parameters.Add("ENTRYID", entryId);
                cmd.Parameters.Add("DRUGNAME", drugName);
                cmd.Parameters.Add("BATCHNO", batchNo);
                cmd.Parameters.Add("SUPPLIER", supplier);
                cmd.Parameters.Add("INVOICENUM", invoiceNum);
                cmd.Parameters.Add("QTY", qty); 
                cmd.Parameters.Add("TOTAMT", totAmt);
                cmd.Parameters.Add("PURCHASE", pType);
                cmd.Parameters.Add("DATEOFENTRY", DateTime.Now.AsFBDateTime());
           
                cmd.ExecuteNonQuery();

                trans.Commit();
                result = true;
            }
            catch (FbException ex)
            {
                trans.Rollback();
                MessageBox.Show("An error occurred.\n" + ex.Message,"ERROR");
            }
            return result;
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
        public static bool AddSale(string drugName, int amtDue, int amtPaid, int bal, string dispenser, char insurance, out string saleid)
        {
            bool res = false;
            saleid = "";
            try { 
            int entryID = GetMaxEntryID("SALES", "SALEID");
            cmd = new FbCommand
         ("INSERT INTO SALES VALUES (@ENTID, @AMTDUE,@AMTPAID, @BAL, @DATE, @TIME, @DISPENSER,@INSURANCE)", conn);
            cmd.Parameters.Add("ENTID", ++entryID);
            saleid = entryID.ToString(); ;  //assign for receipt

            cmd.Parameters.Add("AMTPAID", amtPaid);
            cmd.Parameters.Add("AMTDUE", amtDue);
            cmd.Parameters.Add("BAL", bal);
            cmd.Parameters.Add("DATE", DateTime.Now.AsFBDateTime());
            cmd.Parameters.Add("TIME", DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
            cmd.Parameters.Add("DISPENSER", dispenser);
            cmd.Parameters.Add("INSURANCE", insurance);

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

        internal static bool ResetPassword(string username, string hpassword)
        {

            bool res = false;
            try
            {
                cmd = new FbCommand("UPDATE EMPLOYEES SET HPASS= @PASS WHERE USERNAME=@USERNAME", conn);
                cmd.Parameters.Add("USERNAME", username);
                cmd.Parameters.Add("PASS", hpassword);
                if (cmd.ExecuteNonQuery() > 0) res = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return res;
        }

        public static bool AddDetailedSale(string saleid, string drugName, int qty, int amtDue)
        {
            bool res = false;
            FbTransaction trans = null;
            try
            {
                int entryID = GetMaxEntryID("DETSALES", "ENTRYID");
                trans = conn.BeginTransaction();
                cmd = new FbCommand
             ("INSERT INTO DETSALES VALUES (@ENTID, @SALEID, @DRUGNAME, @QTY, @AMTDUE)", conn, trans);
                cmd.Parameters.Add("ENTID", ++entryID);
                cmd.Parameters.Add("SALEID", saleid);
                cmd.Parameters.Add("DRUGNAME", drugName);
                cmd.Parameters.Add("QTY", qty);
                cmd.Parameters.Add("AMTDUE", amtDue);

                cmd.ExecuteNonQuery();

                cmd = new FbCommand("UPDATE STOCK SET QUANTITY=QUANTITY-@QTY WHERE DRUGNAME=@DRUGNAME", conn, trans);
                cmd.Parameters.Add("DRUGNAME", drugName);
                cmd.Parameters.Add("QTY", qty);

                cmd.ExecuteNonQuery();
                trans.Commit();
                res = true;
            }
            catch (FbException ex)
            {
                trans.Rollback();
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return res;
        }
        public static bool AddExpense(string item, int amount, DateTime date)
        {
            bool res=false;
            int entryID = GetMaxEntryID("EXPENSES", "ENTRYID");
            try
            {
                cmd = new FbCommand("INSERT INTO EXPENSES VALUES (@ENTID, @ITEM, @AMT, @DATE)", conn);
                cmd.Parameters.Add("ENTID", ++entryID);
                cmd.Parameters.Add("ITEM", item);
                cmd.Parameters.Add("AMT", amount);
                cmd.Parameters.Add("DATE", date.AsFBDateTime());
                int aff = cmd.ExecuteNonQuery();
                if (aff > 0) res = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return res;
        }

        public static DataTable GetDetailedSales(DateTime from, DateTime to)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("SELECT r.SALEID, r.DRUGNAME, r.QUANTITY, r.AMOUNTDUE, cast(round(r.AMOUNTDUE*(1-1/s.FACTOR)) as integer)"
                    + " as PROFIT, a.TDATE FROM DETSALES r INNER JOIN SALES a ON r.SALEID=a.SALEID INNER JOIN STOCK s on s.DRUGNAME = r.DRUGNAME" +
                                     " where a.TDATE>=@FROM and a.TDATE<=@TO", conn);
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
        public static SDataTable GetSales(DateTime from, DateTime to)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM SALES a where a.TDATE>=@FROM and a.TDATE<=@TO", conn);
                cmd.Parameters.Add("FROM", from);
                cmd.Parameters.Add("TO", to);
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
        public static DataTable GetInsuranceSales(DateTime from, DateTime to)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM SALES a where a.TDATE>=@FROM and a.TDATE<=@TO AND INSURANCE='Y'", conn);
                cmd.Parameters.Add("FROM", from);
                cmd.Parameters.Add("TO", to);
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
 
        public static DataTable GetSchema(string dbTableName, out FbDataAdapter adapter)
        {
            DataTable dt = new DataTable();
            FbDataAdapter adp = null;
            try
            {
                cmd = new FbCommand("SELECT * FROM " + dbTableName, conn);
                adp = new FbDataAdapter(cmd);
                adp.FillSchema(dt, SchemaType.Source);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            adapter = adp;
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
                cmd = new FbCommand("INSERT INTO EMPLOYEES VALUES (@ID, @FNAME, @LNAME, @PHONE, @USERNAME, @PASS, @CLEARENCE)", conn);
                cmd.Parameters.Add("ID", new GenericUtilities.Random().RandomNumericString(10));
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
        public static string[] GetCredentials(string username)
        {
            string[] cred = new string[2];
            try
            {
                cmd = new FbCommand("SELECT HPASS, CLEARENCE FROM EMPLOYEES WHERE USERNAME ='" + username + "'", conn);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    cred[0] = reader.GetString(0);
                    cred[1] = reader.GetString(1);
                    reader.Close();
                }
            }
           catch (FbException ex)
            {
                MessageBox.Show("An error occurred. This user may not exist in the database.\n" + ex.Message,"ERROR");
            }
            return cred;

        }
        public static List<string> GetListOfInvoices()
        {
            List<string> list = new List<string>();
            try
            {               
                cmd = new FbCommand("SELECT DISTINCT INVOICENUM FROM DETSTOCK", conn);
                FbDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        list.Add(reader.GetString(0));
                    }
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching credit invoices.", "ERROR");
            }
            return list;
        }

        public static SDataTable GetAlerts()
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT r.DRUGNAME, r.QUANTITY, r.EXPIRYDATE FROM STOCK r where r.QUANTITY=0  or " +
                " datediff(day from current_date to r.EXPIRYDATE)<60", conn);
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
                    FbDataAdapter adp = new FbDataAdapter("SELECT * FROM " + tName, conn);
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
                MessageBox.Show("An error occurred. This user may not exist in the database.\n" + ex.Message, "ERROR");
            }
            return dt;
            
        }
        public static DataTable RunQuery(string query)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand(query, conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                cmd.ExecuteNonQuery();
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred. This user may not exist in the database.\n" + ex.Message, "ERROR");
            }
            return dt;
        }
        public static DataTable GetPaymentDetails()
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("SELECT g.*,p.PAID,p.BALANCE,p.PAYMENTDATE,p.REF from ( SELECT  r.SUPPLIER," +
                    " sum(r.COSTPRICE) as ACCUMCREDIT FROM DETSTOCK r where r.PURCHASE='CREDIT' group by r.SUPPLIER) g" +
                    " LEFT JOIN PAYMENTS p on g.SUPPLIER=p.SUPPLIER", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
              
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred. This user may not exist in the database.\n" + ex.Message, "ERROR");
            }
            return dt;
        }
        public static SDataTable GetStatement(string supplier)
        {
            DataTable statement = new DataTable();
            statement.Columns.Add("DATE"); statement.Columns["DATE"].DataType = typeof(DateTime);
            statement.Columns.Add("TRANSACTION");
            statement.Columns.Add("AMOUNT");
            statement.Columns.Add("REFERENCE");

            DataTable purchases = GetPurchases(supplier);
            DataTable payments = GetPayments(false, supplier, false);

            List<string> distinctInvoices = FBDataHelper.GetListOfInvoices();

            if (purchases.Rows.Count > 0)
            {
                try
                {
                    foreach (string invoice in distinctInvoices)
                    {
                        //get totals
                        EnumerableRowCollection<DataRow> matchingRows = (from myRows in purchases.AsEnumerable()
                                                                         where myRows["INVOICENUM"].ToString() == invoice.ToString()
                                                                         select myRows);

                        if (matchingRows.Count() > 0)
                        {
                            int sum = matchingRows.GetColumnTotal("COSTPRICE");
                            DataRow sampleRow = matchingRows.ElementAt(0);
                            string transType = "";
                            if (sampleRow["PURCHASE"].ToString() == "CREDIT") transType = "PURCHASE (CREDIT)";
                            else
                                transType = "PURCHASE (CASH)";
                            statement.Rows.Add(sampleRow["TDATE"], transType, sum, invoice);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while retrieving invoices for this supplier.\n" + ex.Message, "ERROR");
                }
            }
                foreach (DataRow row in payments.Rows)
                {
                    statement.Rows.Add(row["PAYMENTDATE"], "PAYMENT", row["PAID"], row["REF"]);
                }
                statement.DefaultView.Sort = "DATE DESC";

                return statement.DefaultView.ToTable().AsSDataTable();
        }


        private static DataTable GetPurchases(string supplier)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM DETSTOCK g WHERE g.SUPPLIER=@SUPPLIER", conn);
                cmd.Parameters.Add("SUPPLIER", supplier);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred. This user may not exist in the database.\n" + ex.Message, "ERROR");
            }
            return dt;   
        }
        public static Int64 GetTotalCredit(string supplier)
        {
            Int64 credit = 0;
            try
            {
                cmd = new FbCommand("SELECT sum(r.COSTPRICE) from DETSTOCK r where r.PURCHASE='CREDIT' and r.SUPPLIER=@SUP", conn);
                cmd.Parameters.Add("SUP", supplier);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                reader.Read();
                if (!reader.IsDBNull(0))
                {
                   credit = reader.GetInt32(0);
                }
            }
            catch (InvalidOperationException ex)  //there are no data
            {
               credit= 0;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred. This user may not exist in the database.\n" + ex.Message, "ERROR");
            }
            return credit;
        }
       /* public static DataRow GetCurrentCreditState(string supplier)
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("select * from PAYMENTS r where r.SUPPLIER=@SUPPLIER and r.PAYMENTDATE="+
                    "(select max(r.PAYMENTDATE) from PAYMENTS r) AND r.BALANCE = (select min(r.BALANCE) from PAYMENTS r)", conn);
                cmd.Parameters.Add("SUPPLIER", supplier);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            if (dt.Rows.Count > 0)
                return dt.Rows[0];
            else
                return null;
        }*/
        public static SDataTable GetPayments(bool neglectSupplier, string supplier, bool getMostRecentOnly)
        {
            SDataTable dt = new SDataTable();
            try
            {
                if (neglectSupplier)
                {
                    cmd = new FbCommand("select * from PAYMENTS order by PAYMENTDATE desc", conn);
                }
                else
                {
                    if (getMostRecentOnly)
                    {
                        cmd = new FbCommand("SELECT * FROM PAYMENTS r  where r.SUPPLIER=@SUPPLIER and r.PAYMENTDATE=" +
                            "(select max(r.PAYMENTDATE) from PAYMENTS r)", conn);
                        cmd.Parameters.Add("SUPPLIER", supplier);
                    }
                    else
                    {
                        cmd = new FbCommand("SELECT * FROM PAYMENTS r  where r.SUPPLIER=@SUPPLIER", conn);
                        cmd.Parameters.Add("SUPPLIER", supplier);
                    }
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
        public static bool MakePayment(string supplier, Int64 amount,  string reference)
        {
            bool result = false;
            int id = GetMaxEntryID("PAYMENTS", "PAYMENTID");
            try
            {
                    cmd = new FbCommand("INSERT INTO PAYMENTS VALUES(@ID, @SUP, @PAID,@DATE,@REF)", conn);
                    cmd.Parameters.Add("ID", ++id);    
                    cmd.Parameters.Add("SUP", supplier);
                    cmd.Parameters.Add("PAID", amount);
                    cmd.Parameters.Add("DATE", DateTime.Now.AsFBDateTime());
                    cmd.Parameters.Add("REF", reference);
                    int affRows = cmd.ExecuteNonQuery();
                    if (affRows > 0) result = true;
                }
              
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while processing this payment.\n" + ex.Message, "ERROR");
                FBDataHelper.AddLog(DateTime.Now, "Error occured in MakePayment Module.---" + ex.Message);
            }
            return result;
        }

       /* private static DataTable GetAccumulatedPayments()
        {
            DataTable dt = new DataTable();
            try
            {
                cmd = new FbCommand("select r.SUPPLIER, sum(r.PAID) as TOTPAYMENTS  from PAYMENTS r group by r.SUPPLIER", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred. This user may not exist in the database.\n" + ex.Message, "ERROR");
            }
            return dt;

        }*/
       /* public static bool HasAtleastOnePayment(string supplier, out DataRow mostRecentPayment)
        {
            bool result = false;
            mostRecentPayment = null;
            FbDataAdapter adp = new FbDataAdapter("SELECT * FROM PAYMENTS r where r.PAYMENTDATE=" +
                                " (select min(r.PAYMENTDATE) from PAYMENTS r) and r.BALANCE="+
                                " (select min(r.BALANCE) from PAYMENTS r)", conn);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                mostRecentPayment = dt.Rows[0];
                result = true;
            }
            return result;
        }*/
      /*  public static void Import()
        {
            OpenConnection();
            DataTable dt = Utilities.ReadCSVFile("druglist.csv");
            DataTable errors = dt.Clone();
            cmd = new FbCommand("INSERT INTO STOCK VALUES (@DRUGNAME, @COSTPPU,@FACTOR,@QUANTITY,@EXPIRYDATE,@GROUP)", conn);
            foreach (DataRow row in dt.Rows)
            {
                try
                {
                    cmd.Parameters.Add("DRUGNAME", row["DRUGNAME"]);
                    cmd.Parameters.Add("COSTPPU", row["COSTPPU"]);
                    cmd.Parameters.Add("FACTOR", row["FACTOR"]);
                    cmd.Parameters.Add("QUANTITY", row["QUANTITY"]);
                    cmd.Parameters.Add("EXPIRYDATE", DBNull.Value);
                    cmd.Parameters.Add("GROUP", row["GROUP"]);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                catch (Exception ex)
                {
                    errors.ImportRow(row);
                    continue;
                }
            }
            Utilities.WriteCSVFile(errors, "errors.csv");
        }*/

        internal static SDataTable GetPurchases(DateTime from, DateTime to)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM DETSTOCK a where a.TDATE>=@FROM and a.TDATE<=@TO", conn);
                cmd.Parameters.Add("FROM", from.AsFBDateTime());
                cmd.Parameters.Add("TO", to.AsFBDateTime());
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred. This user may not exist in the database.\n" + ex.Message, "ERROR");
            }
            return dt;
        }
    }
}
