using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JMHotel
{
    public class Session
    {
        public String UserName
        {
            get;
            set;
        }
        public Int32 CLRLevel
        {
            get;
            set;
        }
        public DateTime StartTime
        {
            get;
            set;
        }

        public Session(string username, int clrLevel, DateTime startTime)
        {
            UserName = username;
            CLRLevel = clrLevel;
            StartTime = startTime;
        }
    }
}
