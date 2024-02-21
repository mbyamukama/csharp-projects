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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for HomePanel.xaml
    /// </summary>
    public partial class Transcript : UserControl
    {
        public string name, desig, vuRefNo, citizenship, dob, faculty, prog;
        //NB: A4 paper size is 797 * 1123 pixels on 96DPI
        public Transcript()
        {
            InitializeComponent();
        }
    }
}
