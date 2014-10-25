using System.Web.Mvc;

namespace gzsw.web.Areas.WARN
{
    public class WARNAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "WARN";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "WARN_default",
                "WARN/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "gzsw.controller.WARN" }
            );
        }
    }
}
