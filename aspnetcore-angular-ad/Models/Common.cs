using MyMisWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMisWeb.Models
{
    public class RecordImport
    {
        public string Data { get; set; }
    }

    public class RegionCenterName
    {
        public string RegionName { get; set; }
        public string CenterName { get; set; }

        static public RegionCenterName GetName(int centerID, MyMisContext _context)
        {
            var nameInfo = (from center in _context.Centers
                           join region in _context.Regions on center.RegionID equals region.RegionID
                           where center.CenterID == centerID
                           select new RegionCenterName
                           {
                               CenterName = center.Name,
                               RegionName = region.Name
                           }).SingleOrDefault();
            return nameInfo;
        }
    }

    public class ImportReturn
    {
        public string Msg { get; set; }
        public List<string> Data { get; set; }

        public ImportReturn()
        {
            Msg = "";
            Data = new List<string>();
        }

        public delegate string ImportLine(string[] fields);
        static public ImportReturn PerformImport(RecordImport importStr, ImportLine importLineFunction, int headerRowCount=1)
        {
            var importReturn = new ImportReturn();

            var lines = importStr.Data.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            var headerFound = 0;
            var line = "";
            for (int i = 0; i < lines.Length; i++)
            {
                line += lines[i];

                var fields = line.Split(new char[] { '\t' });
                var lastField = fields[fields.Length - 1];
                if (lastField.Length > 0 && lastField[0] == '"')
                {
                    if (lastField.Length == 1 || lastField[lastField.Length - 1] != '\"')
                    {
                        //last field is continued with next line, let's contiue
                        line += "\r\n";
                        continue;
                    }
                }

                if (headerFound < headerRowCount)
                {
                    headerFound++;
                    line = "";
                    continue;
                }

                //process line
                var msg = importLineFunction(fields);
                if (!string.IsNullOrEmpty(msg))
                {
                    importReturn.Data.Add(msg + " -- " + line);
                }

                line = ""; //reset line
            }

            if (importReturn.Data.Count > 0)
            {
                importReturn.Msg = "Errors are found in some lines";
            }
            else
            {
                importReturn.Msg = "Import success!";
            }

            return importReturn;
        }
    }

    public class TimeFromTo
    {
        public int TimeFrom;
        public int TimeTo;

        public DateTime GetDateFrom()
        {
            return new DateTime(TimeFrom / 100, TimeFrom - (TimeFrom / 100) * 100, 1);
        }

        public DateTime GetDateTo()
        {
            return new DateTime(TimeTo / 100, (TimeTo - (TimeTo / 100) * 100) + 1, 1);
        }

        public static int GetMonthFromYearMonth(int yearMonth)
        {
            return (yearMonth - Convert.ToInt32(yearMonth / 100) * 100);
        }

        public static int GetYearFromYearMonth(int yearMonth)
        {
            return yearMonth / 100;
        }

        public static int GetLastYearMonth()
        {
            var firstOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastMonth = firstOfThisMonth.AddMonths(-1);

            return lastMonth.Year * 100 + lastMonth.Month;
        }

        public static int GetYearMonthFromDate(DateTime myDate)
        {
            if (myDate == null)
            {
                return 0;
            }
            return myDate.Year * 100 + myDate.Month;
        }
    }

    public class QueryOption
    {
        public TimeFromTo TimeFromTo { get; set; }
        public string GroupBy { get; set; }
        public string SortBy { get; set; } // - desc/asc
    }
}