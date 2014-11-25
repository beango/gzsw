using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;

namespace gzsw.controller.WARN
{
    public class CurrqueuehistEvaluationController : BaseController
    {
        /// <summary>
        /// 根据评价类型获取排队业务明细
        /// </summary>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="evaluationType">评价类型（1：评价总数  2：未平价总数  3：差评数  4：未评价率  5：满意率  6：差评率）</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [UserAuth("MON_Warning_VIW")]
        public ActionResult Index(string orgId, int evaluationType, int pageIndex = 1, int pageSize = 20)
        {
            var list = SYS_CURRQUEUEHIST_DAL.GetPagerByEvaluationType(orgId, evaluationType, pageIndex, pageSize);

            var indexTitle = string.Empty;

            switch (evaluationType)
            {
                case 1:
                    indexTitle = "评价总数";
                    break;
                case 2:
                    indexTitle = "未评价总数";
                    break;
                case 3:
                    indexTitle = "差评数";
                    break;
                case 4:
                    indexTitle = "未评价率";
                    break;
                case 5:
                    indexTitle = "满意率";
                    break;
                case 6:
                    indexTitle = "差评率";
                    break;
            }

            ViewBag.IndexTitle = indexTitle;
            return View(list);
        }
    }
}
