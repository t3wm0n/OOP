using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace Diagram
{
    public partial class Form3 : Form
    {
        string header; //столбчатая диаграмма
        double[] d;
        int N;
        Graphics g;
        public Form3()
        {
            InitializeComponent();
            try
            {
                StreamReader sr = new StreamReader(Form2.path,
                    Encoding.Default);
                header = sr.ReadLine();
                N = File.ReadAllLines(Form2.path).Length - 1;
                d = new double[N];

                int i = 0;
                string st = sr.ReadLine();
                while ((st != null) && (i < N))
                {
                    d[i++] = Convert.ToDouble(st);
                    st = sr.ReadLine();
                }
                sr.Close();
                Paint += new PaintEventHandler(Diagram);
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message + "\n" + "(" + ex.GetType().ToString() + ")", "График", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "График", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Diagram(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            Font hFont = new Font("Tahoma", 12, FontStyle.Bold);
            int x = (ClientSize.Width - (int)g.MeasureString(header, hFont).Width) / 2;
            g.DrawString(header, hFont, Brushes.Green, x, 5);

            Font gFont = new Font("Tahoma", 9);
            double max = d[0], min = d[0];
            for (int i = 1; i < N; i++)
            {
                if (d[i] > max) max = d[i];
                if (d[i] < min) min = d[i];
            }

            int x1, y1, w, h;
            x1 = 20;
            w = (ClientSize.Width - 40 - 5 * (N - 1)) / N;
            for (int i = 0; i < N; i++)
            {
                y1 = ClientSize.Height - 20 - (int)((ClientSize.Height - 100) * (d[i] - min) / (max - min));
                g.DrawString(d[i].ToString(), gFont, Brushes.Black, x1, y1 - 20);
                h = ClientSize.Height - y1 - 20 + 1;
                g.FillRectangle(Brushes.Chocolate, x1, y1, w, h);
                g.DrawRectangle(Pens.Black, x1, y1, w, h);
                x1 += w + 5;
            }
        }

        private void Form3_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }
    }
}
