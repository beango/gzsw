using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.CHK.Models;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 个人考核数据管理
    ///     纳税人评价修正
    /// </summary>
    public class CurrqueuehistController : BaseController<SYS_CURRQUEUEHIST>
    {
        [UserAuth("CHK_SYS_CURRQUEUEHIST_VIW")]
        public ActionResult Index(string orgId, int pageIndex = 1, int pageSize = 20)
        {
            

            if (string.IsNullOrEmpty(orgId))
            {
                orgId = SetViewBagOrgData();
            }
            else
            {
                SetViewBagOrgData(orgId);
            }

            var list = SYS_CURRQUEUEHIST_DAL.GetListSub(orgId,1, pageIndex, pageSize);

            return View(list);
        }

        [UserAuth("CHK_SYS_CURRQUEUEHIST_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var model = SYS_CURRQUEUEHIST_DAL.GetSub(id);
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_SYS_CURRQUEUEHIST_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var item = SYS_CURRQUEUEHIST_DAL.GetSub(id);
                var model = new CurrqueuehistModel()
                            {
                                CHQUEUE_COUNTER=item.CHQUEUE_COUNTER,
                                CHQUEUE_ETIME=item.CHQUEUE_ETIME,
                                CHQUEUE_NSRMC=item.CHQUEUE_NSRMC,
                                CHQUEUE_NSRSBM=item.CHQUEUE_NSRSBM,
                                CHQUEUE_PJRESULT=item.CHQUEUE_PJRESULT,
                                CHQUEUE_QSERIALID=item.CHQUEUE_QSERIALID,
                                CHQUEUE_SNO=item.CHQUEUE_SNO,
                                CHQUEUE_SYSNO=item.CHQUEUE_SYSNO,
                                CHQUEUE_TRANSCODEID=item.CHQUEUE_TRANSCODEID,
                                HALL_NAM=item.HALL_NAM,
                                Q_SERIALNAME=item.Q_SERIALNAME,
                                STAFF_NAM=item.STAFF_NAM
                            };
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [UserAuth("CHK_SYS_CURRQUEUEHIST_EDT")]
        public ActionResult Edit(CurrqueuehistModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("CHQUEUE_TRANSCODEID", model.CHQUEUE_TRANSCODEID);

                    item.CHQUEUE_PJRESULT = model.CHQUEUE_PJRESULT;


                    var rst = dao.UpdateObject(item);
                    if (rst > 0)
                    {
                        Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                        return Redirect("/Home/Blank");
                    }
                }

                ModelState.AddModelError("", "修改失败。");
                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错！", ex);
                ModelState.AddModelError("", "修改失败！" + ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
