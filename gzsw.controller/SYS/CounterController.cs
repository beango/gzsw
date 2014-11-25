using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using gzsw.util;
using Ninject;
using gzsw.dal.dao;

namespace gzsw.controller.SYS
{
    public class CounterController : BaseController<SYS_COUNTER>
    {
        [Inject]
        public IDao<SYS_ORGANIZE> DaoOrganize { get; set; }
        [Inject]
        public IDao<SYS_HALL> DaoHall { get; set; }
        [Inject]
        public IDao<SYS_QUEUESERIAL> DaoQueueserial { get; set; }

        [UserAuth("SYS_COUNTER_VIW")]
        public ActionResult Index(string hallno, string orgid, string orgnam, int pageIndex = 1,int pageSize=20)
        {
            ViewBag.HALLNO = hallno;
            ViewBag.ORGID = orgid;
            ViewBag.ORGNAM = orgnam;
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            if (string.IsNullOrEmpty(orgid) && orgall != null)
                orgid = orgall.FirstOrDefault(obj => obj.ORG_LEVEL == 4).ORG_ID;
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", orgid);

            var orgs = orgall.Select(obj => obj.ORG_ID);
           
            if (!string.IsNullOrEmpty(orgid))
            {
                orgs = orgs.Where(obj => obj == orgid);
                if (null == orgs || orgs.Count() == 0)
                {
                    orgs = new List<string> { "-1" };
                }
            }
            var halllist = DaoHall.FindList("", "ORG_ID in", orgs);

            var data = dao.GetList(pageIndex, pageSize, "COUNTER_ID", "HALL_NO", hallno, "HALL_NO in", null == halllist ? null : halllist.Select(obj => obj.HALL_NO));
            var orglist = DaoOrganize.FindList();
            foreach (var item in data.Items)
            {
                item.Hall = halllist.FirstOrDefault(obj => obj.HALL_NO == item.HALL_NO);
                item.Org = orglist.FirstOrDefault(obj => obj.ORG_ID == item.Hall.ORG_ID);
            }
            if (null != data)
                InitData(data.Items);
            return View(data);
        }

        private void InitData(List<SYS_COUNTER> data)
        {
            var list = DaoQueueserial.FindList();
            var halllist = DaoHall.FindList();

            if (null != data)
            {
                foreach (var item in data)
                {
                    item.Hall = halllist.FirstOrDefault(obj => obj.HALL_NO == item.HALL_NO);
                    item.PRI1_BUSI_SER_NAM = InitBusiSer(item.PRI1_BUSI_SER, list);
                    item.PRI2_BUSI_SER_NAM = InitBusiSer(item.PRI2_BUSI_SER, list);
                    item.PRI3_BUSI_SER_NAM = InitBusiSer(item.PRI3_BUSI_SER, list);
                    item.PRI4_BUSI_SER_NAM = InitBusiSer(item.PRI4_BUSI_SER, list);
                    item.PRI5_BUSI_SER_NAM = InitBusiSer(item.PRI5_BUSI_SER, list);
                }
            }
        }

        private string InitBusiSer(string busiNer, IList<SYS_QUEUESERIAL> list)
        {
            if (!string.IsNullOrEmpty(busiNer))
            {
                string rst = "";
                var arr1 = busiNer.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in arr1)
                {
                    var q = list.FirstOrDefault(obj => obj.Q_SERIALID == s);
                    if (null != q)
                    {
                        rst += q.Q_SERIALNAME + "-";
                    }
                    else
                    {
                        rst += s + "-";
                    }
                }
                if (rst != "")
                    return rst.Substring(0, rst.Length - 1);
            }
            return "";
        }

