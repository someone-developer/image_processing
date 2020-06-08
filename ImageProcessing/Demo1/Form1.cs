using Alturos.Yolo;
using Alturos.Yolo.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Demo1
{
    public partial class Form1 : Form
    {
        ConfigurationDetector configurationDetector;
        YoloConfiguration config;
        public Form1()
        {
            InitializeComponent();
            configurationDetector = new ConfigurationDetector();
            config = configurationDetector.Detect();
        }
        private IEnumerable<YoloItem> Detect()
        {

            using (var yoloWrapper = new YoloWrapper(config))
            {
                MemoryStream ms = new MemoryStream();

                pictureBox1.Image.Save(ms, ImageFormat.Png);
                Image img = pictureBox1.Image;
                return yoloWrapper.Detect(ms.ToArray());

            }
        }

        private void btnBrow_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "PNG|*.png|JPEG|*.jpg" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {

                    pictureBox1.Image = Image.FromFile(ofd.FileName);
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            IEnumerable<YoloItem> detectItem = Detect();
            DrawDetact(detectItem);
            MessageBox.Show("Done");
        }

        private Brush GetBrush(double confidence)
        {
            if (confidence > 0.5)
            {
                return Brushes.GreenYellow;
            }
            else if (confidence > 0.2 && confidence <= 0.5)
            {
                return Brushes.Orange;
            }

            return Brushes.DarkRed;
        }
        private void DrawDetact(IEnumerable<YoloItem> detectItem)
        {
            if (detectItem != null && detectItem.Count() > 0)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pictureBox1.Invoke((MethodInvoker)delegate
                    {
                        pictureBox1.Image.Save(ms, ImageFormat.Png);

                        Image img = pictureBox1.Image;
                        using (var font = new Font(FontFamily.GenericSansSerif, 16))
                        using (var canvas = Graphics.FromImage(img))
                        {
                            foreach (var item in detectItem)
                            {
                                var x = item.X;
                                var y = item.Y;
                                var width = item.Width;
                                var height = item.Height;

                                var brush = this.GetBrush(item.Confidence);
                                var penSize = img.Width / 100.0f;

                                using (var pen = new Pen(brush, penSize))
                                {
                                    canvas.DrawRectangle(pen, x, y, width, height);
                                    canvas.FillRectangle(brush, x - (penSize / 2), y - 15, width + penSize, 25);
                                }
                            }

                            foreach (var item in detectItem)
                            {
                                canvas.DrawString(item.Type, font, Brushes.White, item.X, item.Y - 12);
                            }


                            canvas.Flush();
                        }
                        pictureBox1.Image = img;
                    });
                }


            }


        }
    }
}
