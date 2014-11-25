using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using System;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using System.Dynamic;

namespace gzsw.controller.WARN
{
    public class WarnInfoController : BaseController<WARN_INFO_DETAIL>
    {
        [Inject]
        public IDao<WARN_SENDINFO_DETAIL> DaoSendInfoDetail { get; set; }
        [Inject]
        public IDao<WARN_ALARM_SENDINFO_DETAIL> DaoAlarmSendInfoDetail { get; set; }
        [Inject]
        public IDao<WARN_COMPLAIN_DETAIL> DaoComplainDetail { get; set; }
        [Inject]
        public IDao<SYS_HALL> DaoHall { get; set; }

        [HttpGet]
        [UserAuth("WARN_INFO_VIW")]
        public ActionResult Index(int? typeq, int? lvlq, string orgid,
            DateTime? start, DateTime? end,
            int pageIndex = 1, int pageSize = 20)
        {
            if (null != start)
                ViewBag.Start = start.Value.ToString("yyyy-MM-dd");
            if (null != end)
                ViewBag.End = end.Value.ToString("yyyy-MM-dd");
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", orgid);

            var selectlist = EnumHelper.GetCategorySelectList(typeof(WARN_INFO_DETAIL.WARN_TYP_ENUM));
            if (null != typeq)
                selectlist.Find(obj => obj.Value == typeq.Value.ToString()).Selected = true;
            ViewBag.WARN_TYP_SELECTLIST = selectlist;
            var selectlist2 = EnumHelper.GetCategorySelectList(typeof(WARN_INFO_DETAIL.WARN_LVL_ENUM));
            if (null != lvlq)
                selectlist2.Find(obj => obj.Value == lvlq.Value.ToString()).Selected = true;
            ViewBag.WARN_LVL_SELECTLIST = selectlist2;

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
            var data = dao.GetList(pageIndex, pageSize, "CREATE_DTIME desc", "WARN_TYP", typeq, "WARN_LEVEL", lvlq, "HALL_NO in", null == halllist ? null : halllist.Select(obj => obj.HALL_NO),
                "CREATE_DTIME>=", start == null ? "'1970-01-01'" : "'" + start.Value + "'",
                "CREATE_DTIME<", end == null ? ("'" + DateTime.MaxValue.ToShortDateString() + "'") : ("'" + end.Value.AddDays(1) + "'"));
            foreach (var item in data.Items)
            {
                var h = halllist.FirstOrDefault(obj => obj.HALL_NO == item.HALL_NO);
                if (null != h)
                    item.HALL_NAM = h.HALL_NAM;
            }
            return View(data);
        }

