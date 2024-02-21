using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows.Markup;
using System.Globalization;
using System.Windows;
using System.Threading;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
          //  Application.Current.Exit += Current_Exit;
        }

        static void Current_Exit(object sender, ExitEventArgs e)
        {
           // EPOSDisplay.Close();
        }
    }
}
