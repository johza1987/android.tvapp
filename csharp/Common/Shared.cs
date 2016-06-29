using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using InternetAccountModel.EDM;
using WebExtension.Models;

public class Shared
{
    public static string MainUrl
    {
        get { return System.Configuration.ConfigurationManager.AppSettings["MAIN_URL"]; }
    }

    public static CultureInfo CultureInfo
    {
        get { return new CultureInfo("en-US"); }
    }

    public static CultureInfo CultureInfoTh
    {
        get { return new CultureInfo("th-TH"); }
    }

    public static string EnryptString(string strEncrypted)
    {
        //return strEncrypted;
        byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(strEncrypted);
        string encryptedPassword = Convert.ToBase64String(b);
        return encryptedPassword;
    }

    public static string DecryptString(string encrString)
    {
        //return encrString;
        byte[] b = Convert.FromBase64String(encrString);
        string decryptedPassword = System.Text.ASCIIEncoding.ASCII.GetString(b);
        return decryptedPassword;
    }

    public static string ClientInfo()
    {
        return "";
        var Request = System.Web.HttpContext.Current.Request;

        string ip = (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? Request.ServerVariables["REMOTE_ADDR"]).Split(',')[0].Trim();

        //string str = "IP Address : {0} Browser : {1} Version : {2} Platform : {3}";
        //str = String.Format(str, ip, Request.Browser.Browser + (Request.Browser.IsMobileDevice ? " MobileDevice" : ""), Request.Browser.Version, Request.Browser.Platform);
        string str = "Browser : {0} Version : {1} Platform : {2}";
        str = String.Format(str, Request.Browser.Browser + (Request.Browser.IsMobileDevice ? " MobileDevice" : ""), Request.Browser.Version, Request.Browser.Platform);
        return str;
    }

    public static string AssemblyVersion
    {
        get
        {
            string strVersion = String.Empty;
            strVersion += Assembly.GetExecutingAssembly().GetName().Version.Major;
            strVersion += "." + Assembly.GetExecutingAssembly().GetName().Version.Minor;
            strVersion += ((Assembly.GetExecutingAssembly().GetName().Version.Build > 0) ? " Build " + Assembly.GetExecutingAssembly().GetName().Version.Build : "");
            //strVersion += ((Assembly.GetExecutingAssembly().GetName().Version.Revision > 0) ? " (" + Assembly.GetExecutingAssembly().GetName().Version.Revision + ")" : "");

            return strVersion;
        }
    }

    public static DateTime AssemblyUpdate
    {
        get{return new FileInfo(Assembly.GetExecutingAssembly().Location).LastAccessTime;}
    }

    public static UserLogon GetUserLogon()
    {
        return new Authen().GetUserLogon();
    }

    public static PermissionDetail UserPermission()
    {
        return new Authen().GetPermission();
    }

    public static int GetPageSize()
    {
        int PageItemLimit = 30;
        var str = System.Configuration.ConfigurationManager.AppSettings["PageItemLimit"];
        if (!string.IsNullOrEmpty(str))
        {
            PageItemLimit = int.Parse(str);
        }
        return PageItemLimit;
    }

    public class JsonDataTable
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public object data { get; set; }
    }
}
