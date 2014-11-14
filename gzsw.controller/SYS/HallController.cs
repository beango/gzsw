using System;
using System.Linq;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using gzsw.util;
using Ninject;
using gzsw.dal.dao;
using System.Collections.Generic;
using gzsw.util.Enum;

namespace gzsw.controller.SYS
{
    public class HallController : BaseController<SYS_HALL>
    {
        [Inject]
        public IDao<SYS_ORGANIZE> DaoOrganize { get; set; }

        [Inject]
        public IDao<SYS_STAFF> DaoStaff { get; set; }

        [Inject]
        public IDao<SYS_TICKETQUEUESERIAL> DaoTicketQuSerial { get; set; }

        [Inject]
        public IDao<SYS_COUNTER> DaoCounter { get; set; }

        [UserAuth("SYS_HALLINFO_VIW")]
        public ActionResult Index(string nam, string orgid, string orgnam, int pageIndex = 1,
            int pageSize = 20)
        {
            ViewBag.NAM = nam;
            ViewBag.ORGNAM = orgnam;
            ViewBag.ORGID = orgid;
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

            var data = dao.GetList(pageIndex, pageSize, "CREATE_DTIME desc"
                , "HALL_NAM like", nam, "ORG_ID in", orgs);
            var orglist = DaoOrganize.FindList();
            foreach (var item in data.Items)
            {
                item.HALLORG = orglist.FirstOrDefault(obj => obj.ORG_ID == item.ORG_ID);
            }
            return View(data);
        }

