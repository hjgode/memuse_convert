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
        memuse_cvt1 cvt1;
        string sImportFilename = "";

        public Form1()
        {
            InitializeComponent();
            cvt1 = new memuse_cvt1();
            cvt1.Event1 += new MyHandler1(cvt1_Event1); 
        }

        void cvt1_Event1(object sender, MyEvent e)
        {
            switch (e.msgType)
            {
                case MsgType.info:
                    addLog(e.message);
                    break;
                case MsgType.progress_max:
                    Invoke(new Action(() => this.progressBar1.Maximum = e.num));
                    Application.DoEvents();
                    break;
                case MsgType.progress:
                    Invoke(new Action(() => this.lblLineCount.Text=e.num.ToString()));
                    Invoke(new Action(() => this.progressBar1.Value = e.num));
                    Invoke(new Action(() => this.progressBar1.Update()));
                    Application.DoEvents();
                    break;
            }
            Application.DoEvents();
        }


        public Form1(string sFile)
        {
            InitializeComponent();
            cvt1 = new memuse_cvt1();
            importFile(sFile);
            exportFile(sFile + ".csv");
        }

        delegate void SetTextCallback(string text);
        public void addLog(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtLog.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(addLog);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (txtLog.Text.Length > 32000)
                    txtLog.Text = "";
                txtLog.Text += text + "\r\n";
                txtLog.SelectionLength = 0;
                txtLog.SelectionStart = txtLog.Text.Length - 1;
                txtLog.ScrollToCaret();
            }
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
            this.Cursor = Cursors.WaitCursor; Application.DoEvents();
            cvt1.doConvert(sFilename);
            this.Cursor = Cursors.Default; Application.DoEvents();
            dataGridView1.DataSource = cvt1._dataTable;
            txtLog.Text+="file '"+sFilename+"' imported\r\n";
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
                txtLog.Text += "Export to '" + savefile + "' Done\r\n";
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            string savefile = setFilename(sImportFilename);
            if (savefile == null)
                return;
            if (dataGridView1.Rows.Count > 0)
            {
                Application.UseWaitCursor = true;
                Application.DoEvents();
                cvt1.ToCSV(cvt1._dataTable, savefile);// "exported.csv");
                Application.UseWaitCursor = false;
                Application.DoEvents();
                txtLog.Text += "Export to '" + savefile + "' Done\r\n";
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
            else
                Filename = null;
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
            else
                Filename = null;
            fd.Dispose();
            return Filename;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            string sFileInput = getFilename();
            if (sFileInput == null)
                return;
            txtFile.Text = sFileInput;
            if (System.IO.File.Exists(sFileInput))
            {
                if(cvt1._dataTable!=null)
                    cvt1._dataTable.Clear();
                importFile(sFileInput);
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string sFileInput in files)
            {
                Console.WriteLine(sFileInput);
                txtFile.Text = sFileInput;
                if (System.IO.File.Exists(sFileInput))
                {
                    if(cvt1._dataTable!=null)
                        cvt1._dataTable.Clear();
                    importFile(sFileInput);
                    string savefile = sFileInput + ".csv";
                    if (dataGridView1.Rows.Count > 0)
                    {
                        Application.UseWaitCursor = true;
                        Application.DoEvents();
                        cvt1.ToCSV(cvt1._dataTable, savefile);// "exported.csv");
                        Application.UseWaitCursor = false;
                        Application.DoEvents();
                        txtLog.Text+="Export to '"+sFileInput+ ".csv' Done\r\n";
                    }

                }
            }
        }

        //allow dragging onto form
        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }
    }
}
