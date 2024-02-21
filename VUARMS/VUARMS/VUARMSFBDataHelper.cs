using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using FirebirdSql.Data.FirebirdClient;
using GenericUtilities;

namespace VUARMS
{
    static class VUARMSFBDataHelper
    {
        // static string connstr = @"User=SYSDBA;Password=peps1c0;Database=10.0.20.103:/home/vuresults/vuresults.fdb";
      static string connstr = @"User=SYSDBA;Password=peps1c0;Database=127.0.0.1:D:\Dropbox\code\databases\VURESULTS.FDB";
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
                MessageBox.Show("An error occurred. Make sure the database file exists on" +
                    " the server and the path is correct.\nThe current properties are:\n" +
                "HOST=" + conn.DataSource + "\nPATH=" + conn.Database + "\n\n TECHNICAL DETAILS: " + e.Message, "ERROR");
            }
            return result;
        }

        internal static List<string> GetBatches()
        {
            List<string> batches = new List<string>();
            try
            {
                cmd = new FbCommand("select distinct(BATCH) from RESULTS", conn);
                FbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    batches.Add(reader.GetString(0));
                }
                reader.Close();
                batches.Add("ALL");
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return batches;
        }

        internal static void AddLog(string userName, string session, string action)
        {
            try
            {
                int id = GetNextEntryID("SESSIONS");
                cmd = new FbCommand("INSERT INTO SESSIONS VALUES(@ENTID,@SID,@USERID,@ACTIONS,CURRENT_TIMESTAMP)", conn);
                cmd.Parameters.Add("ENTID", id);
                cmd.Parameters.Add("SID", session);
                cmd.Parameters.Add("USERID", userName);
                cmd.Parameters.Add("ACTIONS", action);
                cmd.ExecuteNonQuery();
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
        }

        internal static SDataTable GetStudents(string course)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT  * FROM STUDENTS WHERE PROGRAMME='" + course + "'", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                ShowErrorWindow(ex);
            }
            return dt;
        }

        public static int GetNextEntryID(string tableName)
        {
            int id = 10000;
            try
            {
                cmd = new FbCommand("SELECT MAX (ENTRYID) FROM " + tableName, conn);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        id = reader.GetInt32(0);
                    }
                    reader.Close();
                }
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return ++id;
        }

        internal static int GetTuitionBalance(string vuRefNo)
        {
            return 0;
        }

        public static CurrentUser LookUp(string username)
        {
            CurrentUser currentUser = new CurrentUser();
            FbDataAdapter adp = new FbDataAdapter("SELECT * FROM STAFF WHERE USERNAME='" + username + "'", conn);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            try
            {
                if (dt.Rows.Count > 0)
                {
                    currentUser.WasFound = true;
                    currentUser.UserName = username;
                    currentUser.CLRLevel = Convert.ToInt16(dt.Rows[0]["CLEARENCE"]);
                    currentUser.Designation = dt.Rows[0]["DESIGNATION"].ToString();
                    currentUser.HPass = dt.Rows[0]["HPASS"].ToString();
                    currentUser.Faculty = dt.Rows[0]["FACULTY"].ToString();

                }
               /* else
                {
                    adp = new FbDataAdapter("SELECT * FROM STUDENTS WHERE VUREFNO='" + username + "'", conn);
                    adp.Fill(dt);
                    if (dt.Rows.Count > 0)  //found in students
                    {
                        currentUser.WasFound = true;
                        currentUser.UserName = username;
                        currentUser.CLRLevel = 1;
                        currentUser.Designation = dt.Rows[0]["DESIGNATION"].ToString();
                        currentUser.HPass = dt.Rows[0]["HPASS"].ToString();
                    }
                    else
                        currentUser.WasFound = false;
                }  */
            }
            catch (Exception ex)
            {
                MessageBox.Show("The user was not found. The system cannot log you on." +
                    "\nEnsure that your username exists and that the password is correct.\nDETAILS:\n" + ex.Message,
                    "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return currentUser;

        }

        internal static int GetResultCount(string code)
        {
            int res = 0;
            try
            {
                cmd = new FbCommand("SELECT COUNT (COURSECODE) FROM RESULTS WHERE COURSECODE=@CODE", conn);
                cmd.Parameters.Add("CODE", code);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                reader.Read();
                res = reader.GetInt32(0);
                reader.Close();
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return res;
        }

        public static bool UpdateUser(string username, string password)
        {
            bool res = false;
            try
            {
                cmd = new FbCommand("UPDATE STAFF SET HPASS= @PASS WHERE USERNAME=@USERNAME", conn);
                cmd.Parameters.Add("USERNAME", username);
                cmd.Parameters.Add("PASS", password);
                if (cmd.ExecuteNonQuery() > 0) res = true;
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message);
            }
            return res;
        }

        public static DataTable GetResults(string vurefNo, bool filterPassedRetakes, string batch)
        {
            SDataTable dt = new SDataTable();
            try
            {

                string query = "SELECT r.ENTRYID, r.COURSECODE,c.COURSENAME,c.CU, r.TOTAL, r.RT, r.BATCH FROM RESULTS r" +
                             " inner join COURSES c on c.COURSECODE=UPPER(r.COURSECODE) where r.VUREFNO=@VUREFNO";
                if (batch != "ALL")
                {
                    query += " AND r.BATCH=@BATCH ";
                }
                cmd = new FbCommand(query + " order BY C.COURSECODE ASC", conn);
                cmd.Parameters.Add("VUREFNO", vurefNo);
                if (query.Contains("@BATCH"))
                {
                    cmd.Parameters.Add("@BATCH", batch);
                }
                FbDataAdapter adp = new FbDataAdapter(cmd);
                dt.Adapter = adp;
                adp.Fill(dt);
                //remove fail entries of passed retakes
                if (filterPassedRetakes)
                {
                    List<DataRow> passed = new List<DataRow>();
                    List<DataRow> failed = new List<DataRow>();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row.Field<int>("TOTAL") >= 50)
                        {
                            passed.Add(row);
                        }
                        else
                            failed.Add(row);
                    }

                    DataRowEqualityComparer comparer = new DataRowEqualityComparer();

                    for (int i = 0; i < failed.Count; i++)
                    {
                        if (passed.Contains(failed[i], comparer))      //module was passed upon retake
                        {
                            failed[i].Delete();
                        }             
                    }
                }

            }
            catch (FbException ex)
            {
                ShowErrorWindow(ex);
            }

            dt.AcceptChanges();
            return dt;
        }

        public static SDataTable GetAllResults()
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT  *  FROM RESULTS r order by r.ENTRYID", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                dt.Adapter = adp;
                adp.Fill(dt);
            }
            catch (FbException ex)
            {
                ShowErrorWindow(ex);
            }
            return dt;
        }
        public static SDataTable GetAllStudents()
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT  * FROM STUDENTS", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                ShowErrorWindow(ex);
            }
            return dt;
        }
        public static SDataTable GetStudentDetails(string vuRefNo)
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM STUDENTS r WHERE r.VUREFNO=@REFNO", conn);
                cmd.Parameters.Add("REFNO", vuRefNo);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                ShowErrorWindow(ex);
            }
            return dt;
        }
        public static void ShowErrorWindow(Exception ex)
        {
            MessageBox.Show("An error occurred.\nDETAILS: " + ex.Message + "\nInner:\n" + ex.StackTrace, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static int ChangeCodes()
        {
            cmd = new FbCommand("SELECT * FROM COURSES r where r.COURSECODE like '%CSC%'", conn);
            FbDataAdapter adp = new FbDataAdapter(cmd);
            DataTable dt = new DataTable();
            adp.Fill(dt);
            string code, newcode;
            int count = 0;
            foreach (DataRow row in dt.Rows)
            {
                code = row["COURSECODE"].ToString();
                newcode = code.Remove(0, 3).Insert(0, "BCS");
                cmd = new FbCommand("UPDATE COURSES SET COURSECODE=@NEWCODE WHERE COURSECODE=@OLDCODE", conn);
                cmd.Parameters.Add("@NEWCODE", newcode);
                cmd.Parameters.Add("@OLDCODE", code);
                count += cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            return count;
        }
        public static void ImportStudents(string filename)
        {
            DataTable results = Utilities.ReadCSVFile(filename);
            DataTable errors = results.Clone();
            int entries = 0;

            foreach (DataRow row in results.Rows)
            {
                try
                {
                    string refNo = row["VUREFNO"].ToString().Trim().Replace(" ", "");
                    if (GetStudentDetails(refNo).Rows.Count > 0) //student exists, update
                    {
                        cmd = new FbCommand("UPDATE STUDENTS SET FULLNAME=@FULLNAME,GENDER=@GENDER,NATIONALITY=@NATIONALITY,PROGRAMME=@PROGRAMME," +
                            "PHONE=@PHONE,STUDYMODE=@STUDYMODE,EMAIL=@EMAIL WHERE VUREFNO=@VUREFNO", conn);
                        cmd.Parameters.Add("@VUREFNO", refNo);
                        cmd.Parameters.Add("@FULLNAME", row["FULLNAME"]);
                        cmd.Parameters.Add("@GENDER", row["GENDER"]);
                        cmd.Parameters.Add("@NATIONALITY", row["NATIONALITY"]);
                        cmd.Parameters.Add("@PROGRAMME", row["PROGRAMME"]);
                        cmd.Parameters.Add("@STUDYMODE", row["STUDYMODE"]);
                        cmd.Parameters.Add("@PHONE", row["PHONE"]);
                        cmd.Parameters.Add("@EMAIL", row["EMAIL"]);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        ++entries;

                    }
                    else
                    {

                        cmd = new FbCommand("INSERT INTO STUDENTS (VUREFNO,FULLNAME,GENDER,NATIONALITY,PROGRAMME,PHONE,STUDYMODE,EMAIL)" +
                        " VALUES(@VUREFNO,@FULLNAME,@GENDER,@NATIONALITY,@PROGRAMME,@PHONE,@STUDYMODE,@EMAIL)", conn);

                        cmd.Parameters.Add("@VUREFNO", refNo);
                        cmd.Parameters.Add("@FULLNAME", row["FULLNAME"]);
                        cmd.Parameters.Add("@GENDER", row["GENDER"]);
                        cmd.Parameters.Add("@NATIONALITY", row["NATIONALITY"]);
                        cmd.Parameters.Add("@PROGRAMME", row["PROGRAMME"]);
                        cmd.Parameters.Add("@STUDYMODE", row["STUDYMODE"]);
                        cmd.Parameters.Add("@PHONE", row["PHONE"]);
                        cmd.Parameters.Add("@EMAIL", row["EMAIL"]);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        ++entries;
                    }
                }
                catch (Exception ex)
                {
                    errors.ImportRow(row);
                }
            }
            MessageBox.Show(entries + " entries added", "SUCCESS");
            if (errors.Rows.Count > 0)
            {
                Utilities.WriteCSVFile(errors, System.IO.Path.GetDirectoryName(filename) + @"\_errors.csv");
            }
        }


        internal static SDataTable GetCourses()
        {
            SDataTable dt = new SDataTable();
            try
            {
                cmd = new FbCommand("SELECT * FROM COURSES", conn);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (FbException ex)
            {
                ShowErrorWindow(ex);
            }
            return dt;
        }

        private static bool HasRetake(string vuRefNo, string courseCode, out int origMark)
        {
            origMark = 0;
            cmd = new FbCommand("SELECT * FROM RESULTS WHERE VUREFNO=@VUREFNO AND COURSECODE=@CODE", conn);
            cmd.Parameters.Add("VUREFNO", vuRefNo);
            cmd.Parameters.Add("CODE", courseCode);
            FbDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                origMark = reader.GetInt32(5);
                return true;
            }
            else
                return false;
        }


        internal static int InsertResult(string vuRefNo, string coursecode, int cw, int exam, int total, string batchId)
        {
            int affRows = 0, orig = 0;
            string error = "", RT = "N";
            string nextId = AppUtilities.As6Digits(GetNextEntryID("RESULTS")).ToString();
            bool hasRetake = HasRetake(vuRefNo, coursecode, out orig);

            if (hasRetake)
            {
                RT = "Y";
            }
            try
            {
                cmd = new FbCommand("INSERT INTO RESULTS VALUES (@NEXTID, @VUREFNO, @CODE, @CW, @EXAM, @TOTAL, @RT, @BATCH)", conn);
                cmd.Parameters.Add("NEXTID", nextId);
                cmd.Parameters.Add("VUREFNO", vuRefNo);
                cmd.Parameters.Add("CODE", coursecode);
                cmd.Parameters.Add("CW", cw);
                cmd.Parameters.Add("EXAM", exam);
                cmd.Parameters.Add("TOTAL", total);
                cmd.Parameters.Add("RT", RT);
                cmd.Parameters.Add("BATCH", batchId);
                affRows = cmd.ExecuteNonQuery();

            }
            catch (FbException ex)
            {
                error = vuRefNo + "," + coursecode + " , " + ex.Message;
                AppUtilities.errors.Add(error);
            }
            return affRows;
        }

        internal static List<string> GetCourseCodes()
        {
            List<string> codesAvailable = new List<string>();
            cmd = new FbCommand("SELECT COURSECODE FROM COURSES", conn);
            FbDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                codesAvailable.Add(reader.GetString(0));
            }
            return codesAvailable;
        }

        internal static void ResetAllStudentPasswords()
        {
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT * FROM STUDENTS", conn);
                SDataTable dt = new SDataTable();
                adp.Fill(dt);
                dt.Adapter = adp;

                foreach (DataRow row in dt.Rows)
                {
                    row["HPASS"] = Hasher.CreateHash(row["USERNAME"].ToString());
                }
                int affRows = dt.UpdateSource();
            }
            catch (Exception ex)
            {
                ShowErrorWindow(ex);
            }
        }

        public static int AddVUREFNO(string item, string name)
        {
            int affRows = 0;
            try
            {
                cmd = new FbCommand("INSERT INTO STUDENTS (VUREFNO, FULLNAME) VALUES (@VUREFNO, @FULLNAME)", conn);
                cmd.Parameters.Add("VUREFNO", item);
                cmd.Parameters.Add("FULLNAME", name);
                affRows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ShowErrorWindow(ex);
            }
            return affRows;
        }

        public static IEnumerable<string> GetFaculties()
        {
            List<string> faculties = new List<string>();
            try
            {
                cmd = new FbCommand("select distinct(DEPT) from COURSES", conn);
                FbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    faculties.Add(reader.GetString(0));
                }
                reader.Close();
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return faculties;
        }

        internal static IEnumerable<string> GetCohorts()
        {
            List<string> faculties = new List<string>();
            try
            {
                cmd = new FbCommand("select distinct(COHORT) FROM (SELECT substring" +
                    "(trim('VU' from r.VUREFNO) from 1 for 4) as COHORT from STUDENTS r)", conn);
                FbDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    faculties.Add(reader.GetString(0));
                }
                reader.Close();
                faculties.Add("ALL");
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return faculties;
        }

        public static DataTable GetRawGrid(string faculty, string cohort, string batch)
        {
            DataTable dt = new DataTable();
            //1. get all the results that will be in grid
            StringBuilder cmdText = new StringBuilder();
            cmdText.Append("SELECT x.* FROM" +
                 " (SELECT r.VUREFNO, z.PROGRAMME,p.DEPT, r.COURSECODE, s.COURSENAME, r.TOTAL, r.BATCH" +
                 " FROM RESULTS r inner join COURSES s on s.COURSECODE=r.COURSECODE inner join STUDENTS z" +
                 " on z.VUREFNO = r.VUREFNO inner join PROGRAMS p on p.PROGNAME = z.PROGRAMME) x"+
                 " where x.DEPT ='" + faculty + "'");

            if (cohort != "ALL")
            {
                cmdText.Append(" AND x.VUREFNO like '%" + cohort + "%'");
            }

            if (batch != "ALL")
            {
                cmdText.Append(" and x.BATCH = '" + batch + "'");
            }

            cmdText.Append(" order by x.PROGRAMME, COURSECODE ");

            cmd = new FbCommand(cmdText.ToString(), conn);
            FbDataAdapter adp = new FbDataAdapter(cmd);
            adp.Fill(dt);
            return dt;
        }


        internal static string GetCourseName(string code)
        {
            string name = "";
            try
            {
                cmd = new FbCommand("SELECT COURSENAME FROM COURSES WHERE COURSECODE='" + code + "'", conn);
                FbDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                reader.Read();

                if (!reader.IsDBNull(0))
                {
                    name = reader.GetString(0);
                }
                reader.Close();
            }
            catch (FbException ex)
            {
                MessageBox.Show("An error occurred.\n" + ex.Message, "ERROR");
            }
            return name;
        }

        internal static DataTable GetStudents()
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT * FROM STUDENTS", conn);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (Exception ex)
            {
                ShowErrorWindow(ex);
            }
            return dt;
        }
        internal static DataTable GetStaff()
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT * FROM STAFF", conn);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (Exception ex)
            {
                ShowErrorWindow(ex);
            }
            return dt;
        }

        internal static DataTable GetTranscriptDetails(string vurefNo)
        {
            SDataTable dt = new SDataTable();
            try
            {
                FbDataAdapter adp = new FbDataAdapter("SELECT r.VUREFNO, r.FULLNAME, r.GENDER, r.DOB, r.NATIONALITY, r.PROGRAMME, p.FULLNAME as PFNAME ," +
                    " p.DEPT FROM STUDENTS r inner join PROGRAMS p on r.PROGRAMME = p.PROGNAME where r.VUREFNO='" + vurefNo + "'", conn);
                adp.Fill(dt);
                dt.Adapter = adp;
            }
            catch (Exception ex)
            {
                ShowErrorWindow(ex);
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

        internal static int ChangeResult(string vurefno, string coursecode, int newresult)
        {
            int affRows = 0;
            try
            {
                cmd = new FbCommand("UPDATE RESULTS SET TOTAL=@TOT WHERE VUREFNO=@VUREFNO AND COURSECODE=@CCODE", conn);
                cmd.Parameters.Add("TOT", newresult);
                cmd.Parameters.Add("VUREFNO", vurefno);
                cmd.Parameters.Add("CCODE", coursecode);

                affRows = cmd.ExecuteNonQuery();

            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + e.Message, "ERROR");
            }
            return affRows;
        }

        internal static int DeleteResult(string vurefno, string coursecode, int result)
        {
            int affRows = 0;
            try
            {
                //get how many
                cmd = new FbCommand("SELECT * FROM RESULTS WHERE VUREFNO=@VUREFNO AND COURSECODE=@CCODE AND TOTAL=@RESULT", conn);
                cmd.Parameters.Add("VUREFNO", vurefno);
                cmd.Parameters.Add("CCODE", coursecode);
                cmd.Parameters.Add("RESULT", result);
                FbDataAdapter adp = new FbDataAdapter(cmd);
                SDataTable dt = new SDataTable();
                dt.Adapter = adp;
                adp.Fill(dt);

                int rowCount = dt.Rows.Count;
                if (rowCount == 1)  //only one result
                {
                    dt.Rows[0].Delete();
                    affRows = dt.UpdateSource();
                }
                else
                {
                    if ((Convert.ToInt32(dt.Rows[0]["TOTAL"]) == Convert.ToInt32(dt.Rows[1]["TOTAL"]))
                        & (dt.Rows[0]["RT"].ToString() == "Y"))
                    {
                        dt.Rows[0].Delete();
                        affRows = dt.UpdateSource();
                    }
                    if ((Convert.ToInt32(dt.Rows[0]["TOTAL"]) == Convert.ToInt32(dt.Rows[1]["TOTAL"]))
                        & (dt.Rows[1]["RT"].ToString() == "Y"))
                    {
                        dt.Rows[1].Delete();
                        affRows = dt.UpdateSource();
                    }
                }
            }
            catch (FbException e)
            {
                MessageBox.Show("An error occurred.\nDETAILS: " + e.Message, "ERROR");
            }
            return affRows;
        }
    }

    class DataRowEqualityComparer : IEqualityComparer<DataRow>
    {
        public bool Equals(DataRow row1, DataRow row2)
        {
            if (row1["COURSECODE"].ToString().Trim() == row2["COURSECODE"].ToString().Trim())
                return true;
            else
                return false;
        }
        public int GetHashCode(DataRow row)
        {
            int hCode = row.Field<int>("TOTAL") ^ new GenericUtilities.Random().Next(1, 99);
            return hCode.GetHashCode();
        }
    }
}
