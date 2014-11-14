using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using PetaPoco;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 考勤参数
    /// </summary>
    public class TimeScoreController : BaseController<CHK_TIMESCORE_PARAM>
    {

        [Ninject.Inject]
        public IDao<CHK_TIMESCORE_PARAM> timescoreParamDao { get; set; }

        [UserAuth("CHK_TIMESCORE_PARAM_VIW")]
        public ActionResult Index(string orgId, string orgName, int pageIndex = 1, int pageSize = 20)
        {

            SetViewBagOrgData(orgId);

            var timescoreDal = new CHK_TIMESCORE_PARAM_DAL();
            var list = timescoreDal.GetPageList(orgId, UserState.UserID);

            var pages = new Page<CHK_TIMESCORE_PARAM>()
                        {
                            ItemsPerPage=pageSize,
                            CurrentPage = pageIndex,
                            Items = list.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(),
                            TotalItems = list.Count,
                            TotalPages = (list.Count % pageSize) == 0 ? list.Count / pageSize : (list.Count / pageSize) + 1
                        };

            return View(pages);
        }

        [UserAuth("CHK_TIMESCORE_PARAM_ADD")]
        public ActionResult Create()
        {
            SetViewBagOrgData();
            return View();
        }

        [UserAuth("CHK_TIMESCORE_PARAM_ADD")]
        [HttpPost]
        public ActionResult Create(TimeScoreCreateModel model)
        {
            SetViewBagOrgData(model.ORG_ID);
            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_TIMESCORE_PARAM()
                    {
                        ORG_ID=model.ORG_ID,
                        A_BEGIN_TIME=model.A_BEGIN_TIME,
                        A_END_TIME = model.A_END_TIME,
                        EAR_LAST_MIN=model.EAR_LAST_MIN,
                        LAT_LAST_MIN=model.LAT_LAST_MIN,
                        P_BEGIN_TIME=model.P_BEGIN_TIME,
                        P_END_TIME=model.P_END_TIME
                    };

                    var timescoreDal = new CHK_TIMESCORE_PARAM_DAL();
                    timescoreDal.Save(item,UserState.UserID);

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

        [UserAuth("CHK_TIMESCORE_PARAM_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var timescoreDal = new CHK_TIMESCORE_PARAM_DAL();
                var param = timescoreDal.Get(id);
                var item = new TimeScoreCreateModel()
                {
                    A_BEGIN_TIME = param.A_BEGIN_TIME,
                    A_END_TIME=param.A_END_TIME,
                    EAR_LAST_MIN=param.EAR_LAST_MIN,
                    LAT_LAST_MIN=param.LAT_LAST_MIN,
                    ORG_ID=param.ORG_ID,
                    ORG_NAM=param.HALL_NAM,
                    P_BEGIN_TIME=param.P_BEGIN_TIME,
                    P_END_TIME=param.P_END_TIME
                };

                return View(item);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_TIMESCORE_PARAM_EDT")]
        [HttpPost]
        public ActionResult Edit(TimeScoreCreateModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var item = new CHK_TIMESCORE_PARAM()
                    {
                        ORG_ID = model.ORG_ID,
                        A_BEGIN_TIME = model.A_BEGIN_TIME,
                        A_END_TIME = model.A_END_TIME,
                        EAR_LAST_MIN = model.EAR_LAST_MIN,
                        LAT_LAST_MIN = model.LAT_LAST_MIN,
                        P_BEGIN_TIME = model.P_BEGIN_TIME,
                        P_END_TIME = model.P_END_TIME,
                        MODIFY_DTIME=DateTime.Now,
                        MODIFY_ID=UserState.UserID
                    };

                    var rst = timescoreParamDao.UpdateObject(item);
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

        [UserAuth("CHK_TIMESCORE_PARAM_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteId = id.Split(',');
                foreach (var did in deleteId)
                {
                    timescoreParamDao.Delete("ORG_ID", did);
                }
               
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }


        [UserAuth("CHK_TIMESCORE_PARAM_VIW")]
        public ActionResult Detail(string id)
        {
            try
            {
                var timescoreDal = new CHK_TIMESCORE_PARAM_DAL();
                var param = timescoreDal.Get(id);
                var item = new TimeScoreCreateModel()
                {
                    A_BEGIN_TIME = param.A_BEGIN_TIME,
                    A_END_TIME = param.A_END_TIME,
                    EAR_LAST_MIN = param.EAR_LAST_MIN,
                    LAT_LAST_MIN = param.LAT_LAST_MIN,
                    ORG_ID = param.ORG_ID,
                    ORG_NAM = param.HALL_NAM,
                    P_BEGIN_TIME = param.P_BEGIN_TIME,
                    P_END_TIME = param.P_END_TIME
                };

                return View(item);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
