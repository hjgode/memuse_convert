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
            memuse_logfile mLog = new memuse_logfile(@"D:\!iDaten\_SalesForce_Cases\00616480_cn51_touch\memusage_log_org.txt");

            dataGridView1.DataSource = mLog._dataTable;
        }
    }
}
