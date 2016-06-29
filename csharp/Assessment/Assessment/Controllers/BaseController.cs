using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assessment.Controllers
{
    public class BaseController : Controller
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            string controller = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
            string action = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();

            base.Initialize(requestContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            if (!UserIsAuthenticated())
            {
                System.Web.Security.FormsAuthentication.SignOut();

                filterContext.Result = RedirectToAction("Login", "Main");  // Redirect to login page
            }
        }

        private bool UserIsAuthenticated()
        {
            // Do some custom authentication
            return true;
        }

    }
}