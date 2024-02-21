using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace StockApp
{
    /// <summary>
    /// Interaction logic for ChartOptions.xaml
    /// </summary>
    public partial class ChartOptions : Window
    {
        public GraphSettings graphSettings = null;
        private bool _windowReady = false;

        public ChartOptions(GraphSettings graphSettings)
        {
            InitializeComponent();
            this.graphSettings = graphSettings;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_windowReady)
            {
                graphSettings.MovingAveragePeriod = (int)periodSlider.Value;
                lblMVA.Content = "Moving Average Period = " + graphSettings.MovingAveragePeriod + " days";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            graphSettings.MovingAveragePeriod = (int)periodSlider.Value;
            _windowReady = true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            graphSettings.IsMovingAverageEnabled = chkMVAEnabled.IsChecked.Value;
            graphSettings.IsMainSeriesEnabled = chkMainEnabled.IsChecked.Value;
            this.DialogResult = true;
            this.Close();
        }
    }
    public class GraphSettings
    {
        public Int32 MovingAveragePeriod { get; internal set; }
        public Boolean IsMovingAverageEnabled { get; internal set; }
        public bool IsMainSeriesEnabled { get; internal set; }

        public GraphSettings()
        {

        }
    }
}
