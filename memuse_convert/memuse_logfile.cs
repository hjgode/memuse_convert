using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using System.Data;

namespace memuse_convert
{
    class memuse_logfile
    {
        //public DataTable _dataTable;
        public List<memuse> myMemuse = new List<memuse>();
        //we need a list of times and process names with there mem usage
        //          name1       name2...
        //time1     mem11       mem21...
        //time2     mem12       mem22...
        string sFileName = "";

        int intCounter = 0;

        public DataTable transpose()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("time"));
            //find all time values
            var times1 = myMemuse.Distinct(new memuse.TimeComparer());
            var times = times1.Select(item => item.dt);
            var procs = myMemuse.Distinct(new memuse.Comparer());
            foreach (memuse m in procs)
            {
                dt.Columns.Add(new DataColumn(m.procname));
            }
            //go thru all time values
            foreach ( DateTime d in times )
            {
                DataRow dr = dt.Rows.Add(new object[]{ d.ToString() });
                //go thru all processes for this time
                var query = from item in myMemuse
                            where item.dt == d
                            select new
                            {
                                item.procID,
                                item.procname,
                                item.procmem
                            };
                //add the mem used value to the right column
                foreach (var m in query)
                {
                    dr[m.procname] = m.procmem;
                }
            }
            return dt;
        }

        public memuse_logfile(string sFile)
        {
            sFileName = sFile;
            string line;

            string sFileTemp = System.IO.Path.GetTempFileName();

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(sFileName);
            System.IO.StreamWriter fileTemp = new StreamWriter(sFileTemp);
            while ((line = file.ReadLine()) != null)
            {
                System.Console.Write(".");
                StringBuilder sb=new StringBuilder();
                string[] newCol = getProcData(line);
                if (newCol[0] == "")
                    continue;
                if (newCol.Length != 98)//66)
                    System.Diagnostics.Debugger.Break();
                for (int x = 0; x < newCol.Length;x++ )
                {
                    if (x < newCol.Length - 1)
                        sb.Append(newCol[x] + "\t");
                    else
                        sb.Append(newCol[x]);
                }
                fileTemp.WriteLine(sb.ToString());
            }
            file.Close();
            fileTemp.Close();

            ////prepare the datatable
            //DataTable dt = new DataTable();
            //dt.Columns.Add("time", typeof(DateTime));
            //for (int x = 1; x < 65; x++)
            //{
            //    dt.Columns.Add("name" + x.ToString(), typeof(string));
            //    dt.Columns.Add("memuse" + x.ToString(), typeof(string));
            //    x++;
            //}
            //dt.Columns.Add("total", typeof(int));

            ////TransferCSVToTable(dt, sFileTemp);

            ////the table needs to be transposed to get one column per process name
            ////then add new lines for every time stamp with the memory usage of each process

            ////DataTable dtNew = this.GenerateTransposedTable(dt);
            ////_dataTable = dtNew;
            //_dataTable = dt;
        }


        static DateTime lastDateTime=DateTime.Now;
        string[] getProcData(string sLine)
        {
            //normalize column count
            //max is timecol + 32Slots*3 [ProcId,ProcName,ProcMem] + 1 column with total = 98 columns
            //we ignore the ProcID column:
            //max is timecol + 32Slots*2 [ProcName,ProcMem] + 1 column with total = 98 columns
            const int maxCols=98;
            //empty Row
            string[] newRow = new string[maxCols];
            int currColoum = 0;
            string[] splitted = sLine.Split(new char[] { '\t' });
            //first is always datetim
            DateTime date = DateTime.Now + new TimeSpan(0, 0, 5*intCounter); //use a generic datetime if fails
            try
            {
                date = DateTime.Parse(splitted[0]);
            }
            catch (Exception)
            {
                date = DateTime.Now + new TimeSpan(0, 0, 5 * intCounter);
            }
            //if (lastDateTime == date)
            //    date = date + new TimeSpan(0, 0, 0, 0, 1);
            lastDateTime = date;
            //assign time to first col
            newRow[currColoum++] = date.ToShortDateString() + " " + date.ToShortTimeString();

            intCounter++;
            string proc_name = "";
            uint proc_mem = 0;
            uint proc_id = 0;
            
            for (int x = 1; x < splitted.Length; x++)
            {

                if (splitted[x].Length == 0)    //not interested in empty cells
                    continue;
                if (splitted[x].StartsWith("0x"))
                {   //process IDs
                    proc_id = Convert.ToUInt32(splitted[x].Substring(2), 16);
                    x++;
                    if (splitted[x].StartsWith("'"))    //the process name
                    {
                        splitted[x] = splitted[x].Trim(new char[] { '\'' });
                        proc_name = splitted[x];
                        if (x + 1 < splitted.Length)
                        {    //next is proc_mem
                            proc_mem = uint.Parse(splitted[x + 1]);
                            //now we have a datetime, a name and the memory
                            //proc_dict.Add(proc_name, new process_memory_usage(date, proc_name, proc_mem));
                            //procList.Add(new process_memory_usage(date, proc_name, proc_mem));
                            newRow[currColoum++] = proc_name;
                            newRow[currColoum++] = proc_mem.ToString();
                            x++;
                            myMemuse.Add(new memuse(date, proc_id, proc_name, proc_mem));
                            continue;
                        }
                    }
                    continue;
                }
                if (x==splitted.Length-1 && splitted[x].StartsWith("("))
                {    //the total value is within brackets
                    splitted[x] = splitted[x].Trim(new char[] { '(', ')' });
                    myMemuse.Add(new memuse(date, 0, "Total", uint.Parse(splitted[x])));
                    //proc_dict.Add("total", new process_memory_usage(date, "total", Math.Abs(int.Parse(splitted[x]))));
                    //procList.Add(new process_memory_usage(date, "total", Math.Abs(int.Parse(splitted[x]))));
                    newRow[maxCols - 1] = Math.Abs(int.Parse(splitted[x])).ToString();
                }
            }
            return newRow;
        }


    }
}
