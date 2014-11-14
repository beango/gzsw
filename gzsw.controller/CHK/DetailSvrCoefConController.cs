using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.CHK.Models;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using Ninject;
using NPOI.SS.Formula.Functions;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 个人考核指标管理
    /// 明细业务系统
    ///     考核明细业务系数配置
    /// </summary>
    public class DetailSvrCoefConController : BaseController<CHK_DETAIL_SVR_COEF_CON>
    {
        [Inject]
        IDao<SYS_DETAILSERIAL> detailSerialDao { get; set; }

        [UserAuth("CHK_DETAIL_SVR_COEF_CON_VIW")]
        public ActionResult Index(string orgId,int pageIndex=1,int pageSize=20)
        {
            orgId=setOrgData(orgId);

            var list = CHK_DETAIL_SVR_COEF_CON_DAL.GetList(orgId, UserState.UserID, pageIndex, pageSize);

            return View(list);
        }

        [UserAuth("CHK_DETAIL_SVR_COEF_CON_VIW")]
        public ActionResult Details(string id,string orgId)
        {
            try
            {
                var item = CHK_DETAIL_SVR_COEF_CON_DAL.Get(orgId, id);
                var model = new DetailSvrCoefConModel()
                {
                    ORG_ID = item.ORG_ID,
                    ORG_NAM = item.ORG_NAM,
                    COEFFICIENT = item.COEFFICIENT,
                    MODIFY_DTIME = item.MODIFY_DTIME,
                    SERIALID = item.SERIALID,
                    SERIALNAME = item.SERIALNAME,
                    USER_NAM = item.USER_NAM
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_DETAIL_SVR_COEF_CON_ADD")]
        public ActionResult Create()
        {
            setOrgData(null);
            setSerials(null);
            return View();
        }

        [HttpPost]
        [UserAuth("CHK_DETAIL_SVR_COEF_CON_ADD")]
        public ActionResult Create(DetailSvrCoefConAddModel model)
        {
            setOrgData(model.ORG_ID);
            setSerials(model.SERIALID);

            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_DETAIL_SVR_COEF_CON()
                    {
                        ORG_ID = model.ORG_ID,
                        COEFFICIENT=model.COEFFICIENT,
                        MODIFY_DTIME=DateTime.Now,
                        MODIFY_ID=UserState.UserID,
                        SERIALID=model.SERIALID
                    };

                    var rst = dao.AddObject(item);
                    if (null != rst)
                    {
                        Alter("新增成功！", util.Enum.AlterTypeEnum.Success, false, true);
                        return Redirect("/Home/Blank");
                    }

                    ModelState.AddModelError("", "新增失败。");
                    return View(model);
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("新增出错。", ex);
                    ModelState.AddModelError("", "新增出错。");
                    return View(model);
                }
            }

            return View(model);
        }

        /// <summary>
        /// 一键增加
        /// </summary>
        /// <returns></returns>
        [UserAuth("CHK_DETAIL_SVR_COEF_CON_ADD")]
        public ActionResult OneCreate()
        {
            setOrgData(null);
            return View();
        }

        [HttpPost]
        [UserAuth("CHK_DETAIL_SVR_COEF_CON_ADD")]
        public ActionResult OneCreate(DetailSvrCoefConOneAddModel model)
        {
            setOrgData(model.ORG_ID);

            if (ModelState.IsValid)
            {
                try
                {
                    var list= detailSerialDao.FindList("SERIALID");
                    foreach (var deatial in list)
                    {
                        var entity= dao.GetEntity("SERIALID", deatial.SERIALID, "ORG_ID", model.ORG_ID);
                        if (entity == null)
                        {
                            var item = new CHK_DETAIL_SVR_COEF_CON()
                            {
                                ORG_ID = model.ORG_ID,
                                COEFFICIENT = model.COEFFICIENT,
                                MODIFY_DTIME = DateTime.Now,
                                MODIFY_ID = UserState.UserID,
                                SERIALID = deatial.SERIALID
                            };
                            dao.AddObject(item);
                        }
                    }

                    Alter("新增成功！", util.Enum.AlterTypeEnum.Success, false, true);
                    return Redirect("/Home/Blank");
           
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("新增出错。", ex);
                    ModelState.AddModelError("", "新增出错。");
                    return View(model);
                }
            }

            return View(model);
        }


        [UserAuth("CHK_DETAIL_SVR_COEF_CON_EDT")]
        public ActionResult Edit(string id, string orgId)
        {
            try
            {
                var item = CHK_DETAIL_SVR_COEF_CON_DAL.Get(orgId,id);
                var model = new DetailSvrCoefConModel()
                {
                    ORG_ID = item.ORG_ID,
                    ORG_NAM = item.ORG_NAM,
                    COEFFICIENT=item.COEFFICIENT,
                    MODIFY_DTIME=item.MODIFY_DTIME,
                    SERIALID=item.SERIALID,
                    SERIALNAME=item.SERIALNAME,
                    USER_NAM=item.USER_NAM
                };

                return View(model);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [UserAuth("CHK_DETAIL_SVR_COEF_CON_EDT")]
        public ActionResult Edit(DetailSvrCoefConModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = dao.GetEntity("ORG_ID", model.ORG_ID, "SERIALID", model.SERIALID);
                    item.MODIFY_DTIME = DateTime.Now;
                    item.COEFFICIENT = model.COEFFICIENT;
                    item.MODIFY_ID = UserState.UserID;

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

        [UserAuth("CHK_DETAIL_SVR_COEF_CON_DEL")]
        public ActionResult Delete(string id, string orgId)
        {
            try
            {
                var deleteId = id.Split(',');
                var orgIds = orgId.Split(',');
                for (var i = 0; i < deleteId.Length; i++)
                {
                    CHK_DETAIL_SVR_COEF_CON_DAL.Delele(orgIds[i], deleteId[i]);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// 验证业务ID
        /// </summary>
        public ActionResult ValidateSerialId(string SERIALID, string ORG_ID)
        {
            var item = dao.GetEntity("SERIALID", SERIALID, "ORG_ID", ORG_ID);
            return Json(item == null, JsonRequestBehavior.AllowGet);
        }

        private string setOrgData(string orgId)
        {

            if (string.IsNullOrEmpty(orgId))
            {
                orgId = SetViewBagOrgData(orgId, "2");
            }
            else
            {
                SetViewBagOrgData(orgId, "2");
            }
            return orgId;
        }

        private void setSerials(string serialId)
        {
            var list= detailSerialDao.FindList("SERIALID");
            ViewBag.Serials = new SelectList(list, "SERIALID", "SERIALNAME", serialId);
        }
    }
}
