using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace memuse_convert
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
			if(args.Length==1){
				memuse_cvt1 cvt1=new memuse_cvt1();
				cvt1.doConvert(args[0]);
				//dataGridView1.DataSource = cvt1._dataTable;
				Application.UseWaitCursor=true;
				Application.DoEvents();
				cvt1.ToCSV(cvt1._dataTable, "exported.csv");
				Application.UseWaitCursor=false;
				Application.DoEvents();
				MessageBox.Show("Done");
				Application.Exit();
			}
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
