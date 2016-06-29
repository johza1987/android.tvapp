using System.Web;
using System.Web.Optimization;

namespace Assessment
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
                        "~/Scripts/bootstrap3-dialog/js/bootstrap-dialog.js",
                        "~/Scripts/metisMenu.js",
                        "~/Scripts/sb-admin-2.js"));

            bundles.Add(new StyleBundle("~/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/bootstrap-datepicker.css",
                        "~/Scripts/bootstrap3-dialog/css/bootstrap-dialog.css",
                        "~/Content/font-awesome.css",
                        "~/Content/line-icons.css",
                        "~/Content/metisMenu.css",
                        "~/Content/sb-admin-2.css"));
        }
    }
}