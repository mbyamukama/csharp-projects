using System;
using System.Windows;
using System.Collections.Generic;
using StockApp.AppExtensions;
using System.Windows.Media;
using System.Windows.Controls;
using System.Data;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Windows.Documents;
using System.Drawing.Drawing2D;
using StockApp.AppExtensions;
using System.Linq;

namespace StockApp
{
  static class DocumentHelper
  {
    /// <summary>
    /// Resize the image to the specified width and height.
    /// </summary>
    /// <param name="image">The image to resize.</param>
    /// <param name="width">The width to resize to.</param>
    /// <param name="height">The height to resize to.</param>
    /// <returns>The resized image.</returns>
    public static System.Drawing.Bitmap ResizeImage(this System.Drawing.Bitmap image, int width, int height)
    {
      var destRect = new System.Drawing.Rectangle(0, 0, width, height);
      var destImage = new System.Drawing.Bitmap(width, height);

      destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

      using (var graphics = System.Drawing.Graphics.FromImage(destImage))
      {
        graphics.CompositingMode = CompositingMode.SourceCopy;
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

        using (var wrapMode = new ImageAttributes())
        {
          wrapMode.SetWrapMode(WrapMode.TileFlipXY);
          graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel, wrapMode);
        }
      }

      return destImage;
    }

    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr gdiObject);

    public static BitmapSource ToBitmapSource(this System.Drawing.Bitmap source)
    {
      IntPtr gdiObject = source.GetHbitmap();

      BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(gdiObject,
          IntPtr.Zero, Int32Rect.Empty,
              System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
      DeleteObject(gdiObject); //release
      return bitmapSource;
    }

    public static FixedDocument AppendFirstPage(this PageContent[] pages, FixedPage page1)
    {

      FixedDocument newDoc = new FixedDocument()
      {
        Name = "Report"
      };

      if (page1 != null)
      {
        PageContent p = new PageContent();
        p.Child = page1;
        newDoc.Pages.Add(p);
      }

      foreach (PageContent pc in pages)
      {
        newDoc.Pages.Add(pc);
      }
      return newDoc; ;
    }

    public static FixedDocument GenerateReceipt(ReceiptDataModel rDataModel)
    {
      string paytype = rDataModel.PayType;
      int cash = rDataModel.Cash;
      int redeemed = rDataModel.Redeemed;
      int total = rDataModel.Total;
      int discount = rDataModel.Discount;
      int balance = rDataModel.Balance;
      string recNo = rDataModel.ReceiptNo;
      int pointsEarned = rDataModel.PointsEarned;
      int pointsUsed = rDataModel.PointsUsed;
      int totalPoints = rDataModel.TotalPoints;
      int pointsValue = rDataModel.PointsValue;
      string dateString = rDataModel.DateString;
      string teller = rDataModel.Teller;

      DataTable itemsBought = rDataModel.ItemsBought.Copy();
      string receiptHeader = "";
      try
      {
        receiptHeader = ((from myRows in GlobalSystemData.Settings.AsEnumerable()
                          where myRows.Field<string>("ITEM") == "ReceiptHeader"
                          select myRows).ElementAt(0)["VALUE1"]).ToString().Replace(';', '\n');
      }
      catch
      {
        MessageBox.Show("The receipt header seems not to be available. An empty header will be used. Please fill in item ReceiptHeader in the database.", "No Receipt Header", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      itemsBought.Columns.Remove("BARCODE");//no longer necessary
      if (itemsBought.Columns.Contains("AVAILQTY"))
      {
        itemsBought.Columns.Remove("AVAILQTY");//no longer necessary
      }

      //add change rows
      itemsBought.Rows.Add(null, null, null, null);
      itemsBought.Rows.Add("DISCOUNT", null, null, discount);
      itemsBought.Rows.Add("BILL TOTAL", null, null, total);
      itemsBought.Rows.Add("AMT REDEEMED", null, null, redeemed);
      if (paytype == "CASH")
      {
        itemsBought.Rows.Add("CASH", null, null, cash);
        itemsBought.Rows.Add("CHANGE", null, null, balance);
      }
      else
      {
        itemsBought.Rows.Add("CARD", null, null, total);
        itemsBought.Rows.Add("CHANGE", null, null, 0);
      }

      if (pointsEarned != 0 || pointsUsed != 0)
      {
        itemsBought.Rows.Add("POINTS EARNED", "", "", pointsEarned);
        itemsBought.Rows.Add("POINTS USED", "", "", pointsUsed);
        itemsBought.Rows.Add("CURRENT POINTS ", "", "", totalPoints);
        itemsBought.Rows.Add("VALUE ", "", "", pointsValue);
      }

      List<DataTable> tables = itemsBought.Split(35);
      FixedDocument doc = new FixedDocument();

      int columns = itemsBought.Columns.Count;
      int rows = itemsBought.Rows.Count;
      const int LEFT_MARGIN = 0, TOP_MARGIN = 0;


      for (int i = 0; i < tables.Count; i++)
      {
        double left = LEFT_MARGIN, top = TOP_MARGIN, height = 25;

        Grid grid = new Grid()
        {
          Name = "root",
          Width = 300,
          Margin = new Thickness(0, 0, 0, 0)
        };

        if (i == 0)
        {
          //title
          Label title = new Label()
          {
            Width = 280,
            Height = 70,
            FontFamily = new FontFamily("Arial"),
            FontSize = 14,
            HorizontalContentAlignment = HorizontalAlignment.Center,
            Content = receiptHeader,
            Margin = new Thickness(LEFT_MARGIN, top, 0, 0),
            BorderThickness = new Thickness(1),
            BorderBrush = Brushes.Black,
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top
          };

          grid.Children.Add(title);
          top += title.Height;

          //receipt
          Label receipt = new Label();
          receipt.Width = 280;
          receipt.Height = 25;
          receipt.FontFamily = new FontFamily("Arial");
          title.FontSize = 11;
          receipt.Content = "Receipt No: " + recNo + "   Date: " + dateString;
          receipt.Margin = new Thickness(LEFT_MARGIN, top, 0, 0);
          receipt.BorderThickness = new Thickness(1);
          receipt.BorderBrush = Brushes.Gray;
          receipt.HorizontalAlignment = HorizontalAlignment.Left;
          receipt.VerticalAlignment = VerticalAlignment.Top;
          receipt.HorizontalContentAlignment = HorizontalAlignment.Left;
          grid.Children.Add(receipt);
          top += receipt.Height;

          //header
          Label header = new Label();
          header.Width = 280;
          header.Height = 25;
          header.FontFamily = new FontFamily("Arial");
          header.FontSize = 11;
          header.Content = "ITEM\t\t\tQTY   PPU        AMT";
          header.Margin = new Thickness(LEFT_MARGIN, top, 0, 0);
          header.BorderThickness = new Thickness(1);
          header.BorderBrush = Brushes.Black;
          header.HorizontalAlignment = HorizontalAlignment.Left;
          header.VerticalAlignment = VerticalAlignment.Top;
          header.HorizontalContentAlignment = HorizontalAlignment.Center;
          grid.Children.Add(header);
          top += header.Height;
        }

        foreach (DataRow row in tables[i].Rows)
        {
          foreach (DataColumn col in tables[i].Columns)
          {
            Label label = new Label()
            {
              Margin = new Thickness(left, top, 0, 0),
              BorderBrush = Brushes.Gray,
              HorizontalAlignment = HorizontalAlignment.Left,
              VerticalAlignment = VerticalAlignment.Top,
              BorderThickness = new Thickness(0.5),

              Content = col.ColumnName == "ITEMNAME" ? row[col].ToString().PadRight(100).Substring(0, 25) :
                              col.ColumnName == "QUANTITY" ? row[col] :
                              ((col.ColumnName == "SELLPPU" || col.ColumnName == "AMOUNTDUE") && row[col].ToString() != "") ?
                              Convert.ToInt32(row[col]).ToString("N0") :
                              null,

              Width = col.Ordinal == 0 ? 150 : col.Ordinal == 1 ? 30 : 50,
              Height = height,
              FontFamily = new FontFamily("Arial"),
              FontSize = 11,
              VerticalContentAlignment = VerticalAlignment.Top,
              HorizontalContentAlignment = HorizontalAlignment.Left
            };

            if (label.Content != null && label.Content.ToString().Length > 6) label.FontSize = 9;
            grid.Children.Add(label);
            left += label.Width;
          }
          top += height;
          left = LEFT_MARGIN;
        }

        if (i == tables.Count - 1)
        {
          //footer
          Label footer = new Label();
          footer.Width = 280;
          footer.Height = 70;
          footer.FontFamily = new FontFamily("Arial");
          footer.FontSize = 11;
          footer.Content = "You were served by: " + teller +
              "\nPrices include VAT where applicable. Thank You!\n" +
              "POS Software by +256-706-256168\nGOODS ONCE SOLD ARE NOT RETURNABLE";
          footer.Margin = new Thickness(LEFT_MARGIN, top, 0, 0);
          footer.BorderThickness = new Thickness(1);
          footer.BorderBrush = Brushes.Gray;
          footer.HorizontalAlignment = HorizontalAlignment.Left;
          footer.VerticalAlignment = VerticalAlignment.Top;
          footer.HorizontalContentAlignment = HorizontalAlignment.Left;
          grid.Children.Add(footer);
          //  top += footer.Height + 2;
        }

        FixedPage page = new FixedPage();
        page.Children.Add(grid);
        PageContent pc = new PageContent();
        pc.Child = page;
        pc.Margin = new Thickness(0, 0, 0, 0);
        doc.Pages.Add(pc);
      }
      return doc;
    }
    public static FixedDocument GetBarCodes(string text, string descr, string paperType, int copies, bool compress, int leftMargin, int spacing)
    {
      Grid grid = new Grid()
      {
        Name = "root",
        Margin = new Thickness(0, 0, 0, 0),
        HorizontalAlignment = HorizontalAlignment.Left

      };

      System.Drawing.Bitmap image =
      (System.Drawing.Bitmap)Code128Rendering.MakeBarcodeImage(text, 1, true, descr);
      int left = 50;
      int top = 10;
      if (compress)
      {
        image = image.ResizeImage(image.Width * 9 / 10, image.Height * 9 / 10);
      }

      if (paperType == "A4")
      {
        for (int i = 1; i <= copies; i++)
        {
          Label label = new Label()
          {
            Margin = new Thickness(left, top, 0, 0),
            HorizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            Content = new Image()
            {
              Source = image.ToBitmapSource(),
              Stretch = Stretch.None
            },
            HorizontalContentAlignment = HorizontalAlignment.Left,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1, 1, 1, 1)
          };
          grid.Children.Add(label);
          left += 250;
          if (i % 3 == 0)
          {
            if (compress)
            {
              top += 80;
            }
            else
            {
              top += 100;
            }
            left = 50;
          }
        }

      }

      if (paperType == "45x55mm")
      {
        top = 5;
        left = leftMargin;

        for (int i = 0; i < copies; i++)
        {
          Label label = new Label()
          {
            Margin = new Thickness(left, top, 0, 0),
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            VerticalAlignment = System.Windows.VerticalAlignment.Top,
            Content = new Image()
            {
              Source = image.ToBitmapSource(),
              Stretch = Stretch.None
            },
            HorizontalContentAlignment = HorizontalAlignment.Left,
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(1, 1, 1, 1)
          };
          grid.Children.Add(label);
          if (compress) top += (spacing * 9 / 10);
          else
            top += spacing;
        }
      }

      FixedPage page = new FixedPage();
      page.Children.Add(grid);
      //  page.Margin = new Thickness(0, 0, 0, 0);

      PageContent pc = new PageContent();
      pc.Child = page;
      //  pc.Margin = new Thickness(0, 0, 0, 0);
      FixedDocument doc = new FixedDocument();
      doc.Pages.Add(pc);
      return doc;
    }
  }
}
