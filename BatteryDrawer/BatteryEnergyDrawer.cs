using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BatteryDrawer
{
    class BatteryEnergyDrawer
    {
        public BatteryEnergyDrawer()
        {

        }
        public Bitmap DrawBattery(int width, int height, int segments, int chargedPercent, Color chargeOK, Color chargePreWarning, Color chargeLow)
        {
            Color fillColor = chargeOK;
            if (chargedPercent <= 25)
                fillColor = chargePreWarning;
            if (chargedPercent <= 10)
                fillColor = chargeLow;

            Bitmap bmp = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            SolidBrush brush = new SolidBrush(fillColor);
            float segHeight = bmp.Height * 0.90F / segments; //this represents x%. e.g for 10 segments, 10%
            float space = bmp.Height * 0.10F / segments;
            int originX = 0;
            float originY = bmp.Height-space;
            float padding = 5;

            //segments is the number of rectangles at full charge: 100%
            double segmentsNeeded = chargedPercent * 0.01 * segments;
            int fullSegments = (int)Math.Floor(segmentsNeeded);
            double partialHeight = (segmentsNeeded - fullSegments) * segHeight; //the partial height of the last rectangle
            RectangleF rect;
            float y = originY;
            for (int N = 1; N <= fullSegments; N++)
            {
                y = originY - N * (segHeight + space);
                rect = new RectangleF(originX + padding, y, bmp.Width - 2 * padding, segHeight);
                g.FillRectangle(brush, rect);
            }
            rect = new RectangleF(originX + padding, (float)(y - space - partialHeight), bmp.Width - 2 * padding, (float)partialHeight);
            g.FillRectangle(brush, rect);

            if (chargedPercent < 25)
                g.DrawString("!", new Font("Arial", 45), new SolidBrush(fillColor), new PointF(width / 3.5F, height-height / 1.5F));

            return bmp;
        }
    }
    
}
