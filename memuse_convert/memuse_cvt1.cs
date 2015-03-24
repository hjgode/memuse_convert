using System;
using System.IO;

using System.Data;
using System.Data.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Text;

namespace memuse_convert
{
	/// <summary>
	/// Memuse_cvt1.
	/// read memuse log file (tab delimitted
	/// first field alwas timestamp, use DateTime.ticks
	/// possibly empty fields 0 => skip
	/// skip fields starting with 0x, these are Process IDs
	/// a new dataset is dt.Ticks, Process Name, Process Memory
	/// fields starting with ' are process name
	/// following field is memory use
	/// iterate thru all rows, pick dt.ticks and add new row
	/// to add a new set first check if a column with proc name exists
	/// if yes, just add the memory for this column
	/// if no, add a new column and then add memory reading
	/// </summary>
    public class memuse_cvt1
    {
        public DataTable _dataTable;

        public memuse_cvt1()
        {
            _dataTable = new DataTable();
        }

        public int doConvert(string sFile)
        {
            string sline;
            updateStatus("starting convert");

            //            string sFileTemp = System.IO.Path.GetTempFileName();

            // Read the file and display it line by line.
            System.IO.StreamReader file = File.OpenText(sFile);// new System.IO.StreamReader(sFileName);
            //            System.IO.StreamWriter fileTemp = new StreamWriter(sFileTemp);

            long dtCurrent = DateTime.Now.Ticks;
            //			int iCurrentColumn=0;
            string processName = "";
            string processMemory = "";
            string totalmem = "";

            DataColumn dcTicks = new DataColumn("time", typeof(DateTime));
            _dataTable.Columns.Add(dcTicks);

            int lineCount = 0;
            int errorLines = 0;
            while ((sline = file.ReadLine()) != null)
            {
                System.Console.Write(".");
                string[] splitted = sline.Split(new char[] { '\t' });
                updateStatus(lineCount++.ToString());
                try
                {
                    //first column is datetime
                    dtCurrent = DateTime.Parse(splitted[0], new CultureInfo("de-DE").DateTimeFormat).Ticks;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Exception in DateTime cvt: " + ex.Message);
                    updateStatus("Exception in DateTime cvt: " + ex.Message);
                    errorLines++;
                    continue;//read next line
                }
                DataRow dr = _dataTable.NewRow();
                dr[0] = new DateTime(dtCurrent);
                _dataTable.Rows.Add(dr);

                for (int x = 1; x < splitted.Length; x++)
                {
                    if (splitted[x] == "" || splitted[x].StartsWith("0x"))
                        continue;
                    if (splitted[x].StartsWith("'"))
                    {//process name
                        processName = splitted[x].Trim(new char[] { '\'' });
                        processMemory = splitted[x + 1];
                        //add new column?
                        if (!_dataTable.Columns.Contains(processName))
                        {
                            _dataTable.Columns.Add(processName, typeof(string));
                        }
                        dr[processName] = processMemory;
                        continue;
                    }
                    if (splitted[x].StartsWith("("))
                    {
                        totalmem = splitted[x].Trim(new char[] { '(', ')' });
                        if (!_dataTable.Columns.Contains("total"))
                        {
                            _dataTable.Columns.Add("total", typeof(string));
                        }
                        dr["total"] = totalmem;
                        continue;
                    }
                }

            }

            file.Close();
            //move total column to end
            _dataTable.Columns["total"].SetOrdinal(_dataTable.Columns.Count - 1);
            return 0;
        }

        public void ToCSV(DataTable dtDataTable, string strFilePath)
        {
            StreamWriter sw = new StreamWriter(strFilePath, false);
            //headers  
            for (int i = 0; i < dtDataTable.Columns.Count; i++)
            {
                sw.Write(dtDataTable.Columns[i]);
                if (i < dtDataTable.Columns.Count - 1)
                {
                    sw.Write("\t");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
						string value="";
						if(i==0){ //first column is datetime
							DateTime dt=(DateTime)dr[i];
							value=dt.ToString("dd.MM.yyyy HH:mm:ss");
						}
						else
                        	value = dr[i].ToString();
                        if (value.Contains(","))
                        {
                            value = String.Format("\"{0}\"", value);
                        }
                        sw.Write(value);
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        sw.Write("\t");
                    }
                }
                sw.Write(sw.NewLine);
            }
            sw.Close();
        }

        //event stuff
        public event MyHandler1 Event1;
        private void updateStatus(string s)
        {
            FireEvent(MsgType.progress, s);
        }
        private void FireEvent(MsgType m, string message)
        {
            MyEvent e1 = new MyEvent(m,message);
            e1.message = message;

            if (Event1 != null)
            {
                Event1(this, e1);               
            }

            e1 = null;
        }
    }
}

