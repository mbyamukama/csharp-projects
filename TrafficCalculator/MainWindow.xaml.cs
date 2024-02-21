using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace TrafficCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = null;
        int count = 0;
        int seconds = 0, minCounter=0, prevPeoplePerMin= 0;
        DateTime now;
        TimeSpan elapsed;
        double rate = 0;
        DataTable dt = null;
        public MainWindow()
        {
            InitializeComponent();
            dt = new DataTable();
            dt.Columns.Add("DATE");
            dt.Columns.Add("COUNT");
            dt.Columns.Add("CUMULATIVE");
            dt.Columns.Add("RATE");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            now = DateTime.Now;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            seconds++;
            lblSysTime.Content = "System time: " +  DateTime.Now.ToString("dd-MM-yyyy, HH:mm:ss");
            elapsed= DateTime.Now.Subtract(now);
            lblTimeElapsed.Content = "Time Elapsed: "+ elapsed.Hours.ToString("00") + ":" + elapsed.Minutes.ToString("00") + ":" + elapsed.Seconds.ToString("00");
            lblHumanCount.Content = "Count: " + count;

            if (elapsed.Seconds > 0)
            {
                lblRate.Content = "Traffic Rate: " + (rate = Math.Round(count * 1.0 / elapsed.TotalSeconds, 2)).ToString("0.0") + " per sec" + "     (" + (rate * 60).ToString("00.0") + " per min)";
            }

            if(seconds>30)
            {
                seconds = 0;
                dt.Rows.Add(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"), count -prevPeoplePerMin, count, rate*60);
                prevPeoplePerMin = count;
            }
            
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                ++count;
                lblHumanCount.Content = "Count: " + count;
                
            }
            if(e.Key==Key.Back)
            {
                --count;
                lblHumanCount.Content = "Count: " + count;
            }
        }

        private void MenuReset_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you wish to reset?", "Reset Data", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                count = seconds = minCounter = prevPeoplePerMin = 0;
                rate = 0;
                now = DateTime.Now;
                elapsed = new TimeSpan();
                dt.Clear();
            }

        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MenuRawData_Click(object sender, RoutedEventArgs e)
        {
            TableWindow tw = new TableWindow(dt);
            tw.ShowDialog();
        }

        private void BtnTimerStop_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
        }

        private void BtnMinus_Click(object sender, RoutedEventArgs e)
        {
            --count;
            lblHumanCount.Content = "Count: " + count;
        }

        private void BtnPlus_Click(object sender, RoutedEventArgs e)
        {
            ++count;
            lblHumanCount.Content = "Count: " + count;
        }
    }
}
