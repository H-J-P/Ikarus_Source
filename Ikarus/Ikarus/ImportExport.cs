using System;
using System.Collections.Generic;
using System.Data;

namespace Ikarus
{
    static class ImportExport
    {
        public static List<string> logItems = new List<string>();
        private static int maxListcount = 1000;

        public static void LogMessage(string item, bool showTime = true)
        {
            try
            {
                if (logItems.Count > maxListcount) logItems.RemoveAt(6);

                if (showTime) logItems.Add(DateTime.Now.ToString("HH:mm:ss.fff") + "  " + item);
                else logItems.Add(item);
            }
            catch (Exception e)
            {
                LogMessage("Read file to dataset: ... " + e.ToString());
            }
        }

        public static void XmlToDataSet(string fileName, DataSet ds)
        {
            if (ds == null)
                return;

            string fileLoction = "";

            if (fileName.IndexOf("\\") > 0)
                fileLoction = fileName;
            else
                fileLoction = Environment.CurrentDirectory + "\\" + fileName;

            if (System.IO.File.Exists(fileLoction))
            {
                try
                {
                    ds.ReadXml(fileLoction);
                    LogMessage("Read file to dataset: " + fileName);
                }
                catch (Exception e)
                {
                    LogMessage("Read file to dataset: " + fileLoction + " ... " + e.ToString());
                }
            }
            else
            {
                LogMessage("++++++ File not found: " + fileLoction);
            }
        }

        public static void DatasetToXml(string fileName, DataSet ds)
        {
            string fileLoction = "";

            if (fileName.IndexOf("\\") > 0)
                fileLoction = fileName;
            else
                fileLoction = Environment.CurrentDirectory + "\\" + fileName;

            try
            {
                if (ds != null) { ds.WriteXml(fileLoction); }
            }
            catch (Exception e)
            {
                LogMessage("DatasetToXml: " + fileLoction + " ... " + e);
            }
        }

        public static void txtToDataTable(string fileName, ref DataTable dataTable)
        {
            dataTable.Clear();

            if (System.IO.File.Exists(Environment.CurrentDirectory + "\\" + fileName))
            {
                string[] lines = System.IO.File.ReadAllLines(Environment.CurrentDirectory + "\\" + fileName);

                foreach (string line in lines)
                {
                    if (line != "")
                    {
                        DataRow dataRow = dataTable.NewRow();
                        dataRow[0] = line;
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }
            else
            {
                LogMessage("File not found: " + Environment.CurrentDirectory + "\\" + fileName);
            }
        }
    }
}
