using System;
using System.Collections.Generic;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;

namespace gzsw.controller.SYS
{
    public class NSRHYController : BaseController<SYS_NSRHY>
    {
        [UserAuth("SYS_NSRHY_VIW")]
        public ActionResult Index(string hynam, int pageIndex = 1,int pageSize=20)
        {
            ViewBag.HYNAM = hynam;
            var data = dao.GetList(pageIndex, pageSize, "", "HY_NAME like", hynam);
            return View(data);
        }

        [HttpGet]
        [UserAuth("SYS_NSRHY_VIW")]
        public ActionResult Details(int id)
        {
            try
            {
                var qserial = dao.GetEntity("HY_ID", id);

                return View(qserial);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看纳税人行业信息出错", ex);
                return RedirectToAction("Error","Home");
            }
        }

        [HttpGet]
        [UserAuth("SYS_NSRHY_ADD")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [UserAuth("SYS_NSRHY_ADD")]
        public ActionResult Create(SYS_NSRHY nsrhy)
        {
            try
            {
                ChkValid(nsrhy);
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var rst = dao.AddObject(nsrhy);
                if (null != rst)
                {
                    Alter("新增成功！", util.Enum.AlterTypeEnum.Success, true, true);
                    return View();
                }
                else
                {
                    Alter("新增失败！", util.Enum.AlterTypeEnum.Error, false, false);
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误。", ex);
                Alter("系统错误！", util.Enum.AlterTypeEnum.Error, false, false);
                return View();
            }
        }

        private void ChkValid(SYS_NSRHY nsrhy,bool isEdit = false)
        {
            if (string.IsNullOrEmpty(nsrhy.HY_NAME))
            {
                ModelState.AddModelError("HY_NAME", "行业名称不能为空！");
            }
            else
            {
              var exist=  dao.GetEntity("HY_NAME", nsrhy.HY_NAME);
              if (!isEdit)
              {
                  if (exist != null)
                      ModelState.AddModelError("HY_NAME", "行业名称已经存在！");
              }
              else
              {
                  if (exist != null && exist.HY_ID != nsrhy.HY_ID)
                  {
                      ModelState.AddModelError("HY_NAME", "行业名称已经存在！");
                  }
              }
            }
        }

        [HttpGet]
        [UserAuth("SYS_NSRHY_EDT")]
        public ActionResult Edit(int id)
        {
            try
            {
                var hy = GetEdtDT(id);

                return View(hy);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改行业信息出错", ex);
                return Redirect("/Home/Error");
            }
        }

        private SYS_NSRHY GetEdtDT(int id)
        {
            var detailSerial = dao.GetEntity("HY_ID", id);
            return detailSerial;
        }

        [HttpPost]
        [UserAuth("SYS_NSRHY_EDT")]
        public ActionResult Edit(SYS_NSRHY nsrhy, int id)
        {
            try
            {
                var hy = GetEdtDT(id);

                ChkValid(nsrhy,true);
                if (!ModelState.IsValid)
                {
                    return View(nsrhy);
                }

                var rst = dao.UpdateObject(nsrhy, "HY_ID");
                if (rst > 0)
                {
                    Alter("修改成功！", util.Enum.AlterTypeEnum.Success, true, true);
                    return View(nsrhy);
                }
                else
                {
                    Alter("修改失败！", util.Enum.AlterTypeEnum.Error, false, false);
                    return View(nsrhy);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                Alter("系统错误！", util.Enum.AlterTypeEnum.Error, false, false);
                return View(nsrhy);
            }
        }

        [UserAuth("SYS_NSRHY_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    dao.Delete("HY_ID", int.Parse(_id));
                }
                
                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除行业信息出错", ex);
                return Redirect("/Home/Error");
            }
        }
    }
}
