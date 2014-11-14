using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using NPOI.SS.Formula.Functions;

namespace gzsw.controller.CHK
{
    public class ApplyDetailController : BaseController<CHK_STAFF_APPLYDETAIL>
    {

        [Ninject.Inject]
        public IDao<CHK_STAFF_APPLYDETAIL> AppDao { get; set; }
        [Ninject.Inject]
        public IDao<CHK_STAFF_APPLYITEM> ApplyitemDao { get; set; }


        [UserAuth("CHK_STAFF_APPLYDETAIL_VIW")]
        public ActionResult Index(string orgId, string staffNo, string staffName, int pageIndex = 1, int pageSize = 20)
        {
            if (string.IsNullOrEmpty(orgId))
            {
                orgId = SetViewBagOrgData();
            }
            else
            {
                SetViewBagOrgData(orgId);
            }
            ViewBag.StaffNo = staffNo;
            ViewBag.StaffName = staffName;

            var applyDal = new CHK_STAFF_APPLYDETAIL_DAL();
            var list = applyDal.GetPageList(orgId,staffNo, staffName, pageIndex, pageSize);
            return View(list);
        }

        [UserAuth("CHK_STAFF_APPLYDETAIL_ADD")]
        public ActionResult Create()
        {
            GetCreateDT();
            return View();
        }

