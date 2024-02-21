using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VUARMS
{
    class CurrentUser
    {
        public String UserName { get; set; }
        public Int16 CLRLevel { get; set; }
        public String Designation { get; set; }
        public String HPass { get; set; }
        public bool WasFound { get; set; }
        public String Faculty { get; set; }

        public CurrentUser()
        {
            
        }
    }
}
