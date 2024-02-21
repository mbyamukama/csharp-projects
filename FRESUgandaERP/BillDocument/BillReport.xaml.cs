using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FRESUGERP.AppUtilities
{
    /// <summary>
    /// IGenerate the FRES UG Bill Document for a list of clients
    /// </summary>
    public partial class BillReport : UserControl
    {
        public BillReport(string title, string [] descrCols, string [] values)
        {
           InitializeComponent();
           foreach (UIElement element in rootGrid.Children)
           {
               if (element.GetType() == typeof(Label))
               {
                   if (((Label)element).Name == "label1") ((Label)element).Content = descrCols[0];
                   if (((Label)element).Name == "label3") ((Label)element).Content = descrCols[1];
                   if (((Label)element).Name == "label5") ((Label)element).Content = descrCols[2];
                   if (((Label)element).Name == "label7") ((Label)element).Content = descrCols[3];

                   if (((Label)element).Name == "label2") ((Label)element).Content = values[0];
                   if (((Label)element).Name == "label4") ((Label)element).Content = values[1];
                   if (((Label)element).Name == "label6") ((Label)element).Content = values[2];
                   if (((Label)element).Name == "label8") ((Label)element).Content = values[3];

                   if (((Label)element).Content.ToString() == String.Empty)
                   {
                       ((Label)element).BorderThickness = new Thickness(0);
                   }
               }
           }
            lbltitle.Content = title;
        }
    }
}
