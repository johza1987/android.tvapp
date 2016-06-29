using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebExtension.Models;

public class BaseController : Controller
{
    public int PageItemLimit = 30;
    public UserLogon UserLogon = null;
    public PermissionDetail UserPermission = null;
    public string MainUrl = string.Empty;

    protected new JsonResult Json(object data, JsonRequestBehavior behavior)
    {
        return Json(data, "application/json", Encoding.UTF8, behavior);
    }

    protected new JsonResult Json(object data)
    {
        return Json(data, "application/json", Encoding.UTF8, JsonRequestBehavior.DenyGet);
    }

    protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
    {
        return new JsonServiceStackResult
        {
            Data = data,
            ContentType = contentType,
            ContentEncoding = contentEncoding
        };
    }

    protected override void Initialize(System.Web.Routing.RequestContext requestContext)
    {
        //Shared.Clear();

        MainUrl = System.Configuration.ConfigurationManager.AppSettings["MAIN_URL"];
        var str = System.Configuration.ConfigurationManager.AppSettings["PageItemLimit"];
        if (!string.IsNullOrEmpty(str))
        {
            this.PageItemLimit = int.Parse(str);
        }

        //string controller = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
        string action = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
        if (action != "logout" && action != "login")
        {
            var entity = Shared.GetUserLogon();
            if (entity != null)
            {
                this.UserLogon = entity;
                this.UserPermission = new Authen().GetPermission(entity.UserCode);
            }
        }


        base.Initialize(requestContext);
    }

    protected virtual new CustomPrincipal User
    {
        get { return HttpContext.User as CustomPrincipal; }
    }
}