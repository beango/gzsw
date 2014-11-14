using System;
using System.Linq;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using gzsw.util;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Ninject;
using gzsw.dal.dao;

namespace gzsw.controller.SYS
{
    public class TicketQueSerialController : BaseController<SYS_TICKETQUEUESERIAL>
    {
        [Inject]
        public IDao<SYS_ORGANIZE> DaoOrganize { get; set; }

        [Inject]
        public IDao<SYS_QUEUESERIAL> DaoQueueserial { get; set; }

        [Inject]
        public IDao<SYS_HALL> DaoHall { get; set; }

        [UserAuth("SYS_TICKETQUEUESERIAL_VIW")]
        public ActionResult Index(string snam, string orgid, string orgnam, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.SNAM = snam;
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

            var data = dao.GetList(pageIndex, pageSize, "Q_ID desc", "Q_SERIALNAME like", snam, "Q_SYSNO in", null == halllist ? null : halllist.Select(obj => obj.HALL_NO));

            if (data != null && data.Items != null)
            {
                halllist = DaoHall.FindList();
                var orglist = DaoOrganize.FindList();
                foreach (var item in data.Items)
                {
                    item.Hall = halllist.FirstOrDefault(obj => obj.HALL_NO == item.Q_SYSNO);
                    item.Org = orglist.FirstOrDefault(obj => obj.ORG_ID == item.Hall.ORG_ID);
                }
            }
            return View(data);
        }

