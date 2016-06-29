using Excel;
using SinetWifi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace SinetWifi.Common
{
    public static class Shared
    {    
        public static CultureInfo CultureInfo
        {
            get { return new CultureInfo("en-US"); }
        }

        public static CultureInfo CultureInfoTh
        {
            get { return new CultureInfo("th-TH"); }
        }

        public static string DateFormatFromServer
        {
            get { return ConfigurationManager.AppSettings["DateTimeFormatFromServer"].ToString(); }
        }

        public static string DateFormatToServer
        {
            get { return ConfigurationManager.AppSettings["DateTimeFormatToServer"].ToString(); }
        }

        public static string GetMsg(bool success, string msg)
        {
            if (!string.IsNullOrEmpty(msg))
                msg = msg.Replace("'", "\"");

            return success ? "<i class=\"fa fa-check-circle icon-success\"></i>" + (string.IsNullOrEmpty(msg) ? "สำเร็จ !! บันทึกข้อมูลเรียบร้อยแล้ว" : msg) + "" : "<i class=\"fa fa-times-circle icon-error\"></i>" + (string.IsNullOrEmpty(msg) ? "ผิดพลาด !! ไม่สามารถบันทึกข้อมูล" : msg) + "";
        }

        public static string RandomPassword()
        {
            //var chars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ0123456789";
            var chars = "0123456789";
            var random = new Random();
            Thread.Sleep(1000);
            string password = new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
            return password;
        }

        public static int GetPageNumber(int? p)
        {
            p = (p <= 0) ? 1 : p;

            int pageNumber = (p ?? 1);

            if (pageNumber == 0)
            {
                pageNumber = 1;
            }

            return pageNumber;
        }

        public static string ThaiBaht(string txt)
        {
            string bahtTxt, n, bahtTH = "";
            double amount;
            try { amount = Convert.ToDouble(txt); }
            catch { amount = 0; }
            bahtTxt = amount.ToString("####.00");
            string[] num = { "ศูนย์", "หนึ่ง", "สอง", "สาม", "สี่", "ห้า", "หก", "เจ็ด", "แปด", "เก้า", "สิบ" };
            string[] rank = { "", "สิบ", "ร้อย", "พัน", "หมื่น", "แสน", "ล้าน" };
            string[] temp = bahtTxt.Split('.');
            string intVal = temp[0];
            string decVal = temp[1];
            if (Convert.ToDouble(bahtTxt) == 0)
                bahtTH = "ศูนย์บาทถ้วน";
            else
            {
                for (int i = 0; i < intVal.Length; i++)
                {
                    n = intVal.Substring(i, 1);
                    if (n != "0")
                    {
                        if ((i == (intVal.Length - 1)) && (n == "1"))
                            bahtTH += "เอ็ด";
                        else if ((i == (intVal.Length - 2)) && (n == "2"))
                            bahtTH += "ยี่";
                        else if ((i == (intVal.Length - 2)) && (n == "1"))
                            bahtTH += "";
                        else
                            bahtTH += num[Convert.ToInt32(n)];
                        bahtTH += rank[(intVal.Length - i) - 1];
                    }
                }
                bahtTH += "บาท";
                if (decVal == "00")
                    bahtTH += "ถ้วน";
                else
                {
                    for (int i = 0; i < decVal.Length; i++)
                    {
                        n = decVal.Substring(i, 1);
                        if (n != "0")
                        {
                            if ((i == decVal.Length - 1) && (n == "1"))
                                bahtTH += "เอ็ด";
                            else if ((i == (decVal.Length - 2)) && (n == "2"))
                                bahtTH += "ยี่";
                            else if ((i == (decVal.Length - 2)) && (n == "1"))
                                bahtTH += "";
                            else
                                bahtTH += num[Convert.ToInt32(n)];
                            bahtTH += rank[(decVal.Length - i) - 1];
                        }
                    }
                    bahtTH += "สตางค์";
                }
            }
            return bahtTH;
        }

        public static class RoleName
        {
            public static string Admintstrator { get { return "Administrator"; } }
            public static string StandardUser { get { return "ผู้ใช้ทั่วไป"; } }
        }
    }    

    public class JsonResponse
    {
        public bool Status { get; set; }
        public object Data { get; set; }
        public int Total { get; set; }
        public string Message { get; set; }
    }

    public class ExcelSheetHelper
    {
        public ExcelSheetHelper()
        {
            // Constructor
        }

        public string SetConnectionString(string filePath)
        {
            string excelConnectionString = string.Empty;

            int count = filePath.Split('.').Count();

            string fileExtension = filePath.Split('.')[count - 1];

            if (fileExtension.ToLower().Equals("xls"))
            {
                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=\"Excel 8.0;HDR=No;IMEX=1;\"";
            }
            else if (fileExtension.ToLower().Equals("xlsx"))
            {
                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + filePath + ";Extended Properties=\"Excel 12.0;HDR=No;IMEX=1\"";
            } 

            return excelConnectionString;
        }

        public DataTable GetDataFromExcel(string filePath)
        {
            OleDbConnection excelConnection = new OleDbConnection(SetConnectionString(filePath));

            excelConnection.Open();

            DataTable dt = new DataTable();

            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }

            String[] excelSheets = new String[dt.Rows.Count];

            int t = 0;

            //excel data saves in temp file here.
            foreach (DataRow row in dt.Rows)
            {
                excelSheets[t] = row["TABLE_NAME"].ToString();
                t++;
            }

            DataTable data = new DataTable();

            string query = string.Format("Select * from [{0}]", excelSheets[0]);

            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection))
            {
                dataAdapter.Fill(data);
            }

            excelConnection.Close();

            return data;
        }
    }

    public class CSVReader
    {
        public string[] GetCSVLine(string xline)
        {
            string data = xline.Replace("'", "''");
            if (data == null) return null;
            if (data.Length == 0) return new string[0];
            ArrayList result = new ArrayList();
            this.ParseCSVFields(result, data);
            return (string[])result.ToArray(typeof(string));
        }

        public void ParseCSVFields(ArrayList result, string data)
        {
            int pos = -1;
            while (pos < data.Length)
            {
                result.Add(this.ParseCSVField(data, ref pos));
            }
        }

        public string ParseCSVField(string data, ref int startSeparatorPosition)
        {
            if (startSeparatorPosition == data.Length - 1)
            {
                startSeparatorPosition++;
                // The last field is empty
                return "";
            }

            int fromPos = startSeparatorPosition + 1;

            if (data[fromPos] == '"')
            {

                if (fromPos == data.Length - 1)
                {
                    fromPos++;
                    return "\"";
                }

                int nextSingleQuote = this.FindSingleQuote(data, fromPos + 1);
                startSeparatorPosition = nextSingleQuote + 1;
                return data.Substring(fromPos + 1, nextSingleQuote - fromPos - 1).Replace("\"\"", "\"");
            }


            int nextComma = data.IndexOf(',', fromPos);
            if (nextComma == -1)
            {
                startSeparatorPosition = data.Length;
                return data.Substring(fromPos);
            }
            else
            {
                startSeparatorPosition = nextComma;
                return data.Substring(fromPos, nextComma - fromPos);
            }
        }

        public int FindSingleQuote(string data, int startFrom)
        {

            int i = startFrom - 1;
            while (++i < data.Length)
                if (data[i] == '"')
                {
                    // If this is a double quote, bypass the chars
                    if (i < data.Length - 1 && data[i + 1] == '"')
                    {
                        i++;
                        continue;
                    }
                    else
                        return i;
                }
            return i;
        }

        public DataTable GetDataTable(string filePath ,bool IsFirstRowAsColumnNames = false)
        {
            DataTable dt = new DataTable();

            var LineCollention = File.ReadAllLines(filePath, Encoding.GetEncoding("windows-874"));

            int totalLine = LineCollention.Count();

            if (totalLine > 0)
            {
                var headers = this.GetCSVLine(LineCollention[0]).ToList();
                int totalHeader = headers.Count;
                for (int i = 0; i < totalHeader; i++)
                {
                    dt.Columns.Add(IsFirstRowAsColumnNames ? headers[i] : "column" + i);
                }

                int startrow = IsFirstRowAsColumnNames ? 1 : 0;
                for (int i = startrow; i < totalLine; i++)
                {
                    DataRow row = dt.NewRow();
                    row.ItemArray = this.GetCSVLine(LineCollention[i]);
                    dt.Rows.Add(row);
                }
            }

            return dt;
        }
    }

    public static class QueryableExtension
    {
        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            if (condition)
                return source.Where(predicate);
            else
                return source;
        }
    }

    public static class DateExtension
    {
        public static string ToStringCultureInfoTh(this DateTime? date)
        {
            return (!string.IsNullOrEmpty(date.ToString()) ? date.Value.ToString("dd/MM/yyyy", Shared.CultureInfoTh) : "");
        }
        public static string ToStringCultureInfoTh(this DateTime date)
        {
            return (!string.IsNullOrEmpty(date.ToString()) ? date.ToString("dd/MM/yyyy", Shared.CultureInfoTh): "");
        }
    }

    public static class StringExtension
    {
        public static string ToStringEmpty(this string str)
        {
            return (!string.IsNullOrEmpty(str) ? str : string.Empty);
        }
    }

    public static class DecimalExtension
    {
        public static decimal ToValueEmpty(this decimal dec)
        {
            return (!string.IsNullOrEmpty(dec.ToString()) ? dec : dec = 0);
        }
        public static int ToValueEmpty(this int dec)
        {
            return (!string.IsNullOrEmpty(dec.ToString()) ? dec : dec = 0);
        }
    }

    public class WrappedJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write("<html><body><textarea id=\"jsonResult\" name=\"jsonResult\">");
            base.ExecuteResult(context);
            context.HttpContext.Response.Write("</textarea></body></html>");
            context.HttpContext.Response.ContentType = "text/html";
        }
    }
}