using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;

namespace memuse_convert
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFile();
        }

        void openFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Txt files|*.txt|Log files|*.log|All files|*.*";
            ofd.FilterIndex = 0;
            ofd.RestoreDirectory = true;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    memuse_logfile mLog = new memuse_logfile(ofd.FileName);
                    dataGridView1.DataSource = mLog.transpose();
                    drawGraph();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("File error." + ex.Message);
                }
            }
        }

        void drawGraph()
        {
            DataTable dt = (DataTable) dataGridView1.DataSource;
            this.SuspendLayout();

            DataRow[] r = dt.Select("time <> ''", "time ASC");
            string minS = (string)r.First()[0];
            long min = DateTime.Parse(minS).ToFileTime() / 10000000L; ;
            string maxS = (string)r.Last()[0];
            long max = DateTime.Parse(maxS).ToFileTime() / 10000000L;
            chart1.DataSource = dt;
            for (int col = 1; col < dt.Columns.Count; col++)
            {
                try
                {
                    System.Windows.Forms.DataVisualization.Charting.Series serie = chart1.Series.Add(dt.Columns[col].ColumnName);
                    serie.XValueMember = "time";
                    serie.YValueMembers = dt.Columns[col].ColumnName;
                    serie.Name = dt.Columns[col].ColumnName;
                    serie.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastLine;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception: " + ex.Message + " col=" + col.ToString());
                }
                chart1.DataBind();
            }

            //List<GraphLib.DataSource> dataList = new List<GraphLib.DataSource>();
            //plotterDisplayEx1.PanelLayout = GraphLib.PlotterGraphPaneEx.LayoutMode.NORMAL;
            //plotterDisplayEx1.DoubleBuffering = true;
            //long xsteps = dt.Rows.Count;
            //for (int col = 8; col < 10 /*dt.Columns.Count*/; col++)
            //{
            //    GraphLib.DataSource ds = new GraphLib.DataSource();
            //    ds.Length = dt.Rows.Count;
            //    ds.AutoScaleX=true;
            //    ds.AutoScaleY=true;
            //    ds.Name = dt.Columns[col].ColumnName;
            //    ds.OnRenderXAxisLabel = RenderXLabel;
            //    ds.OnRenderYAxisLabel = RenderYLabel;
            //    try
            //    {
            //        for (int row = 0; row < dt.Rows.Count; row++)
            //        {
            //            long myX = DateTime.Parse(dt.Rows[row][0].ToString()).ToFileTime() / 10000000L;
            //            ds.Samples[row].x = myX;
            //            float fValue=0;
            //            if (float.TryParse(dt.Rows[row][col].ToString(), out fValue))
            //            {
            //                ds.Samples[row].y = float.Parse(dt.Rows[row][col].ToString());
            //                //   (new GraphControl.XY(myX, float.Parse(dt.Rows[row][col].ToString()), Color.Red));
            //            }
            //            else
            //            {
            //                ds.Samples[row].y = 0;
            //                //graph1.xyc.Add(new GraphControl.XY(myX, 0, Color.Red));
            //            }
            //        }
            //        plotterDisplayEx1.DataSources.Add(ds);
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine("Excpetion: " + ex.Message + " for col=" + col.ToString());
            //    }
            //}

            this.ResumeLayout();
        }
        private String RenderXLabel(GraphLib.DataSource s, int idx)
        {
            if (s.AutoScaleX)
            {
                int Value = (int)(s.Samples[idx].x);
                return "" + Value;
            }
            else
            {
                int Value = (int)(s.Samples[idx].x / 200);
                String Label = "" + Value + "\"";
                return Label;
            }
        }

        private String RenderYLabel(GraphLib.DataSource s, float value)
        {
            return String.Format("{0:0.0}", value);
        }
        void export2csv(DataGridView dGV, string filename)
        {
            if (dGV.RowCount == 0)
                return;
            string stOutput = "sep=\t\r\n";
            // Export titles:
            string sHeaders = "";

            for (int j = 0; j < dGV.Columns.Count; j++)
                sHeaders = sHeaders.ToString() + Convert.ToString(dGV.Columns[j].HeaderText) + "\t";
            stOutput += sHeaders + "\r\n";
            // Export data.
            for (int i = 0; i < dGV.RowCount - 1; i++)
            {
                string stLine = "";
                for (int j = 0; j < dGV.Rows[i].Cells.Count; j++)
                    stLine = stLine.ToString() + Convert.ToString(dGV.Rows[i].Cells[j].Value) + "\t";
                stOutput += stLine + "\r\n";
            }
            Encoding utf16 = Encoding.GetEncoding(1254);
            byte[] output = utf16.GetBytes(stOutput);
            FileStream fs = new FileStream(filename, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(output, 0, output.Length); //write the encoded file
            bw.Flush();
            bw.Close();
            fs.Close();
        }

        private void exportToCsvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CheckFileExists = false;
            sfd.OverwritePrompt = true;
            sfd.CheckPathExists = true;
            sfd.DefaultExt = ".csv";
            sfd.Filter = "CSV file|*.csv|All files|*.*";
            sfd.FilterIndex = 0;
            sfd.AddExtension = true;
            sfd.RestoreDirectory = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                export2csv(this.dataGridView1, sfd.FileName);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }      
    }
}
