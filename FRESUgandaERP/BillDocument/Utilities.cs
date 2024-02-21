using System;
using System.Windows;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Media;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Data;

namespace FRESUGERP.AppUtilities
{

    public static class UtilityExtensions
    {
        public static Session currentSession = null;
        public static string As3Digits(int x)
        {
            if (x.ToString().Length == 1) return "00" + x;
            if (x.ToString().Length == 2) return "0" + x;
            else return x.ToString();
        }
        public static string As2Digits(int x)
        {
            if (x.ToString().Length == 1) return "0" + x;
            else return x.ToString();
        }
        public static string As4Digits(int x)
        {
            if (x.ToString().Length == 1) return "000" + x;
            if (x.ToString().Length == 2) return "00" + x;
            if (x.ToString().Length == 3) return "0" + x;
            else return x.ToString();
        }
        public static string As6Digits(int x)
        {
            if (x.ToString().Length == 1) return "00000" + x;
            if (x.ToString().Length == 2) return "0000" + x;
            if (x.ToString().Length == 3) return "000" + x;
            if (x.ToString().Length == 4) return "00" + x;
            if (x.ToString().Length == 5) return "0" + x;
            else return x.ToString();
        }
        public static int[] Decompose(int totbillamt, int servCurr)
        {
            int[] ar = new int[4];
            ar[0] = ar[1] = ar[2] = ar[3] = 0;
            ar[0] = servCurr;
            ar[1] = ((totbillamt - servCurr) >= servCurr) ? servCurr : ((totbillamt - servCurr) > 0 ? (totbillamt - servCurr) : 0);
            ar[2] = ((totbillamt - 2 * servCurr) >= servCurr) ? servCurr : ((totbillamt - 2 * servCurr) > 0 ? (totbillamt - 2 * servCurr) : 0);
            ar[3] = (totbillamt - 3 * servCurr) > 0 ? (totbillamt - 3 * servCurr) : 0;
            return ar;
        }
        public static int[] DecomposeMtn(int age)
        {
            int[] ar = new int[3];
            ar[0] = ar[1] = ar[2] = 0;
            ar[0] = (age > 0 && age <=180) ? age : (age > 180) ? 180 : 0;
            ar[1] = (age > 180 && age <= 360) ? age - 180 : age > 360 ? 180 : 0;
            ar[2] = (age >= 360) ? age - 360 : 0;
            return ar;
        }
        public static int GetBillAge(this int[] decomposed)
        {
            int i = 0;
            for (i = 0; i < decomposed.Length; ++i)
            {
                if (decomposed[i] == 0)
                {
                    break;
                }
            }
            return i * 30;
        }
        
      //  public static DataTable 
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr gdiObject);

        public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
        {
            IntPtr gdiObject = source.GetHbitmap();

            BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(gdiObject,
                IntPtr.Zero, Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(gdiObject); //release
            return bitmapSource;
        }
        public static string AsFBDateTime(this DateTime dateTime)
        {
            return dateTime.Day + "." + As2Digits(dateTime.Month) + "." + dateTime.Year;
        }
        public static string GetSelectedItemContent(this ComboBox cbx)
        {
            return (cbx.SelectedItem as ComboBoxItem).Content.ToString();
        }
        public static int GetSum(this DataColumn col)
        {
            int sum = 0;
            foreach (DataRow row in col.Table.Rows)
            {
                if(!row[col].Equals(DBNull.Value))
                sum += Convert.ToInt32(row[col]);
            }
            return sum;
        }
        public static string MapEnergyCentre(string centre)
        {
            string value = "";
            if (centre == "RWAMPARA") value = "1";
            else if (centre == "ISINGIRO") value = "2";
            else if (centre == "KASHARI") value = "3";
            else if (centre == "BUSHENYI") value = "4";
            else value = "ALL";
            return value;
        }

      
    }
}
