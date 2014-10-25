using System.Web.Mvc;

namespace gzsw.web.Areas.MON
{
    public class MONAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MON";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MON_default",
                "MON/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "gzsw.controller.MON" }
            );
        }
    }
}
