using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AppExtensions.Data;
using System.Windows;

namespace Test
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
          System.Data.DataTable dt= FileUtilities.ReadCSVFile("data.csv");
          AppExtensions.Data.Windows.LabelTable(dt, new Thickness(1),12,"Arial",30,new int[] {30,30,30,30,30,30,30 }, 
              new string []{"L", "L", "L", "L", "L", "L", "L", "L" });
            Console.WriteLine("Finished");
           
        }
    }
}
