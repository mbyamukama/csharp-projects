
using GenericUtilities;
using System.Windows;

namespace VUARMS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void btnResults_Click(object sender, RoutedEventArgs e)
        {
            if (AppUtilities.CurrentUser.CLRLevel > 1)
            {
                new ViewResults().Show();
            }
        }

        private void btnStudents_Click(object sender, RoutedEventArgs e)
        {            
                new ViewStudents().Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("Closing this window will terminate this session. \nAre you sure you'd like to close this window?",
                                 "CONFIRM", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                VUARMSFBDataHelper.AddLog(AppUtilities.CurrentUser.UserName, AppUtilities.sessionID, "Logged out.");
                Application.Current.Shutdown();
            }
        }

        private void btnCourses_Click(object sender, RoutedEventArgs e)
        {
            if (AppUtilities.CurrentUser.CLRLevel > 1)
            {
                new CourseInfo().Show();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            homePanel.name = AppUtilities.CurrentUser.UserName;
            homePanel.desig = AppUtilities.CurrentUser.Designation;


            /* if (!AppUtilities.CurrentUser.IsStaff) // this is a student
             {
                 dt = VUARMSFBDataHelper.GetStudentDetails(AppUtilities.CurrentUser.UserName);
                 string vuRefNo = dt.Rows[0]["VUREFNO"].ToString();
                 results = VUARMSFBDataHelper.GetResults(vuRefNo, false, "ALL");

                 //populate homepanel
                 homePanel.dob += dt.Rows[0]["DOB"];
                 homePanel.name += dt.Rows[0]["FULLNAME"];
                 homePanel.spDuties += dt.Rows[0]["SDUTIES"];
                 homePanel.citizenship += dt.Rows[0]["NATIONALITY"];
                 homePanel.email += dt.Rows[0]["EMAIL"];
                 homePanel.phone += dt.Rows[0]["PHONE"];
                 homePanel.vuRefNo += vuRefNo;
                 homePanel.desig += AppUtilities.CurrentUser.Designation;

                 homePanel.academic = "COURSE: " + dt.Rows[0]["PROGRAMME"] + "  CU: " + AppUtilities.GetTotalCredits(results) + "  RETAKES: "
                     + AppUtilities.GetRetakes(results) + "  TUITION BAL: " + VUARMSFBDataHelper.GetTuitionBalance(vuRefNo);
             }
             else if(AppUtilities.CurrentUser.IsStaff) //this is staff
             {  */
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (AppUtilities.CurrentUser.CLRLevel > 4)  // superadmin ONLY
            {
                if (e.Key == System.Windows.Input.Key.F7)
                {
                    RawDataView rdv = new RawDataView();
                    rdv.Show();
                }
                if (e.Key == System.Windows.Input.Key.F6)
                {

                    if (MessageBox.Show("This will reset ALL students passwords to default. Continue?", "RESET",
                        MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        VUARMSFBDataHelper.ResetAllStudentPasswords();
                    }
                }
                if (e.Key == System.Windows.Input.Key.F5)
                {
                    if (MessageBox.Show("This will prompt you to import students' details. Continue?", "RESET",
                       MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                    {
                        Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                        string fileName = "";
                        if (ofd.ShowDialog() == true)
                        {
                            fileName = ofd.FileName;
                        }
                        VUARMSFBDataHelper.ImportStudents(fileName);
                    }
                }
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            ChangePass cp = new ChangePass();
            if (cp.ShowDialog() == true)
            {
                string newPassword = cp.NewPassword;

                if (VUARMSFBDataHelper.UpdateUser(AppUtilities.CurrentUser.UserName, Hasher.CreateHash(newPassword)))
                {
                    MessageBox.Show("The password for user " + AppUtilities.CurrentUser.UserName + " was updated.",
                        "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