        [HttpGet]
        [UserAuth("SYS_TICKETQUEUESERIAL_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var qdetail = dao.GetEntity("Q_ID", id);
                if (null != qdetail)
                {
                    qdetail.Hall = DaoHall.GetEntity("HALL_NO", qdetail.Q_SYSNO);
                }
                return View(qdetail);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看信息出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_TICKETQUEUESERIAL_ADD")]
        public ActionResult Create(string copyid)
        {
            GetCreateDT();
            return View(dao.GetEntity("Q_ID", copyid));
        }

        private void GetCreateDT()
        {
            ViewData["Q_SYSNOLIST"] = new SelectList(DaoHall.FindList(), "HALL_NO", "HALL_NAM");
            ViewData["QUSERIAL"] = new SelectList(DaoQueueserial.FindList(), "Q_SERIALID", "Q_SERIALNAME");
        }

        [HttpPost]
        [UserAuth("SYS_TICKETQUEUESERIAL_ADD")]
        public ActionResult Create(SYS_TICKETQUEUESERIAL info)
        {
            try
            {
                ChkData(info);
                GetCreateDT();

                if (!ModelState.IsValid)
                {
                    return View();
                }

                info.SYS_LRTIME = DateTime.Now;
                info.SYS_LRUSER = UserState.UserID;
                var rst = dao.AddObject(info);
                if (null != rst)
                {
                    if (Request["isCopy"] == "true")
                    {
                        Alter("新增成功！", util.Enum.AlterTypeEnum.Success, false, false);
                        return RedirectToAction("Create", new { copyid = info.Q_ID });
                    }
                    Alter("新增成功！", util.Enum.AlterTypeEnum.Success, false, true);
                    return View();
                }
                else
                    return View();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误", ex);
                Alter("系统错误！", util.Enum.AlterTypeEnum.Error, false, false);
                return View();
            }
        }

        private void ChkData(SYS_TICKETQUEUESERIAL info, bool isEdit = false)
        {
            if (string.IsNullOrEmpty(info.Q_SYSNO))
            {
                ModelState.AddModelError("Q_SYSNO", "服务厅不能为空！");
            }
            string Q_SERIALNAME = "";
            if (string.IsNullOrEmpty(info.Q_SERIALID))
            {
                ModelState.AddModelError("Q_SERIALID", "排队业务不能为空！");
            }
            else
            {
                var quequSerial = DaoQueueserial.GetEntity("Q_SERIALID", info.Q_SERIALID);
                if (null == quequSerial)
                    ModelState.AddModelError("Q_SERIALID", "排队业务不正确！");
                else
                    Q_SERIALNAME = quequSerial.Q_SERIALNAME;
            }
            var exit = dao.GetEntity("Q_SYSNO", info.Q_SYSNO, "Q_SERIALID", info.Q_SERIALID);
            if (!isEdit)//新增
            {
                if (null != exit)
                {
                    ModelState.AddModelError("Q_SERIALID", "排队业务已经存在！");
                }
            }
            else
            {
                if (null != exit && exit.Q_ID != info.Q_ID)
                {
                    ModelState.AddModelError("Q_SERIALID", "排队业务已经存在！");
                }
            }
            info.Q_SERIALNAME = Q_SERIALNAME;
            if (!ModelState.IsValidField("Q_SERIALNAME"))
            {
                ModelState.Remove("Q_SERIALNAME");
            }
            if (string.IsNullOrEmpty(info.Q_NUMBERQCHAR))
            {
                ModelState.AddModelError("Q_NUMBERQCHAR", "前缀不能为空！");
            }
            string pat = @"^(([0-1][0-9])|([2][0-3])):([0-5]?[0-9])?$";
            Regex r = new Regex(pat, RegexOptions.IgnoreCase);
            TimeSpan t;
            if (string.IsNullOrEmpty(info.Q_1ASTIME))
            {
                ModelState.AddModelError("Q_1ASTIME", "取号时间段不能为空！");
            }
            else if (info.Q_1ASTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_1ASTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_1ASTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_1ASTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_1ASTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_1ASTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_2ASTIME))
            {
                ModelState.AddModelError("Q_2ASTIME", "取号时间段不能为空！");
            }
            else if (info.Q_2ASTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_2ASTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_2ASTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_2ASTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_2ASTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_2ASTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_3ASTIME))
            {
                ModelState.AddModelError("Q_3ASTIME", "取号时间段不能为空！");
            }
            else if (info.Q_3ASTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_3ASTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_3ASTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_3ASTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_3ASTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_3ASTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_4ASTIME))
            {
                ModelState.AddModelError("Q_4ASTIME", "取号时间段不能为空！");
            }
            else if (info.Q_4ASTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_4ASTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_4ASTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_4ASTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_4ASTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_4ASTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_5ASTIME))
            {
                ModelState.AddModelError("Q_5ASTIME", "取号时间段不能为空！");
            }
            else if (info.Q_5ASTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_5ASTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_5ASTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_5ASTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_5ASTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_5ASTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_6ASTIME))
            {
                ModelState.AddModelError("Q_6ASTIME", "取号时间段不能为空！");
            }
            else if (info.Q_6ASTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_6ASTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_6ASTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_6ASTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_6ASTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_6ASTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_7ASTIME))
            {
                ModelState.AddModelError("Q_7ASTIME", "取号时间段不能为空！");
            }
            else if (info.Q_7ASTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_7ASTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_7ASTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_7ASTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_7ASTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_7ASTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_1PSTIME))
            {
                ModelState.AddModelError("Q_1PSTIME", "取号时间段不能为空！");
            }
            else if (info.Q_1PSTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_1PSTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_1PSTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_1PSTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_1PSTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_1PSTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_2PSTIME))
            {
                ModelState.AddModelError("Q_2PSTIME", "取号时间段不能为空！");
            }
            else if (info.Q_2PSTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_2PSTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_2PSTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_2PSTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_2PSTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_2PSTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_3PSTIME))
            {
                ModelState.AddModelError("Q_3PSTIME", "取号时间段不能为空！");
            }
            else if (info.Q_3PSTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_3PSTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_3PSTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_3PSTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_3PSTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_3PSTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_4PSTIME))
            {
                ModelState.AddModelError("Q_4PSTIME", "取号时间段不能为空！");
            }
            else if (info.Q_4PSTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_4PSTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_4PSTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_4PSTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_4PSTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_4PSTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_5PSTIME))
            {
                ModelState.AddModelError("Q_5PSTIME", "取号时间段不能为空！");
            }
            else if (info.Q_5PSTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_5PSTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_5PSTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_5PSTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_5PSTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_5PSTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_6PSTIME))
            {
                ModelState.AddModelError("Q_6PSTIME", "取号时间段不能为空！");
            }
            else if (info.Q_6PSTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_6PSTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_6PSTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_6PSTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_6PSTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_6PSTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_7PSTIME))
            {
                ModelState.AddModelError("Q_7PSTIME", "取号时间段不能为空！");
            }
            else if (info.Q_7PSTIME.Split(new[] { '-', '－' }).Length != 2 ||
                !TimeSpan.TryParse(info.Q_7PSTIME.Split(new[] { '-', '－' })[0], out t) ||
                !TimeSpan.TryParse(info.Q_7PSTIME.Split(new[] { '-', '－' })[1], out t) ||
                !r.Match(info.Q_7PSTIME.Split(new[] { '-', '－' })[0]).Success ||
                !r.Match(info.Q_7PSTIME.Split(new[] { '-', '－' })[1]).Success)
            {
                ModelState.AddModelError("Q_7PSTIME", "取号时间段格式不正确！");
            }
            if (string.IsNullOrEmpty(info.Q_MINNUMBER))
            {
                ModelState.AddModelError("Q_MINNUMBER", "业务开始号不能为空！");
            }
            else if (info.Q_MINNUMBER.Length != 3)
            {
                ModelState.AddModelError("Q_MINNUMBER", "业务开始号只能为3位！");
            }
            if (string.IsNullOrEmpty(info.Q_MAXNUMBER))
            {
                ModelState.AddModelError("Q_MAXNUMBER", "该业务结束号不能为空！");
            }
            else if (info.Q_MAXNUMBER.Length != 3)
            {
                ModelState.AddModelError("Q_MAXNUMBER", "业务结束号只能为3位！");
            }
        }

        [HttpGet]
        [UserAuth("SYS_TICKETQUEUESERIAL_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                LoadSelect();
                var info = dao.GetEntity("Q_ID", id);
                if (null != info)
                {
                    info.Hall = DaoHall.GetEntity("HALL_NO", info.Q_SYSNO);
                }
                return View(info);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改信息出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpPost]
        [UserAuth("SYS_TICKETQUEUESERIAL_EDT")]
        public ActionResult Edit(SYS_TICKETQUEUESERIAL info)
        {
            try
            {
                LoadSelect();
                if (null != info && !string.IsNullOrEmpty(info.Q_SYSNO))
                {
                    info.Hall = DaoHall.GetEntity("HALL_NO", info.Q_SYSNO);
                }
                ChkData(info, true);
                if (!ModelState.IsValid)
                {
                    return View(info);
                }

                var old = dao.GetEntity("Q_ID", info.Q_ID);
                info.Q_CURRNUMBER = old.Q_CURRNUMBER;
                info.SYS_LASTTIME = DateTime.Now;
                info.SYS_LASTUSER = UserState.UserID;
                var rst = dao.UpdateObject(info, "Q_ID");

                if (rst > 0)
                {
                    if (Request["isCopy"] == "true")
                    {
                        Alter("修改成功", util.Enum.AlterTypeEnum.Success, false, false);
                        return RedirectToAction("Create", new { copyid = info.Q_ID });
                    }
                    Alter("修改成功", util.Enum.AlterTypeEnum.Success, false, true);
                    return View(info);
                }
                else
                {
                    return View(info);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                ModelState.AddModelError("", "系统错误！");
                Alter("系统错误！", util.Enum.AlterTypeEnum.Error, false, false);
                return View(info);
            }
        }

        private void LoadSelect()
        {
            ViewData["Q_SYSNOLIST"] = new SelectList(DaoHall.FindList(), "HALL_NO", "HALL_NAM");
            ViewData["QUSERIAL"] = new SelectList(DaoQueueserial.FindList(), "Q_SERIALID", "Q_SERIALNAME");
        }

        [UserAuth("SYS_TICKETQUEUESERIAL_DEL")]
        public ActionResult Delete(string id)
        {
            var deleteId = id.Split(',');
            try
            {
                foreach (var did in deleteId)
                {
                    dao.Delete("Q_ID", did);
                }

                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            }
        }
    }
}
