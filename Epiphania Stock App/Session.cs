using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockApp
{
   public class User
    {
        
        private string username;
        private int clrlevel = 1;

        public String UserName
        {
            get { return username; }
        }
        public Int32 CLRlevel
        {
            get { return clrlevel; }
        }
        public User(string username, int clrlevel)
        {
            this.username = username;
            this.clrlevel = clrlevel;

        }
    }

    public class Session
    {        
        
        private DateTime start;
        private DateTime end;
        private User user;
        public Boolean IsPrintingEnabled = true;
        public String currentDbName = "";
        
        public User User
        {
            get { return user; }
            set { user = value; }
        }
        public DateTime StartTime
        {
            get { return start; }
        }
        public DateTime EndTime
        {
            get { return end; }
            set { end = value; }
        }
        public Session(User user, DateTime start)
        {
            this.user = user;
            this.start = start;
        }
    }
}
