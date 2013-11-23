using System.Web.Mvc;

namespace OptimizationAlgorithms.GeneticAlgorithm.Demos.SantasRoute.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}