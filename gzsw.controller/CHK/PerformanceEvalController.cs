 
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.model.dto;
using gzsw.util;
using gzsw.util.Enum;
using Ninject;
using PetaPoco;

namespace gzsw.controller.CHK
{
    /// <summary>
    ///  
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/2 15:59:26</para>
    /// </remark>
    public class PerformanceEvalController : BaseController<CHK_STAFF_QUALITY_MARK>
    {
        // 服务厅
        [Inject]
        public IDao<SYS_HALL> SYS_HALLBll { get; set; }
        
        /// <summary>
        /// 员工
        /// </summary>
        [Inject]
        public IDao<SYS_STAFF> SYS_STAFFBll { get; set; }
        
        // 事项大类
        [Inject]
        public IDao<SYS_DLSERIAL> SYS_DLSERIALBll { get; set; }

        /// <summary>
        /// 事项明细
        /// </summary>
        [Inject]
        public IDao<SYS_DETAILSERIAL> SYS_DETAILSERIALBll { get; set; }

        /// <summary>
        /// 个人考核质量差错打分 
        /// </summary>
        [Inject]
        public IDao<CHK_STAFF_QUALITY_MARK> CHK_STAFF_QUALITY_MARKBll { get; set; }

        /// <summary>
        /// 质量类型
        /// </summary>
        [Inject]
        public IDao<CHK_QUALITY_CON> CHK_QUALITY_CONBll { get; set; }


