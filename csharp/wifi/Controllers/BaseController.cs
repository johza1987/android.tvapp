using SinetWifi.Common;
using SinetWifi.Models;
using System;
using System.Web;
using System.Web.Mvc;

namespace SinetWifi.Controllers
{
    public class BaseController : Controller
    {
        protected UserOnline UserOnline = new UserOnline();

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UserOnline = UserOnline.GetUserData(System.Web.HttpContext.Current.Request);

            var RouteData = System.Web.HttpContext.Current.Request.RequestContext.RouteData;
            string action = RouteData.GetRequiredString("action").ToLower();
            string controller = RouteData.GetRequiredString("controller").ToLower();
            string area = (RouteData.DataTokens["area"] != null) ? RouteData.DataTokens["area"].ToString().ToLower() : string.Empty;

            bool IsAuthenticated = (!string.IsNullOrEmpty(UserOnline.Username) && User != null && User.Identity != null && User.Identity.IsAuthenticated) ? true : false;
            bool IsLoginPage = (controller.Equals("Main", StringComparison.OrdinalIgnoreCase) && action.Equals("Login", StringComparison.OrdinalIgnoreCase)) ? true : false;
            bool IsAdmin =  (UserOnline.Role != null) ? UserOnline.Role.Equals(Shared.RoleName.Admintstrator, StringComparison.OrdinalIgnoreCase) : false;

            if (!IsAuthenticated && !IsLoginPage) // Redirect to login page
            {
                System.Web.Security.FormsAuthentication.SignOut();
                filterContext.Result = RedirectToAction("Login", "Main");  
            }
            else if (IsAuthenticated && IsLoginPage) // Redirect to default page if User = IsAuthenticated
            {
                if (IsAdmin)
                    filterContext.Result = RedirectToAction("Coupon", "Report", new { area = "Admin" });
                else 
                    filterContext.Result = RedirectToAction("List", "Customer");  
            }
            else
            {
                if (area.Equals("Admin", StringComparison.OrdinalIgnoreCase) && !IsAdmin)
                    filterContext.Result = RedirectToAction("List", "Customer", new { area = "" });
                else
                    base.OnActionExecuting(filterContext);
            }
        }
    }
}