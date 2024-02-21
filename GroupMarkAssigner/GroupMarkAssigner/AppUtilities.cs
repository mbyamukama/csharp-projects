using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GroupMarkAssigner
{
    public static class AppUtilities
    {
        public static bool ContainsAnyOf(this String s, string[] items)
        {
            bool result = false;
            foreach (string st in items)
            {
                if (s.Contains(st) && !(st == null || st == string.Empty))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

    }
}
