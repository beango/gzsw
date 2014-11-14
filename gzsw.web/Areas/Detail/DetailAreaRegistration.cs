using System.Web.Mvc;

namespace gzsw.web.Areas.Detail
{
    public class DetailAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Detail";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Detail_default",
                "Detail/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "gzsw.controller.Detail" }
            );
        }
    }
}
