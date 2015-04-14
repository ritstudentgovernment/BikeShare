using System.Web.Optimization;

namespace BikeShare
{
    /// <summary>
    /// Handles bundling and minification of resources.
    /// </summary>
    public class BundleConfig
    {
        /// <summary>
        /// Registers the bundled resources.
        /// </summary>
        /// <param name="bundles"></param>
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
          "~/Content/js/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrap").Include("~/Content/bootstrap.css"));
            bundles.Add(new LessBundle("~/Content/less").Include("~/Content/*.less"));
        }
    }
}