using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Diagram
{
    public partial class Form1 : Form
    {
        string header; //Круговая диаграмма
        int N = 0;
        double[] dat;
        double[] p;
        string[] title;
            
        public Form1()
        {
            InitializeComponent();
            try
            {
                StreamReader sr = new StreamReader(Form2.path,
                    Encoding.Default);
                header = sr.ReadLine();
                N = (File.ReadAllLines(Form2.path).Length) / 2;
                dat = new double[N];
                p = new double[N];
                title = new string[N];

                int i = 0;
                string st = sr.ReadLine();
                while ((st != null) && (i < N)) 
                {
                    title[i] = st;
                    st = sr.ReadLine();
                    dat[i++] = Convert.ToDouble(st);
                    st = sr.ReadLine();
                }
                sr.Close();
                Paint += new PaintEventHandler(CDiagram);

                double sum = 0;
                for (int j = 0; j < N; j++) sum += dat[j];
                for (int j = 0; j < N; j++) p[j] = (double)(dat[j] / sum);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Диаграмма", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CDiagram(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Font hFont = new Font("Tahoma", 12, FontStyle.Bold);
            int x = (ClientSize.Width - (int)g.MeasureString(header, hFont).Width) / 2;
            g.DrawString(header, hFont, Brushes.Green, x, 10);

            Font lFont = new Font("Tahoma", 9);
            int d = ClientSize.Height - 70, //Диаметр диаграммы
                x0 = 30, // координаты верхнего левого угла
                y0 = (ClientSize.Height - d) / 2 + 10,
                lx = 60 + d, //Область легенды
                ly = y0 + (d - N * 20 + 10) / 2,
                swe, //Длина дуги сектора
                sta = -90; //Начальная точко дуги сектора

           
            Brush fBrush = Brushes.White;
            Brush br;
            Color MarkerColor;
            Random r = new Random();
            for (int i = 0; i < N; i++)
            {
                MarkerColor = Color.FromArgb(r.Next(255), r.Next(255), r.Next(255));
                br = new SolidBrush(MarkerColor);
                swe = (int)(360 * p[i]);
                fBrush = br;
                if (i == N - 1) swe = 270 - sta;
                g.FillPie(fBrush, x0, y0, d, d, sta, swe);
                g.DrawPie(Pens.Black, x0, y0, d, d, sta, swe);

                g.FillRectangle(fBrush, lx, ly + i * 20, 20, 10);
                g.DrawRectangle(Pens.Black, lx, ly + i * 20, 20, 10);
                g.DrawString(title[i] + " - " + p[i].ToString("p"), lFont, Brushes.Black, lx + 24, ly + i * 20 - 3);
                sta += swe;
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
