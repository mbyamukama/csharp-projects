using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using System.Data;
using CEDAT.MathLab;

namespace HerTiming
{

  class Program
  {
    static void Main(string[] args)
    {
      List<DateTime> dates = new List<DateTime>();
      System.IO.StreamReader sr = new System.IO.StreamReader("dates.bin");
      string line = "";
      while ((line = sr.ReadLine()) != null)
      {
          dates.Add(DateTime.Parse(line));
      }

      double[] durations = new double[dates.Count - 1];
      int actualCount = dates.Count;

      int k = 0;
      for (k = 1; k < dates.Count; k++)
      {
        durations[k - 1] = dates[k].Subtract(dates[k - 1]).Days;
        Console.WriteLine("PERIOD " + k + ": " + durations[k - 1]);
      }
      Console.WriteLine();
      k = 0;

      double avCycleDuration = durations.Average();
      double stdDev = Statistics.STD(durations);

      Console.WriteLine("AVD:\t{0} days\nSTD  :\t{1} days", Math.Round(avCycleDuration, 2), Math.Round(stdDev, 2));

      for (k = dates.Count; k < 21; k++)
      {
        dates.Insert(k, dates[k - 1].AddDays(avCycleDuration));
      }

      Console.WriteLine("\n MONTH \t\tSTART DATE\n");
      foreach (DateTime item in dates)
      {
        if (dates.IndexOf(item) > actualCount-1)
        {
          string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(item.Month);
          DateTime peak = item.AddDays(-14.0);
          Console.WriteLine(" " + monthName + "\t\t\t" + item.Day + "\t ABSTAIN from\t" + item.AddDays(7).ToShortDateString() + " until "
              + item.AddDays(18).ToShortDateString());

        }

      }
      Console.ReadLine();
    }
  }
}
