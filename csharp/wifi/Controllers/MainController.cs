using SinetWifi.Common;
using SinetWifi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace SinetWifi.Controllers
{
    public class MainController : BaseController
    {
        public ActionResult Index()
        {
            return RedirectToAction("Login");      
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Login model, string returnUrl)
        {
            string msg = "Incorrect username or password.";

            if (this.ModelState.IsValid)
            {
                try
                {
                    using (DatabaseContext context = new DatabaseContext())
                    {
                        HotspotUser user = context.HotspotUser.Where(u => u.username.Equals(model.Username) && u.password.Equals(model.Password) && u.status).FirstOrDefault();

                        if (user != null && user.Hotspot != null)
                        {
                            UserOnline userData = new UserOnline();
                            userData.Username = user.username;
                            userData.FullName = user.Hotspot.name;
                            userData.Location = user.Hotspot.type;
                            userData.CustomerCode = user.Hotspot.code;
                            userData.HotspotId = user.Hotspot.id;
                            userData.Role = Shared.RoleName.StandardUser;

                            FormsAuthenticationTicket myticket = new FormsAuthenticationTicket(1, user.username, DateTime.Now, DateTime.Now.AddDays(1), true, new JavaScriptSerializer().Serialize(userData));
                            string encTicket = FormsAuthentication.Encrypt(myticket);
                            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                            if (this.Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return Redirect("Customer/List");
                            }
                        }
                        else
                        {
                            Operator admin = context.Operator.Where(u => u.username.Equals(model.Username) && u.password.Equals(model.Password)).FirstOrDefault();

                            if (admin != null)
                            {
                                UserOnline userData = new UserOnline();
                                userData.Username = admin.username;
                                userData.Role = Shared.RoleName.Admintstrator;

                                FormsAuthenticationTicket myticket = new FormsAuthenticationTicket(1, admin.username, DateTime.Now, DateTime.Now.AddDays(1), true, new JavaScriptSerializer().Serialize(userData));
                                string encTicket = FormsAuthentication.Encrypt(myticket);
                                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));

                                if (this.Url.IsLocalUrl(returnUrl))
                                {
                                    return Redirect(returnUrl);
                                }
                                else
                                {
                                    return Redirect("Admin/Report");
                                }
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    msg = ex.Message + (ex.InnerException != null ?  ex.InnerException.ToString() : string.Empty);
                }
            }

            this.ModelState.AddModelError("", msg);

            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Main");
        }
    }
}
