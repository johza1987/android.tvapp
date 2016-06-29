using System.Web.Mvc;

namespace SinetWifi.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Report", action = "Coupon", id = UrlParameter.Optional },
                new[] { "SinetWifi.Areas.Admin.Controllers" }
            );
        }
    }
}
