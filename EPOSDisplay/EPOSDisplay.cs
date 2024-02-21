using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace EPOSDisplay
{
    public static class EPOSDisplay
    {
        static SerialPort sp = null;
        public static bool Initialize(string comPort)
        {
            bool res = false;
            try
            {
                sp = new SerialPort(comPort, 9600);
                if (!sp.IsOpen)
                {
                    sp.Open();
                    res = true;
                }
                else
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred initializing the display.\nDETAILS: " + ex.Message);
            }
            return res;
        }


        public static void WriteLine(string text)
        {
            try
            {
                if (text.Length >= 16)
                {
                    text = text.Substring(0, 16);
                }
                sp.WriteLine(text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred writing to the display.\nDETAILS: " + ex.Message);
            }
        }

        public static void Clear()
        {
            try
            {
                sp.WriteLine("                 ");
                sp.WriteLine("                 ");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred clearing to the display.\nDETAILS: " + ex.Message);
            }
        }
    }
}
