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
using PetaPoco;
using gzsw.util.Enum;

namespace gzsw.controller.SYS
{
    public class DLSerialController : BaseController<SYS_DLSERIAL>
    {
        [Ninject.Inject]
        public IDao<SYS_DLSERIAL> DaoDlserial { get; set; }

        [UserAuth("SYS_DLSERIAL_VIW")]
        public ActionResult Index(string dlid, string dlnam, int pageIndex = 1,int pageSize=20)
        {
            ViewBag.DLID = dlid;
            ViewBag.DLNAM = dlnam;
            Page<SYS_DLSERIAL> data = dao.GetList(pageIndex, pageSize, "", "DLS_SERIALID", dlid, "DLS_SERIALNAME like", dlnam);
            return View(data);
        }

        [HttpGet]
        [UserAuth("SYS_DLSERIAL_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var qserial = DaoDlserial.GetEntity("DLS_SERIALID",id);

                return View(qserial);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看业务大类出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_DLSERIAL_ADD")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [UserAuth("SYS_DLSERIAL_ADD")]
        public ActionResult Create(SYS_DLSERIAL dlSerial)
        {
            try
            {
                if (string.IsNullOrEmpty(dlSerial.DLS_SERIALID))
                {
                    ModelState.AddModelError("DLS_SERIALID", "编码不能为空！");
                }
                else
                {
                    if (DaoDlserial.GetEntity("DLS_SERIALID", dlSerial.DLS_SERIALID) != null)
                    {
                        ModelState.AddModelError("DLS_SERIALID", "编码已经存在！");
                    }
                }
                if (string.IsNullOrEmpty(dlSerial.DLS_SERIALNAME))
                {
                    ModelState.AddModelError("DLS_SERIALNAME", "名称不能为空！");
                }
                if (!ModelState.IsValid)
                {
                    return View();
                }
                dlSerial.SYS_LRTIME = DateTime.Now;
                dlSerial.SYS_LRUSER = UserState.UserID;
                var rst = DaoDlserial.AddObject(dlSerial);
                if (null != rst)
                {
                    Alter("新增成功！", util.Enum.AlterTypeEnum.Success, false, true);
                    return View();
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_DLSERIAL_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var role = DaoDlserial.GetEntity("DLS_SERIALID", id);
                return View(role);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改业务大类出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpPost]
        [UserAuth("SYS_DLSERIAL_EDT")]
        public ActionResult Edit(SYS_DLSERIAL dlSerial,string id)
        {
            try
            {
                var role = DaoDlserial.GetEntity("DLS_SERIALID", id);
                if (string.IsNullOrEmpty(dlSerial.DLS_SERIALNAME))
                {
                    ModelState.AddModelError("DLS_SERIALNAME", "名称不能为空！");
                    return View(role);
                }
                if (!ModelState.IsValid)
                {
                    return View(role);
                }
                dlSerial.SYS_LASTTIME = DateTime.Now;
                dlSerial.SYS_LASTUSER = UserState.UserID;
                var rst = dao.UpdateObject(dlSerial, "DLS_SERIALID");
                if (rst > 0)
                {
                    Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                    return View(role);
                }
                else
                {
                    return View(role);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改业务大类出错！", ex);
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("SYS_DLSERIAL_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    DaoDlserial.Delete("DLS_SERIALID", _id);
                }
                
                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除业务大类出错", ex);
                return Redirect("/Home/Error");
            }
        }
    }
}
