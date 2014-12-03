using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.WARN
{
    public class ReleaseController : BaseController<WARN_RELEASE_STAFF_DETAIL>
    {
        [Inject]
        public IDao<SYS_STAFF> DaoStaff { get; set; }
        [Inject]
        public IDao<SYS_COUNTER> DaoCounter { get; set; }
        [Inject]
        public IDao<SYS_HALL> DaoHall { get; set; }
        [Inject]
        public IDao<WARN_RELEASE_COUNTER_DETAIL> DaoReleaseCounter { get; set; }
        [Inject]
        public IDao<WARN_RELEASE_TABLE_DETAIL> DaoReleaseTable { get; set; }
        [Inject]
        public IDao<TR_RELEASE_TABLE_INFO> DaoTable { get; set; }

        [UserAuth("WARN_RELEASE_VIW")]
        public ActionResult Index(string titl, int pageIndex = 1,int pageSize=20)
        {
            ViewBag.TITL = titl;
            var orgs = UserState.UserOrgs.Select(obj => obj.ORG_ID);
            ViewBag.ORGS = new SelectList(UserState.UserOrgs.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM");

            var data = new WARN_RELEASE_DAL().GetList(pageIndex, pageSize, null, null, titl);
            return View(data);
        }

        [HttpGet]
        [UserAuth("WARN_RELEASE_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                int mt = int.Parse(id.Split('-')[1]);
                int _id = int.Parse(id.Split('-')[0]);
                var info = new WARN_RELEASE_DAL().GetList(1, 100, mt, _id, "").Items.FirstOrDefault();
                return View(info);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看信息出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("WARN_RELEASE_ADD")]
        public ActionResult Create()
        {
            ViewBag.ORGS = new SelectList(UserState.UserOrgs.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM");
            var typelist = new List<SelectListItem>()
            {
                new SelectListItem(){Value="1",Text="员工",Selected = true},
                new SelectListItem(){Value="2",Text="窗口"},
                new SelectListItem(){Value="3",Text="看板"}
            };
            ViewBag.TypeList = typelist;
            return View();
        }

        public ActionResult Release_Partial(int mt, string org)
        {
            if (mt == 1)
            {
                ViewBag.STAFFLIST = new SelectList(DaoStaff.FindList("STAFF_ID asc", "ORG_ID", org), "STAFF_ID", "STAFF_NAM");
                return PartialView("Partial/Release_STAFF");
            }
            if (mt == 2)
            {
                var hall = DaoHall.GetEntity("ORG_ID", org);
                if (null != hall)
                    ViewBag.COUNTERLIST = new SelectList(DaoCounter.FindList("COUNTER_ID asc", "HALL_NO", hall.HALL_NO)
                        , "COUNTER_ID", "COUNTER_ID");
                return PartialView("Partial/Release_COUNTER");
            }
            if (mt == 3)
            {
                var hall = DaoHall.GetEntity("ORG_ID", org);
                if (null != hall)
                    ViewBag.TABLELIST = new SelectList(DaoTable.FindList("", "HALL_NO", hall.HALL_NO), "TABLE_CD", "TABLE_NAM");
                return PartialView("Partial/Release_TABLE");
            }
            return Redirect("/Home/Error");
        }

        [UserAuth("WARN_RELEASE_ADD")]
        [HttpPost]
        public ActionResult Create(WARN_RELEASE_STAFF_DETAIL info)
        {
            try
            {
                CHKVALID(info);
                info.RELEASE_TIME = DateTime.Now;
                info.RELEASE_USER_ID = UserState.UserID;
                string mtype = Request["MType"];
                if (mtype == "3" || mtype == "2")//发送给看板
                {
                    if (ModelState.ContainsKey("STAFF_ID"))
                    {
                        ModelState.Remove("STAFF_ID");
                    }
                }

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "数据验证出错！");
                    return JsonResult(false, "新增失败！", "WARN");
                }
                if (mtype == "1")//发送给员工
                {
                    string reqstaffid = Request["STAFF_ID"];
                    if (string.IsNullOrEmpty(reqstaffid))
                    {
                        ModelState.AddModelError("STAFF_ID", "请选择所要发布的员工编码！");
                        return JsonResult(false, "新增失败！", "WARN");
                    }
                    var arrstaffid = reqstaffid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<WARN_RELEASE_STAFF_DETAIL> stafflist = new List<WARN_RELEASE_STAFF_DETAIL>();
                    foreach (string staffid in arrstaffid)
                    {
                        var obj = CommonHelper.DeepClone<WARN_RELEASE_STAFF_DETAIL>(info);
                        obj.STAFF_ID = staffid;
                        obj.CLI_READ_IND = false;
                        stafflist.Add(obj);
                    }
                    dao.AddObject(stafflist);
                }
                if (mtype == "2")//发送给窗口
                {
                    string reqCOUNTER_ID = Request["COUNTER_ID"];
                    if (string.IsNullOrEmpty(reqCOUNTER_ID))
                    {
                        ModelState.AddModelError("COUNTER_ID", "请选择所要发布的窗口！");
                        return JsonResult(false, "新增失败！", "WARN");
                    }
                    var arrcounter = reqCOUNTER_ID.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<WARN_RELEASE_COUNTER_DETAIL> counterlist = new List<WARN_RELEASE_COUNTER_DETAIL>();
                    var counterall = DaoCounter.FindList();
                    foreach (string counterid in arrcounter)
                    {
                        var nobj = new WARN_RELEASE_COUNTER_DETAIL
                        {
                            RELEASE_USER_ID = info.RELEASE_USER_ID,
                            RELEASE_TIME = info.RELEASE_TIME,
                            RELEASE_MESSAGE = info.RELEASE_MESSAGE,
                            HALL_NO = counterall.FirstOrDefault(obj => obj.COUNTER_ID == int.Parse(counterid)).HALL_NO,
                            COUNTER_ID = int.Parse(counterid),
                            BEGIN_TIME = info.BEGIN_TIME,
                            END_TIME = info.END_TIME,
                            TITLE = info.TITLE
                        };
                        counterlist.Add(nobj);
                    }
                    DaoReleaseCounter.AddObject(counterlist);
                }
                if (mtype == "3")//发送给看板
                {
                    string reqTABLE_CD = Request["TABLE_CD"];
                    if (string.IsNullOrEmpty(reqTABLE_CD))
                    {
                        ModelState.AddModelError("TABLE_CD", "请选择所要发布的看板！");
                        return JsonResult(false, "新增失败！", "WARN");
                    }
                    var arrtable = reqTABLE_CD.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    List<WARN_RELEASE_TABLE_DETAIL> tablelist = new List<WARN_RELEASE_TABLE_DETAIL>();
                    var tableall = DaoTable.FindList();
                    foreach (string table in arrtable)
                    {
                        var nobj = new WARN_RELEASE_TABLE_DETAIL
                        {
                            RELEASE_USER_ID = info.RELEASE_USER_ID,
                            RELEASE_TIME = info.RELEASE_TIME,
                            RELEASE_MESSAGE = info.RELEASE_MESSAGE,
                            HALL_NO = tableall.FirstOrDefault(obj => obj.TABLE_CD == table).HALL_NO,
                            TABLE_CD = table,
                            BEGIN_TIME = info.BEGIN_TIME,
                            END_TIME = info.END_TIME,
                            TITLE = info.TITLE
                        };
                        tablelist.Add(nobj);
                    }
                    DaoReleaseTable.AddObject(tablelist);
                }
                return JsonResult(true, "新增成功！", "WARN");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误", ex);
                ModelState.AddModelError("", "系统错误！");
                return Redirect("/Home/Error");
            }
        }

        private void CHKVALID(WARN_RELEASE_STAFF_DETAIL info)
        {
            if (string.IsNullOrEmpty(info.TITLE))
            {
                ModelState.AddModelError("TITLE", "标题不能为空!");
            }
            if (string.IsNullOrEmpty(info.RELEASE_MESSAGE))
            {
                ModelState.AddModelError("RELEASE_MESSAGE", "信息详情不能为空!");
            }
            if (info.BEGIN_TIME<DateTime.Today)
            {
                ModelState.AddModelError("BEGIN_TIME", "开始时间必须大于当天!");
            }
            if (info.BEGIN_TIME >= info.END_TIME)
            {
                ModelState.AddModelError("END_TIME", "结束时间必须大于开始时间!");
            }
            if (!string.IsNullOrEmpty(info.RELEASE_MESSAGE))
            {
                info.RELEASE_MESSAGE = info.RELEASE_MESSAGE.Replace("\n\r", "<br>");
                info.RELEASE_MESSAGE = info.RELEASE_MESSAGE.Replace("\r", "<br>");
                info.RELEASE_MESSAGE = info.RELEASE_MESSAGE.Replace("\t", "　　"); 
            }
        }

        [UserAuth("WARN_RELEASE_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    int mt = int.Parse(_id.Split('-')[1]);
                    if (mt == 1)
                        dao.Delete("STAFF_DETAIL_ID", int.Parse(_id.Split('-')[0]));
                    if (mt == 2)
                        DaoReleaseCounter.Delete("COUNTER_DETAIL_ID", int.Parse(_id.Split('-')[0]));
                    if (mt == 3)
                        DaoReleaseTable.Delete("TABLE_DETAIL_ID", int.Parse(_id.Split('-')[0]));
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
