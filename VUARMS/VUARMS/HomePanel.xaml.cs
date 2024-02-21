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
    public partial class HomePanel : UserControl
    {
        public string name, desig, vuRefNo, spDuties, citizenship, dob, email, phone, academic;

       /* public HomePanel(string name, string desig, string vuRefNo, string spDuties, string citizenship, 
            string dob, string email, string phone, string academic)
        {
            InitializeComponent();

            this.name = name;
            this.desig = desig;
            this.vuRefNo = vuRefNo;
            this.spDuties = spDuties;
            this.citizenship = citizenship;
            this.dob = dob;
            this.email = email;
            this.phone = phone;
            this.academic = academic;
        }*/

        public HomePanel()
        {
            InitializeComponent();
        }
        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            lblRefNo.Content += vuRefNo;
            lblName.Content += name;
            lblDOB.Content += dob;
            lblSpDuty.Content += spDuties;
            lblCitizen.Content += citizenship;
            lblDesig.Content += desig;
            lblPhone.Content += phone;
            lblEmail.Content += email;
            lblAcademic.Content = academic;
        }
    }
}
