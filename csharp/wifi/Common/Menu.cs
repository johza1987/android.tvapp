using SinetWifi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SinetWifi.Common
{
    public static class Menu
    {
        public static MvcHtmlString RenderNavigationBar(this HtmlHelper html)
        {
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);

            var user = UserOnline.GetUserDataFromCookie(System.Web.HttpContext.Current.Request);

            StringBuilder sb = new StringBuilder();

            sb.Append("<ul class=\"nav navbar-top-links navbar-right\">");
                sb.Append("<li class=\"dropdown\">");
                    sb.Append("<a class=\"dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\">");
                    sb.Append("<i class=\"icon-user\"></i>&nbsp; " + user.Username + ", " + user.FullName + "<span class=\"text-required\"> " + ConfigurationManager.AppSettings["ApplicationTitle"].ToString() + "</span> &nbsp;<i class=\"fa fa-caret-down\"></i>");
                    sb.Append("</a>");
                    sb.Append("<ul class=\"dropdown-menu dropdown-user\">");
                    sb.Append("<li><a href=\"" + urlHelper.Action("LogOut", "Main", new { area = string.Empty }) + "\"><i class=\"icon-power\"></i>&nbsp; Logout</a></li>");
                    sb.Append("</ul>");
                sb.Append("</li>");
            sb.Append("</ul>");

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString RenderSideBar(this HtmlHelper html)
        {
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            var RouteData = System.Web.HttpContext.Current.Request.RequestContext.RouteData;
            string area = (RouteData.DataTokens["area"] != null) ? RouteData.DataTokens["area"].ToString().ToLower() : string.Empty;

            string str = string.Empty;

            switch (area.ToLower())
            {
                case "admin": str = AdminMenu(urlHelper); break;
                default: str = DefaultMenu(urlHelper); break;
            }

            return new MvcHtmlString(str);
        }

        private static string DefaultMenu(UrlHelper urlHelper)
        {
            string groupmenu = string.Empty;
            string controller = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
            string action = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
            string page = controller + "-" + action; ;

            StringBuilder sb = new StringBuilder();

            sb.Append("<ul class=\"nav\" id=\"side-menu\">");

            groupmenu = "Customer";
            sb.Append("<li " + (controller.Equals(groupmenu, StringComparison.OrdinalIgnoreCase) ? "class=\"active\"" : "") + ">");
                sb.Append("<a href=\"#\"><i class=\"icon-users\"></i>&nbsp; ข้อมูลลูกค้า <span class=\"fa arrow\"></span></a>");
                sb.Append("<ul class=\"nav nav-second-level " + (controller.Equals(groupmenu, StringComparison.OrdinalIgnoreCase) ? "collapse in" : "collapse") + "\">");
                    sb.Append("<li>");
                    sb.Append("<a " + (page.Equals(controller + "-list", StringComparison.OrdinalIgnoreCase) ? "class=\"active\"" : "") + " href=\"" + urlHelper.Action("List", groupmenu) + "\">รายชื่อลูกค้า</a>");
                    sb.Append("</li>");
                    sb.Append("<li>");
                    sb.Append("<a " + (page.Equals(controller + "-create", StringComparison.OrdinalIgnoreCase) ? "class=\"active\"" : "") + " href=\"" + urlHelper.Action("Create", groupmenu) + "\">ลงทะเบียนลูกค้า</a>");
                    sb.Append("</li>");
               sb.Append("</ul>");
            sb.Append("</li>");

            groupmenu = "Report";
            sb.Append("<li " + (controller.Equals(groupmenu, StringComparison.OrdinalIgnoreCase) ? "class=\"active\"" : "") + ">");
                sb.Append("<a href=\"#\"><i class=\"icon-bar-chart\"></i>&nbsp; รายงาน <span class=\"fa arrow\"></span></a>");
                sb.Append("<ul class=\"nav nav-second-level " + (controller.Equals(groupmenu, StringComparison.OrdinalIgnoreCase) ? "collapse in" : "collapse") + "\">");
                    sb.Append("<li>");
                    sb.Append("<a " + (page.Equals(controller + "-coupon", StringComparison.OrdinalIgnoreCase) ? "class=\"active\"" : "") + " href=\"" + urlHelper.Action("Coupon", groupmenu) + "\">รายงานการออกคูปอง</a>");
                    sb.Append("</li>");
                sb.Append("</ul>");
            sb.Append("</li>");

            sb.Append("</ul>");

            return sb.ToString();
        }

        private static string AdminMenu(UrlHelper urlHelper)
        {
            string groupmenu = string.Empty;
            string controller = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString().ToLower();
            string action = System.Web.HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString().ToLower();
            string page = controller + "-" + action; ;

            StringBuilder sb = new StringBuilder();

            sb.Append("<ul class=\"nav\" id=\"side-menu\">");

            groupmenu = "Report";
            sb.Append("<li " + (controller.Equals(groupmenu, StringComparison.OrdinalIgnoreCase) ? "class=\"active\"" : "") + ">");
                sb.Append("<a href=\"#\"><i class=\"icon-bar-chart\"></i>&nbsp; รายงาน <span class=\"fa arrow\"></span></a>");
                sb.Append("<ul class=\"nav nav-second-level " + (controller.Equals(groupmenu, StringComparison.OrdinalIgnoreCase) ? "collapse in" : "collapse") + "\">");
                    sb.Append("<li>");
                    sb.Append("<a " + (page.Equals(controller + "-coupon", StringComparison.OrdinalIgnoreCase) ? "class=\"active\"" : "") + " href=\"" + urlHelper.Action("Coupon", groupmenu) + "\">รายงานการออกคูปอง</a>");
                    sb.Append("</li>");
                sb.Append("</ul>");
            sb.Append("</li>");

            //groupmenu = "SMS";
            //sb.Append("<li " + (controller.Equals(groupmenu, StringComparison.OrdinalIgnoreCase) ? "class=\"active\"" : "") + ">");
            //    sb.Append("<a href=\"#\"><i class=\"icon-envelope-letter\"></i>&nbsp; SMS <span class=\"fa arrow\"></span></a>");
            //    sb.Append("<ul class=\"nav nav-second-level " + (controller.Equals(groupmenu, StringComparison.OrdinalIgnoreCase) ? "collapse in" : "collapse") + "\">");
            //        sb.Append("<li>");
            //        sb.Append("<a " + (page.Equals(controller + "-message", StringComparison.OrdinalIgnoreCase) ? "class=\"active\"" : "") + " href=\"" + urlHelper.Action("Message", groupmenu) + "\">ส่งข้อความ</a>");
            //        sb.Append("</li>");
            //    sb.Append("</ul>");
            //sb.Append("</li>");

            sb.Append("</ul>");

            return sb.ToString();
        }
    }
}