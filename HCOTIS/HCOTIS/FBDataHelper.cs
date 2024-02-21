using FirebirdSql.Data.FirebirdClient;
using System;
using System.Data;
using System.Windows;
using GenericUtilities;

namespace HCOTIS
{
    internal class FBDataHelper
    {
        static string connstr = "User=SYSDBA;Password=peps1c0;Database=127.0.0.1:" + AppDomain.CurrentDomain.BaseDirectory + @"HCOTIS.FDB";

        static FbConnection conn = null;
        static FbCommand cmd = null;
        static FbTransaction trans = null;

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
                MessageBox.Show("An error occurred. Make sure the database file exists on" +
                    " the server and the path is correct.\nThe current properties are:\n" +
                "HOST=" + conn.DataSource + "\nPATH=" + conn.Database + "\n\n TECHNICAL DETAILS: " + e.Message, "ERROR");
            }
            return result;
        }

        internal static SDataTable GetUsers()
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT r.USERNAME, r.CLRLEVEL FROM USERS r WHERE r.USERNAME !='SUPERADMIN'", conn);
                dt.Adapter = adp;
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR");
            }
            return dt;
        }

        internal static DataTable GetUpcomingEvents()
        {
            DataTable dt = new DataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT r.CLIENTNAME, r.PHONENO,r.EVENTTYPE, r.EVENTDATE," +
                    "(365 - datediff(day from r.EVENTDATE to current_date)) as DAYSLEFT FROM ORDERS r " +
                    " where 365 - datediff(day from r.EVENTDATE to current_date) <= 30", conn);
                
                adp.Fill(dt);
            
            }
            catch(Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dt;
        }

        internal static void AddPayment(string orderId, int amtPaid, int newbal, string payType)
        {
            try
            {
                string nextId = GetNextId("ENTRYID", "PAYMENTS");
                trans = conn.BeginTransaction(IsolationLevel.Serializable);
                cmd = new FbCommand("INSERT INTO PAYMENTS VALUES (@ENTRYID,@ORDERID,@AMTPAID,@NEWBAL,@DATE,@PAYTYPE)", conn, trans);
                cmd.Parameters.Add("ENTRYID", nextId);
                cmd.Parameters.Add("ORDERID", orderId);
                cmd.Parameters.Add("AMTPAID", amtPaid);
                cmd.Parameters.Add("NEWBAL", newbal);
                cmd.Parameters.Add("DATE", DateTime.Now);
                cmd.Parameters.Add("PAYTYPE", payType);
                cmd.ExecuteNonQuery();

                //update orders balance
                cmd = new FbCommand("UPDATE ORDERS SET BALANCE=@BAL WHERE ORDERID=@ORDERID", conn, trans);
                cmd.Parameters.Add("BAL", newbal);
                cmd.Parameters.Add("ORDERID", orderId);
                cmd.ExecuteNonQuery();
                trans.Commit();
                MessageBox.Show("The transaction finished successfully", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                MessageBox.Show("The transaction failed to complete.\nDETAILS:\n" + ex.Message,
                    "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        internal static bool LookUp(string username, out string hpass, out int clr)
        {
            FbDataAdapter adp = new FbDataAdapter("SELECT * FROM USERS WHERE USERNAME='" + username + "'", conn);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            bool wasFound = false;
            hpass = "";     clr = 1;
            try
            {
                if (dt.Rows.Count > 0)
                {
                    wasFound = true;
                    hpass = dt.Rows[0]["PASSHASH"].ToString();
                    clr = Convert.ToInt32(dt.Rows[0]["CLRLEVEL"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("The user was not found. The system cannot log you on." +
                    "\nEnsure that your username exists and that the password is correct.\nDETAILS:\n" + ex.Message,
                    "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return wasFound;
        }
        public static SDataTable GetOrders(DateTime from, DateTime to, bool all)
        {
            SDataTable dt = new SDataTable();
            try
            {
                if (all)
                {
                    cmd = new FbCommand("SELECT * FROM ORDERS ORDER BY ORDERDATE DESC", conn);
                }
                else
                {
                    cmd = new FbCommand("SELECT * FROM ORDERS where ORDERDATE>=@FROM and ORDERDATE<=@TO ORDER BY ORDERDATE DESC", conn);
                    cmd.Parameters.Add("FROM", from);
                    cmd.Parameters.Add("TO", to);
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

        internal static SDataTable GetPayments(DateTime from, DateTime to)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM PAYMENTS WHERE DATETIME>=@FROM AND DATETIME<=@TO", conn);
                cmd.Parameters.Add("FROM", from);
                cmd.Parameters.Add("TO", to);
               
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return dt;
        }

        internal static string GetNextId(string column, string tableName)
        {
            int id = 0;
            try
            {
                cmd = new FbCommand("SELECT MAX (" + column + ") FROM " + tableName, conn);
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
            return As6Digits(id + 1);
        }

        internal static int AddOrder(string nextId, int invoiceNum, string clientName, string phoneno, string eventType, DateTime eventDate,
            int numofguests, DateTime pickUpDate, string theme, string shape, string size, string flavor, string icing,
            string color, string narration, string accessories, string location, DateTime now, int cost, int deposit, int balance, string teller)
        {
            int affrows = 0;
            try
            {
                cmd = new FbCommand("INSERT INTO ORDERS VALUES(@NEXTID,@INVNUM,@CNAME,@PHONE,@ETYPE,@EDATE,@GUESTS," +
                    "@PDATE,@THEME,@SHAPE,@SIZE,@FLAVOR,@ICING,@COLOR,@NARR,@ACC,@LOC,@NOW,@COST,@DEP,@BAL,@TELLER)", conn);
                cmd.Parameters.Add("NEXTID", nextId);
                cmd.Parameters.Add("INVNUM", invoiceNum);
                cmd.Parameters.Add("CNAME", clientName);
                cmd.Parameters.Add("PHONE", phoneno);
                cmd.Parameters.Add("ETYPE", eventType);
                cmd.Parameters.Add("EDATE", eventDate);
                cmd.Parameters.Add("GUESTS", numofguests);
                cmd.Parameters.Add("PDATE", pickUpDate);
                cmd.Parameters.Add("THEME", theme);
                cmd.Parameters.Add("SHAPE", shape);
                cmd.Parameters.Add("SIZE", size);
                cmd.Parameters.Add("FLAVOR", flavor);
                cmd.Parameters.Add("ICING", icing);
                cmd.Parameters.Add("COLOR", color);
                cmd.Parameters.Add("NARR", narration);
                cmd.Parameters.Add("ACC", accessories);
                cmd.Parameters.Add("LOC", location);
                cmd.Parameters.Add("NOW", now);
                cmd.Parameters.Add("COST", cost);
                cmd.Parameters.Add("DEP", deposit);
                cmd.Parameters.Add("BAL", balance);
                cmd.Parameters.Add("TELLER", teller);

                affrows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred with one of the fields that expects a number. Please review.\n DETAILS: "
                    + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return affrows;

        }

        private static string As6Digits(int x)
        {
            if (x.ToString().Length == 1) return "00000" + x;
            if (x.ToString().Length == 2) return "0000" + x;
            if (x.ToString().Length == 3) return "000" + x;
            if (x.ToString().Length == 4) return "00" + x;
            if (x.ToString().Length == 5) return "0" + x;
            else return x.ToString();
        }

        internal static SDataTable GetPayments(string orderId)
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = null;
                if (orderId != "")
                {
                    adp = new FbDataAdapter("SELECT * FROM PAYMENTS WHERE ORDERID='" + orderId + "'", conn);
                }
                else
                {
                    adp = new FbDataAdapter("SELECT * FROM PAYMENTS ", conn);
                }

                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred.\n DETAILS: " + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dt;
        }
        public static bool AddUser(string username, string pass, int clr)
        {
            string hash = Hasher.CreateHash(pass);
            bool result = false;
            try
            {
                cmd = new FbCommand("INSERT INTO USERS VALUES (@USERNAME,@CLR,@HASH)", conn);
                cmd.Parameters.Add("USERNAME", username.ToUpper());
                cmd.Parameters.Add("HASH", hash);
                cmd.Parameters.Add("CLR", clr);
                if (cmd.ExecuteNonQuery() > 0) result = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred while adding the user into the database.\n" + ex.Message, "ERROR");
            }
            return result;
        }
    }
}