using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ninject;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using gzsw.util.Extensions;
using gzsw.util.Office;
using PetaPoco;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Drawing;
using NPOI.SS.Util;

namespace gzsw.controller.STAT
{
    public class statstaffqueuebusidController : StatController
    {
        [Inject]
        public IDao<STAT_STAFF_QUEUE_BUSI_D> StatstaffqueuebusidDao { get; set; }
        [Inject]
        public IDao<SYS_HALL> hallDao { get; set; }
        [Inject]
        public IDao<SYS_STAFF> staffDao { get; set; }
        [Inject]
        public IDao<SYS_QUEUESERIAL> SysqueueserialDao { get; set; }
        [Inject]
        public IDao<SYS_ORGANIZE> DaoORG { get; set; }

        public ActionResult Index(DateTime? beginTime, DateTime? endTime, int pageIndex = 1, int pageSize = 20, bool export = false)
        {

            base.DateTimeInit(ref beginTime, ref endTime);
            // 初始化日期

            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", "");
            var orgs = orgall.Select(obj => obj.ORG_ID);


            var mainTielt = GetOrgName(null, null);
            ViewBag.MainTitle = GetTitleName(mainTielt, "排队业务分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, "排队业务分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);


            var halllist = base.UserHall.Select(x => x.HALL_NO).ToArray();
            var data = new STAT_STAFF_QUEUE_BUSI_D_DAL().GetStatsInfo(pageIndex, int.MaxValue, halllist, beginTime, endTime);


            if (export)//导出
            {

                var index = 0;
                var temp = data.Select(x => new
                {
                    序号 = ++index,
                    业务编码 = x.QUEUE_BUSI_CD,
                    业务名称 = x.Q_SERIALNAME,
                    呼叫量 = x.CALL_CNT,
                    呼叫率 = CommonHelper.DivisionOfPercent(x.CALL_CNT ,data.Sum(obj => obj.CALL_CNT)),
                    超时等待量 = x.OVERTIME_WAIT_CNT,
                    超时等待率 = CommonHelper.DivisionOfTimeString(x.OVERTIME_WAIT_CNT, data.Sum(obj => obj.OVERTIME_WAIT_CNT)),
                    办理量 = x.HANDLE_CNT,
                    办理率 = CommonHelper.DivisionOfPercent(x.HANDLE_CNT,data.Sum(obj => obj.HANDLE_CNT)),
                    弃号量 = x.ABANDON_CNT,
                    弃号率 = CommonHelper.DivisionOfPercent(x.ABANDON_CNT,data.Sum(obj => obj.ABANDON_CNT)),
                    平均办理时间 = CommonHelper.DivisionOfTimeString(x.HANDLE_DUR , x.HANDLE_CNT),
                    平均等待时间 = CommonHelper.DivisionOfTimeString(x.WAIT_DUR , x.TOT_TICKET_CNT),
                    最长等待时间 = x.MAX_WAIT_DUR,
                    最长办理时间 = x.MAX_HANDLE_DUR,
                    超时办理量 = x.OVERTIME_HANDLE_CNT,
                    超时办理率 = CommonHelper.DivisionOfPercent(x.OVERTIME_HANDLE_CNT , x.TOT_TICKET_CNT),
                    人流量 = x.TOT_TICKET_CNT
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(temp, exceltitle);
            }
            else
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                ds.Tables.Add(dt);
                dt.Columns.Add("Q_SERIALNAME", typeof(string));
                dt.Columns.Add("呼叫量", typeof(int));
                dt.Columns.Add("办理量", typeof(int));
                dt.Columns.Add("弃号量", typeof(int));
                foreach (var item in data)
                {
                    DataRow r = dt.NewRow();
                    r["Q_SERIALNAME"] = item.Q_SERIALNAME;
                    r["呼叫量"] = item.CALL_CNT;
                    r["办理量"] = item.HANDLE_CNT;
                    r["弃号量"] = item.ABANDON_CNT;
                    dt.Rows.Add(r);
                }

                ViewBag.ChartColumn3DXML = CreateMSColumn3DChart("排队业务分析", ds.Tables[0], 430, subtitle);

                ViewBag.ChartSplineXML = CreateMSSplineChart("排队业务报分析", ds, 430, null, null, subtitle);
                return View(data);
            }
            return null;
        }

        private string GetSubTitle(string orgid, DateTime? beginTime, DateTime? endTime)
        {
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            string subTitle = "";
            if (!string.IsNullOrEmpty(orgid))
                subTitle += orgall.FirstOrDefault((o => o.ORG_ID == orgid)).ORG_NAM;
            else
                subTitle += orgall.OrderBy(o => o.ORG_LEVEL).FirstOrDefault().ORG_NAM;
            subTitle += "人流量对比分析";
            if (beginTime != null && endTime != null)
            {
                subTitle += "<span style='font-size:12px;'>（" + beginTime.Value.ToString("yyyy年MM月dd日");
                subTitle += " - " + endTime.Value.ToString("yyyy年MM月dd日") + "）</span>";
            }
            return subTitle;
        }

        /// <summary>
        /// 报表
        /// </summary>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="orgid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [UserAuth("STAT_STAFF_QUEUE_BUSI_D_OTHER_VIW")]
        public ActionResult OtherIndex(DateTime? beginTime, DateTime? endTime, string orgid, int pageIndex = 1, int pageSize = 20, bool export = false)
        {
            base.DateTimeInit(ref beginTime, ref endTime);

            ViewBag.beginTime = beginTime == null ? "" : beginTime.Value.ToString("yyyy-MM-dd");
            ViewBag.endTime = endTime == null ? "" : endTime.Value.ToString("yyyy-MM-dd");
            var t = 3;//员工报表
            ViewBag.NAM = "员工";
            if (string.IsNullOrEmpty(Request.QueryString["t"])
                || !int.TryParse(Request.QueryString["t"], out t))
            {
                t = 1;
            }
            if (t == 1)
            {
                ViewBag.NAM = "省市";
                ViewBag.NAMLink = "/STAT/statstaffqueuebusid/otherindex?t=2";//如果是省级，点击进入市级
            }
            if (t == 2)
            {
                ViewBag.NAM = "服务厅";
                ViewBag.NAMLink = "/STAT/statstaffqueuebusid/otherindex?t=3";//如果是省级，点击进入员工报表
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.市级 && t < 2)//判断是否有权限
            {
                return Redirect("/STAT/statstaffqueuebusid/otherindex?t=2");
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.区级 && t < 2)//判断是否有权限
            {
                return Redirect("/STAT/statstaffqueuebusid/otherindex?t=2");
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.服务厅级 && t < 3)//判断是否有权限
            {
                return Redirect("/STAT/statstaffqueuebusid/otherindex?t=3");
            }
            if (GetHighLV == model.Enums.UserLV_ENUM.无权限)//判断是否有权限
            {
                return NoAuth;
            }
            var re = base.UserHall;
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", orgid);
            if (!string.IsNullOrEmpty(orgid))
            {
                orgall = orgall.Where(obj => obj.ORG_ID == orgid).ToList();
                if (null == orgall || orgall.Count() == 0)
                {
                    orgall = new List<SYS_ORGANIZE> { new SYS_ORGANIZE { ORG_ID = "-1" } };
                }
            }

            if (export)
                pageIndex = 0;
            List<dynamic> data = null;

            if (t == 1)
            {
                data = new STAT_STAFF_QUEUE_BUSI_D_DAL().Q_STATDATA_GROUP_CITY(null, beginTime, endTime, t);
            }
            if (t == 2)
            {
                var halllist = new SYS_HALL_DAL().GetOrgHallAndChild(orgid).ToArray();
                data = new STAT_STAFF_QUEUE_BUSI_D_DAL().Q_STATDATA_GROUP_CITY(halllist, beginTime, endTime, t);
            }
            if (t == 3)
            {
                var halllist = new SYS_HALL_DAL().GetOrgHallAndChild(orgid).ToArray();
                data = new STAT_STAFF_QUEUE_BUSI_D_DAL().Q_STATDATA_GROUP_CITY(halllist, beginTime, endTime, t);
            }
            string subTitle = "";
            if (beginTime == null && endTime == null)
            {
                subTitle = GetSubTitle(orgid, data.Select(o => o.MIN_STAT_DT).Min(), data.Select(o => o.MAX_STAT_DT).Max());
            }
            else
                subTitle = GetSubTitle(orgid, beginTime, endTime);
            ViewBag.subTitle = subTitle;
            if (export)//导出
            {
                string xlnam = "人流量对比分析－省市";
                if (t == 2)
                    xlnam = "人流量对比分析－服务厅";
                if (t == 3)
                    xlnam = "人流量对比分析－排队业务";
                return ExportData(xlnam, subTitle, data);
            }
            else
            {
                subTitle = subTitle.Replace("<span style='font-size:12px;'>", "")
                    .Replace("</span>", "").Replace("人流量对比分析", "");
                DataTable dtCHART = null;
                dtCHART = GroupByNAM(data, t);
                ViewBag.ChartColumn3DXML = CreateMSColumn3DChart("人流量对比分析", dtCHART, 420, subTitle, true);
                ViewBag.ChartSplineXML = CreateMSSplineChart("人流量对比分析", dtCHART, 420, null, null, subTitle);
                var page = new Page<dynamic>() { Items = data.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList(), ItemsPerPage = pageSize, CurrentPage = pageIndex, TotalItems = data.Count };

                return View(page);
            }
        }

        /// <summary>
        /// 排队业务报表--呼叫量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShowCNT(string ct, DateTime? beginTime, DateTime? endTime, string queueBusiCd)
        {
            DateTime bTime = beginTime.HasValue ? beginTime.Value.AddHours(8) : (DateTime)DateTime.Now.DefaultBeginDateTime();
            DateTime eTime = endTime.HasValue ? endTime.Value.AddHours(18) : (DateTime)DateTime.Now.DefaultEndDateTime();

            var halllist = base.UserHall.Select(x => x.HALL_NO).ToArray();
            var relist = new List<STAT_STAFF_QUEUE_BUSI_D>();
            string serialname = "";
            if (string.IsNullOrEmpty(queueBusiCd))
            {
                relist =
                    StatstaffqueuebusidDao.FindList("", "STAT_DT>=",
                                                    beginTime.HasValue
                                                        ? beginTime.Value
                                                        : Convert.ToDateTime("2000-01-01"), "STAT_DT<=",
                                                    endTime.HasValue ? endTime.Value : DateTime.Now).ToList();
            }
            else
            {
                relist =
                 StatstaffqueuebusidDao.FindList("", "STAT_DT>=",
                                                 beginTime.HasValue
                                                     ? beginTime.Value
                                                     : Convert.ToDateTime("2000-01-01"), "STAT_DT<=",
                                                 endTime.HasValue ? endTime.Value : DateTime.Now, "QUEUE_BUSI_CD", queueBusiCd).ToList();
                var ywdl = SysqueueserialDao.GetEntity("Q_SERIALID", queueBusiCd);
                serialname = ywdl.Q_SERIALNAME;
            }
            relist = relist.Where(x => halllist.Contains(x.HALL_NO)).ToList();
            string titleName = "业务分析";
            if (!string.IsNullOrEmpty(serialname))
            {
                titleName = serialname + titleName;
            }
            var lineTable = new DataTable();
            lineTable.Columns.Add("Y_Name", typeof(string));

            switch (ct)
            {
                case "CALL_CNT":
                    lineTable.Columns.Add("呼叫量", typeof(int));
                    titleName = titleName + "--呼叫量";
                    break;
                case "OVERTIME_WAIT_CNT":
                    titleName = titleName + "--超时等候量";
                    lineTable.Columns.Add("超时等候量", typeof(int));
                    break;
                case "HANDLE_CNT":
                    lineTable.Columns.Add("办理量", typeof(int));

                    titleName = titleName + "--办理量";
                    break;

                case "ABANDON_CNT":
                    lineTable.Columns.Add("弃号量", typeof(int));
                    titleName = titleName + "--弃号量";
                    break;
                case "HANDLE_DUR":
                    //总办理时长   
                    lineTable.Columns.Add("平均办理时间", typeof(int));
                    titleName = titleName + "--平均办理时间";
                    break;
                case "WAIT_DUR": //总等待时长

                    lineTable.Columns.Add("平均等待时间", typeof(int));
                    titleName = titleName + "--平均等待时间";
                    break;

                case "MAX_WAIT_DUR":
                    lineTable.Columns.Add("最长等待时间", typeof(int));
                    titleName = titleName + "--最长时间";
                    break;
                case "MAX_HANDLE_DUR":
                    lineTable.Columns.Add("最长办理时长", typeof(int));
                    titleName = titleName + "--最长办理时长";
                    break;
                case "OVERTIME_HANDLE_CNT":
                    lineTable.Columns.Add("超时办理", typeof(int));
                    titleName = titleName + "--超时办理";
                    break;
                case "TOT_TICKET_CNT":
                    lineTable.Columns.Add("人流量", typeof(int));
                    titleName = titleName + "--人流量";
                    break;


            }

            var mainTielt = GetOrgName(null, null);
            ViewBag.MainTitle = GetTitleName(mainTielt, "排队业务分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, "排队业务分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);


            TimeSpan timeSpan = eTime.Subtract(bTime);
            var tlist = new List<string>();
            base.SetLineYName(timeSpan, bTime, lineTable, tlist, eTime);
            for (int i = 0; i < lineTable.Rows.Count; i++)
            {
                var tempbtiem = Convert.ToDateTime(tlist[i]);

                var tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : eTime;

                var soureList = new List<STAT_STAFF_QUEUE_BUSI_D>();
                if (timeSpan.Days < 1)
                {
                    soureList = relist.
                        Where(x => x.TIME_QUANTUM_CD == tempbtiem.Hour).ToList();
                }
                else
                {
                    if (Convert.ToDateTime(tempbtiem.ToShortDateString()) ==
                        Convert.ToDateTime(eTime.ToShortDateString()))
                    {
                        soureList = relist.Where(x => x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) && x.STAT_DT < Convert.ToDateTime(tempetiem.AddDays(1).ToShortDateString())).ToList();
                    }
                    else
                    {
                        soureList = relist.Where(x => x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) && x.STAT_DT < Convert.ToDateTime(tempetiem.ToShortDateString())).ToList();
                    }

                }
                switch (ct)
                {
                    case "CALL_CNT":
                        lineTable.Rows[i]["呼叫量"] = soureList.Count > 0 ? soureList.Sum(x => x.CALL_CNT) : 0;
                        break;
                    case "OVERTIME_WAIT_CNT":
                        lineTable.Rows[i]["超时等候量"] = soureList.Count > 0 ? soureList.Sum(x => x.OVERTIME_WAIT_CNT) : 0;
                        break;
                    case "HANDLE_CNT":
                        lineTable.Rows[i]["办理量"] = soureList.Count > 0 ? soureList.Sum(x => x.HANDLE_CNT) : 0;
                        break;

                    case "ABANDON_CNT":
                        lineTable.Rows[i]["弃号量"] = soureList.Count > 0 ? soureList.Sum(x => x.ABANDON_CNT) : 0;
                        break;
                    case "HANDLE_DUR":
                        //总办理时长   
                        lineTable.Rows[i]["平均办理时间"] = soureList.Count > 0 ? soureList.Sum(x => x.HANDLE_CNT) != 0 ? soureList.Sum(x => x.HANDLE_DUR) / (soureList.Sum(x => x.HANDLE_CNT)) : 0 : 0;
                        break;
                    case "WAIT_DUR": //总等待时长
                        lineTable.Rows[i]["平均等待时间"] = soureList.Count > 0 ? soureList.Sum(x => x.TOT_TICKET_CNT) != 0 ? soureList.Sum(x => x.WAIT_DUR) / (soureList.Sum(x => x.TOT_TICKET_CNT)) : 0 : 0;
                        break;

                    case "MAX_WAIT_DUR":
                        lineTable.Rows[i]["最长等待时间"] = soureList.Count > 0 ? soureList.Max(x => x.MAX_WAIT_DUR) : 0;
                        break;
                    case "MAX_HANDLE_DUR":
                        lineTable.Rows[i]["最长办理时长"] = soureList.Count > 0 ? soureList.Max(x => x.MAX_HANDLE_DUR) : 0;
                        break;
                    case "OVERTIME_HANDLE_CNT":
                        lineTable.Rows[i]["超时办理"] = soureList.Count > 0 ? soureList.Sum(x => x.OVERTIME_HANDLE_CNT) : 0;
                        break;
                    case "TOT_TICKET_CNT":
                        lineTable.Rows[i]["人流量"] = soureList.Count > 0 ? soureList.Sum(x => x.TOT_TICKET_CNT) : 0;
                        break;


                }
            }
            DataSet dss = new DataSet();
            dss.Tables.Add(lineTable);

