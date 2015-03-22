using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
			//string linuxFile="/home/hgode/Projects/memuse/memusage.log.099X1400112.txt";
			string linuxFile="memusage.log.099X1400112.txt";
			string winFile=@"D:\!iDaten\_SalesForce_Cases\00616480_cn51_touch\memusage_log_org.txt";
            //memuse_logfile mLog = new memuse_logfile(linuxfile);
			memuse_cvt1 cvt1=new memuse_cvt1();
			cvt1.doConvert(linuxFile);
			dataGridView1.DataSource = cvt1._dataTable;
			Application.UseWaitCursor=true;
			Application.DoEvents();
			cvt1.ToCSV(cvt1._dataTable, "exported.csv");
			Application.UseWaitCursor=false;
			Application.DoEvents();
			MessageBox.Show("Done");
            //dataGridView1.DataSource = mLog._dataTable;
        }
    }
}
