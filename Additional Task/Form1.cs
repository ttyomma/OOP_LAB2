using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab2.Additional
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private Point centerPoint;
        private int clockRadius;

        public Form1()
        {
            InitializeComponent();

            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) => Invalidate(); // оновлення інтерфейсу кожну секунду
            timer.Start();
            centerPoint = new Point(ClientSize.Width / 2, ClientSize.Height / 2); // розрахунок центру початкового вікна програми
            clockRadius = Math.Min(centerPoint.X, centerPoint.Y) - 10; // розрахунок радіусу годинника відповідно до розміру початкового вікна
        }
        
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            DateTime now = DateTime.Now; // отримання поточного часу компютера

            Draw(e.Graphics, (now.Hour % 12 + now.Minute / 60.0) * 30 - 90, clockRadius * 0.5, 3, Brushes.Black); // годинна
            Draw(e.Graphics, now.Minute * 6 - 90, clockRadius * 0.7, 3, Brushes.Blue); // хвилинна
            Draw(e.Graphics, now.Second * 6 - 90, clockRadius * 0.9, 3, Brushes.Red); // секундна
        }

        private void Draw(Graphics g, double angle, double length, int width, Brush brush) // клас для малювання
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            double angleRadians = angle * Math.PI / 180;
            using (Pen pen = new Pen(brush, width))
            {
                g.DrawLine(pen, centerPoint, new Point((int)(centerPoint.X + length * Math.Cos(angleRadians)),
                (int)(centerPoint.Y + length * Math.Sin(angleRadians))));
            }
        }
    }
}
