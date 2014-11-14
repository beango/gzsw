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

namespace gzsw.controller.SYS
{
    /// <summary>
    /// 业务业务明细管理
    /// </summary>
    public class DetailSerialController : BaseController<SYS_DETAILSERIAL>
    {
        [Ninject.Inject]
        public IDao<SYS_DETAILSERIAL> DaoDetailserial { get; set; }
        [Ninject.Inject]
        public IDao<SYS_DLSERIAL> DaoDlserial { get; set; }
        [Ninject.Inject]
        public IDao<SYS_QUEUESERIAL> DaoQueueserial { get; set; }

        [UserAuth("SYS_DETAILSERIAL_VIW")]
        public ActionResult Index(string sid, string snam, string qid, string dlid, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.SID = sid;
            ViewBag.SNAM = snam;

            var dlSerialList = DaoDlserial.FindList();
            var queSerialList = DaoQueueserial.FindList();

            Page<SYS_DETAILSERIAL> data = dao.GetList(pageIndex, pageSize, "", "SERIALID", sid, "SERIALNAME like", snam, "SSQUEUESERIALID", qid
                , "SSDLSERIALID", dlid);
            foreach (var item in data.Items)
            {
                item.SYS_QUEUESERIAL = queSerialList.SingleOrDefault(obj => obj.Q_SERIALID == item.SSQUEUESERIALID);
                item.SYS_DLSERIAL = dlSerialList.SingleOrDefault(obj => obj.DLS_SERIALID == item.SSDLSERIALID);
            }

            var list1 = new SelectList(DaoDlserial.FindList(), "DLS_SERIALID", "DLS_SERIALNAME", dlid);
            ViewData["DLSERIAL"] = list1;

            var list2 = new SelectList(DaoQueueserial.FindList(), "Q_SERIALID", "Q_SERIALNAME", qid);
            ViewData["QUSERIAL"] = list2;

            return View(data);
        }

        [HttpGet]
        [UserAuth("SYS_DETAILSERIAL_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var qserial = DaoDetailserial.GetEntity("SERIALID", id);
                qserial.SYS_QUEUESERIAL = DaoQueueserial.GetEntity("Q_SERIALID", qserial.SSQUEUESERIALID);
                qserial.SYS_DLSERIAL = DaoDlserial.GetEntity("DLS_SERIALID", qserial.SSDLSERIALID);

                return View(qserial);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_DETAILSERIAL_ADD")]
        public ActionResult Create()
        {
            GetCreateDT();
            return View();
        }

        private void GetCreateDT()
        {
            ViewData["DLSERIAL"] = new SelectList(DaoDlserial.FindList(), "DLS_SERIALID", "DLS_SERIALNAME");
            ViewData["QUSERIAL"] = new SelectList(DaoQueueserial.FindList(), "Q_SERIALID", "Q_SERIALNAME");
        }

        [HttpPost]
        [UserAuth("SYS_DETAILSERIAL_ADD")]
        public ActionResult Create(SYS_DETAILSERIAL detailSerial)
        {
            try
            {
                GetCreateDT();
                CHKValid(detailSerial);
                if (!ModelState.IsValid)
                {
                    return View();
                }
                detailSerial.SYS_LRTIME = DateTime.Now;
                detailSerial.SYS_LRUSER = UserState.UserID;
                var rst = DaoDetailserial.AddObject(detailSerial);
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
                LogHelper.ErrorLog("系统错误！", ex);
                return View();
            }
        }

        private void CHKValid(SYS_DETAILSERIAL detailSerial, bool isEdit = false)
        {
            if (string.IsNullOrEmpty(detailSerial.SERIALID))
            {
                ModelState.AddModelError("SERIALID", "事项编码不能为空！");
            }
            else if (!isEdit && DaoDetailserial.GetEntity("SERIALID", detailSerial.SERIALID) != null)
            {
                ModelState.AddModelError("SERIALID", "事项编码已经存在！");
            }
            if (string.IsNullOrEmpty(detailSerial.SERIALNAME))
            {
                ModelState.AddModelError("SERIALNAME", "业务名称不能为空！");
            }
            else
            {
                SYS_DETAILSERIAL exist = DaoDetailserial.GetEntity("SERIALNAME", detailSerial.SERIALNAME);
                if (!isEdit)
                {
                    if (exist != null)
                        ModelState.AddModelError("SERIALNAME", "业务名称已经存在！");
                }
                else
                {
                    if (exist != null && exist.SERIALID != detailSerial.SERIALID)
                    {
                        ModelState.AddModelError("SERIALNAME", "业务名称已经存在！");
                    }
                }
            }
            if (string.IsNullOrEmpty(detailSerial.SSQUEUESERIALID))
            {
                ModelState.AddModelError("SSQUEUESERIALID", "所属排队队列不能为空！");
            }
            if (string.IsNullOrEmpty(detailSerial.SSDLSERIALID))
            {
                ModelState.AddModelError("SSDLSERIALID", "所属大类业务不能为空！");
            }
        }
        [HttpGet]
        [UserAuth("SYS_DETAILSERIAL_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var detailSerial = GetEdtModel(id);

                return View(detailSerial);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return Redirect("/Home/Error");
            }
        }

        private SYS_DETAILSERIAL GetEdtModel(string id)
        {
            try
            {
                var detailSerial = DaoDetailserial.GetEntity("SERIALID", id);

                var list1 = new SelectList(DaoDlserial.FindList(), "DLS_SERIALID", "DLS_SERIALNAME", detailSerial.SSDLSERIALID);
                ViewData["DLSERIAL"] = list1;

                var list2 = new SelectList(DaoQueueserial.FindList(), "Q_SERIALID", "Q_SERIALNAME", detailSerial.SSQUEUESERIALID);
                ViewData["QUSERIAL"] = list2;
                return detailSerial;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return null;
            }
        }

        [HttpPost]
        [UserAuth("SYS_DETAILSERIAL_EDT")]
        public ActionResult Edit(SYS_DETAILSERIAL detailSerial, string id)
        {
            var extdetailSerial = GetEdtModel(id);
            try
            {
                CHKValid(detailSerial, true);
                if (!ModelState.IsValid)
                {
                    return View(extdetailSerial);
                }
                detailSerial.SYS_LASTTIME = DateTime.Now;
                detailSerial.SYS_LASTUSER = UserState.UserID;
                var rst = dao.UpdateObject(detailSerial);
                if (rst > 0)
                {
                    Alter("修改成功！", util.Enum.AlterTypeEnum.Success, true, true);
                    return View(extdetailSerial);
                }
                else
                {
                    return View(extdetailSerial);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                Alter("系统出错！", util.Enum.AlterTypeEnum.Success, false, false);
                return View(extdetailSerial);
            }
        }

        [UserAuth("SYS_DETAILSERIAL_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    DaoDetailserial.DeleteObject(DaoDetailserial.GetEntity("SERIALID", _id));
                }
                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return Redirect("/Home/Error");
            }
        }
    }
}
