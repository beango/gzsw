using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ninject;
using PetaPoco;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util.Extensions;
using gzsw.util.Office;

namespace gzsw.controller.STAT
{
    public class statbusinessController : StatController
    {
        [Inject]
        public IDao<STAT_STAFF_LARGE_BUSI_D> StatstafflargebusiDDao { get; set; }

        [Inject]
        public IDao<SYS_HALL> hallDao { get; set; }
        [Inject]
        public IDao<SYS_STAFF> staffDao { get; set; }

        public ActionResult Index(DateTime? beginTime, DateTime? endTime,string orgid, int pageIndex = 1, int pageSize = 20, bool export = false)
        {

            base.DateTimeInit(ref beginTime, ref endTime);
            // 初始化日期

            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", orgid);
            var orgs = orgall.Select(obj => obj.ORG_ID);

            var halllist = base.UserHall.Select(x => x.HALL_NO).ToArray();
            var data = new STAT_STAFF_LARGE_BUSI_D_DAL().GetStatsInfo(halllist, beginTime, endTime);
             
            if (export)//导出
            { 
                var index = 0;
                var temp = data.Select(x => new
                {
                    序号 = ++index,
                    业务大类 = x.DLS_SERIALNAME,
                    业务笔数 = x.BUSI_CNT,
                    业务折合量 = x.CONVERT_BUSI_CNT,
                    平均版办理时间 = ( ((int)(x.BUSI_CNT == 0 ? 0 : (x.HANDLE_DUR / x.BUSI_CNT))).ToTimeString()) + "%",
                    超时办理时间 = x.OVERTIME_HANDLE_CNT,
                    超时率 = ((x.BUSI_CNT == 0 ? 0 : (x.OVERTIME_HANDLE_CNT / x.BUSI_CNT)).ToString("P")) + "%",
                    同城业务笔数 = x.LOCAL_CNT,
                    同城办理率 = ((x.LOCAL_CNT * 100.0 / x.BUSI_CNT)).ToString("f2") + "%",
               
                }).ToList().ToDataTable();
                return AsposeExcelHelper.OutFileToRespone(temp, "业务大类分析总表");
            }
            else
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                ds.Tables.Add(dt);
                dt.Columns.Add("NAME", typeof(string));
                dt.Columns.Add("业务笔数", typeof(int));
                dt.Columns.Add("业务折合量", typeof(int));
                dt.Columns.Add("总办理时长", typeof(int));      
                dt.Columns.Add("超时办理量", typeof(int));
                dt.Columns.Add("同城业务量", typeof(int));

                foreach (var item in data)
                {
                    var r = dt.NewRow();
                    r["NAME"] = item.DLS_SERIALNAME;
                    r["业务笔数"] = item.BUSI_CNT;
                    r["业务折合量"] = item.CONVERT_BUSI_CNT;
                    r["总办理时长"] = item.HANDLE_DUR;
                    r["超时办理量"] = item.OVERTIME_HANDLE_CNT;
                    r["同城业务量"] = item.LOCAL_CNT;
                    dt.Rows.Add(r);
                }
                         
                ViewBag.ChartColumn3DXML = CreateMSColumn3DChart("业务大类分析总表", ds,430);

                ViewBag.ChartSplineXML = CreateMSSplineChart("业务大类分析总表", ds, 430);
                return View(data);
            }
            return null;
        }

