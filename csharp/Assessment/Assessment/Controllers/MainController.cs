using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assessment.Controllers
{
    public class MainController : Controller
    {
        //
        // GET: /Main/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string AuthenType, string username, string passsword)
        {
            if (AuthenType.Equals("teacher")) // คุณครู
            {
                Session["AuthenType"] = AuthenType;
                return Redirect("Assessment/Create");
            }
            else if (AuthenType.Equals("officer")) // เจ้าหน้าที่
            {
                Session["AuthenType"] = AuthenType;
                return Redirect("School/Index");
            }

            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string account)
        {
            return View("Login");
        }
    }
}
