using System.Web.Mvc;

namespace gzsw.web.Areas.STAT
{
    public class STATAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "STAT";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "STAT_default",
                "STAT/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "gzsw.controller.STAT" }
            );
        }
    }
}