        /// <summary>
        /// 排队业务报表--呼叫量
        /// </summary>
        /// <returns></returns>
        [HttpGet] 
        public ActionResult ShowCNT(string ct, DateTime? beginTime, DateTime? endTime, string dlsserialid)
        {
            DateTime bTime = beginTime.HasValue ? beginTime.Value.AddHours(8) : (DateTime)DateTime.Now.DefaultBeginDateTime();
            DateTime eTime = endTime.HasValue ? endTime.Value.AddHours(18) : (DateTime)DateTime.Now.DefaultEndDateTime();


            var relist = new List<STAT_STAFF_LARGE_BUSI_D>();
            var halllist = base.UserHall.Select(x => x.HALL_NO).ToArray();
            if (string.IsNullOrEmpty(dlsserialid))
            {
                relist =
                    StatstafflargebusiDDao.FindList("", "STAT_DT>=",
                                                    beginTime.HasValue
                                                        ? beginTime.Value
                                                        : Convert.ToDateTime("2000-01-01"), "STAT_DT<=",
                                                    endTime.HasValue ? endTime.Value : DateTime.Now).ToList();
            }
            else
            {
                relist =
                 StatstafflargebusiDDao.FindList("", "STAT_DT>=",
                                                 beginTime.HasValue
                                                     ? beginTime.Value
                                                     : Convert.ToDateTime("2000-01-01"), "STAT_DT<=",
                                                 endTime.HasValue ? endTime.Value : DateTime.Now, "DLS_SERIALID", dlsserialid).ToList();
            }
            relist = relist.Where(x => halllist.Contains(x.HALL_NO)).ToList();
            string titleName = "业务大类分析";
            var lineTable = new DataTable();
            lineTable.Columns.Add("Y_Name", typeof(string));

            switch (ct)
            {
                case "BUSI_CNT":
                    lineTable.Columns.Add("业务笔数", typeof(int));
                    titleName = titleName + "--业务笔数";
                    break;
                case "CONVERT_BUSI_CNT":
                    titleName = titleName + "--业务折合量";
                    lineTable.Columns.Add("业务折合量", typeof(int));
                    break;
                case "HANDLE_DUR":
                    lineTable.Columns.Add("平均办理时长", typeof(int));

                    titleName = titleName + "--平均办理时长";
                    break;

                case "OVERTIME_HANDLE_CNT":
                    lineTable.Columns.Add("超时办理量", typeof(int));
                    titleName = titleName + "--超时办理量";
                    break;
                case "LOCAL_CNT":
                    //总办理时长   
                    lineTable.Columns.Add("同城业务量", typeof(int));
                    titleName = titleName + "--同城业务量";
                    break; 
            }



            TimeSpan timeSpan = eTime.Subtract(bTime);
            var tlist = new List<string>();
            TimeSpenEnum tempsan;
            base.SetLineYName(timeSpan, bTime, lineTable, tlist, eTime);
            
            for (int i = 0; i < lineTable.Rows.Count; i++)
            {
                //var tempbtiem = Convert.ToDateTime(tlist[i]);

                //var tempetiem = DateTime.Now;
             

               // var soureList = new List<STAT_STAFF_LARGE_BUSI_D>();
                //switch (tempsan)
                //{
                //    case  TimeSpenEnum.Hour:
                //        tempbtiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : eTime.AddHours(1);
                //        soureList = relist.
                //        Where(x => x.TIME_QUANTUM_CD == Convert.ToByte(tempbtiem.Hour.ToString())).ToList();
                //        break;
                //    case TimeSpenEnum.Day:
                //         tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : eTime.AddDays(1);
                //            soureList = relist.Where(x => x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString())
                //                                          &&
                //                                          x.STAT_DT <Convert.ToDateTime(
                //                                              tempetiem.AddDays(1).ToShortDateString())).ToList();
                      
                //        break;
                //    case TimeSpenEnum.Month:
                //        tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : eTime.AddMonths(1);
                //            soureList =
                //                relist.Where(
                //                    x =>
                //                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                //                    x.STAT_DT < Convert.ToDateTime(tempetiem.ToShortDateString())).ToList(); 
                //        break;
                //    case TimeSpenEnum.Year:
                //        tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : eTime.AddYears(1);
                //            soureList =
                //                relist.Where(
                //                    x =>
                //                    x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) &&
                //                    x.STAT_DT < Convert.ToDateTime(tempetiem.ToShortDateString())).ToList(); 
                //        break;


                //}  
                var tempbtiem = Convert.ToDateTime(tlist[i]);

                var tempetiem = i + 1 < lineTable.Rows.Count ? Convert.ToDateTime(tlist[i + 1]) : eTime;

                var soureList = new List<STAT_STAFF_LARGE_BUSI_D>();
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
                        soureList = relist.Where(x => x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) && x.STAT_DT < Convert.ToDateTime(tempetiem.AddDays(1).ToShortDateString())).ToList();
                    }
                    else
                    {
                        soureList = relist.Where(x => x.STAT_DT >= Convert.ToDateTime(tempbtiem.ToShortDateString()) && x.STAT_DT < Convert.ToDateTime(tempetiem.ToShortDateString())).ToList();
                    }

                }
                switch (ct)
                {
                    case "BUSI_CNT":
                     
                        lineTable.Rows[i]["业务笔数"] =soureList.Count>0? soureList.Sum(x => x.BUSI_CNT):0;
                        break;
                    case "CONVERT_BUSI_CNT":
                        lineTable.Rows[i]["业务折合量"] = soureList.Count > 0 ? soureList.Sum(x => x.CONVERT_BUSI_CNT) : 0;
                        break;
                    case "HANDLE_DUR":


                        lineTable.Rows[i]["平均办理时长"] = soureList.Count > 0 ? (((int)(soureList.Sum(x => x.BUSI_CNT) == 0 ? 0 : (soureList.Sum(x => x.HANDLE_DUR) / soureList.Sum(x => x.BUSI_CNT))))) : 0;
                        break;

                    case "OVERTIME_HANDLE_CNT":

                        lineTable.Rows[i]["超时办理量"] = soureList.Count > 0 ? soureList.Sum(x => x.OVERTIME_HANDLE_CNT) : 0;
                        break;
                    case "LOCAL_CNT":
                        //总办理时长    
                        lineTable.Rows[i]["同城业务量"] = soureList.Sum(x => x.LOCAL_CNT);
                        break;  

                }
            }
            DataSet dss = new DataSet();
            dss.Tables.Add(lineTable);
            if (ct == "HANDLE_DUR")
            {
                titleName = titleName + "  (单位：秒)";
            }

            ViewBag.ChartColumn3DXML = CreateMSColumn3DChart(titleName, dss, 430);
            ViewBag.ChartSplineXML = CreateMSSplineChart(titleName, dss, 430);
            ViewBag.ChartPie3DXML = CreatePie3DChart(titleName, dss, 430);
            return View(lineTable);
            return null;
        }

    }
}
