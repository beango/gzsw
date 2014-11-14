 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using gzsw.util.Enum;
using Ninject;
using PetaPoco;

namespace gzsw.controller.CHK
{
    /// <summary>
    ///  绩效考核控制器
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/2 11:23:32</para>
    /// </remark>
    public class PerformanceController : BaseController<CHK_QUALITY_CON>
    {
        [Inject]
        public IDao<CHK_STAFF_QUALITY_MARK> QualityDao { get; set; }

         [Inject]
        public IDao<SYS_USER> SysUserDao { get; set; }

        /// <summary>
        /// 质量考核列表
        /// </summary>
        /// <param name="performanceCode">质量考核编码</param>
        /// <param name="performanceName">质量考核名称</param>
        /// <param name="pageIndex">开始页码</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        [UserAuth("CHK_QUALITY_CON_VIW")]
        public ActionResult Index(
            string performanceCode = "",
            string performanceName = "",
            int pageIndex = 1,
            int pageSize=20) 
        {
            ViewBag.performanceCode = performanceCode;
            ViewBag.performanceName = performanceName; 
            Page<CHK_QUALITY_CON> data = dao.GetList(
               pageIndex,
               pageSize,
               "MODIFY_DTIME desc",
               "QUALITY_CD like", performanceCode,
               "QUALITY_NAM like", performanceName);
            return View(data);
        }

        [HttpGet]
        [UserAuth("CHK_QUALITY_CON_VIW")]
        public ActionResult Details(string id)
        {
            var performance = dao.GetEntity("QUALITY_CD", id);
            var createUser = SysUserDao.GetEntity("USER_ID", performance.MODIFY_ID);
            if (createUser != null)
            {
                ViewData["UserName"] = createUser.USER_NAM;
            }
            return View(performance);
        }

        [HttpGet]
        [UserAuth("CHK_QUALITY_CON_ADD")]
        public ActionResult Create()
        {
            return View(new CHK_QUALITY_CON()
            {
                 DEDUCT = 0
            });
        }
        
        [HttpPost]
        [UserAuth("CHK_QUALITY_CON_Add")]
        public ActionResult Create(CHK_QUALITY_CON viewModel)
        {
            try
            {
                if (string.IsNullOrEmpty(viewModel.QUALITY_CD))
                {
                    ModelState.AddModelError("QUALITY_CD", "质量类型编码不能为空！");
                    return View(new CHK_QUALITY_CON()
                    {
                        DEDUCT = 0
                    }); 
                }

                if (string.IsNullOrEmpty(viewModel.QUALITY_NAM))
                {
                    ModelState.AddModelError("QUALITY_NAM", "质量类型名称不能为空！");
                    return View(new CHK_QUALITY_CON()
                    {
                        DEDUCT = 0
                    }); 
                }
                 
                if (dao.FindList("", "QUALITY_CD", viewModel.QUALITY_CD.Trim()).Any())
                {
                    ModelState.AddModelError("QUALITY_CD", "已经存在该质量类型编码！");
                    return View(new CHK_QUALITY_CON()
                    {
                        DEDUCT = 0
                    }); 
                }

                dao.AddObject(new CHK_QUALITY_CON()
                {
                    DEDUCT = viewModel.DEDUCT,
                    QUALITY_CD=viewModel.QUALITY_CD,
                    MODIFY_DTIME =DateTime.Now,
                    MODIFY_ID = UserState.UserID,
                    QUALITY_NAM = viewModel.QUALITY_NAM
                });
                Alter("新增成功！", AlterTypeEnum.Success, true, true);
                return View(new CHK_QUALITY_CON()
                {
                    DEDUCT = 0
                }); 
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
        [UserAuth("CHK_QUALITY_CON_EDT")]
        public ActionResult Edit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = dao.GetEntity("QUALITY_CD", id);
                return View(user);
            }
            return Redirect("/Home/Error");
        }

        [HttpPost]
        [UserAuth("CHK_QUALITY_CON_EDT")]
        public ActionResult Edit(CHK_QUALITY_CON viewModel,string id)
        {
            try
            {
                if (string.IsNullOrEmpty(viewModel.QUALITY_CD))
                {
                    ModelState.AddModelError("QUALITY_CD", "质量类型编码不能为空！");
                    return View(new CHK_QUALITY_CON()
                    {
                        DEDUCT = 0
                    });
                }

                if (string.IsNullOrEmpty(viewModel.QUALITY_NAM))
                {
                    ModelState.AddModelError("QUALITY_NAM", "质量类型名称不能为空！");
                    return View(new CHK_QUALITY_CON()
                    {
                        DEDUCT = 0
                    });
                }

                var haveNodeList = dao.FindList("", "QUALITY_CD", viewModel.QUALITY_CD.Trim()).Any(x => x.QUALITY_CD!=viewModel.QUALITY_CD);
                if (haveNodeList)
                {
                    ModelState.AddModelError("QUALITY_CD", "已经存在该质量类型编码！");
                    return View(new CHK_QUALITY_CON()
                    {
                        DEDUCT = 0
                    });
                } 
                var model = dao.GetEntity("QUALITY_CD", id);
                model.QUALITY_NAM = viewModel.QUALITY_NAM;
                model.DEDUCT = viewModel.DEDUCT;
                model.MODIFY_DTIME = DateTime.Now;
                model.MODIFY_ID = UserState.UserID; 
                var rst = dao.UpdateObject(model);
                if (rst > 0)
                {
                    Alter("修改成功！", AlterTypeEnum.Success, true, true);
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("", "修改失败！");
                    Alter("修改失败！", AlterTypeEnum.Error, false, false);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                ModelState.AddModelError("", "系统错误！");
                Alter("系统错误！", AlterTypeEnum.Error, false, false);
                return View(viewModel);
            }
        }



        [UserAuth("CHK_QUALITY_CON_DEL")]
        public ActionResult Delete(string id)
        {
            var deleteId = id.Split(',');
            try
            {
                var rtconBll = new CHK_STAFF_QUALITY_MARK_DAL();
                if (rtconBll.IsHaveForType(id))
                {
                    Alter("删除失败，原因：考核质量差错类型下存在个人考核质量差错打分数据，不允许删除！", AlterTypeEnum.Warning, false, true); 
                    return RedirectToAction("Index");
                }
                else
                {
                    var bll = new CHK_QUALITY_CON_DAL();
                    bll.Delete(deleteId);
                    Alter("删除成功!", AlterTypeEnum.Success);
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除考核质量差错类型出错", ex);
                return Redirect("/Home/Error");
            }
        }

    }
}
