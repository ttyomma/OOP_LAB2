using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab2._1
{
    public partial class FormMain : Form
    {
        Pen pen;
        public FormMain()
        {
            InitializeComponent();
            pen = new Pen(Color.HotPink, 3);
        }

        private void FormMain_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(pen, 40, 75, 50, 75);
            e.Graphics.DrawLine(pen, 65, 50, 65, 100);
            e.Graphics.FillRectangle(Brushes.HotPink, 50, 65, 10, 20);

            e.Graphics.DrawLine(pen, 140, 75, 125, 75);
            e.Graphics.DrawLine(pen, 100, 75, 120, 75);
            e.Graphics.DrawLine(pen, 65, 75, 90, 75);

            e.Graphics.DrawLine(pen, 155, 75, 170, 75);
            e.Graphics.DrawLine(pen, 155, 50, 155, 100);
            e.Graphics.FillRectangle(Brushes.HotPink, 140, 65, 10, 20);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            pen.Dispose();
        }
    }
}
