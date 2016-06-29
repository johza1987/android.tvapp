using SinetWifi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SinetWifi.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index()
        {
            return View("Error");
        }

        public ActionResult NotFound(string aspxerrorpath)
        {
            ViewData["error_path"] = aspxerrorpath;

            return View();
        }

        public ActionResult AccessDenied(string action)
        {
            ViewData["action"] = action;

            return View();
        }

        public ActionResult TestConnect()
        {
            using (DatabaseContext context = new DatabaseContext())
            {
                try
                {
                    var x = context.Hotspot.ToList<Hotspot>();

                    ViewData["Result"] = "Success";
                }
                catch(Exception ex)
                {
                    ViewData["Result"] = "Fail!! " + ex.Message.ToString();
                }
            }

            return View();
        }
    }
}