            ViewBag.ChartColumn3DXML = CreateColumn3DChart(titleName, lineTable, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, lineTable, 550, null, null, subtitle);

            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, dss, 430);

            if (ct == "HANDLE_DUR" || ct == "WAIT_DUR" || ct == "MAX_WAIT_DUR" || ct == "MAX_HANDLE_DUR")
            {
                ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, lineTable, 550, subtitle, true, true, "秒", "60", "分");
                ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, lineTable, 550, null, null, subtitle, true, "分", "60", "分");
            }
            return View(lineTable);
        }

        private DataTable GroupByNAM(List<dynamic> data, int t)
        {
            var query = from tt in data.AsEnumerable()
                        group tt by new { tk1 = tt.ID, tk2 = tt.NAM } into m
                        select m;
            if (t == 3)
                query = from tt in data.AsEnumerable()
                        group tt by new { tk1 = tt.QUEUE_BUSI_CD, tk2 = tt.Q_SERIALNAME } into m
                        select m;
            var dtquery = query.Select(m => new
            {
                ID = m.Key.tk1,
                NAM = m.Key.tk2,
                CALL_CNT = m.Sum(n => n.CALL_CNT),
                OVERTIME_WAIT_CNT = m.Sum(n => n.OVERTIME_WAIT_CNT),
                HANDLE_CNT = m.Sum(n => n.HANDLE_CNT),
                ABANDON_CNT = m.Sum(n => n.ABANDON_CNT),
                HANDLE_DUR = m.Sum(n => n.HANDLE_DUR),
                WAIT_DUR = m.Sum(n => n.WAIT_DUR),
                MAX_WAIT_DUR = m.Sum(n => n.MAX_WAIT_DUR),
                MAX_HANDLE_DUR = m.Sum(n => n.MAX_HANDLE_DUR),
                OVERTIME_HANDLE_CNT = m.Sum(n => n.OVERTIME_HANDLE_CNT),
                TOT_TICKET_CNT = m.Sum(n => n.TOT_TICKET_CNT)
            });
            DataTable dtCHART = new DataTable();
            dtCHART.Columns.Add("NAM", typeof(string));
            dtCHART.Columns.Add("呼叫量,P", typeof(string));
            dtCHART.Columns.Add("办理量,P", typeof(string));
            dtCHART.Columns.Add("弃号量,S", typeof(string));
            foreach (var item in dtquery)
            {
                DataRow nr = dtCHART.NewRow();

                nr["NAM"] = item.NAM;
                nr["呼叫量,P"] = item.CALL_CNT;
                nr["办理量,P"] = item.HANDLE_CNT;
                nr["弃号量,S"] = item.ABANDON_CNT;
                dtCHART.Rows.Add(nr);
            }
            return dtCHART;
        }

        /// <summary>
        /// 排队业务报表--呼叫量
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ShowQueueCNT(string ct, DateTime? beginTime, DateTime? endTime, string queueBusiCd, string orgid)
        {
            DateTime bTime = beginTime.HasValue ? beginTime.Value.AddHours(8) : (DateTime)DateTime.Now.DefaultBeginDateTime();
            DateTime eTime = endTime.HasValue ? endTime.Value.AddHours(18) : (DateTime)DateTime.Now.DefaultEndDateTime();

            var halllist = base.UserHall.Select(x => x.HALL_NO).ToArray();
            var relist = new List<STAT_STAFF_QUEUE_BUSI_D>();
            string serialname = "";
            var hallandchild = new SYS_HALL_DAL().GetOrgHallAndChild(orgid).ToArray();

            if (string.IsNullOrEmpty(queueBusiCd))
            {
                relist =
                    StatstaffqueuebusidDao.FindList("", "STAT_DT>=",
                                                    beginTime.HasValue
                                                        ? beginTime.Value
                                                        : Convert.ToDateTime("2000-01-01"), "STAT_DT<=",
                                                    endTime.HasValue ? endTime.Value : DateTime.Now,
                                                    "HALL_NO in", hallandchild).ToList();
            }
            else
            {
                relist =
                 StatstaffqueuebusidDao.FindList("", "STAT_DT>=",
                                                 beginTime.HasValue
                                                     ? beginTime.Value
                                                     : Convert.ToDateTime("2000-01-01"), "STAT_DT<=",
                                                 endTime.HasValue ? endTime.Value : DateTime.Now, "QUEUE_BUSI_CD", queueBusiCd,
                                                "HALL_NO in", hallandchild).ToList();
                var ywdl = SysqueueserialDao.GetEntity("Q_SERIALID", queueBusiCd);
                serialname = ywdl.Q_SERIALNAME;
            }
            string titleName = "业务分析";
            if (!string.IsNullOrEmpty(serialname))
            {
                titleName = serialname + titleName;
            }
            var lineTable = new DataTable();
            lineTable.Columns.Add("Y_Name", typeof(string));

            switch (ct)
            {
                case "CALL_CNT":
                    lineTable.Columns.Add("呼叫量", typeof(int));
                    titleName = titleName + "--呼叫量";
                    break;
                case "OVERTIME_WAIT_CNT":
                    titleName = titleName + "--超时等候量";
                    lineTable.Columns.Add("超时等候量", typeof(int));
                    break;
                case "HANDLE_CNT":
                    lineTable.Columns.Add("办理量", typeof(int));

                    titleName = titleName + "--办理量";
                    break;

                case "ABANDON_CNT":
                    lineTable.Columns.Add("弃号量", typeof(int));
                    titleName = titleName + "--弃号量";
                    break;
                case "HANDLE_DUR":
                    //总办理时长   
                    lineTable.Columns.Add("平均办理时间", typeof(int));
                    titleName = titleName + "--平均办理时间";
                    break;
                case "WAIT_DUR": //总等待时长

                    lineTable.Columns.Add("平均等待时间", typeof(int));
                    titleName = titleName + "--平均等待时间";
                    break;

                case "MAX_WAIT_DUR":
                    lineTable.Columns.Add("最长等待时间", typeof(int));
                    titleName = titleName + "--最长时间";
                    break;
                case "MAX_HANDLE_DUR":
                    lineTable.Columns.Add("最长办理时长", typeof(int));
                    titleName = titleName + "--最长办理时长";
                    break;
                case "OVERTIME_HANDLE_CNT":
                    lineTable.Columns.Add("超时办理", typeof(int));
                    titleName = titleName + "--超时办理";
                    break;
                case "TOT_TICKET_CNT":
                    lineTable.Columns.Add("人流量", typeof(int));
                    titleName = titleName + "--人流量";
                    break;


            }

            var mainTielt = GetOrgName(null, null);
            ViewBag.MainTitle = GetTitleName(mainTielt, "排队业务分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault());
            var subtitle = GetTitleName(mainTielt, "", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);
            var exceltitle = GetTitleName(mainTielt, "排队业务分析", beginTime.GetValueOrDefault(), endTime.GetValueOrDefault(), false);


            TimeSpan timeSpan = eTime.Subtract(bTime);
            var tlist = new List<string>();
            base.SetLineYName(timeSpan, bTime, lineTable, tlist, eTime);
            for (int i = 0; i < lineTable.Rows.Count; i++)
            {
                var tempbtiem = Convert.ToDateTime(tlist[i]);

                var tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : eTime;

                var soureList = new List<STAT_STAFF_QUEUE_BUSI_D>();
                if (timeSpan.Days < 1)
                {
                    soureList = relist.
                        Where(x => x.TIME_QUANTUM_CD == tempbtiem.Hour).ToList();
                }
                else
                {
                    if (Convert.ToDateTime(tempbtiem.ToShortDateString()) ==
                        Convert.ToDateTime(eTime.ToShortDateString()))
                    {
                        soureList = relist.Where(x => x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) && x.STAT_DT < Convert.ToDateTime(tempetiem.AddDays(1).ToShortDateString())).ToList();
                    }
                    else
                    {
                        soureList = relist.Where(x => x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) && x.STAT_DT < Convert.ToDateTime(tempetiem.ToShortDateString())).ToList();
                    }

                }
                switch (ct)
                {
                    case "CALL_CNT":
                        lineTable.Rows[i]["呼叫量"] = soureList.Count > 0 ? soureList.Sum(x => x.CALL_CNT) : 0;
                        break;
                    case "OVERTIME_WAIT_CNT":
                        lineTable.Rows[i]["超时等候量"] = soureList.Count > 0 ? soureList.Sum(x => x.OVERTIME_WAIT_CNT) : 0;
                        break;
                    case "HANDLE_CNT":
                        lineTable.Rows[i]["办理量"] = soureList.Count > 0 ? soureList.Sum(x => x.HANDLE_CNT) : 0;
                        break;

                    case "ABANDON_CNT":
                        lineTable.Rows[i]["弃号量"] = soureList.Count > 0 ? soureList.Sum(x => x.ABANDON_CNT) : 0;
                        break;
                    case "HANDLE_DUR":
                        //总办理时长   
                        lineTable.Rows[i]["平均办理时间"] = soureList.Count > 0 ? soureList.Sum(x => x.HANDLE_CNT) != 0 ? soureList.Sum(x => x.HANDLE_DUR) / (soureList.Sum(x => x.HANDLE_CNT)) : 0 : 0;
                        break;
                    case "WAIT_DUR": //总等待时长
                        lineTable.Rows[i]["平均等待时间"] = soureList.Count > 0 ? soureList.Sum(x => x.TOT_TICKET_CNT) != 0 ? soureList.Sum(x => x.WAIT_DUR) / (soureList.Sum(x => x.TOT_TICKET_CNT)) : 0 : 0;
                        break;

                    case "MAX_WAIT_DUR":
                        lineTable.Rows[i]["最长等待时间"] = soureList.Count > 0 ? soureList.Max(x => x.MAX_WAIT_DUR) : 0;
                        break;
                    case "MAX_HANDLE_DUR":
                        lineTable.Rows[i]["最长办理时长"] = soureList.Count > 0 ? soureList.Max(x => x.MAX_HANDLE_DUR) : 0;
                        break;
                    case "OVERTIME_HANDLE_CNT":
                        lineTable.Rows[i]["超时办理"] = soureList.Count > 0 ? soureList.Sum(x => x.OVERTIME_HANDLE_CNT) : 0;
                        break;
                    case "TOT_TICKET_CNT":
                        lineTable.Rows[i]["人流量"] = soureList.Count > 0 ? soureList.Sum(x => x.TOT_TICKET_CNT) : 0;
                        break;


                }
            }
            DataSet dss = new DataSet();
            dss.Tables.Add(lineTable);

            ViewBag.ChartColumn3DXML = CreateColumn3DChart(titleName, lineTable, subtitle);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, lineTable, 550, null, null, subtitle);

            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, dss, 430);

            if (ct == "HANDLE_DUR" || ct == "WAIT_DUR" || ct == "MAX_WAIT_DUR" || ct == "MAX_HANDLE_DUR")
            {
                ViewBag.ChartColumn3DXML = CreateColumn3DChart(titleName, lineTable, subtitle, true, "秒", "60", "分");
                ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, lineTable, 550, null, null, subtitle, true, "秒", "60", "分");
            }
            return View(lineTable);
        }

        public ActionResult ExportData(string sheet, string subTitle, List<dynamic> exportData)
        {
            #region 加载模板文件到工作簿对象中

            HSSFWorkbook hssfworkbook;
            using (FileStream file = new FileStream(Server.MapPath("/Areas/STAT/Views/STATTemp/人流量对比分析.xls"), FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            #endregion

            #region 根据模板设置工作表的内容

            ISheet sheet1 = hssfworkbook.GetSheet("人流量对比分析");
            string maintitle = subTitle.Replace("<span style='font-size:12px;'>", "").Replace("</span>", "");
            sheet1.GetRow(0).Cells[0].SetCellValue(maintitle);
            sheet1.GetRow(0).Cells.RemoveAt(1);
            sheet1.GetRow(1).Cells.RemoveAt(1);
            sheet1.GetRow(2).Cells.RemoveAt(1);
            sheet1.GetRow(3).Cells.RemoveAt(1);
            int rowIndex = 4;
            if (null != exportData)
            {
                #region 数据单元格样式
                var cellFont = hssfworkbook.CreateFont();
                var cellStyle = hssfworkbook.CreateCellStyle();

                cellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.White.Index;
                cellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                #endregion
                sheet1.GetRow(3).Cells[1].SetCellValue(sheet.Split('－')[1]);
                foreach (var row in exportData)
                {
                    IRow xlsrow = sheet1.CreateRow(rowIndex);
                    xlsrow.CreateCell(0).SetCellValue(rowIndex - 3);
                    xlsrow.CreateCell(1).SetCellValue(row.NAM);
                    xlsrow.CreateCell(2).SetCellValue(row.Q_SERIALNAME);
                    xlsrow.CreateCell(3).SetCellValue(row.CALL_CNT);
                    xlsrow.CreateCell(4).SetCellValue(CommonHelper.DivisionOfPercent(row.CALL_CNT, row.TOT_TICKET_CNT));
                    xlsrow.CreateCell(5).SetCellValue(row.HANDLE_CNT);
                    xlsrow.CreateCell(6).SetCellValue(CommonHelper.DivisionOfPercent(row.HANDLE_CNT, row.TOT_TICKET_CNT));
                    xlsrow.CreateCell(7).SetCellValue(row.ABANDON_CNT);
                    xlsrow.CreateCell(8).SetCellValue(CommonHelper.DivisionOfPercent(row.ABANDON_CNT, row.TOT_TICKET_CNT));
                    xlsrow.CreateCell(9).SetCellValue(CommonHelper.DivisionOfTimeString(row.HANDLE_DUR, row.HANDLE_CNT));
                    xlsrow.CreateCell(10).SetCellValue(CommonHelper.DivisionOfTimeString(row.WAIT_DUR , row.CALL_CNT));
                    xlsrow.CreateCell(11).SetCellValue(row.OVERTIME_WAIT_CNT);
                    xlsrow.CreateCell(12).SetCellValue(CommonHelper.DivisionOfPercent(row.OVERTIME_WAIT_CNT, row.TOT_TICKET_CNT));
                    xlsrow.CreateCell(13).SetCellValue(((int)row.MAX_WAIT_DUR).ToTimeString());
                    xlsrow.CreateCell(14).SetCellValue(((int)row.MAX_HANDLE_DUR).ToTimeString());
                    xlsrow.CreateCell(15).SetCellValue(row.OVERTIME_HANDLE_CNT);
                    xlsrow.CreateCell(16).SetCellValue(CommonHelper.DivisionOfPercent(row.OVERTIME_HANDLE_CNT, row.HANDLE_CNT));
                    xlsrow.CreateCell(17).SetCellValue(row.TOT_TICKET_CNT);
                    #region 设置单元格样式
                    foreach (var cell in xlsrow.Cells)
                    {
                        cell.CellStyle = cellStyle;
                        cell.CellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                        cell.CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                        cell.CellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                        cell.CellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                        cell.CellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                        cell.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    }
                    #endregion
                    sheet1.GetRow(rowIndex).Cells.RemoveAt(1);
                    rowIndex++;
                }
                IRow footer = sheet1.CreateRow(rowIndex);

                double totalCALLCNT = exportData.Sum(obj => obj.CALL_CNT);
                footer.CreateCell(0).SetCellValue("合计");//#fffdbb
                footer.CreateCell(1).SetCellValue("合计");//#fffdbb
                footer.CreateCell(2).SetCellValue("合计");
                footer.CreateCell(3).SetCellValue(totalCALLCNT);
                footer.CreateCell(4).SetCellValue(CommonHelper.DivisionOfPercent(exportData.Sum(obj => obj.CALL_CNT), exportData.Sum(obj => obj.TOT_TICKET_CNT)));
                footer.CreateCell(5).SetCellValue(exportData.Sum(obj => obj.HANDLE_CNT));
                footer.CreateCell(6).SetCellValue(CommonHelper.DivisionOfPercent(exportData.Sum(obj => obj.HANDLE_CNT), exportData.Sum(obj => obj.TOT_TICKET_CNT)));
                footer.CreateCell(7).SetCellValue(exportData.Sum(obj => obj.ABANDON_CNT));
                footer.CreateCell(8).SetCellValue(CommonHelper.DivisionOfPercent(exportData.Sum(obj => obj.ABANDON_CNT), exportData.Sum(obj => obj.CALL_CNT)));
                footer.CreateCell(9).SetCellValue(CommonHelper.DivisionOfTimeString(exportData.Sum(obj => obj.HANDLE_DUR), exportData.Sum(obj => obj.HANDLE_CNT)));
                footer.CreateCell(10).SetCellValue(CommonHelper.DivisionOfTimeString(exportData.Sum(obj => obj.WAIT_DUR) , exportData.Sum(obj => obj.CALL_CNT)));

                footer.CreateCell(11).SetCellValue((exportData.Sum(obj => obj.OVERTIME_WAIT_CNT)) + "%");
                footer.CreateCell(12).SetCellValue(CommonHelper.DivisionOfPercent(exportData.Sum(obj => obj.OVERTIME_WAIT_CNT), exportData.Sum(obj => obj.TOT_TICKET_CNT)));
                footer.CreateCell(13).SetCellValue(((int)exportData.Max(obj => obj.MAX_WAIT_DUR)).ToTimeString());
                footer.CreateCell(14).SetCellValue(((int)exportData.Max(obj => obj.MAX_HANDLE_DUR)).ToTimeString());
                footer.CreateCell(15).SetCellValue(exportData.Sum(obj => obj.OVERTIME_HANDLE_CNT));
                footer.CreateCell(16).SetCellValue(CommonHelper.DivisionOfPercent(exportData.Sum(obj => obj.OVERTIME_HANDLE_CNT) , exportData.Sum(obj => obj.HANDLE_CNT)));
                footer.CreateCell(17).SetCellValue(exportData.Sum(obj => obj.TOT_TICKET_CNT));
                #region 底部样式
                var footercellFont = hssfworkbook.CreateFont();
                var footercellStyle = hssfworkbook.CreateCellStyle();

                footercellFont.Boldweight = (short)NPOI.SS.UserModel.FontBoldWeight.Bold;
                footercellStyle.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Yellow.Index;
                footercellStyle.FillPattern = NPOI.SS.UserModel.FillPattern.SolidForeground;
                var HSSFPalette = hssfworkbook.GetCustomPalette();
                Color c = ToColor("#fffdbb");
                HSSFPalette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.Yellow.Index, c.R, c.G, c.B);
                #endregion

                foreach (var cell in footer.Cells)
                {
                    cell.CellStyle = footercellStyle;
                    cell.CellStyle.VerticalAlignment = NPOI.SS.UserModel.VerticalAlignment.Center;
                    cell.CellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    cell.CellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cell.CellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                }
                CellRangeAddress cellRangeAddress = new CellRangeAddress(rowIndex, rowIndex, 0, 2);
                sheet1.AddMergedRegion(cellRangeAddress);
                sheet1.GetRow(rowIndex).Cells.RemoveAt(1);
            }
            //强制Excel重新计算表中所有的公式
            sheet1.ForceFormulaRecalculation = true;
            sheet1.RemoveColumnBreak(1);
            #endregion

            #region 写入到客户端

            MemoryStream ms = new MemoryStream();
            //将工作簿的内容放到内存流中
            hssfworkbook.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return File(ms, "application/vnd.ms-excel", sheet + ".xls");

            #endregion
        }
    }
}