        [HttpGet]
        [UserAuth("WARN_INFO_VIW,DETAIL_WARN_DISPOSAL_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var info = dao.GetEntity("WARN_INFO_DETAIL_ID", id);
                var h = DaoHall.GetEntity("HALL_NO", info.HALL_NO);
                if (null != h)
                    info.HALL_NAM = h.HALL_NAM;
                return View(info);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("WARN_INFO_VIW,DETAIL_WARN_DISPOSAL_VIW")]
        public ActionResult SendDetails(int id)
        {
            try
            {
                var sendlist = DaoSendInfoDetail.FindList("", "WARN_INFO_DETAIL_ID", id);
                return View(sendlist);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("WARN_INFO_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var info = dao.GetEntity("WARN_INFO_DETAIL_ID", id);
                var h = DaoHall.GetEntity("HALL_NO", info.HALL_NO);
                if (null != h)
                    info.HALL_NAM = h.HALL_NAM;
                ViewBag.STATELIST = EnumHelper.GetCategorySelectList(typeof(WARN_INFO_DETAIL.STATE_ENUM), false);
                return View(info);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpPost]
        [UserAuth("WARN_INFO_EDT")]
        public ActionResult Edit(string id, WARN_INFO_DETAIL model)
        {
            try
            {
                ViewBag.STATELIST = EnumHelper.GetCategorySelectList(typeof(WARN_INFO_DETAIL.STATE_ENUM), false);
                var state = byte.Parse(Request["STATE"]);
                var method = Request["WARN_METHOD"];
                var reason = Request["WARN_REASON"];
                if (string.IsNullOrEmpty(reason))
                {
                    ModelState.AddModelError("WARN_REASON", "预警原因不能为空！");
                    return View(model);
                }
                if (string.IsNullOrEmpty(method))
                {
                    ModelState.AddModelError("WARN_METHOD", "预警处理不能为空！");
                    return View(model);
                }
                var old = dao.GetEntity("WARN_INFO_DETAIL_ID", model.WARN_INFO_DETAIL_ID);
                if (old == null || old.STATE == 2)
                {
                    ModelState.AddModelError("", "该预警已经处理！");
                    Alter("该预警已经处理！", util.Enum.AlterTypeEnum.Warning, false, false);
                    return View(model);
                }
                model.HANDLE_TIME = DateTime.Now;
                model.HANDLE_USER = UserState.UserID;
                var res = new WARN_INFO_DETAIL_DAL().HandlerState(model);
                if (res)
                    Alter("处理成功！", util.Enum.AlterTypeEnum.Success, true, true);
                else
                    Alter("处理失败！", util.Enum.AlterTypeEnum.Success, false, false);
                return View(model);
            }
            catch (Exception ex)
            {
                Alter("系统出错！", util.Enum.AlterTypeEnum.Error, false, false);
                LogHelper.ErrorLog("系统出错！", ex);
                return View(model);
            }
        }


        public ActionResult SendInfoTip()
        {
            var dallist = new WARN_INFO_DETAIL_DAL().GetNoSendInfoList(UserState.UserID);
            List<TipModel> unread = dallist
                .Select(item => new TipModel { typ = "WARNSENDINFODETAIL", id = item.WARN_INFO_DETAIL_ID, msg = item.WARN_INFO })
                .ToList();
            new WARN_SENDINFO_DETAIL_DAL().UPDATE_WARN_SENDINFO_DETAIL(dallist.Select(o => o.SENDINFO_DETAIL_ID));

            var dal1 = new WARN_ALARM_INFO_DETAIL_DAL();
            var unreadalarmlist = dal1.GetALARMSendInfo(UserState.UserID);
            var unreadalarm = unreadalarmlist.Select(item => new TipModel { typ = "ALARMSENDINFODETAIL", id = item.ALARM_SEQ, msg = item.ALARM_INFO })
                .ToList();
            dal1.UPDATE_WARN_ALARM_INFO_DETAIL(unreadalarmlist.Select(o => o.SENDINFO_DETAIL_ID));

            var _hall = DAO_WARN_ALARM_SEND_USER_CON.FindList("", "ALARM_TYP", 4, "USER_ID", UserState.UserID);
            var unreadalarm2 = DaoComplainDetail.FindList("", "STATE", 1, "HALL_NO in", _hall.Select(o => o.HALL_NO))
                .Select(item => new TipModel { typ = "COMPLAINSENDINFODETAIL", id = item.SEQ, msg = item.COMPLAIN_PRO })
                .ToList();

            if (null != unreadalarm && unreadalarm.Count() > 0)
                unread.AddRange(unreadalarm);
            if (null != unreadalarm2 && unreadalarm2.Count() > 0)
                unread.AddRange(unreadalarm2);

            if (null != unread && unread.Count() > 0)
                return View(unread);
            else
                return Content("");
        }

        [Inject]
        public IDao<WARN_ALARM_SEND_USER_CON> DAO_WARN_ALARM_SEND_USER_CON { get; set; }

        public ActionResult ReadSendInfoDeail(int id)
        {
            bool res = new WARN_INFO_DETAIL_DAL().UpdateSendDetailReadState(id);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }

    public class TipModel
    {
        public long id { get; set; }
        public string typ { get; set; }
        public string msg { get; set; }
    }
}
