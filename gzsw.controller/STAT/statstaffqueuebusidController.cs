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

        public ActionResult Index(DateTime? beginTime, DateTime? endTime, int pageIndex = 1, int pageSize = 20, bool export = false)
        {

            base.DateTimeInit(ref beginTime, ref endTime);
            // 初始化日期
            
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", "");
            var orgs = orgall.Select(obj => obj.ORG_ID);
            var halllist =base.UserHall.Select(x=>x.HALL_NO).ToArray();
            var data = new STAT_STAFF_QUEUE_BUSI_D_DAL().GetStatsInfo(pageIndex, int.MaxValue,halllist, beginTime, endTime);
            if (export)//导出
            { 

                var index = 0;
                var temp = data.Select(x => new
                {
                    序号 = ++index,
                    业务编码 = x.QUEUE_BUSI_CD,
                    业务名称 = x.Q_SERIALNAME,
                    呼叫量 = x.CALL_CNT,
                    呼叫率 = ((x.CALL_CNT * 100.0 / data.Sum(obj => obj.CALL_CNT)).ToString("f2")) + "%",
                    超时等待量 = x.OVERTIME_WAIT_CNT,
                    超时等待率 = (data.Sum(obj => obj.OVERTIME_WAIT_CNT) == 0 ? "0" : (x.OVERTIME_WAIT_CNT * 100.0 / data.Sum(obj => obj.OVERTIME_WAIT_CNT)).ToString("f2")) + "%",
                    办理量 = x.HANDLE_CNT,
                    办理率 = ((x.HANDLE_CNT * 100.0 / data.Sum(obj => obj.HANDLE_CNT)).ToString("f2")) + "%",
                    弃号量 = x.ABANDON_CNT,
                    弃号率 = ((x.ABANDON_CNT * 100.0 / data.Sum(obj => obj.ABANDON_CNT)).ToString("f2")) + "%",
                    平均办理时间 = (x.HANDLE_CNT!=0?((x.HANDLE_DUR / x.HANDLE_CNT).ToString("f2")):0),
                    平均等待时间 = ((x.WAIT_DUR / x.TOT_TICKET_CNT).ToString("f2")) + "%",
                    最长等待时间 = x.MAX_WAIT_DUR,
                    最长办理时间 = x.MAX_HANDLE_DUR,
                    超时办理量 = x.OVERTIME_HANDLE_CNT,
                    超时办理率 = ((x.OVERTIME_HANDLE_CNT * 100.0 / x.TOT_TICKET_CNT).ToString("f2")) + "%",
                    人流量 = x.TOT_TICKET_CNT
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(temp, "排队业务报总表"); 
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
                
                ViewBag.ChartColumn3DXML = CreateMSColumn3DChart("排队业务报总表",ds,430);

                ViewBag.ChartSplineXML = CreateMSSplineChart("排队业务报总表", ds, 430); 
                return View(data);
            }
            return null;
        }
          
        /// <summary>
        /// 排队业务报表--呼叫量
        /// </summary>
        /// <returns></returns>
        [HttpGet] 
        public ActionResult ShowCNT(string ct, DateTime? beginTime, DateTime? endTime, string queueBusiCd)
        {
            DateTime bTime = beginTime.HasValue ? beginTime.Value.AddHours(8) : (DateTime)DateTime.Now.DefaultBeginDateTime();
            DateTime eTime = endTime.HasValue ? endTime.Value.AddHours(18) : (DateTime) DateTime.Now.DefaultEndDateTime();

            var halllist = base.UserHall.Select(x => x.HALL_NO).ToArray();
            var relist = new List<STAT_STAFF_QUEUE_BUSI_D>();
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
                                                    endTime.HasValue ? endTime.Value : DateTime.Now,"QUEUE_BUSI_CD",queueBusiCd).ToList();
            }
            relist = relist.Where(x => halllist.Contains(x.HALL_NO)).ToList();
            string titleName = "排队业务分析";
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
                        lineTable.Rows[i]["呼叫量"] =soureList.Count>0?soureList.Sum(x => x.CALL_CNT):0;
                        break;
                    case "OVERTIME_WAIT_CNT":
                        lineTable.Rows[i]["超时等候量"] = soureList.Count>0?soureList.Sum(x => x.OVERTIME_WAIT_CNT):0;
                        break;
                    case "HANDLE_CNT":
                        lineTable.Rows[i]["办理量"] = soureList.Count>0?soureList.Sum(x => x.HANDLE_CNT):0;
                        break;

                    case "ABANDON_CNT":
                        lineTable.Rows[i]["弃号量"] = soureList.Count>0?soureList.Sum(x => x.ABANDON_CNT):0;
                        break;
                    case "HANDLE_DUR":
                        //总办理时长   
                        lineTable.Rows[i]["平均办理时间"] = soureList.Count>0?soureList.Sum(x => x.HANDLE_CNT) != 0 ? soureList.Sum(x => x.HANDLE_DUR) / soureList.Sum(x => x.HANDLE_CNT) : 0:0;
                        break;
                    case "WAIT_DUR": //总等待时长
                        lineTable.Rows[i]["平均等待时间"] = soureList.Count>0?soureList.Sum(x => x.TOT_TICKET_CNT) != 0 ? soureList.Sum(x => x.WAIT_DUR) / soureList.Sum(x => x.TOT_TICKET_CNT) : 0:0;
                        break;

                    case "MAX_WAIT_DUR":
                        lineTable.Rows[i]["最长等待时间"] = soureList.Count>0?soureList.Max(x => x.MAX_WAIT_DUR):0;
                        break;
                    case "MAX_HANDLE_DUR":
                        lineTable.Rows[i]["最长办理时长"] = soureList.Count>0?soureList.Max(x => x.MAX_HANDLE_DUR):0;
                        break;
                    case "OVERTIME_HANDLE_CNT":
                        lineTable.Rows[i]["超时办理"] =soureList.Count>0? soureList.Sum(x => x.OVERTIME_HANDLE_CNT):0;
                        break;
                    case "TOT_TICKET_CNT":
                        lineTable.Rows[i]["人流量"] = soureList.Count>0?soureList.Sum(x => x.TOT_TICKET_CNT):0;
                        break;


                }
            }
            DataSet dss = new DataSet();
            dss.Tables.Add(lineTable);


            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName,dss, 430);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, dss, 430);
            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, dss, 430);
            return View(lineTable);
        }



        public ActionResult OtherIndex(DateTime? beginTime, DateTime? endTime, int pageIndex = 1, int pageSize = 20,
                                 bool export = false)
        {
            base.DateTimeInit(ref beginTime, ref endTime);
            // 初始化日期

            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", "");
            var orgs = orgall.Select(obj => obj.ORG_ID);
            var halllist= base.UserHall.Select(x => x.HALL_NO).ToArray();
            var data = new STAT_STAFF_QUEUE_BUSI_D_DAL().GetStatsInfo(pageIndex, int.MaxValue, halllist,beginTime, endTime);
            if (export)//导出
            {

                var index = 0;
                var temp = data.Select(x => new
                {
                    序号 = ++index,
                    业务编码 = x.QUEUE_BUSI_CD,
                    业务名称 = x.Q_SERIALNAME,
                    呼叫量 = x.CALL_CNT,
                    呼叫率 = ((x.CALL_CNT * 100.0 / data.Sum(obj => obj.CALL_CNT)).ToString("f2")) + "%",
                    超时等待量 = x.OVERTIME_WAIT_CNT,
                    超时等待率 = (data.Sum(obj => obj.OVERTIME_WAIT_CNT) == 0 ? "0" : (x.OVERTIME_WAIT_CNT * 100.0 / data.Sum(obj => obj.OVERTIME_WAIT_CNT)).ToString("f2")) + "%",
                    办理量 = x.HANDLE_CNT,
                    办理率 = ((x.HANDLE_CNT * 100.0 / data.Sum(obj => obj.HANDLE_CNT)).ToString("f2")) + "%",
                    弃号量 = x.ABANDON_CNT,
                    弃号率 = ((x.ABANDON_CNT * 100.0 / data.Sum(obj => obj.ABANDON_CNT)).ToString("f2")) + "%",
                    平均办理时间 = (x.HANDLE_CNT != 0 ? ((x.HANDLE_DUR / x.HANDLE_CNT).ToString("f2")) : 0),
                    平均等待时间 = ((x.WAIT_DUR / x.TOT_TICKET_CNT).ToString("f2")) + "%",
                    最长等待时间 = x.MAX_WAIT_DUR,
                    最长办理时间 = x.MAX_HANDLE_DUR,
                    超时办理量 = x.OVERTIME_HANDLE_CNT,
                    超时办理率 = ((x.OVERTIME_HANDLE_CNT * 100.0 / x.TOT_TICKET_CNT).ToString("f2")) + "%",
                    人流量 = x.TOT_TICKET_CNT
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(temp, "排队业务报总表");
            }
            else
            {
                DateTime bTime = beginTime.HasValue ? beginTime.Value.AddHours(8) : (DateTime)DateTime.Now.DefaultBeginDateTime();
                DateTime eTime = endTime.HasValue ? endTime.Value.AddHours(18) : (DateTime)DateTime.Now.DefaultEndDateTime();
 
                var relist = new List<STAT_STAFF_QUEUE_BUSI_D>();
             
                    relist =
                        StatstaffqueuebusidDao.FindList("", "STAT_DT>=",
                                                        beginTime.HasValue
                                                            ? beginTime.Value
                                                            : Convert.ToDateTime("2000-01-01"), "STAT_DT<=",
                                                        endTime.HasValue ? endTime.Value : DateTime.Now).ToList();

                relist = relist.Where(x => halllist.Contains(x.HALL_NO)).ToList();
                string titleName = "排队业务分析";
                var lineTable = new DataTable();
                lineTable.Columns.Add("Y_Name", typeof(string));
 
                        lineTable.Columns.Add("呼叫量", typeof(int));
                       // titleName = titleName + "--呼叫量"; 
                        titleName = titleName + "、超时等候量";
                        lineTable.Columns.Add("超时等候量", typeof(int));
                        lineTable.Columns.Add("办理量", typeof(int));

                        //titleName = titleName + "、办理量";
                      
                        lineTable.Columns.Add("弃号量", typeof(int));
                        titleName = titleName + "、弃号量";
                    
                        //总办理时长   
                        lineTable.Columns.Add("平均办理时间", typeof(int));
                     //   titleName = titleName + "、平均办理时间";
                    //总等待时长

                        lineTable.Columns.Add("平均等待时间", typeof(int));
                     //   titleName = titleName + "、平均等待时间";
                     
                        lineTable.Columns.Add("最长时间", typeof(int));
                   //     titleName = titleName + "、最长时间";
                        
                        lineTable.Columns.Add("最长办理时长", typeof(int));
                   //     titleName = titleName + "、最长办理时长";
                      
                        lineTable.Columns.Add("超时办理", typeof(int));
                    //    titleName = titleName + "、超时办理";
                     
                        lineTable.Columns.Add("人流量", typeof(int));
                 //       titleName = titleName + "、人流量";
                     
                



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
                            Where(x => x.TIME_QUANTUM_CD == Convert.ToByte(tempbtiem.Hour.ToString())).ToList();
                    }
                    else
                    {
                        if (Convert.ToDateTime(tempbtiem.ToShortDateString()) ==
                            Convert.ToDateTime(eTime.ToShortDateString()))
                        {
                            soureList =
                                relist.Where(
                                    x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT < Convert.ToDateTime(tempetiem.AddDays(1).ToShortDateString())).ToList();
                        }
                        else
                        {
                            soureList =
                                relist.Where(
                                    x =>
                                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                                    x.STAT_DT < Convert.ToDateTime(tempetiem.ToShortDateString())).ToList();
                        }

                    }
                     
                    lineTable.Rows[i]["呼叫量"] = soureList.Count > 0?soureList.Sum(x => x.CALL_CNT):0;

                    lineTable.Rows[i]["超时等候量"] =soureList.Count > 0? soureList.Sum(x => x.OVERTIME_WAIT_CNT):0;

                    lineTable.Rows[i]["办理量"] = soureList.Count > 0?soureList.Sum(x => x.HANDLE_CNT):0;

                    lineTable.Rows[i]["弃号量"] = soureList.Count > 0 ? soureList.Sum(x => x.ABANDON_CNT) : 0;

                        //总办理时长   
                    lineTable.Rows[i]["平均办理时间"] = soureList.Count > 0
                                                      ? (soureList.Sum(x => x.HANDLE_CNT) != 0
                                                             ? soureList.Sum(x => x.HANDLE_DUR)/
                                                               soureList.Sum(x => x.HANDLE_CNT)
                                                             : 0)
                                                      : 0;

                    lineTable.Rows[i]["平均等待时间"] = soureList.Count > 0
                                                      ? (soureList.Sum(x => x.TOT_TICKET_CNT) != 0
                                                             ? soureList.Sum(x => x.WAIT_DUR)/
                                                               soureList.Sum(x => x.TOT_TICKET_CNT)
                                                             : 0)
                                                      : 0;

                    lineTable.Rows[i]["最长时间"] =soureList.Count > 0?(soureList.Max(x => x.MAX_WAIT_DUR)):0;

                    lineTable.Rows[i]["最长时间"] = soureList.Count > 0?soureList.Max(x => x.MAX_HANDLE_DUR):0;

                    lineTable.Rows[i]["超时办理"] =soureList.Count > 0? soureList.Sum(x => x.OVERTIME_HANDLE_CNT):0;

                    lineTable.Rows[i]["人流量"] =soureList.Count > 0? soureList.Sum(x => x.TOT_TICKET_CNT):0;
                 



                }

                DataSet dss = new DataSet();
                dss.Tables.Add(lineTable);


                ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, dss, 430);
                ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, dss, 430);
                ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, dss, 430);
                return View(data);
            }
            return null;
        }
    }
}
