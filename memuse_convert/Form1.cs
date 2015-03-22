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
        memuse_cvt1 cvt1 = new memuse_cvt1();
        string sImportFilename = "";
        public Form1()
        {
            InitializeComponent();
        }
        public Form1(string sFile)
        {
            InitializeComponent(); 
            importFile(sFile);
            exportFile(sFile + ".csv");
        }

        private void button1_Click(object sender, EventArgs e)
        {
			//string linuxFile="/home/hgode/Projects/memuse/memusage.log.099X1400112.txt";
			string linuxFile="memusage.log.099X1400112.txt";
			string winFile=@"D:\!iDaten\_SalesForce_Cases\00616480_cn51_touch\memusage_log_org.txt";
            string sFilename = txtFile.Text;
            importFile(sFilename);
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        void importFile(string sFilename)
        {
            if (sFilename.Length == 0 || !System.IO.File.Exists(sFilename))
            {
                MessageBox.Show("No file");
                return;
            }
            sImportFilename = sFilename;
            //memuse_logfile mLog = new memuse_logfile(linuxfile);
            cvt1 = new memuse_cvt1();
            this.Cursor = Cursors.WaitCursor; Application.DoEvents();
            cvt1.doConvert(sFilename);
            this.Cursor = Cursors.Default; Application.DoEvents();
            dataGridView1.DataSource = cvt1._dataTable;
        }

        void exportFile(string savefile)
        {
            //if (!System.IO.File.Exists(savefile))
            //{
            //    MessageBox.Show("file not exists");
            //    return;
            //}
            if (dataGridView1.Rows.Count > 0)
            {
                Application.UseWaitCursor = true;
                Application.DoEvents();
                cvt1.ToCSV(cvt1._dataTable, savefile);// "exported.csv");
                Application.UseWaitCursor = false;
                Application.DoEvents();
                MessageBox.Show("Done");
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            string savefile = setFilename(sImportFilename);
            if (dataGridView1.Rows.Count > 0)
            {
                Application.UseWaitCursor = true;
                Application.DoEvents();
                cvt1.ToCSV(cvt1._dataTable, savefile);// "exported.csv");
                Application.UseWaitCursor = false;
                Application.DoEvents();
                MessageBox.Show("Done");
            }
        }

        string getFilename()
        {
            string Filename = "";
            OpenFileDialog fd = new OpenFileDialog();
            fd.CheckPathExists = true;
            fd.CheckFileExists = true;
            fd.Filter = "txt|*.txt|all|*.*";
            fd.FilterIndex = 0;
            fd.RestoreDirectory = true;
            fd.Multiselect = false;
            if (fd.ShowDialog() == DialogResult.OK)
                Filename = fd.FileName;
            fd.Dispose();
            return Filename;
        }
        string setFilename(string baseFilename)
        {
            string Filename = baseFilename + ".csv";
            SaveFileDialog fd = new SaveFileDialog();
            fd.FileName = Filename;
            fd.CheckPathExists = true;
            fd.OverwritePrompt = true;
            fd.Filter = "csv|*.csv|all|*.*";
            fd.FilterIndex = 0;
            fd.RestoreDirectory = true;
            if (fd.ShowDialog() == DialogResult.OK)
                Filename = fd.FileName;
            fd.Dispose();
            return Filename;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string sFileInput = getFilename();
            txtFile.Text = sFileInput;
            cvt1._dataTable.Clear();
            importFile(sFileInput);
        }
    }
}
