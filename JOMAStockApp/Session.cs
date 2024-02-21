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
        private string hash = "";

        public String UserName
        {
            get { return username; }
        }
        public Int32 CLRlevel
        {
            get { return clrlevel; }
        }
        public String Hash
        {
            get { return hash; }
            set { hash = value; }
        }
        public User(string username, int clrlevel, string hash)
        {
            this.username = username;
            this.clrlevel = clrlevel;
            this.hash = hash;
        }

        public bool IsStandardUser()
        {
            if (clrlevel == 1) return true;
            else return false;
        }
        public bool IsManager()
        {
            if (clrlevel > 1) return true;
            else return false;
        }
        public bool IsAdmin()
        {
            if (clrlevel > 2) return true;
            else return false;
        }
        public bool IsSuperAdmin()
        {
            if (clrlevel > 3) return true;
            else return false;
        }
		// Override ToString method to return username
		public override string ToString()
		{
			return username;
		}

	}

    public class Session
    {        
        private DateTime start;
        private DateTime end;
        private User currentUser; 

        public String Log = "";
        public Object temp = null;  //temporary object to hold ANYTHING!!

        public Int32 OpenSaleWindows = 0;
        
        public User CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; }
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
        public String CurrentCustomer
        {
            get;
            set;
        }
        public Session(User user, DateTime start)
        {
            this.currentUser = user;
            this.start = start;
            Log += "USER: "+ user.UserName + "\nSession Started at " + start + "\n";
        }
    }
}
