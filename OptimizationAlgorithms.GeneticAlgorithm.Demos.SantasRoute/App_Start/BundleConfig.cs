using System.Web.Optimization;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/lib")
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/knockout-{version}.js")
                .Include("~/Scripts/jquery.signalR-{version}.js"));
            bundles.Add(new ScriptBundle("~/bundles/app")
                .Include("~/Scripts/app/Mapper.js")
                .Include("~/Scripts/app/Router.js")
                .Include("~/Scripts/app/ViewModels.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));
        }
    }
}