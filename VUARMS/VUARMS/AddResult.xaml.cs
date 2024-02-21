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

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for AddResult.xaml
    /// </summary>
    public partial class AddResult : Window
    {
        string VUREFNO = "", ccode="";
        int cw = 0, exam = 0, total = 0;
        public AddResult(string vuRefNo, string name)
        {
            InitializeComponent();
            txtDetails.Text = "Add Result for " + vuRefNo + ": " + name;
            VUREFNO = vuRefNo;
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ccode = txtCourse.Text;
                cw = Convert.ToInt32(txtCW.Text);
                exam = Convert.ToInt32(txtExam.Text);
                total = Convert.ToInt32(txtTotal.Text);
               int affRows= VUARMSFBDataHelper.InsertResult(VUREFNO, ccode, cw, exam, total, "");
               if (affRows > 0)
               {
                   VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName, AppUtilities.sessionID, "Student Result Change: " + VUREFNO + ": " + " ADDED: " + total);
                   MessageBox.Show("The result was added.", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
               }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred. \nDETAILS: "
                    + ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void txtCourse_GotFocus(object sender, RoutedEventArgs e)
        {
            DataViewer viewer = new DataViewer("COURSES");
            if (viewer.ShowDialog() == true)
            {
                System.Data.DataRowView drv = viewer.SelectedItem;
                txtCourse.Text = drv.Row["COURSECODE"].ToString();
            }
        }
    }
}
