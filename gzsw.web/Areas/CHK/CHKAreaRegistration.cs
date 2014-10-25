using System.Web.Mvc;

namespace gzsw.web.Areas.CHK
{
    public class CHKAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "CHK";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CHK_default",
                "CHK/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new string[] { "gzsw.controller.CHK" }
            );
        }
    }
}
