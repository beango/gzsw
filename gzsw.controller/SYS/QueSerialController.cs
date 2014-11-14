using System;
using System.Collections.Generic;
using System.Linq;
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
    /// 排队业务管理
    /// </summary>
    public class QueSerialController : BaseController<SYS_QUEUESERIAL>
    {
        [Ninject.Inject]
        public IDao<SYS_QUEUESERIAL> DaoQueueserial { get; set; }

        [UserAuth("SYS_QUEUESERIAL_VIW")]
        public ActionResult Index(string qid, string qnam, int pageIndex = 1, int pageSize=20)
        {
            ViewBag.QID = qid;
            ViewBag.QNAM = qnam;
            Page<SYS_QUEUESERIAL> data = dao.GetList(pageIndex, pageSize, "", "Q_SERIALID", qid, "Q_SERIALNAME like", qnam);

            return View(data);
        }

        [HttpGet]
        [UserAuth("SYS_QUEUESERIAL_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var qserial = DaoQueueserial.GetEntity("Q_SERIALID", id);

                return View(qserial);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_QUEUESERIAL_ADD")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [UserAuth("SYS_QUEUESERIAL_ADD")]
        public ActionResult Create(SYS_QUEUESERIAL quSerial)
        {
            try
            {
                if (string.IsNullOrEmpty(quSerial.Q_SERIALID))
                {
                    ModelState.AddModelError("Q_SERIALID", "编码不能为空！");
                }
                else
                {
                    if (quSerial.Q_SERIALID.Length != 3)
                    {
                        ModelState.AddModelError("Q_SERIALID", "长度必须是3位！");
                    }
                    if (DaoQueueserial.GetEntity("Q_SERIALID", quSerial.Q_SERIALID) != null)
                    {
                        ModelState.AddModelError("Q_SERIALID", "编码已经存在！");
                    }
                }

                if (string.IsNullOrEmpty(quSerial.Q_SERIALNAME))
                {
                    ModelState.AddModelError("Q_SERIALNAME", "名称不能为空！");
                }
                if (!ModelState.IsValid)
                {
                    return View();
                }

                quSerial.SYS_LRTIME = DateTime.Now;
                quSerial.SYS_LRUSER = UserState.UserID;
                var rst = DaoQueueserial.AddObject(quSerial);
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
                ModelState.AddModelError("", "系统错误！");
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("SYS_QUEUESERIAL_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                var role = DaoQueueserial.GetEntity("Q_SERIALID", id);
                return View(role);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpPost]
        [UserAuth("SYS_QUEUESERIAL_EDT")]
        public ActionResult Edit(SYS_QUEUESERIAL quSerial)
        {
            try
            {
                if (string.IsNullOrEmpty(quSerial.Q_SERIALNAME))
                {
                    ModelState.AddModelError("Q_SERIALNAME", "队列名称不能为空！");
                }

                if (!ModelState.IsValid)
                {
                    return View(quSerial);
                }

                quSerial.SYS_LASTTIME = DateTime.Now;
                quSerial.SYS_LASTUSER = UserState.UserID;
                var rst = dao.UpdateObject(quSerial, "Q_SERIALID");
                if (rst > 0)
                {
                    Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                    return View(quSerial);
                }
                else
                {
                    return View(quSerial);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错！", ex);
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("SYS_QUEUESERIAL_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new []{','},StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    DaoQueueserial.DeleteObject(DaoQueueserial.GetEntity("Q_SERIALID", _id));
                }
                
                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            }
        }

        #region 树
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        [UserAuth("SYS_QUEUESERIAL_VIW")]
        public ActionResult GetQueSerialTree(string check, int? disabled, string searchNam)
        {
            var all = dao.FindList();
            var tree = new ZtreeNode
            {
                id = "",
                name = "顶点",
                @checked = check == null,
                open = true,
                isParent = true,
                nocheck = true,
                highlight = (!string.IsNullOrEmpty(searchNam) && "顶点".IndexOf(searchNam) > -1),
                children = GetQueSerialTree(all, null, check, disabled, searchNam)
            };

            if (!string.IsNullOrEmpty(searchNam))
            {
                GenQueSerialOpen(tree);
            }
            return Json(tree, JsonRequestBehavior.AllowGet);
        }

        private List<ZtreeNode> GetQueSerialTree(IEnumerable<SYS_QUEUESERIAL> all, string parid, string check, int? disabled, string searchNam)
        {
            return all.Select(obj => new ZtreeNode
                {
                    id = obj.Q_SERIALID,
                    name = obj.Q_SERIALNAME,
                    @checked = (check != null && ("-" + check + "-").IndexOf("-" + obj.Q_SERIALID + "-")>-1),
                    open = false,
                    isParent = false,
                    highlight = (!string.IsNullOrEmpty(searchNam) && obj.Q_SERIALNAME.IndexOf(searchNam) > -1)
                }).ToList();
        }
        private void GenQueSerialOpen(ZtreeNode tree)
        {
            if (tree.children != null && tree.children.Any(obj => obj.highlight))
            {
                tree.open = true;
            }
            foreach (var item in tree.children)
            {
                GenQueSerialOpen(item);
            }
        }
        #endregion

    }
}
