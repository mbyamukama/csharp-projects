using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BatteryDrawer
{
    public partial class Form1 : Form
    {
        BatteryEnergyDrawer bed = new BatteryEnergyDrawer();
        int charge = 100;
        public Form1()
        {
            InitializeComponent();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {      
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Bitmap bmp = bed.DrawBattery(pictureBox1.Width, pictureBox1.Height, 10, --charge, Color.Green, Color.Orange, Color.Red);
            pictureBox1.Image = bmp;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }
    }
}