        [UserAuth("CHK_STAFF_APPLYDETAIL_ADD")]
        [HttpPost]
        public ActionResult Create(ApplyDetailModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_STAFF_APPLYDETAIL()
                    {
                        APPLY_STATE = (byte)6,
                        APPLY_TIME = DateTime.Now,
                        APPLY_USR_ID = UserState.UserID,
                        AUD_TIME = DateTime.Now,
                        AUD_USR_ID = UserState.UserID,
                        BEGIN_TIME = model.BeginTime.AddHours(model.BeginTimeHours),
                        CHK_STAFF_ID = model.CHK_STAFF_ID,
                        END_TIME = model.EndTime.AddHours(model.EndTimeHours),
                        HOLLI_TYP = (byte) model.HOLLI_TYP,
                        APPLY_REASON=model.APPLY_REASON
                    };

                    item = AppDao.AddObject(item);

                    var detail = new CHK_STAFF_APPLYITEM()
                    {
                        HOLLI_TYP = item.HOLLI_TYP,
                        APPLY_DATE = item.BEGIN_TIME,
                        APPLYDETAIL_ID = item.APPLYDETAIL_ID,
                        CHK_STAFF_ID = item.CHK_STAFF_ID,
                        A_P_TYP = 1
                    };

                    var d = getDiffDays(model.BeginTime, model.EndTime);
                        
                    for (var i = 0; i < d; i++)
                    {
                        detail.APPLY_DATE = model.BeginTime.Date;
                        detail.A_P_TYP = 1;
                        ApplyitemDao.AddObject(detail);

                        detail.A_P_TYP = 2;
                        ApplyitemDao.AddObject(detail);

                        model.BeginTime = model.BeginTime.AddDays(1);
                    }
                    if (model.BeginTimeHours == 8 && model.EndTimeHours >= 12)
                    {
                        detail.A_P_TYP = 1;
                        detail.APPLY_DATE = model.BeginTime.Date;
                        ApplyitemDao.AddObject(detail);
                    }
                    if (model.BeginTimeHours == 8 && model.EndTimeHours == 17)
                    {
                        detail.A_P_TYP = 2;
                        detail.APPLY_DATE = model.BeginTime.Date;
                        ApplyitemDao.AddObject(detail);
                    }
                    if (model.BeginTimeHours == 12)
                    {
                        detail.A_P_TYP = 2;
                        detail.APPLY_DATE = model.BeginTime.Date;
                        ApplyitemDao.AddObject(detail);
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

        private int getDiffDays(DateTime being, DateTime end)
        {
            var ts = new TimeSpan();
            ts = end - being;
            return ts.Days;
        }

        [UserAuth("CHK_STAFF_APPLYDETAIL_VIW")]
        public ActionResult Details(int id)
        {
            try
            {
                var applyDal = new CHK_STAFF_APPLYDETAIL_DAL();
                var item = applyDal.Get(id);
                return View(item);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAFF_APPLYDETAIL_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteId = id.Split(',');

                for (var i = 0; i < deleteId.Length; i++)
                {
                    ApplyitemDao.Delete("APPLYDETAIL_ID", deleteId[i]);
                    AppDao.Delete("APPLYDETAIL_ID", deleteId[i]);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_STAFF_APPLYDETAIL_EDT")]
        public ActionResult AllUndo(string id)
        {
            try
            {
                var deleteId = id.Split(',');

                for (var i = 0; i < deleteId.Length; i++)
                {
                    ApplyitemDao.Delete("APPLYDETAIL_ID", deleteId[i]);
                    AppDao.Delete("APPLYDETAIL_ID", deleteId[i]);
                }

                return Json(new { isok = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Json(new { isok=false }, JsonRequestBehavior.AllowGet);
            }
        }

        [UserAuth("CHK_STAFF_APPLYDETAIL_EDT")]
        public ActionResult Edit(int id)
        {
            try
            {
                var applyDal = new CHK_STAFF_APPLYDETAIL_DAL();
                var item = applyDal.Get(id);
                var model = new ApplyDetailModel2()
                            {
                                APPLYDETAIL_ID=item.APPLYDETAIL_ID,
                                APPLY_REASON=item.APPLY_REASON,
                                BeginTime=item.BEGIN_TIME.ToString("yyyy-MM-dd"),
                                BeginTimeHours = item.BEGIN_TIME.Hour,
                                CHK_STAFF_ID=item.CHK_STAFF_ID,
                                EndTime = item.END_TIME.ToString("yyyy-MM-dd"),
                                EndTimeHours=item.END_TIME.Hour,
                                HOLLI_TYP=item.HOLLI_TYP,
                                ORG_ID=item.ORG_ID,
                                ORG_NAM=item.ORG_NAM,
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
        [UserAuth("CHK_STAFF_APPLYDETAIL_EDT")]
        public ActionResult Edit(ApplyDetailModel2 model)
        {
            try
            {
                var applyDal = new CHK_STAFF_APPLYDETAIL_DAL();
                var item = applyDal.Get(model.APPLYDETAIL_ID);
                ApplyitemDao.Delete("APPLYDETAIL_ID", model.APPLYDETAIL_ID);

                var BeginTime = Convert.ToDateTime(model.BeginTime);
                var EndTime = Convert.ToDateTime(model.EndTime);

                item.APPLY_STATE = (byte) 6;
                item.APPLY_USR_ID = UserState.UserID;
                item.AUD_USR_ID = UserState.UserID;
                item.BEGIN_TIME = BeginTime.AddHours(model.BeginTimeHours);
                item.END_TIME = EndTime.AddHours(model.EndTimeHours);
                item.HOLLI_TYP = (byte) model.HOLLI_TYP;
                item.APPLY_REASON = model.APPLY_REASON;
                

                var id = AppDao.UpdateObject(item);

                var detail = new CHK_STAFF_APPLYITEM()
                {
                    HOLLI_TYP = item.HOLLI_TYP,
                    APPLY_DATE = item.BEGIN_TIME,
                    APPLYDETAIL_ID = item.APPLYDETAIL_ID,
                    CHK_STAFF_ID = item.CHK_STAFF_ID,
                    A_P_TYP = 1
                };

                var d = getDiffDays(BeginTime, EndTime);

                for (var i = 0; i < d; i++)
                {
                    detail.APPLY_DATE = BeginTime.Date;
                    detail.A_P_TYP = 1;
                    ApplyitemDao.AddObject(detail);

                    detail.A_P_TYP = 2;
                    ApplyitemDao.AddObject(detail);

                    BeginTime = BeginTime.AddDays(1);
                }
                if (model.BeginTimeHours == 8 && model.EndTimeHours >= 12)
                {
                    detail.A_P_TYP = 1;
                    detail.APPLY_DATE = BeginTime.Date;
                    ApplyitemDao.AddObject(detail);
                }
                if (model.BeginTimeHours == 8 && model.EndTimeHours == 17)
                {
                    detail.A_P_TYP = 2;
                    detail.APPLY_DATE = BeginTime.Date;
                    ApplyitemDao.AddObject(detail);
                }
                if (model.BeginTimeHours == 12)
                {
                    detail.A_P_TYP = 2;
                    detail.APPLY_DATE = BeginTime.Date;
                    ApplyitemDao.AddObject(detail);
                }

                Alter("撤消成功！", util.Enum.AlterTypeEnum.Success, false, true);
                return Redirect("/Home/Blank");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("撤消出错。", ex);
                ModelState.AddModelError("", "撤消出错。");
                return View(model);
            }
        }

        private void GetCreateDT(string org = null)
        {
            ViewBag.STAR_LEVELLIST = EnumHelper.GetCategorySelectList(typeof(SYS_STAFF.STAR_LEVEL_ENUM));
            ViewBag.STAFF_TYPLIST = EnumHelper.GetCategorySelectList(typeof(SYS_STAFF.STAFF_TYP_ENUM));

            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");

            ViewBag.UserORG = new SelectList(orgs, "ORG_ID", "ORG_NAM", org);
        }
    }
}
