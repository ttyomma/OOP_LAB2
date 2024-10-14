using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab2._2_Paint
{
    public partial class Form1 : Form
    {
        private Color penColor = Color.Black;
        private bool isDrawing = false;
        private float zoomFactor = 1.0f;
        private Bitmap originalImage;
        private Bitmap scaledImage;
        private Point previousPoint;
        private int penWidth = 1;
        private bool isPanning = false;
        private Point panStartPoint;
        private Point imageOffset = new Point(0, 0);

        private int gridStep = 10; // основне завдання - крок сітки

        public Form1()
        {
            InitializeComponent();
            originalImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            scaledImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.Image = scaledImage;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            UpdateScaledImage();
        }

        private void DrawGrid(Graphics g) // основне завдання - метод малювання сітки
        {
            for (int x = 0; x < originalImage.Width; x += gridStep)
            {
                g.DrawLine(Pens.LightGray, x, 0, x, originalImage.Height);
            }

            for (int y = 0; y < originalImage.Height; y += gridStep)
            {
                g.DrawLine(Pens.LightGray, 0, y, originalImage.Width, y);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                isPanning = true;
                panStartPoint = e.Location;
            }
            else
            {
                isDrawing = true;

                int centerX = (pictureBox1.Width - (int)(originalImage.Width * zoomFactor)) / 2;
                int centerY = (pictureBox1.Height - (int)(originalImage.Height * zoomFactor)) / 2;

                int x = (int)((e.X - centerX) / zoomFactor);
                int y = (int)((e.Y - centerY) / zoomFactor);

                previousPoint = e.Location;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                isPanning = false;
            }
            else
            {
                isDrawing = false;
                previousPoint = Point.Empty;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            toolStripStatusLabel1.Text = $"{e.X}, {e.Y - 25} пкс";

            if (isPanning)
            {
                int offsetX = e.X - panStartPoint.X;
                int offsetY = e.Y - panStartPoint.Y;
                imageOffset.X += offsetX;
                imageOffset.Y += offsetY;

                UpdateScaledImage();
                panStartPoint = e.Location;
            }
            else
            {
                if (isDrawing)
                {
                    int centerX = (pictureBox1.Width - (int)(originalImage.Width * zoomFactor)) / 2 + imageOffset.X;
                    int centerY = (pictureBox1.Height - (int)(originalImage.Height * zoomFactor)) / 2 + imageOffset.Y;

                    int x = (int)((e.X - centerX) / zoomFactor);
                    int y = (int)((e.Y - centerY) / zoomFactor);
                    int prevX = (int)((previousPoint.X - centerX) / zoomFactor);
                    int prevY = (int)((previousPoint.Y - centerY) / zoomFactor);

                    if (previousPoint != Point.Empty)
                    {
                        using (Graphics bufferGraphics = Graphics.FromImage(originalImage))
                        {
                            DrawLine(bufferGraphics, prevX, prevY, x, y, penColor);
                        }
                        UpdateScaledImage();
                    }

                    previousPoint = e.Location;
                }
            }
        }

        // метод для апскейлу на майбутнє
        private void UpdateScaledImage()
        {
            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                g.Clear(pictureBox1.BackColor);
            }

            scaledImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            using (Graphics g = Graphics.FromImage(scaledImage))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                DrawGrid(g);

                int x = (pictureBox1.Width - (int)(originalImage.Width * zoomFactor)) / 2 + imageOffset.X;
                int y = (pictureBox1.Height - (int)(originalImage.Height * zoomFactor)) / 2 + imageOffset.Y;
                g.DrawImage(originalImage, x, y, (int)(originalImage.Width * zoomFactor), (int)(originalImage.Height * zoomFactor));

                g.DrawRectangle(Pens.Black, x, y, (int)(originalImage.Width * zoomFactor) - 1, (int)(originalImage.Height * zoomFactor) - 1); // рамка
            }

            pictureBox1.Image = scaledImage;
            pictureBox1.Invalidate();
            pictureBox1.Update();
        }

        //алгоритму Брезенхема з інтернету для того, аби не було розриву між пікселями
        private void DrawLine(Graphics g, int x1, int y1, int x2, int y2, Color color)
        {
            int dx = Math.Abs(x2 - x1);
            int dy = Math.Abs(y2 - y1);
            int sx = x1 < x2 ? 1 : -1;
            int sy = y1 < y2 ? 1 : -1;
            int err = (dx > dy ? dx : -dy) / 2;

            while (true)
            {
                if (x1 >= 0 && x1 < originalImage.Width && y1 >= 0 && y1 < originalImage.Height)
                {
                    for (int i = -penWidth / 2; i <= penWidth / 2; i++)
                    {
                        for (int j = -penWidth / 2; j <= penWidth / 2; j++)
                        {
                            if (x1 + i >= 0 && x1 + i < originalImage.Width && y1 + j >= 0 && y1 + j < originalImage.Height)
                            {
                                originalImage.SetPixel(x1 + i, y1 + j, color);
                            }
                        }
                    }
                }

                if (x1 == x2 && y1 == y2) break;

                int e2 = err;

                if (e2 > -dx)
                {
                    err -= dy;
                    x1 += sx;
                }

                if (e2 < dy)
                {
                    err += dx;
                    y1 += sy;
                }
            }
        }



        private void UpdatePictureBoxSize()
        {
            int oldWidth = pictureBox1.Width;
            int oldHeight = pictureBox1.Height;

            pictureBox1.Width = (int)(originalImage.Width * zoomFactor);
            pictureBox1.Height = (int)(originalImage.Height * zoomFactor);

            Bitmap newOriginalImage = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            using (Graphics g = Graphics.FromImage(newOriginalImage))
            {
                g.DrawImage(originalImage, 0, 0, oldWidth, oldHeight);
            }
            originalImage = newOriginalImage;
            UpdateScaledImage();
            UpdateStatusBar();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            zoomFactor += 0.1f;
            UpdatePictureBoxSize();
            UpdateStatusBar();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            zoomFactor -= 0.1f;
            if (zoomFactor < 0.1f) zoomFactor = 0.1f;
            UpdatePictureBoxSize();
            UpdateStatusBar();
        }

        private void UpdateStatusBar()
        {
            string zoomText = $"Scale: {(int)(zoomFactor * 100)}%";
            StatusSizeLabel.Text = zoomText;
        }

        private void ColorButton_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                penColor = colorDialog1.Color;
            }
        }

        private void SizeButton_Click(object sender, EventArgs e)
        {
            using (NumericUpDown numericUpDown = new NumericUpDown())
            {
                numericUpDown.Minimum = 1;
                numericUpDown.Maximum = 10;
                numericUpDown.Value = penWidth;

                Button okButton = new Button();
                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;

                Form form = new Form();
                form.Text = "Choose Size";
                form.Controls.Add(numericUpDown);
                form.Controls.Add(okButton);

                numericUpDown.Location = new Point(10, 10);
                okButton.Location = new Point(10, 40);
                form.Size = new Size(230, 130);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    penWidth = (int)numericUpDown.Value;
                }
            }
        }

        private void changeSizeStrip_Click(object sender, EventArgs e) // основне завдання - зміна кроку сітки в діалоговому вікні
        {
            using (NumericUpDown numericUpDown = new NumericUpDown())
            {
                numericUpDown.Minimum = 1;
                numericUpDown.Maximum = 100;
                numericUpDown.Value = gridStep;

                Button okButton = new Button();
                okButton.Text = "OK";
                okButton.DialogResult = DialogResult.OK;

                Form form = new Form();
                form.Text = "Change Grid";
                form.Controls.Add(numericUpDown);
                form.Controls.Add(okButton);

                numericUpDown.Location = new Point(10, 10);
                okButton.Location = new Point(10, 40);

                form.Size = new Size(280, 120);

                if (form.ShowDialog() == DialogResult.OK)
                {
                    gridStep = (int)numericUpDown.Value;
                    UpdateScaledImage();
                }
            }
        }
    }
}