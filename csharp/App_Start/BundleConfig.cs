using System.Web;
using System.Web.Optimization;

namespace WebExtension
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/script").Include("~/assets/pages/scripts/custom.js"));
            bundles.Add(new StyleBundle("~/css").Include("~/assets/layouts/layout/css/custom.css"));
        }
    }
}
