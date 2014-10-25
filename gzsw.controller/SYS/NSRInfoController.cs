using System;
using System.Linq;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using System.Web;
using System.IO;
using System.Data;

namespace gzsw.controller.SYS
{
    public class NSRInfoController : BaseController<SYS_NSRINFO>
    {
        [Ninject.Inject]
        private IDao<SYS_NSRHY> DaoNsrhy { get; set; }

        [UserAuth("SYS_NSRINFO_VIW")]
        public ActionResult Index(string sbm, string nam, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.SBM = sbm;
            ViewBag.NAM = nam;

            var hylist = DaoNsrhy.FindList();
            var data = dao.GetList(pageIndex, pageSize, "NSR_LRRQ desc", "NSR_SBM", sbm, "NSR_NAME like", nam);
            foreach (var item in data.Items)
            {
                item.SysNsrhy = hylist.FirstOrDefault(obj => obj.HY_ID.ToString() == item.NSR_HYNAME);
            }
            return View(data);
        }

        [HttpGet]
        [UserAuth("SYS_NSRINFO_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var qserial = dao.GetEntity("NSR_SBM", id);
                qserial.SysNsrhy = DaoNsrhy.GetEntity("HY_NAME", qserial.NSR_HYNAME);
                return View(qserial);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看纳税人信息出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_NSRINFO_ADD")]
        public ActionResult Create()
        {
            GetCreateDT();
            return View();
        }

        private void GetCreateDT()
        {
            ViewData["NSR_HYNAME"] = new SelectList(DaoNsrhy.FindList(), "HY_NAME", "HY_NAME");
        }

        [HttpPost]
        [UserAuth("SYS_NSRINFO_ADD")]
        public ActionResult Create(SYS_NSRINFO nsrinfo)
        {
            try
            {
                GetCreateDT();
                CHKValid(nsrinfo);
                if (!ModelState.IsValid)
                {
                    return View();
                }
                var rst = dao.AddObject(nsrinfo);
                if (null != rst)
                {
                    Alter("新增成功！", util.Enum.AlterTypeEnum.Success, true, true);
                    return View();
                }
                else
                {
                    Alter("新增失败！", util.Enum.AlterTypeEnum.Success, false, false);
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                Alter("系统错误！", util.Enum.AlterTypeEnum.Success, false, false);
                return View();
            }
        }

        private void CHKValid(SYS_NSRINFO nsrinfo, bool isEdit = false)
        {
            if (string.IsNullOrEmpty(nsrinfo.NSR_SBM))
            {
                ModelState.AddModelError("NSR_SBM", "识别码不能为空！");
            }
            else
            {
                var exist = dao.GetEntity("NSR_SBM", nsrinfo.NSR_SBM);
                if (!isEdit)
                {
                    if (exist != null)
                        ModelState.AddModelError("NSR_SBM", "识别码已经存在！");
                }
            }
            if (string.IsNullOrEmpty(nsrinfo.NSR_NAME))
            {
                ModelState.AddModelError("NSR_NAME", "纳税人名不能为空！");
            }
            if (string.IsNullOrEmpty(nsrinfo.NSR_SWJGDM))
            {
                ModelState.AddModelError("NSR_SWJGDM", "纳税人机关代码不能为空！");
            }
        }

        [HttpGet]
        [UserAuth("SYS_NSRINFO_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var detailSerial = GetEdtDT(id);

                return View(detailSerial);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改纳税人信息出错", ex);
                return Redirect("/Home/Error");
            }
        }

        private SYS_NSRINFO GetEdtDT(string id)
        {
            var detailSerial = dao.GetEntity("NSR_SBM", id);
            ViewData["HYNAME"] = new SelectList(DaoNsrhy.FindList(), "HY_NAME", "HY_NAME", detailSerial.NSR_HYNAME);
            return detailSerial;
        }

        [HttpPost]
        [UserAuth("SYS_NSRINFO_EDT")]
        public ActionResult Edit(SYS_NSRINFO nsrinfo, string id)
        {
            try
            {
                var detailSerial = GetEdtDT(id);
                CHKValid(nsrinfo,true);
                if (!ModelState.IsValid)
                {
                    return View(detailSerial);
                }

                var rst = dao.UpdateObject(nsrinfo, "NSR_SBM");

                if (rst > 0)
                {
                    Alter("修改成功！", util.Enum.AlterTypeEnum.Success, true, true);
                    return View(nsrinfo);
                }
                else
                {
                    Alter("修改失败！", util.Enum.AlterTypeEnum.Error, false, false);
                    return View(nsrinfo);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                Alter("系统错误！", util.Enum.AlterTypeEnum.Error, false, false);
                return View(nsrinfo);
            }
        }

        [UserAuth("SYS_NSRINFO_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    dao.Delete("NSR_SBM", _id);
                }
                
                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除纳税人出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("SYS_NSRINFO_ADD")]
        public ActionResult ImportNSR()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [UserAuth("SYS_NSRINFO_ADD")]
        public ActionResult ImportNSRSubmit(HttpPostedFileBase fileData)
        {
            try
            {
                if (fileData != null)
                {
                    try
                    {
                        // 文件上传后的保存路径
                        string filePath = Server.MapPath("~/Uploads/NSRInfo/");
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        string fileName = Path.GetFileName(fileData.FileName);// 原始文件名称
                        string fileExtension = Path.GetExtension(fileName); // 文件扩展名
                        string saveName = Guid.NewGuid().ToString() + fileExtension; // 保存文件名称

                        fileData.SaveAs(filePath + saveName);
                        var dt = NPOIHelper.ImportToDataTable(filePath + saveName);
                        if (null!=dt)
                        {
                            dt.Columns[0].ColumnName = "NSR_SBM";
                            dt.Columns[1].ColumnName = "NSR_NAME";
                            dt.Columns[2].ColumnName = "NSR_SWJGDM";
                            dt.Columns[3].ColumnName = "NSR_HYNAME";
                            dt.Columns[4].ColumnName = "NSR_BSYPHONE";
                            dt.Columns[5].ColumnName = "NSR_FRPHONE";
                            dt.Columns.Add("NSR_LRRQ",typeof(DateTime));
                            dt.Columns.Add("NSR_LRR",typeof(string));
                            dt.Columns.Add("BATCH_ID", typeof(string));

                            Guid guid = Guid.NewGuid();
                            foreach (DataRow item in dt.Rows)
                            {
                                item["BATCH_ID"] = guid;
                                item["NSR_LRRQ"] = DateTime.Now;
                                item["NSR_LRR"] = UserState.UserID;
                            }                           
                            SqlHelper.BulkInsert("gzswEntities2", "IMPORT_NSRINFO", dt);
                            new SYS_NSRINFO_DAL().IMPORT_NSR(guid);
                        }
                        return JsonResult(true, dt.Rows.Count.ToString());
                    }
                    catch (Exception ex)
                    {
                        LogHelper.ErrorLog("上传失败！", ex);
                        return JsonResult(false, "上传失败！");
                    }
                }
                else
                {
                    return JsonResult(false, "请选择要上传的文件！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return JsonResult(false, "系统出错！");
            }
        }
    }
}