        [HttpGet]
        [UserAuth("SYS_HALLINFO_VIW")]
        public ActionResult Details(string id, string orgid)
        {
            try
            {
                SYS_HALL info = new SYS_HALL();
                if (!string.IsNullOrEmpty(id))
                    info = GetEdtDT(id);
                if (!string.IsNullOrEmpty(orgid))
                    info = dao.GetEntity("ORG_ID", orgid);

                if (null != info)
                    info.HALLORG = DaoOrganize.GetEntity("ORG_ID", info.ORG_ID);
                return View(info);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看信息出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_HALLINFO_ADD")]
        public ActionResult Create()
        {
            GetViewData();
            return View();
        }

        private void GetViewData()
        {
            ViewBag.UserORG = new SelectList(UserState.UserOrgs.Where(obj => obj.ORG_LEVEL == 4)
                   , "ORG_ID", "ORG_NAM");
            ViewBag.TICKETNUMLIST = EnumHelper.GetCategorySelectList(typeof(SYS_HALL.TICKETNUM_ENUM), false);
        }

        [HttpPost]
        [UserAuth("SYS_HALLINFO_ADD")]
        public ActionResult Create(SYS_HALL info)
        {
            try
            {
                GetViewData();
                CHKValid(info);
                if (!ModelState.IsValid)
                {
                    return View();
                }

                info.CREATE_DTIME = DateTime.Now;
                info.CREATE_ID = UserState.UserID;
                var rst = dao.AddObject(info);
                if (null != rst)
                {
                    Alter("新增成功！", AlterTypeEnum.Success, true, true);
                    return View();
                }
                else
                {
                    ModelState.AddModelError("", "新增失败。");
                    Alter("修改失败！", AlterTypeEnum.Error, false, false);
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                ModelState.AddModelError("", "系统出错！");
                Alter("系统出错！", AlterTypeEnum.Error, false, false);
                return View();
            }
        }

        private void CHKValid(SYS_HALL info, bool isEdit = false)
        {
            if (string.IsNullOrEmpty(info.HALL_NO))
            {
                ModelState.AddModelError("HALL_NO", "不能为空！");
            }
            else
            {
                if (!isEdit)
                {
                    var exists = dao.GetEntity("HALL_NO", info.HALL_NO);
                    if (null != exists)
                    {
                        ModelState.AddModelError("HALL_NO", "已经存在！");
                    }
                }
            }
            if (string.IsNullOrEmpty(info.HALL_NAM))
            {
                ModelState.AddModelError("HALL_NAM", "不能为空！");
            }
            else
            {
                var exists = dao.GetEntity("HALL_NAM", info.HALL_NAM);
                if (!isEdit)
                {
                    if (null != exists)
                    {
                        ModelState.AddModelError("HALL_NAM", "已经存在！");
                    }
                }
                else
                {
                    if (null != exists && exists.HALL_NO != info.HALL_NO)
                    {
                        ModelState.AddModelError("HALL_NAM", "已经存在！");
                    }
                }
            }
            if (string.IsNullOrEmpty(info.ADDRESS))
            {
                ModelState.AddModelError("ADDRESS", "不能为空！");
            }
            if (string.IsNullOrEmpty(info.LONGITUDE))
            {
                ModelState.AddModelError("LONGITUDE", "不能为空！");
            }
            if (string.IsNullOrEmpty(info.DIMENSION))
            {
                ModelState.AddModelError("DIMENSION", "不能为空！");
            }
            if (info.HEAD == null)
            {
                ModelState.AddModelError("HEAD", "不能为空！");
            }
            if (info.HEAD_TEL == null)
            {
                ModelState.AddModelError("HEAD_TEL", "不能为空！");
            }
            if (info.ORG_ID == null)
            {
                ModelState.AddModelError("ORG_ID", "不能为空！");
            }
            else
            {
                var exists = dao.GetEntity("ORG_ID", info.ORG_ID);
                if (!isEdit && null != exists)
                {
                    ModelState.AddModelError("ORG_ID", "已经存在服务厅！");
                }
                if (isEdit)
                {
                    if (null != exists && exists.HALL_NO != info.HALL_NO)
                    {
                        ModelState.AddModelError("ORG_ID", "已经存在服务厅！");
                    }
                }
            }
            if (info.LIMIT_CNT==null)
            {
                ModelState.AddModelError("LIMIT_CNT", "不能为空！");
            }
            if (info.LIMIT_CNT <=0)
            {
                ModelState.AddModelError("LIMIT_CNT", "必须大于0！");
            }
            if (info.COUNTER_CNT <= 0)
            {
                ModelState.AddModelError("COUNTER_CNT", "必须大于0！");
            }
            if (info.AUTO_CALL_SEC <= 0)
            {
                ModelState.AddModelError("AUTO_CALL_SEC", "必须大于0！");
            }
            if (info.AUTO_END_SEC > 0 && info.AUTO_END_SEC <= 10)
            {
                ModelState.AddModelError("AUTO_END_SEC", "必须大于10，或等于0！");
            }
            if (info.AUTO_EVAL_SEC > 0 && info.AUTO_EVAL_SEC <= 10)
            {
                ModelState.AddModelError("AUTO_EVAL_SEC", "必须大于0，或等于0！");
            }
            if (info.EVAL_STAY_SEC > 0 && info.EVAL_STAY_SEC <= 10)
            {
                ModelState.AddModelError("EVAL_STAY_SEC", "必须大于0，或等于0！");
            }
        }

        [HttpGet]
        [UserAuth("SYS_HALLINFO_EDT")]
        public ActionResult Edit(string id, string orgid)
        {
            try
            {
                SYS_HALL info = new SYS_HALL();
                if (!string.IsNullOrEmpty(id))
                    info = GetEdtDT(id);
                if (!string.IsNullOrEmpty(orgid))
                {
                    info = dao.GetEntity("ORG_ID", orgid);
                    info.HALLORG = DaoOrganize.GetEntity("ORG_ID", info.ORG_ID);
                }
                    
                GetEDTData(orgid, info.TICKETNUM);
                return View(info);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改信息出错", ex);
                return Redirect("/Home/Error");
            }
        }

        private void GetEDTData(string orgid,byte? ticknum)
        {
            //ViewBag.UserORG = new SelectList(UserState.UserOrgs.Where(obj => obj.ORG_LEVEL == 4)
            //, "ORG_ID", "ORG_NAM", orgid);
            //ViewBag.UserORG = UserState.UserOrgs.FirstOrDefault(obj => obj.ORG_ID == orgid);
            var selelist = EnumHelper.GetCategorySelectList(typeof(SYS_HALL.TICKETNUM_ENUM), false);
            ViewBag.TICKETNUMList = new SelectList(selelist, "Value", "Text", ticknum);
        }

        private SYS_HALL GetEdtDT(string id)
        {
           var hall = dao.GetEntity("HALL_NO", id);
           hall.HALLORG= DaoOrganize.GetEntity("ORG_ID", hall.ORG_ID);
           return hall;
        }

        [HttpPost]
        [UserAuth("SYS_HALLINFO_EDT")]
        public ActionResult Edit(SYS_HALL info, string id, string orgid)
        {
            try
            {
                GetEDTData(orgid, null);
                CHKValid(info, true);
                if (!ModelState.IsValid)
                {
                    return View(info);
                }

                var rst = dao.UpdateObject(info, "HALL_NO");
                if (rst > 0)
                {
                    Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                    if (Request["FromOrg"] == "1")
                        return Redirect("/SYS/Hall/Details"+"?orgid="+orgid);
                    Alter("修改成功！", util.Enum.AlterTypeEnum.Success, true, true);
                    return View(info);
                }
                else
                {
                    ModelState.AddModelError("", "修改失败。");
                    Alter("修改失败！", util.Enum.AlterTypeEnum.Error, false, false);
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                ModelState.AddModelError("", "系统错误！" + ex.Message);
                Alter("系统错误！", util.Enum.AlterTypeEnum.Error, false, false);
                return View(info);
            }
        }

        [UserAuth("SYS_HALLINFO_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var deleteId = id.Split(',');
                foreach (var dId in deleteId)
                {
                    var info = dao.GetEntity("HALL_NO", dId);
                    if (null == info)
                    {
                        LogHelper.ErrorLog("系统错误");
                        return Redirect("/Home/Error");
                    }
                    if (DaoStaff.Exists("ORG_ID", info.ORG_ID))
                    {
                        Alter("不允许删除，该服务厅还存在员工！", util.Enum.AlterTypeEnum.Error, true);
                        return RedirectToAction("/");
                    }
                    if (DaoTicketQuSerial.Exists("Q_SYSNO", info.HALL_NO))
                    {
                        Alter("不允许删除，该服务厅还存在取号队列！", util.Enum.AlterTypeEnum.Error, true);
                        return RedirectToAction("/");
                    }
                    if (DaoCounter.Exists("HALL_NO", info.HALL_NO))
                    {
                        Alter("不允许删除，该服务厅还存在窗口！", util.Enum.AlterTypeEnum.Error, true);
                        return RedirectToAction("/");
                    }
                    dao.Delete("HALL_NO", dId);
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
