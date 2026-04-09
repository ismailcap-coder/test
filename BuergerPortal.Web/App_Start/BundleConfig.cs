using System.Web.Optimization;
using System.Web.Hosting;
using System.IO;

namespace BuergerPortal.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Only register script bundles if the Scripts directory exists (prevents runtime errors when files/folders are missing)
            var scriptsPhysicalPath = HostingEnvironment.MapPath("~/Scripts");
            if (!string.IsNullOrEmpty(scriptsPhysicalPath) && Directory.Exists(scriptsPhysicalPath))
            {
                bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                    "~/Scripts/jquery-{version}.js"));

                bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                    "~/Scripts/jquery.unobtrusive*",
                    "~/Scripts/jquery.validate*"));
            }

            // Only register style bundles if the Content directory exists
            var contentPhysicalPath = HostingEnvironment.MapPath("~/Content");
            if (!string.IsNullOrEmpty(contentPhysicalPath) && Directory.Exists(contentPhysicalPath))
            {
                bundles.Add(new StyleBundle("~/Content/css").Include(
                    "~/Content/Site.css"));
            }
        }
    }
}
