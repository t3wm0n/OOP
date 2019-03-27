using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Diagram
{
    public partial class Form2 : Form
    {
        string header; //график
        double[] d;
        int N;
        Color MarkerColor = Color.FromName("Green");
        public static string path = Application.StartupPath + "\\g.txt";
        Graphics g;
        string[] srr = 
        {
            "Доллар США",
            "Евро",
            "Норвежских крон",
            "Белорусский рубль",
            "Казахстанских тенге",
            "Польский злотый",
            "Японских иен",
            "Молдавских леев",
            "Армянских драмов",
            "Австралийский доллар"
        };

        public Form2()
        {
            InitializeComponent();
            try
            {
                StreamReader sr = new StreamReader(path,
                    Encoding.Default);
                header = sr.ReadLine();
                N = File.ReadAllLines(path).Length - 1;
                d = new double[N];

                int i = 0;
                string st = sr.ReadLine();
                while ((st != null) && (i < N))
                {
                    d[i++] = Convert.ToDouble(st);
                    st = sr.ReadLine();
                }
                sr.Close();
                Paint += new PaintEventHandler(Graf);
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

        private void Graf(object sender, PaintEventArgs e)
        {
            Brush br = new SolidBrush(MarkerColor);
            g = e.Graphics;
            Font hFont = new Font("Tahoma", 12, FontStyle.Bold);
            int x = (ClientSize.Width - (int)g.MeasureString(header, hFont).Width) / 2;
            g.DrawString(header, hFont, Brushes.Green, x, 5);

            Font gFont = new Font("Tahoma", 9);
            int sw = (int)((ClientSize.Width - 40) / (N - 1));
            double max = d[0], min = d[0];
            for (int i = 1; i < N; i++)
            {
                if (d[i] > max) max = d[i];
                if (d[i] < min) min = d[i];
            }
            int x1, y1, x2, y2;
            x1 = 20;
            y1 = ClientSize.Height - 20 - (int)((ClientSize.Height - 100) * (d[0] - min) / (max - min));
            g.FillRectangle(Brushes.Green, x1 - 2, y1 - 2, 4, 4);
            g.DrawString(d[0].ToString(), gFont, Brushes.Black, x1 - 10, y1 - 20);

            for (int i = 1; i < N; i++)
            {
                x2 = 8 + i * sw;
                y2 = ClientSize.Height - 20 - (int)((ClientSize.Height - 100) * (d[i] - min) / (max - min));
                if ((d[i] == max) || (d[i] == min))
                    g.FillRectangle(br, x2 - 2, y2 - 2, 8, 8);
                g.FillRectangle(br, x2 - 2, y2 - 2, 4, 4);
                g.DrawLine(Pens.Black, x1, y1, x2, y2);
                g.DrawString(d[i].ToString(), gFont, Brushes.Black, x2 - 10, y2 - 20);
                x1 = x2;
                y1 = y2;
                g.DrawString(d[i].ToString(), gFont, Brushes.Black, -2, y2 - 20);
                g.DrawString(d[i].ToString(), gFont, Brushes.Black, x2 - 10, ClientSize.Height - 12);
            }
            g.DrawLine(Pens.Black, new Point(14, 1), new Point(14, ClientSize.Height - 10));
            g.DrawLine(Pens.Black, new Point(ClientSize.Width - 5,ClientSize.Height - 10), new Point(14, ClientSize.Height - 10));
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            Invalidate();
        }

        private void ChangeColor_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            MarkerColor = colorDialog1.Color;
            Invalidate();
        }

        private void form1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form1 fr = new Form1();
            fr.ShowDialog();
        }

        private void form3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 fr = new Form3();
            fr.ShowDialog();
        }

        private void ChooseFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            
            Invalidate();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            path = openFileDialog1.FileName;
            Invalidate();
        }

        private string Valuta(string val)
        {
            string data = string.Empty;
            string url = "http://www.cbr.ru/currency_base/D_print.aspx?date_req=";
            string html = string.Empty;
            string valuta = val;
            string pattern = valuta + @"</td>[\s\S].......<td>.......</td>";
            string pattern2 = @"\d\d.\d\d\d\d";

            DateTime today = DateTime.Now;
            data = today.Date.ToShortDateString();
            url += data;

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            StreamReader myStreamReader = new StreamReader(myHttpWebResponse.GetResponseStream());
            html = myStreamReader.ReadToEnd();

            RegexOptions opt = RegexOptions.Singleline;
            Match match = Regex.Match(html, pattern, opt);
            Match match2 = Regex.Match(match.Value, pattern2, opt);
            return match2.Value;
        }
        double[] dd; 
        private void Valuts_Click(object sender, EventArgs e)
        {
            dd = new double[10];
            int i = 0;
            Paint -= new PaintEventHandler(Graf);
            Paint += new PaintEventHandler(Graf2);
            foreach (string str in srr)
            {
                double val = Convert.ToDouble(Valuta(str));
                dd[i++] = val;
            }
            Invalidate();
        }

        private void Graf2(object sender, PaintEventArgs e)
        {

            Brush br = new SolidBrush(MarkerColor);
            g = e.Graphics;
            Font hFont = new Font("Tahoma", 12, FontStyle.Bold);
            int x = (ClientSize.Width - (int)g.MeasureString(header, hFont).Width) / 2;
            g.DrawString(header, hFont, Brushes.Green, x, 5);

            Font gFont = new Font("Tahoma", 9);
            int sw = (int)((ClientSize.Width - 40) / (N - 1));
            double max = dd[0], min = dd[0];
            for (int i = 1; i < N; i++)
            {
                if (dd[i] > max) max = dd[i];
                if (dd[i] < min) min = dd[i];
            }
            int x1, y1, x2, y2;
            x1 = 20;
            y1 = ClientSize.Height - 20 - (int)((ClientSize.Height - 100) * (dd[0] - min) / (max - min));
            g.FillRectangle(Brushes.Green, x1 - 2, y1 - 2, 4, 4);
            g.DrawString(dd[0].ToString(), gFont, Brushes.Black, x1 - 10, y1 - 20);

            for (int i = 1; i < N; i++)
            {
                x2 = 8 + i * sw;
                y2 = ClientSize.Height - 20 - (int)((ClientSize.Height - 100) * (dd[i] - min) / (max - min));
                if ((dd[i] == max) || (dd[i] == min))
                    g.FillRectangle(br, x2 - 2, y2 - 2, 8, 8);
                g.FillRectangle(br, x2 - 2, y2 - 2, 4, 4);
                g.DrawLine(Pens.Black, x1, y1, x2, y2);
                g.DrawString(dd[i].ToString(), gFont, Brushes.Black, x2 - 10, y2 - 20);
                x1 = x2;
                y1 = y2;
                g.DrawString(dd[i].ToString(), gFont, Brushes.Black, -2, y2 - 20);
                g.DrawString(dd[i].ToString(), gFont, Brushes.Black, x2 - 10, ClientSize.Height - 12);
            }
            g.DrawLine(Pens.Black, new Point(14, 1), new Point(14, ClientSize.Height - 10));
            g.DrawLine(Pens.Black, new Point(ClientSize.Width - 5, ClientSize.Height - 10), new Point(14, ClientSize.Height - 10));
        }
    }
}
