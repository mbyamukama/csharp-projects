using Java.Lang;
using System.Runtime.InteropServices.ComTypes;

namespace AlcMixerCalc;

public partial class MainPage : ContentPage
{
  double c1 = 0, v1 = 0, c2 = 0, v2 = 0;
  

  public MainPage()
  {
    InitializeComponent();

  }

  void ParseText(string text)
  {

  }
  void getConc()
  {
    if (v1 != 0 && v2 != 0)
      txtfinalConc.Text = Math.Round((c1 * v1 + c2 * v2) / (v1 + v2),1) + "%";
    else txtfinalConc.Text = "N/A";
  }
  private void entryBaseVol_TextChanged(object sender, TextChangedEventArgs e)
  {
    Double.TryParse(entryBaseVol.Text, out v1);
    getConc();
  }

  private void entryBaseConc_TextChanged(object sender, TextChangedEventArgs e)
  {
    Double.TryParse(entryBaseConc.Text, out c1);
    getConc();
  }

  private void entryMixerVol_TextChanged(object sender, TextChangedEventArgs e)
  {
    Double.TryParse(entryMixerVol.Text, out v2);
    getConc();
  }

  private void entryMixerConc_TextChanged(object sender, TextChangedEventArgs e)
  {
    Double.TryParse(entryMixerConc.Text, out c2);
    getConc();
  }
}

