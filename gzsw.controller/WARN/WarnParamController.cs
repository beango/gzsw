using gzsw.controller.MyAuth;
using gzsw.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.WARN
{
    public class WarnParamController : BaseController<WARN_PARAM>
    {
        [UserAuth("WARN_PARAM_VIW")]
        public ActionResult Index(int pageIndex = 1, int pageSize=20)
        {
            var data = dao.GetList(pageIndex, pageSize, "");
            
            return View(data);
        }

        [UserAuth("WARN_PARAM_ADD")]
        public ActionResult Create()
        {
            return View();
        }
    }
}
