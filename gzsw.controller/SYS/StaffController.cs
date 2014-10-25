using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using gzsw.util.Enum;
using Ninject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace gzsw.controller.SYS
{
    /// <summary>
    /// 员工管理
    /// </summary>
    public class StaffController :BaseController<SYS_STAFF>
    {
        [Inject]
        public IDao<SYS_ORGANIZE> DaoOrganize { get; set; }
        [Inject]
        public IDao<SYS_DETAILSERIAL> DaoDetailserial { get; set; }
        [Inject]
        public IDao<SYS_STAFFBUSI> DaoStaffbusi { get; set; }

        [UserAuth("SYS_STAFF_VIW")]
        public ActionResult Index(string nam, string orgid, string orgnam, int? stafftype, int pageIndex = 1,int pageSize=20)
        {
            ViewBag.NAM = nam;
            ViewBag.ORGID = orgid;
            ViewBag.ORGNAM = orgnam;
            ViewBag.STAFFTYP = stafftype;
            ViewBag.UserORG = new SelectList(UserState.UserOrgs.Where(obj => obj.ORG_LEVEL == 4)
               , "ORG_ID", "ORG_NAM", orgid);

            GetCreateDT(orgid);

            var orgs = UserState.UserOrgs.Select(obj => obj.ORG_ID);
            if (UserState.UserID == "admin")
            {
                orgs = DaoOrganize.FindList().Select(obj=>obj.ORG_ID);
            }
            if (!string.IsNullOrEmpty(orgid))
            {
                orgs = orgs.Where(obj => obj == orgid);
                if (null == orgs || orgs.Count() == 0)
                {
                    orgs = new List<string> { "-1" };
                }
            }
            var data = dao.GetList(pageIndex, pageSize, "", "STAFF_NAM like", nam
                , "ORG_ID in", orgs, "STAFF_TYP", stafftype);
            var orglist = DaoOrganize.FindList();
            foreach (var item in data.Items)
                item.ORG = orglist.FirstOrDefault(obj => obj.ORG_ID == item.ORG_ID);

            return View(data);
        }

        [HttpGet]
        [UserAuth("SYS_STAFF_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var detail = dao.GetEntity("STAFF_ID", id);
                detail.ORG = DaoOrganize.GetEntity("ORG_ID",detail.ORG_ID);
                return View(detail);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看信息出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_STAFF_ADD")]
        public ActionResult Create()
        {
            GetCreateDT();
            return View();
        }

        private void GetCreateDT(string org=null)
        {
            ViewBag.STAR_LEVELLIST = EnumHelper.GetCategorySelectList(typeof(SYS_STAFF.STAR_LEVEL_ENUM));  
            ViewBag.STAFF_TYPLIST = EnumHelper.GetCategorySelectList(typeof(SYS_STAFF.STAFF_TYP_ENUM));
            ViewBag.UserORG = new SelectList(UserState.UserOrgs.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM", org);
        }

        [HttpPost]
        [UserAuth("SYS_STAFF_ADD")]
        public ActionResult Create(SYS_STAFF info)
        {
            try
            {
                GetCreateDT();
                STAFFVALID(info);

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "新增出错！");
                    return JsonResult(false, "新增失败！", "SYS");
                }

                var rst = dao.AddObject(info);
                if (null != rst)
                {
                    return JsonResult(true, "新增成功！", "SYS");
                }
                else
                {
                    ModelState.AddModelError("", "新增失败！");
                    return JsonResult(false, "新增失败！", "SYS");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("新增出错。", ex);
                ModelState.AddModelError("", "新增出错。");
                return JsonResult(false, "新增失败！", "SYS");
            }
        }

        private void STAFFVALID(SYS_STAFF info,bool isEdt = false)
        {
            if (string.IsNullOrEmpty(info.STAFF_ID))
            {
                ModelState.AddModelError("STAFF_ID", "员工编号不能为空！");
            }
            else if (!isEdt && dao.GetEntity("STAFF_ID", info.STAFF_ID) != null)
            {
                ModelState.AddModelError("STAFF_ID", "员工编号已经存在！");
            }
            if (string.IsNullOrEmpty(info.STAFF_NAM))
            {
                ModelState.AddModelError("STAFF_NAM", "员工姓名不能为空！");
            }
            if (!isEdt && string.IsNullOrEmpty(info.STAFF_PASSWORD))
            {
                ModelState.AddModelError("STAFF_PASSWORD", "密码不能为空！");
            }
            if (!string.IsNullOrEmpty(info.STAFF_PASSWORD))
                info.STAFF_PASSWORD = CryptTools.Md5(info.STAFF_PASSWORD);
            else
            {
                if (!string.IsNullOrEmpty(info.STAFF_ID))
                {
                    var old = dao.GetEntity("STAFF_ID", info.STAFF_ID);
                    if (null != old)
                    {
                        info.STAFF_PASSWORD = old.STAFF_PASSWORD;
                        if (!ModelState.IsValidField("STAFF_PASSWORD"))
                            ModelState.Remove("STAFF_PASSWORD");
                    }
                        
                }
            }
            //if (null == info.STAR_LEVEL)
            //{
            //    ModelState.AddModelError("STAR_LEVEL", "星级不能为空！");
            //}
            //if (info.STAFF_TYP == null)
            //{
            //    ModelState.AddModelError("STAFF_TYP", "员工类型不能为空！");
            //}
            if (info.STAR_EVAL_TYP == null)
            {
                ModelState.AddModelError("STAR_EVAL_TYP", "请选择是否参与星级评定！");
            }
            if (string.IsNullOrEmpty(info.ORG_ID))
            {
                ModelState.AddModelError("ORG_ID", "组织机构不能为空！");
            }
            if (!string.IsNullOrEmpty(info.PHOTE_URL) && !string.IsNullOrEmpty(Request["isup"]))
            {
                string filepath = Server.MapPath(info.PHOTE_URL);
                if (!System.IO.File.Exists(filepath))
                {
                    ModelState.AddModelError("PHOTE_URL", "上传失败，请重新上传！");
                }
                string fileName = Path.GetFileName(filepath);//原始文件名称
                string fileExtension = Path.GetExtension(fileName); //文件扩展名
                string newfile = "/Uploads/StaffPhoto/"+ info.STAFF_ID+ fileExtension;
                //改名
                FileInfo fi = new FileInfo(filepath);
                LogHelper.ErrorLog(filepath);
                LogHelper.ErrorLog(newfile);
                if (System.IO.File.Exists(Server.MapPath("~/Uploads/StaffPhoto/") + info.STAFF_ID + fileExtension))
                    System.IO.File.Delete(Server.MapPath("~/Uploads/StaffPhoto/") + info.STAFF_ID + fileExtension);
                fi.MoveTo(Server.MapPath("~/Uploads/StaffPhoto/")+ info.STAFF_ID+ fileExtension);
                info.PHOTE_URL = newfile;
            }
        }

        [HttpGet]
        [UserAuth("SYS_STAFF_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                GetCreateDT();
                var hy = GetEdtDT(id);
                ViewBag.UserORG = new SelectList(UserState.UserOrgs.Where(obj => obj.ORG_LEVEL == 4)
                , "ORG_ID", "ORG_NAM",hy.ORG_ID);
                return View(hy);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错", ex);
                return Redirect("/Home/Error");
            }
        }

        private SYS_STAFF GetEdtDT(string id)
        {
            var info = dao.GetEntity("STAFF_ID", id);
            return info;
        }

        [HttpPost]
        [UserAuth("SYS_STAFF_EDT")]
        public ActionResult Edit(SYS_STAFF info, string id)
        {
            try
            {
                var hy = GetEdtDT(id);

                STAFFVALID(info,true);
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "修改出错！");
                    return JsonResult(false, "修改出错！", "SYS");
                }

                var rst = dao.UpdateObject(info, "STAFF_ID");
                if (rst > 0)
                {
                    return JsonResult(true, "修改成功！", "SYS");
                }
                else
                {
                    ModelState.AddModelError("", "修改失败！");
                    return JsonResult(false, "修改失败！", "SYS");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错！", ex);
                ModelState.AddModelError("", "修改失败！" + ex.Message);
                return JsonResult(false, "修改失败！", "SYS");
            }
        }

        [UserAuth("SYS_STAFF_DEL")]
        public ActionResult Delete(string id)
        {
            var deleteId = id.Split(',');
            try
            {
                foreach (var did in deleteId)
                {
                    dao.Delete("STAFF_ID", did);
                }
                
                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("SYS_STAFFBUSI_VIW")]
        public ActionResult StaffBusi(string id)
        {
            try
            {
                ViewBag.StaffID = id;
                ViewBag.DetailSerialList = DaoDetailserial.FindList("SYS_LRTIME desc");
                var staffbusi = DaoStaffbusi.FindList("","STAFF_ID",id);
                return View(staffbusi);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpPost]
        [UserAuth("SYS_STAFFBUSI_ADD,SYS_STAFFBUSI_EDT,SYS_STAFFBUSI_DEL")]
        public ActionResult StaffBusiSubmit()
        {
            try
            {
                List<SYS_STAFFBUSI> newstaffbusi = new List<SYS_STAFFBUSI>();
                string strSerID = Request.Form["SerID"];
                if (!string.IsNullOrEmpty(strSerID))
                {
                    var ArrSerID = strSerID.Split(',');
                    foreach (var _SerID in ArrSerID)
                    {
                        newstaffbusi.Add(new SYS_STAFFBUSI()
                        {
                            BUSI_CD = _SerID,
                            STAFF_ID = Request["STAFF_ID"]
                        });
                    }
                }
                var rst = new SYS_STAFF_DAL().AddStaffBusi(newstaffbusi, Request["STAFF_ID"]);
                if (rst)
                {
                    return JsonResult(true, "提交成功", "SYS");
                }
                return JsonResult(false,"提交失败","SYS");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错", ex);
                return JsonResult(false, "系统出错", "SYS");
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ImportStaffPHOTE(HttpPostedFileBase fileData)
        {
            try
            {
                if (fileData != null)
                {
                    try
                    {
                        // 文件上传后的保存路径
                        string filePath = Server.MapPath("~/Uploads/StaffPhoto/");
                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        string fileName = Path.GetFileName(fileData.FileName);// 原始文件名称
                        string fileExtension = Path.GetExtension(fileName); // 文件扩展名
                        string saveName = Guid.NewGuid() + fileExtension; // 保存文件名称

                        fileData.SaveAs(filePath + saveName);

                        return JsonResult(true, "/Uploads/StaffPhoto/" + saveName);
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
