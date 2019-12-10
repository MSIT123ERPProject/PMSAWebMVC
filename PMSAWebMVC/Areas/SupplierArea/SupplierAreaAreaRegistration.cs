using System.Web.Mvc;

namespace PMSAWebMVC.Areas.SupplierArea
{
    public class SupplierAreaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "SupplierArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "SupplierArea_default",
                "SupplierArea/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}