using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using InternetAccountModel.EDM;
using WebExtension.Common;
using WebExtension.Models;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace WebExtension.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
           string responseMessage = String.Empty;
            //var sap = new FTTx();
            //sap.Connecting(out responseMessage);
            //if (sap.company.Connected)
            //{
            //    responseMessage = "Connected :)";
            //}
            //else
            //{
            //    responseMessage = sap.company.GetLastErrorDescription();
            //}
            ViewBag.Connect = responseMessage;
            return View();
        }

        public ActionResult Login()
        {
            return Redirect(this.MainUrl);
        }

        public ActionResult Logout()
        {
            using (var db = new InternetAccountEntities())
            {
                if (!string.IsNullOrEmpty(System.Web.HttpContext.Current.User.Identity.Name))
                {
                    var user = new Authen().GetUserLogon();
                    if (user != null)
                    {
                        db.UserOnlines.Where(r => r.UserId == user.UserId).ToList().ForEach(r => db.UserOnlines.Remove(r));
                        db.SaveChanges();
                    }
                }
            }
           
            if (Request.Cookies[FormsAuthentication.FormsCookieName + "SINET"] != null)
            {
                var c = new HttpCookie(FormsAuthentication.FormsCookieName + "SINET");
                c.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(c);
            }

            FormsAuthentication.SignOut();
            System.Web.HttpContext.Current.Session.RemoveAll();
            return Redirect(this.MainUrl);
        }
    }
}