        [HttpGet]
        [UserAuth("CHK_STAFF_QUALITY_MARK_VIW")]
        public ActionResult Details(int id)
        {
            return View(new CHK_STAFF_QUALITY_MARK_DAL().Get(id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="personNo">员工编码</param>
        /// <param name="workNo">事项编码</param>
        /// <param name="performanceTypeNo">质量类型编码</param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index(
            string personNo,
            string workNo,
            string performanceTypeNo,
            int pageIndex = 1,
            int pageSize=20) 
        {
            ViewBag.personNo = personNo;
            ViewBag.workNo = workNo; 
            ViewBag.performanceTypeNo = performanceTypeNo;

            Page<CHK_STAFF_QUALITY_MARKDto> data = new CHK_STAFF_QUALITY_MARK_DAL().GetList(
                performanceTypeNo, 
                workNo,personNo,
                 pageIndex, 
                 pageSize);   
            return View(data);
        }


        [HttpGet]
        [UserAuth("CHK_STAFF_QUALITY_MARK_ADD")]
        public ActionResult Create()
        {
            LoadAllSelect();
            return View(new CHK_STAFF_QUALITY_MARK()
            {
                AMOUNT = 0
            });
        }



        [HttpPost]
        [UserAuth("CHK_STAFF_QUALITY_MARK_ADD")]
        public ActionResult Create(CHK_STAFF_QUALITY_MARK viewModel)
        { 
            try
            {
                if (string.IsNullOrEmpty(viewModel.STAFF_ID))
                {
                    LoadAllSelect();
                    Alter("员工编码不能为空！", AlterTypeEnum.Error, false, false);
                    return View();
                }
                if (string.IsNullOrEmpty(viewModel.SERIALID))
                {
                    LoadAllSelect();
                    Alter("事项编码不能为空！", AlterTypeEnum.Error, false, false);
                    return View();
                }
                if (string.IsNullOrEmpty(viewModel.QUALITY_CD))
                {
                    LoadAllSelect();
                    Alter("质量类型不能为空！", AlterTypeEnum.Error, false, false);
                    return View();
                }
                if (viewModel.OCCUR_DT == null || viewModel.OCCUR_DT== DateTime.MinValue)
                {
                    LoadAllSelect();
                    Alter("差错发生日期不能为空！", AlterTypeEnum.Error, false, false);
                    return View();
                }

                CHK_STAFF_QUALITY_MARKBll.AddObject(new CHK_STAFF_QUALITY_MARK()
                {
                    AMOUNT = viewModel.AMOUNT,
                    MODIFY_DTIME = DateTime.Now,
                    MODIFY_ID = UserState.UserID,
                    OCCUR_DT = viewModel.OCCUR_DT,
                    QUALITY_CD = viewModel.QUALITY_CD,
                    SEQ = viewModel.SEQ,
                    SERIALID = viewModel.SERIALID,
                    STAFF_ID = viewModel.STAFF_ID
                }); 
                LoadAllSelect();
                Alter("新增成功！", AlterTypeEnum.Success, true, true);
                return View(); 
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                ModelState.AddModelError("", "系统错误！");
                Alter("系统错误！", AlterTypeEnum.Error, false, false);
                return View();
            } 
        }

        
        [HttpGet]
        [UserAuth("CHK_STAFF_QUALITY_MARK_EDT")]
        public ActionResult Edit(int id)
        {
            if (id>0)
            {
                LoadAllSelect();
                var viewModel = new CHK_STAFF_QUALITY_MARK_DAL().Get(id);
                return View(viewModel);
            }
            return Redirect("/Home/Error");
        }



        [HttpPost]
        [UserAuth("CHK_STAFF_QUALITY_MARK_EDT")]
        public ActionResult Edit(CHK_STAFF_QUALITY_MARKDto viewModel)
        {
            try
            {
                if (string.IsNullOrEmpty(viewModel.STAFF_ID))
                { 
                    Alter("员工编码不能为空！", AlterTypeEnum.Error, false, false);
                    return Redirect(Url.Action("Edit", "PerformanceEval", new {id = viewModel.SEQ}));
                }
                if (string.IsNullOrEmpty(viewModel.SERIALID))
                {
                   
                    Alter("事项编码不能为空！", AlterTypeEnum.Error, false, false);
                    return Redirect(Url.Action("Edit", "PerformanceEval", new {id = viewModel.SEQ}));
                }
                if (string.IsNullOrEmpty(viewModel.QUALITY_CD))
                { 
                    Alter("质量类型不能为空！", AlterTypeEnum.Error, false, false);
                   return Redirect(Url.Action("Edit", "PerformanceEval", new {id = viewModel.SEQ}));
                }
                if (viewModel.OCCUR_DT == null || viewModel.OCCUR_DT == DateTime.MinValue)
                { 
                    Alter("差错发生日期不能为空！", AlterTypeEnum.Error, false, false);
                   return Redirect(Url.Action("Edit", "PerformanceEval", new {id = viewModel.SEQ}));
                }

                var obj = dao.GetEntity("SEQ", viewModel.SEQ);
                if (obj == null)
                {
                     Alter("未找到该数据！", AlterTypeEnum.Error, false, false);
                    return Redirect(Url.Action("Edit", "PerformanceEval", new {id = viewModel.SEQ}));
                }
                obj.AMOUNT = viewModel.AMOUNT;
                obj.MODIFY_DTIME = DateTime.Now;
                obj.MODIFY_ID = UserState.UserID;
                obj.OCCUR_DT = viewModel.OCCUR_DT;
                obj.QUALITY_CD = viewModel.QUALITY_CD;
                obj.SERIALID = viewModel.SERIALID;
                obj.STAFF_ID = viewModel.STAFF_ID;
                 
                CHK_STAFF_QUALITY_MARKBll.UpdateObject(obj); 
                Alter("修改成功！", AlterTypeEnum.Success, true, true);
                return Redirect(Url.Action("Edit", "PerformanceEval", new { id = viewModel.SEQ }));
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                ModelState.AddModelError("", "系统错误！");
                Alter("系统错误！", AlterTypeEnum.Error, false, false);
                return Redirect(Url.Action("Edit", "PerformanceEval", new { id = viewModel.SEQ }));
            } 
        }


        [UserAuth("CHK_STAFF_QUALITY_MARK_DEL")]
        public ActionResult Delete(string id)
        {
            var deleteId = id.Split(',');
            try
            { 
                new CHK_STAFF_QUALITY_MARK_DAL().Delete(deleteId);
                Alter("删除成功!", AlterTypeEnum.Success);
                return RedirectToAction("Index");
                
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除考核质量差错类型出错", ex);
                return Redirect("/Home/Error");
            }
        }
         
        /// <summary>
        /// 获取营业厅员工
        /// </summary>
        /// <param name="hallId">营业厅编码</param>
        /// <returns></returns>
        public ActionResult GetDropDownListForStaff(string hallId)
        {
           var list =   SYS_STAFFBll.FindList("", "ORG_ID", hallId);
           var result = new StringBuilder("<option selected=\"selected\" value=\"\">--请选择--</option>");
            foreach (var item in list)
            {
                result.AppendFormat(string.Format("<option   value=\"{0}\">{1}</option>", item.STAFF_ID,item.STAFF_NAM));
            }
            return Content(result.ToString());
        }

        /// <summary>
        /// 获取事项
        /// </summary>
        /// <param name="ssdlserialidId"></param>
        /// <returns></returns>
        public ActionResult GetDropDownListForSerialid(string ssdlserialidId)
        {
            var list = SYS_DETAILSERIALBll.FindList("", "SSDLSERIALID", ssdlserialidId);
            var result = new StringBuilder("<option selected=\"selected\" value=\"\">--请选择--</option>");
            foreach (var item in list)
            {
                result.AppendFormat(string.Format("<option   value=\"{0}\">{1}</option>", item.SERIALID, item.SERIALNAME));
            }
            return Content(result.ToString());
        }

        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <returns></returns>
        public ActionResult Excel()
        {
            return View();

        }


        #region Helper

        /// <summary>
        /// 加载营业厅/事项大类/质量类型
        /// </summary>
        private void LoadAllSelect()
        {
            LoadHALL();
            LoadDlserial();
            LoadQUALITYCon();
        }

        /// <summary>
        /// 加载服务厅
        /// </summary>
        private void LoadHALL()
        {
            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");
            ViewBag.Hall = new SelectList(orgs, "ORG_ID", "ORG_NAM");
        }

        /// <summary>
        /// 加载事项大类
        /// </summary>
        private void LoadDlserial()
        {
            var dlserial = SYS_DLSERIALBll.FindList();
            ViewBag.Dlserial = new SelectList(dlserial, "DLS_SERIALID", "DLS_SERIALNAME");
        }

        /// <summary>
        /// 加载质量类型
        /// </summary>
        private void LoadQUALITYCon()
        {
            var qualiyCon = CHK_QUALITY_CONBll.FindList();
            ViewBag.QUALITY_CD = new SelectList(qualiyCon, "QUALITY_CD", "QUALITY_NAM");
        }

        #endregion

        [HttpPost]
        public ActionResult ImportExcel(string filename)
        {
            try
            {


                var filepath = Request.MapPath(filename);
                var dt = NPOIHelper.ImportToDataTable(filename);
                //if (dt.Rows.Count > 15 || dt.Rows.Count < 10)
                //{
                //    return Json(new {success = -1, message = "文件模板错误"}, JsonRequestBehavior.AllowGet)
                //}
                string errormsg = "";
                for(int i=1;i<dt.Rows.Count;i++)
                {
                     if (string.IsNullOrEmpty(dt.Rows[i]["员工工号"].ToString()))
                     {
                         errormsg += dt.Rows[i]["序号"].ToString() + "员工工号不能为空;<br/>";
                         continue;
                     }
                if (string.IsNullOrEmpty(dt.Rows[i]["事项编码"].ToString()))
                {
                   
                     if (string.IsNullOrEmpty(dt.Rows[i]["事项编码"].ToString()))
                     {
                         errormsg += dt.Rows[i]["序号"].ToString() + "事项编码不能为空;<br/>";
                         continue;
                     }
                }
                if (string.IsNullOrEmpty(dt.Rows[i]["质量类型"].ToString()))
                {
                  
                       if (string.IsNullOrEmpty(dt.Rows[i]["质量类型"].ToString()))
                     {
                         errormsg += dt.Rows[i]["序号"].ToString() + "质量类型不能为空;<br/>";
                         continue;
                     }
                }
                if (string.IsNullOrEmpty(dt.Rows[i]["差错发生日期"].ToString()))
                {
                
                       if (string.IsNullOrEmpty(dt.Rows[i]["差错发生日期"].ToString()))
                     {
                         errormsg += dt.Rows[i]["序号"].ToString() + "差错发生日期不能为空;<br/>";
                         continue;
                     }
                }

                CHK_STAFF_QUALITY_MARKBll.AddObject(new CHK_STAFF_QUALITY_MARK()
                {
                    AMOUNT =Convert.ToInt32(dt.Rows[i]["数量"].ToString()),
                    MODIFY_DTIME = DateTime.Now,
                    MODIFY_ID = UserState.UserID,
                    OCCUR_DT =Convert.ToDateTime(dt.Rows[i]["差错发生日期"].ToString()),
                    QUALITY_CD = dt.Rows[i]["质量类型编码"].ToString(),
                   // SEQ = viewModel.SEQ,
                    SERIALID = dt.Rows[i]["员工编码"].ToString(),
                    STAFF_ID = dt.Rows[i]["事项编码"].ToString()
                }); 
              
                }
                Alter("删除成功!", AlterTypeEnum.Success);
                return Json(new {success = 0, message = "上传文件成功."}, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new {success = -1, message = "导入文件失败，请稍后再试"}, JsonRequestBehavior.AllowGet);

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

                        var dt = NPOIHelper.GetDataTable(filePath + saveName);
                        //if (dt.Rows.Count > 15 || dt.Rows.Count < 10)
                        //{
                        //    return Json(new {success = -1, message = "文件模板错误"}, JsonRequestBehavior.AllowGet)
                        //}
                        string errormsg = "";
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (string.IsNullOrEmpty(dt.Rows[i]["序号"].ToString()))
                            {
                                continue;
                            }
                            if (string.IsNullOrEmpty(dt.Rows[i]["员工工号"].ToString()))
                            {
                                errormsg += dt.Rows[i]["序号"].ToString() + "员工工号不能为空;<br/>";
                                continue;
                            }
                          
                                if (string.IsNullOrEmpty(dt.Rows[i]["业务编码"].ToString()))
                                {
                                    errormsg += dt.Rows[i]["序号"].ToString() + "业务编码不能为空;<br/>";
                                    continue;
                                }
                          
                            

                                if (string.IsNullOrEmpty(dt.Rows[i]["质量类型编码"].ToString()))
                                {
                                    errormsg += dt.Rows[i]["序号"].ToString() + "质量类型编码不能为空;<br/>";
                                    continue;
                                }



                            if (string.IsNullOrEmpty(dt.Rows[i]["差错日期"].ToString()))
                            {
                                errormsg += dt.Rows[i]["序号"].ToString() + "差错日期不能为空;<br/>";
                                continue;
                            }
                            else
                            {
                                DateTime time;
                                if (!DateTime.TryParse(dt.Rows[i]["差错日期"].ToString(), out time))
                                {
                                    errormsg += dt.Rows[i]["序号"].ToString() + "差错日期格式错误;<br/>";
                                    continue;

                                }
                            }
                            var amount = 0;
                            if (string.IsNullOrEmpty(dt.Rows[i]["数量"].ToString()))
                            {
                                errormsg += dt.Rows[i]["序号"].ToString() + "数量不能为空;<br/>";
                                continue;
                          
                               
                            }
                            else
                            {
                                if (!int.TryParse(dt.Rows[i]["数量"].ToString(), out amount))
                                {
                                    errormsg += dt.Rows[i]["序号"].ToString() + "数量格式错误;<br/>";
                                    continue;
                                }
                            }
                            CHK_STAFF_QUALITY_MARKBll.AddObject(new CHK_STAFF_QUALITY_MARK()
                            {
                                AMOUNT =amount,
                                MODIFY_DTIME = DateTime.Now,
                                MODIFY_ID = UserState.UserID,
                                OCCUR_DT = Convert.ToDateTime(dt.Rows[i]["差错日期"].ToString()),
                                QUALITY_CD = dt.Rows[i]["质量类型编码"].ToString(),
                                // SEQ = viewModel.SEQ,
                                SERIALID = dt.Rows[i]["业务编码"].ToString(),
                                STAFF_ID =dt.Rows[i]["员工工号"].ToString()
                            });

                        }
                        if (string.IsNullOrEmpty(errormsg))
                        {
                            return Json(new {success = 0, message = "上传文件成功."}, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json(new { success = 1, message =errormsg }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.ErrorLog("上传失败！", ex);
                        return Json(new { success = 1, message = "上传失败." }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = 1, message = "上传失败." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return Json(new { success = 1, message = "上传失败." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