        [HttpGet]
        [UserAuth("SYS_COUNTER_VIW")]
        public ActionResult Details(int id)
        {
            try
            {
                var detail = dao.GetEntity("COUNTER_ID", id);
                InitData(new List<SYS_COUNTER> { detail });
                return View(detail);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_COUNTER_ADD")]
        public ActionResult Create(string orgid)
        {
            if (null != orgid)
            {
                ViewBag.ORG = DaoOrganize.GetEntity("ORG_ID", orgid);
            }
            GetCreateData();
            return View();
        }

        private void GetCreateData(string sele=null)
        {
            ViewBag.HallList = new SelectList(DaoHall.FindList(), "HALL_NO", "HALL_NAM");
            var statelist = EnumHelper.GetCategorySelectList(typeof(SYS_COUNTER.STATE_ENUM));
            if (!string.IsNullOrEmpty(sele) && statelist != null)
            {
                var selitem = statelist.FirstOrDefault(obj => obj.Value == sele);
                if (null != selitem)
                    selitem.Selected = true;
            }
            ViewBag.STATE = statelist;
            ViewBag.QueueSerial = new SelectList(DaoQueueserial.FindList(), "Q_SERIALID", "Q_SERIALNAME");
        }

        [HttpPost]
        [UserAuth("SYS_COUNTER_ADD")]
        public ActionResult Create(SYS_COUNTER info)
        {
            try
            {
                GetCreateData();
                STAFFVALID(info);
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "新增出错！");
                    return JsonResult(false, "新增出错！", "SYS");
                }
                info.CREATE_DTIME = DateTime.Now;
                info.CREATE_ID = UserState.UserID;
                var rst = dao.AddObject(info);
                if (null != rst)
                {
                    return JsonResult(true, "新增成功！", "AUTH", "", false);
                }
                else
                {
                    ModelState.AddModelError("", "新增失败！");
                    return JsonResult(false, "新增错误！", "SYS");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                ModelState.AddModelError("", "系统错误！");
                return JsonResult(false, "系统错误！", "SYS");
            }
        }

        private void STAFFVALID(SYS_COUNTER info, bool isEdit = false)
        {
            if (!string.IsNullOrEmpty(Request.Form["PRI1_BUSI_SER"]))
                info.PRI1_BUSI_SER = Request.Form["PRI1_BUSI_SER"].Replace(",","-");
            if (!string.IsNullOrEmpty(Request.Form["PRI2_BUSI_SER"]))
                info.PRI2_BUSI_SER = Request.Form["PRI2_BUSI_SER"].Replace(",", "-");
            if (!string.IsNullOrEmpty(Request.Form["PRI3_BUSI_SER"]))
                info.PRI3_BUSI_SER = Request.Form["PRI3_BUSI_SER"].Replace(",", "-");
            if (!string.IsNullOrEmpty(Request.Form["PRI4_BUSI_SER"]))
                info.PRI4_BUSI_SER = Request.Form["PRI4_BUSI_SER"].Replace(",", "-");
            if (!string.IsNullOrEmpty(Request.Form["PRI5_BUSI_SER"]))
                info.PRI5_BUSI_SER = Request.Form["PRI5_BUSI_SER"].Replace(",", "-");

            if (string.IsNullOrEmpty(info.HALL_NO))
            {
                ModelState.AddModelError("HALL_NO", "服务厅不能为空！");
            }
            if (info.COUNTER_ID == 0)
            {
                ModelState.AddModelError("COUNTER_ID", "窗口ID不能为空！");
            }
            else
            {
                if (!isEdit && dao.GetEntity("COUNTER_ID", info.COUNTER_ID,"HALL_NO", info.HALL_NO) != null)
                    ModelState.AddModelError("COUNTER_ID", "窗口ID已经存在！");
            }
            if (info.PRI1_BUSI_SER == null)
                info.PRI1_BUSI_SER = "";
            var quArr = new List<string>();
            if (!string.IsNullOrEmpty(info.PRI1_BUSI_SER))
            {
                var arr1 = info.PRI1_BUSI_SER.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in arr1)
                {
                    if (quArr.Any(obj => obj == s))
                    {
                        ModelState.AddModelError("PRI1_BUSI_SER", Request["PRI1_BUSI_SERN"] + "已经存在！");
                    }
                }
                quArr.AddRange(arr1);
            }
            if (info.PRI2_BUSI_SER == null)
                info.PRI2_BUSI_SER = "";
            if (!string.IsNullOrEmpty(info.PRI2_BUSI_SER))
            {
                var arr1 = info.PRI2_BUSI_SER.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in arr1)
                {
                    if (quArr.Any(obj => obj == s))
                    {
                        ModelState.AddModelError("PRI2_BUSI_SER", Request["PRI2_BUSI_SERN"] + "已经存在！");
                    }
                }
                quArr.AddRange(arr1);
            }
            if (info.PRI3_BUSI_SER == null)
                info.PRI3_BUSI_SER = "";
            if (!string.IsNullOrEmpty(info.PRI3_BUSI_SER))
            {
                var arr1 = info.PRI3_BUSI_SER.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in arr1)
                {
                    if (quArr.Any(obj => obj == s))
                    {
                        ModelState.AddModelError("PRI3_BUSI_SER", Request["PRI3_BUSI_SERN"] + "已经存在！");
                    }
                }
                quArr.AddRange(arr1);
            }
            if (info.PRI4_BUSI_SER == null)
                info.PRI4_BUSI_SER = "";
            if (!string.IsNullOrEmpty(info.PRI4_BUSI_SER))
            {
                var arr1 = info.PRI4_BUSI_SER.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in arr1)
                {
                    if (quArr.Any(obj => obj == s))
                    {
                        ModelState.AddModelError("PRI4_BUSI_SER", Request["PRI4_BUSI_SERN"] + "已经存在！");
                    }
                }
                quArr.AddRange(arr1);
            }
            if (info.PRI5_BUSI_SER == null)
                info.PRI5_BUSI_SER = "";
            if (!string.IsNullOrEmpty(info.PRI5_BUSI_SER))
            {
                var arr1 = info.PRI5_BUSI_SER.Split(new[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var s in arr1)
                {
                    if (quArr.Any(obj => obj == s))
                    {
                        ModelState.AddModelError("PRI5_BUSI_SER", Request["PRI5_BUSI_SERN"] + "已经存在！");
                    }
                }
                quArr.AddRange(arr1);
            }
            if (info.MAX_BUSI_CNT==null)
            {
                ModelState.AddModelError("MAX_BUSI_CNT", "不能为空！");
            }
            else if (info.MAX_BUSI_CNT.Value <=0)
            {
                ModelState.AddModelError("MAX_BUSI_CNT", "必须大于0！");
            }
        }
        [HttpGet]
        [UserAuth("SYS_COUNTER_EDT")]
        public ActionResult Edit(string hallno,int id)
        {
            try
            {
                var info = dao.GetEntity("HALL_NO",hallno,"COUNTER_ID", id);
                GetCreateData(info.STATE.ToString());
                InitData(new List<SYS_COUNTER> { info });
                return View(info);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                return Redirect("/Home/Error");
            }
        }
        [HttpPost]
        [UserAuth("SYS_COUNTER_EDT")]
        public ActionResult Edit(SYS_COUNTER info)
        {
            try
            {
                STAFFVALID(info,true);

                if (!ModelState.IsValid)
                {
                    Alter("修改出错！", util.Enum.AlterTypeEnum.Error, false, false);
                    ModelState.AddModelError("", "修改出错！");
                    return JsonResult(false, "修改出错！", "SYS");
                }

                info.MODIFY_DTIME = DateTime.Now;
                info.MODIFY_ID = UserState.UserID;
                var rst = new SYS_COUNTER_DAL().Update(info);
                if (rst > 0)
                {
                    return JsonResult(true, "修改成功！", "AUTH", "", false);
                }
                else
                {
                    return JsonResult(false, "修改失败！", "SYS");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                ModelState.AddModelError("", "系统错误！" + ex.Message);
                return Redirect("/Home/Error");
            }
        }
        [UserAuth("SYS_COUNTER_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteId = id.Split(',');
                
                foreach (var did in deleteId)
                {
                    dao.Delete("COUNTER_ID", did);
                }
                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误", ex);
                return Redirect("/Home/Error");
            }
        }

    }
}
