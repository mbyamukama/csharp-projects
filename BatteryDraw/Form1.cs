using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BatteryDraw
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            pictureBox1.Image = BatteryDrawer.DrawBattery(
                  (float)new Random().NextDouble(),
                  pictureBox1.ClientSize.Width,
                  pictureBox1.ClientSize.Height,
                  Color.Transparent, Color.Gray,
                  Color.LightGreen, Color.White,
                  true);

        }

        private void Form1_Click(object sender, EventArgs e)
        {
           
        }
    }
}
