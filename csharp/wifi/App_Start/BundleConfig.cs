using System.Web;
using System.Web.Optimization;

namespace SinetWifi
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/scripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/bootstrap-datepicker.js",
                        "~/Scripts/bootstrap-datepicker-thai.js",
                        "~/Scripts/locales/bootstrap-datepicker.th.js",
                        "~/Content/plugins/bootstrap-dialog/js/bootstrap-dialog.js",
                        "~/Content/plugins/jquery-validation/js/jquery.validate.js",
                        "~/Scripts/back-to-top.js",
                        "~/Scripts/metisMenu.js",
                        "~/Scripts/sb-admin-2.js"));

            bundles.Add(new StyleBundle("~/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/bootstrap-datepicker.css",
                        "~/Content/plugins/bootstrap-dialog/css/bootstrap-dialog.css",
                        "~/Content/font-awesome.css",
                        "~/Content/line-icons.css",
                        "~/Content/metisMenu.css",
                        "~/Content/sb-admin-2.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}