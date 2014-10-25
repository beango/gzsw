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
using Ninject.Infrastructure.Language;

namespace gzsw.controller.CHK
{
    /// <summary>
    /// 窗口值班管理
    /// </summary>
    public class CounterController : BaseController<CHK_COUNTER>
    {
        [Ninject.Inject]
        public IDao<SYS_COUNTER> SysCounterDao { get; set; }

        [Ninject.Inject]
        public IDao<CHK_COUNTER> chkCounterDao { get; set; }

        [UserAuth("CHK_COUNTER_VIW")]
        public ActionResult Index(string hallNo,string hallName, int pageIndex = 1,int pageSize=20)
        {
            ViewBag.HallNo = hallNo;
            ViewBag.HallName = hallName;
            var counterDal = new CHK_COUNTER_DAL();
            var list = counterDal.GetCounterFuncs(hallNo, hallName, UserState.UserID, pageIndex, pageSize);

            return View(list);
        }

        /// <summary>
        /// 获取窗口号
        /// </summary>
        /// <param name="hallNo">服务厅编号</param>
        /// <returns></returns>
        public ActionResult GetTabs(string hallNo)
        {
            var list = SysCounterDao.FindList("COUNTER_ID","HALL_NO", hallNo);
            return Json(list.Select(m=>new SelectListItem()
                                       {
                                           Text = m.COUNTER_ID.ToString(),
                                           Value = m.COUNTER_ID.ToString()
                                       }),JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 员工信息
        /// </summary>
        /// <param name="hallNo">服务厅编号</param>
        /// <returns></returns>
        public ActionResult GetStaffs(string hallNo)
        {
            var staffDal = new SYS_STAFF_DAL();
            var list= staffDal.GetListByHallNo(hallNo);
            return Json(list.Select(m=>new SelectListItem()
                                       {
                                           Text=m.STAFF_NAM,
                                           Value = m.STAFF_ID
                                       }),JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="CounterId"></param>
        /// <param name="HallNo"></param>
        /// <returns></returns>
        public ActionResult ValidateCounterId(int CounterId,string HallNo)
        {
            var item = chkCounterDao.GetEntity("HALL_NO", HallNo, "COUNTER_ID", CounterId);
            return Json(item==null, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [UserAuth("CHK_COUNTER_ADD")]
        public ActionResult Create()
        {
            return View(new CounterModel());
        }

        [HttpPost]
        [UserAuth("CHK_COUNTER_ADD")]
        public ActionResult Create(CounterModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var item = new CHK_COUNTER()
                               {
                                   COUNTER_ID = model.CounterId,
                                   HALL_NO = model.HallNo,
                                   NOTE = model.Note,
                                   W1A_STAFF_ID = model.W1A_STAFF_ID,
                                   W1P_STAFF_ID = model.W1P_STAFF_ID,
                                   W2A_STAFF_ID = model.W2A_STAFF_ID,
                                   W2P_STAFF_ID = model.W2P_STAFF_ID,
                                   W3A_STAFF_ID = model.W3A_STAFF_ID,
                                   W3P_STAFF_ID = model.W3P_STAFF_ID,
                                   W4A_STAFF_ID = model.W4A_STAFF_ID,
                                   W4P_STAFF_ID = model.W4P_STAFF_ID,
                                   W5A_STAFF_ID = model.W5A_STAFF_ID,
                                   W5P_STAFF_ID = model.W5P_STAFF_ID,
                                   W6A_STAFF_ID = model.W6A_STAFF_ID,
                                   W6P_STAFF_ID = model.W6P_STAFF_ID,
                                   W7A_STAFF_ID = model.W7A_STAFF_ID,
                                   W7P_STAFF_ID = model.W7P_STAFF_ID
                               };

                    var rst = chkCounterDao.AddObject(item);
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


        [UserAuth("CHK_COUNTER_VIW")]
        public ActionResult Detail(string hallNo, int id)
        {
            try
            {
                var staffDal = new SYS_STAFF_DAL();
                var list = staffDal.GetListByHallNo(hallNo);
                var staffSelectList = list.Select(m => new SelectListItem()
                {
                    Text = m.STAFF_NAM,
                    Value = m.STAFF_ID
                }).ToList();
                staffSelectList.Insert(0, new SelectListItem()
                {
                    Text = "公假",
                    Value = "-1"
                });
                var staffMap = staffSelectList.ToDictionary(m => m.Value);

                var counterDal = new CHK_COUNTER_DAL();
                var counter = counterDal.GetCounterByHallNoCounterId(hallNo, id);
                var item = new CunterViewModel()
                {
                    CounterId = counter.COUNTER_ID,
                    HallName = counter.HALL_NAM,
                    HallNo = counter.HALL_NO,
                    Note = counter.NOTE,
                    W1A_STAFF_NAME = staffMap.ContainsKey(counter.W1A_STAFF_ID) ? staffMap[counter.W1A_STAFF_ID].Text : "公假",
                    W1P_STAFF_NAME = staffMap.ContainsKey(counter.W1P_STAFF_ID) ? staffMap[counter.W1P_STAFF_ID].Text : "公假",
                    W2A_STAFF_NAME = staffMap.ContainsKey(counter.W2A_STAFF_ID) ? staffMap[counter.W2A_STAFF_ID].Text : "公假",
                    W2P_STAFF_NAME = staffMap.ContainsKey(counter.W2P_STAFF_ID) ? staffMap[counter.W2P_STAFF_ID].Text : "公假",
                    W3A_STAFF_NAME = staffMap.ContainsKey(counter.W3A_STAFF_ID) ? staffMap[counter.W3A_STAFF_ID].Text : "公假",
                    W3P_STAFF_NAME = staffMap.ContainsKey(counter.W3P_STAFF_ID) ? staffMap[counter.W3P_STAFF_ID].Text : "公假",
                    W4A_STAFF_NAME = staffMap.ContainsKey(counter.W4A_STAFF_ID) ? staffMap[counter.W4A_STAFF_ID].Text : "公假",
                    W4P_STAFF_NAME = staffMap.ContainsKey(counter.W4P_STAFF_ID) ? staffMap[counter.W4P_STAFF_ID].Text : "公假",
                    W5A_STAFF_NAME = staffMap.ContainsKey(counter.W5A_STAFF_ID) ? staffMap[counter.W5A_STAFF_ID].Text : "公假",
                    W5P_STAFF_NAME = staffMap.ContainsKey(counter.W5P_STAFF_ID) ? staffMap[counter.W5P_STAFF_ID].Text : "公假",
                    W6A_STAFF_NAME = staffMap.ContainsKey(counter.W6A_STAFF_ID) ? staffMap[counter.W6A_STAFF_ID].Text : "公假",
                    W6P_STAFF_NAME = staffMap.ContainsKey(counter.W6P_STAFF_ID) ? staffMap[counter.W6P_STAFF_ID].Text : "公假",
                    W7A_STAFF_NAME = staffMap.ContainsKey(counter.W7A_STAFF_ID) ? staffMap[counter.W7A_STAFF_ID].Text : "公假",
                    W7P_STAFF_NAME = staffMap.ContainsKey(counter.W7P_STAFF_ID) ? staffMap[counter.W7P_STAFF_ID].Text : "公假"
                };

                return View(item);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_COUNTER_DEL")]
        public ActionResult Delete(string hallNo, string id)
        {
            try
            {
                var deleteId = id.Split(',');
                var hallNos = hallNo.Split(',');
                var counterDal = new CHK_COUNTER_DAL();
                for (var i = 0; i < deleteId.Length; i++)
                {
                    var did = 0;
                    int.TryParse(deleteId[i],out did);
                    counterDal.Delete(hallNos[i], did);
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [UserAuth("CHK_COUNTER_EDT")]
        public ActionResult Edit(string hallNo, int id)
        {
            try
            {
                var counterDal = new CHK_COUNTER_DAL();
                var counter = counterDal.GetCounterByHallNoCounterId(hallNo, id);
                var item = new CunterEditModel()
                           {
                               CounterId=counter.COUNTER_ID,
                               HallName=counter.HALL_NAM,
                               HallNo=counter.HALL_NO,
                               Note=counter.NOTE,
                               W1A_STAFF_ID=counter.W1A_STAFF_ID,
                               W1P_STAFF_ID = counter.W1P_STAFF_ID,
                               W2A_STAFF_ID = counter.W2A_STAFF_ID,
                               W2P_STAFF_ID = counter.W2P_STAFF_ID,
                               W3A_STAFF_ID = counter.W3A_STAFF_ID,
                               W3P_STAFF_ID = counter.W3P_STAFF_ID,
                               W4A_STAFF_ID = counter.W4A_STAFF_ID,
                               W4P_STAFF_ID = counter.W4P_STAFF_ID,
                               W5A_STAFF_ID = counter.W5A_STAFF_ID,
                               W5P_STAFF_ID = counter.W5P_STAFF_ID,
                               W6A_STAFF_ID = counter.W6A_STAFF_ID,
                               W6P_STAFF_ID = counter.W6P_STAFF_ID,
                               W7A_STAFF_ID = counter.W7A_STAFF_ID,
                               W7P_STAFF_ID = counter.W7P_STAFF_ID
                           };

                setSatffSelectItem(hallNo);
                return View(item);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错", ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [UserAuth("CHK_COUNTER_EDT")]
        public ActionResult Edit(CunterEditModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var counterDal = new CHK_COUNTER_DAL();
                    var item = new CHK_COUNTER()
                               {
                                   COUNTER_ID = model.CounterId,
                                   HALL_NO = model.HallNo,
                                   NOTE = model.Note,
                                   W1A_STAFF_ID = model.W1A_STAFF_ID,
                                   W1P_STAFF_ID = model.W1P_STAFF_ID,
                                   W2A_STAFF_ID = model.W2A_STAFF_ID,
                                   W2P_STAFF_ID = model.W2P_STAFF_ID,
                                   W3A_STAFF_ID = model.W3A_STAFF_ID,
                                   W3P_STAFF_ID = model.W3P_STAFF_ID,
                                   W4A_STAFF_ID = model.W4A_STAFF_ID,
                                   W4P_STAFF_ID = model.W4P_STAFF_ID,
                                   W5A_STAFF_ID = model.W5A_STAFF_ID,
                                   W5P_STAFF_ID = model.W5P_STAFF_ID,
                                   W6A_STAFF_ID = model.W6A_STAFF_ID,
                                   W6P_STAFF_ID = model.W6P_STAFF_ID,
                                   W7A_STAFF_ID = model.W7A_STAFF_ID,
                                   W7P_STAFF_ID = model.W7P_STAFF_ID,
                                   MODIFY_DTIME = DateTime.Now,
                                   MODIFY_ID = UserState.UserID
                               };

                    var rst = counterDal.Update(item);
                    if (rst > 0)
                    {
                        Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                        return Redirect("/Home/Blank");
                    }
                }

                setSatffSelectItem(model.HallNo);
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

        private void setSatffSelectItem(string hallNo)
        {
            var staffDal = new SYS_STAFF_DAL();
            var list = staffDal.GetListByHallNo(hallNo);
            var staffSelectList = list.Select(m => new SelectListItem()
            {
                Text = m.STAFF_NAM,
                Value = m.STAFF_ID
            }).ToList();
            staffSelectList.Insert(0, new SelectListItem()
            {
                Text = "公假",
                Value = "-1"
            });
            ViewBag.StaffSelectList = staffSelectList;
        }
    }
}
