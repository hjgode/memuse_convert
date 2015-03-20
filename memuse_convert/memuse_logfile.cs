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
        public DataTable _dataTable;
        //we need a list of times and process names with there mem usage
        //          name1       name2...
        //time1     mem11       mem21...
        //time2     mem12       mem22...
        string sFileName = "";

        int intCounter = 0;
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
                if (newCol.Length != 66)
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

            DataTable dt = new DataTable();
            dt.Columns.Add("time", typeof(DateTime));
            for (int x = 1; x < 65; x++)
            {
                dt.Columns.Add("name" + x.ToString(), typeof(string));
                dt.Columns.Add("memuse" + x.ToString(), typeof(string));
                x++;
            }
            dt.Columns.Add("total", typeof(int));

            TransferCSVToTable(dt, sFileTemp);
            DataTable dtNew = this.GenerateTransposedTable(dt);
            _dataTable = dtNew;
        }

        static DateTime lastDateTime=DateTime.Now;
        string[] getProcData(string sLine)
        {
            //normalize column count
            //max is timecol + 32Slots*3 [ProcId,ProcName,ProcMem] + 1 column with total = 98 columns
            //we ignore the ProcID column:
            //max is timecol + 32Slots*2 [ProcName,ProcMem] + 1 column with total = 98 columns
            const int maxCols=66;
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
            if (lastDateTime == date)
                date = date + new TimeSpan(0, 0, 0, 0, 1);
            lastDateTime = date;
            //assign time to first col
            newRow[currColoum++] = date.ToShortDateString() + " " + date.ToShortTimeString();

            intCounter++;
            string proc_name = "";
            int proc_mem = 0;
            for (int x = 1; x < splitted.Length; x++)
            {

                if (splitted[x].Length == 0)    //not interested in empty cells
                    continue;
                if (splitted[x].StartsWith("0x"))   //not interested in process IDs
                    continue;
                if (splitted[x].StartsWith("'"))    //the process name
                {
                    splitted[x] = splitted[x].Trim(new char[] { '\'' });
                    proc_name = splitted[x];
                    if (x + 1 < splitted.Length)
                    {    //next is proc_mem
                        proc_mem = int.Parse(splitted[x+1]);
                        //now we have a datetime, a name and the memory
                        //proc_dict.Add(proc_name, new process_memory_usage(date, proc_name, proc_mem));
                        //procList.Add(new process_memory_usage(date, proc_name, proc_mem));
                        newRow[currColoum++] = proc_name;
                        newRow[currColoum++] = proc_mem.ToString();
                        x++;
                        continue;
                    }
                }
                if (x==splitted.Length-1 && splitted[x].StartsWith("("))
                {    //the total value is within brackets
                    splitted[x] = splitted[x].Trim(new char[] { '(', ')' });
                    //proc_dict.Add("total", new process_memory_usage(date, "total", Math.Abs(int.Parse(splitted[x]))));
                    //procList.Add(new process_memory_usage(date, "total", Math.Abs(int.Parse(splitted[x]))));
                    newRow[maxCols - 1] = Math.Abs(int.Parse(splitted[x])).ToString();
                }
            }
            return newRow;
        }
        public static void TransferCSVToTable(DataTable dt, string filePath)
        {
            string[] csvRows = System.IO.File.ReadAllLines(filePath);
            string[] fields = null;
            foreach (string csvRow in csvRows)
            {
                fields = csvRow.Split('\t');
                DataRow row = dt.NewRow();
                row.ItemArray = fields;
                dt.Rows.Add(row);
            }
        }
        private DataTable GenerateTransposedTable(DataTable inputTable)
        {
            DataTable outputTable = new DataTable();

            // Add columns by looping rows

            // Header row's first column is same as in inputTable
            outputTable.Columns.Add(inputTable.Columns[0].ColumnName.ToString());

            int count = 0;
            // Header row's second column onwards, 'inputTable's first column taken
            foreach (DataRow inRow in inputTable.Rows)
            {
                string newColName = inRow[0].ToString();
                outputTable.Columns.Add(count.ToString()); //newColName);
                count++;
                System.Console.Write("+");
            }

            // Add rows by looping columns        
            for (int rCount = 1; rCount <= inputTable.Columns.Count - 1; rCount++)
            {
                DataRow newRow = outputTable.NewRow();

                // First column is inputTable's Header row's second column
                newRow[0] = inputTable.Columns[rCount].ColumnName.ToString();
                for (int cCount = 0; cCount <= inputTable.Rows.Count - 1; cCount++)
                {
                    string colValue = inputTable.Rows[cCount][rCount].ToString();
                    System.Console.Write("#");
                    newRow[cCount + 1] = colValue; 
                }
                outputTable.Rows.Add(newRow);
            }

            return outputTable;
        }

    }

    //class process_memory_usage
    //{
    //    public DateTime datetime;
    //    public String name;
    //    public int process_memory;
    //    public process_memory_usage(DateTime dt, string n, int mem)
    //    {
    //        datetime = dt;
    //        name = n;
    //        process_memory = mem;
    //    }
    //}

}
