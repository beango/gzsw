using System.Web.Mvc;

namespace gzsw.web.Areas.AUTH
{
    public class AuthAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "AUTH";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "AUTH_default",
                "AUTH/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "gzsw.controller.AUTH" }
            );
        }
    }
}
