using System;
using GenericUtilities;

namespace Temp
{
    class Program
    {
        static void Main(string[] args)
        {
            object[] items = { 2, "k", 4.5, "f" };
            foreach(var item in items)
            {
                try
                {
                    int f = 2 * Convert.ToInt16(item);
                    Console.WriteLine(f);
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
