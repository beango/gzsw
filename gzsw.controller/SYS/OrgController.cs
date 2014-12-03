using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using System.Web.Mvc;
using PetaPoco;
using gzsw.util;
using gzsw.dal.dao;

namespace gzsw.controller.SYS
{
    public class OrgController : BaseController<SYS_ORGANIZE>
    {
        [Ninject.Inject]
        public IDao<SYS_ORGANIZE> DaoOrg { get; set; }
        [Ninject.Inject]
        public IDao<SYS_HALL> DaoHall { get; set; }

        [UserAuth("SYS_ORG_VIW")]
        public ActionResult Index(string orgnam, int page = 1)
        {
            ViewBag.ORGNAM = orgnam;
            return View();
        }

        [HttpGet]
        [UserAuth("SYS_ORG_ADD")]
        public ActionResult Create()
        {
            var selectlist = EnumHelper.GetCategorySelectList(typeof(SYS_ORGANIZE.ORG_LEVEL_ENUM));
            if (Request["orglev"] != null && selectlist != null)
            {
                selectlist.FirstOrDefault(obj => obj.Value == Request["orglev"]).Selected = true;
            }
            ViewBag.ORG_LEVELLIST = selectlist;
            TempData["ORGALL"] = DaoOrg.FindList();
            return View();
        }

        [HttpGet]
        [UserAuth("SYS_ORG_VIW")]
        public ActionResult Details(string id)
        {
            try
            {
                var orgall = DaoOrg.FindList();
                var org = DaoOrg.GetEntity("ORG_ID", id);
                if (null != org)
                    org.Par_OrgList = orgall.Where(obj => obj.ORG_ID == org.PAR_ORG_ID).ToList();
                return View(org);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看组织架构出错", ex);
                return Json(new { Result = false, Text = "系统出错！" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [UserAuth("SYS_ORG_ADD")]
        public ActionResult Create(SYS_ORGANIZE org)
        {
            try
            {
                CreateValid(org);
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "新增出错！");
                    return JsonResult(false, "新增出错！", "SYS", "/", false);
                }

                string rst = null;
                if (org.ORG_LEVEL == 4)
                {
                    SYS_HALL hal = new SYS_HALL()
                    {
                        HALL_NO = org.ORG_ID,
                        HALL_NAM = org.ORG_NAM,
                        ADDRESS = "",
                        LONGITUDE = org.LONGITUDE,
                        DIMENSION = org.DIMENSION,
                        CREATE_DTIME = DateTime.Now,
                        CREATE_ID = UserState.UserID,
                        HEAD = org.HEAD,
                        HEAD_TEL = org.HEAD_TEL,
                        ORG_ID = org.ORG_ID
                    };
                    rst = new SYS_ORGANIZE_DAL().Add(org, hal);
                }
                else
                {
                    DaoOrg.AddObject(org);
                    rst = org.ORG_ID;
                }

                if (!string.IsNullOrEmpty(rst))
                {
                    return JsonResult(true, "新增成功！", "SYS", "/", false);
                }
                else
                {
                    ModelState.AddModelError("", "新增出错！");
                    return JsonResult(false, "新增出错！", "SYS", "/", false);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("新增出错", ex);
                ModelState.AddModelError("", "新增出错！");
                return JsonResult(false, "新增出错！", "SYS", "/", false);
            }
        }

        private void CreateValid(SYS_ORGANIZE org, bool isEdit = false)
        {
            if (string.IsNullOrEmpty(org.ORG_ID))
            {
                ModelState.AddModelError("ORG_ID", "组织机构编码不能为空！");
            }
            if (!isEdit && !string.IsNullOrEmpty(org.ORG_ID) && DaoOrg.GetEntity("ORG_ID", org.ORG_ID) != null)
            {
                ModelState.AddModelError("ORG_ID", "组织机构编码已经存在！");
            }
            if (string.IsNullOrEmpty(org.ORG_NAM))
            {
                ModelState.AddModelError("ORG_NAM", "组织机构名称不能为空！");
            }
            if (string.IsNullOrEmpty(org.HEAD))
            {
                ModelState.AddModelError("HEAD", "负责人不能为空！");
            }
            if (string.IsNullOrEmpty(org.HEAD_TEL))
            {
                ModelState.AddModelError("HEAD_TEL", "负责人电话不能为空！");
            }
            if (string.IsNullOrEmpty(org.LONGITUDE))
            {
                ModelState.AddModelError("LONGITUDE", "经度不能为空！");
            }
            if (string.IsNullOrEmpty(org.DIMENSION))
            {
                ModelState.AddModelError("DIMENSION", "纬度不能为空！");
            }
            if (string.IsNullOrEmpty(org.DUTY_TEL))
            {
                ModelState.AddModelError("DUTY_TEL", "值班领导手机不能为空！");
            }
            if (org.ORG_LEVEL == 0)
            {
                ModelState.AddModelError("ORG_LEVEL", "组织机构级别不能为空！");
            }
            if (org.PAR_ORG_ID == null)
                org.PAR_ORG_ID = "";
        }

        [UserAuth("SYS_ORG_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var exists = DaoHall.GetEntity("ORG_ID", id);
                if (exists != null)
                    return JsonResult(false, "该机构还有服务大厅，不允许删除！", "SYS", "/", false);
                new SYS_ORGANIZE_DAL().DeleteAndChildren(id);
                return JsonResult(true, "删除成功！", "SYS", "/", false);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return JsonResult(false, "删除出错！", "SYS", "/", false);
            }
        }

        [HttpGet]
        [UserAuth("SYS_ORG_EDT")]
        public ActionResult Edit(string id)
        {
            try
            {
                ViewBag.ORG_LEVELLIST = EnumHelper.GetCategorySelectList(typeof(SYS_ORGANIZE.ORG_LEVEL_ENUM));
                TempData["ORGALL"] = DaoOrg.FindList();
                var role = DaoOrg.GetEntity("ORG_ID", id);
                return View(role);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改组织架构出错", ex);
                return Json(new { result = false, desc = "系统错误，请重试" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [UserAuth("SYS_ORG_EDT")]
        public ActionResult Edit(SYS_ORGANIZE org)
        {
            try
            {
                CreateValid(org, true);
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "修改出错！");
                    return JsonResult(false, "修改出错！", "SYS");
                }

                var rst = dao.UpdateObject(org, "ORG_ID");
                var hall = DaoHall.GetEntity("HALL_NO", org.ORG_ID);
                if (null != hall)
                {
                    hall.HALL_NAM = org.ORG_NAM;
                    DaoHall.UpdateObject(hall, "HALL_NO", hall.HALL_NO);
                }
                if (rst > 0)
                {
                    return JsonResult(true, "修改成功！", "SYS", "/", false);
                }
                else
                {
                    ModelState.AddModelError("", "修改失败！");
                    return JsonResult(false, "修改失败！", "SYS");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改组织架构出错！", ex);
                ModelState.AddModelError("", "修改组织机构失败！" + ex.Message);
                return JsonResult(false, "修改失败！", "SYS");
            }
        }

        #region 组织结构树-组件
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        //[UserAuth("AUTH_FUNC_VIW")]
        public ActionResult GetOrgsTree(string id, string check, int? disabled, string searchNam)
        {
            var all = dao.FindList();
            foreach (var org in all.Where(obj => obj.PAR_ORG_ID == ""))
            {
                org.PAR_ORG_ID = null;
            }
            var tree = new ZtreeNode_ORG
            {
                id = "",
                name = "顶级机构",
                @checked = check == null,
                open = true,
                isParent = true,
                nocheck = true,
                highlight = (!string.IsNullOrEmpty(searchNam) && "顶级机构".IndexOf(searchNam) > -1),
                children = GenOrgsTree(all, null, check, disabled, searchNam)
            };
            var treelist = new List<ZtreeNode_ORG> { tree };
            if (!string.IsNullOrEmpty(id) || Request.Form["lv"] == "0")
            {
                if (id == "")
                    id = null;
                treelist = GenOrgsTree(all, id, check, disabled, searchNam);
            }
            if (!string.IsNullOrEmpty(searchNam))
            {
                GenOrgsTreeOpen(tree);
                GenOrgsTreeOpen(tree);//递归达不到足够的深度，改为多次调用
                GenOrgsTreeOpen(tree);
            }
            return Json(treelist, JsonRequestBehavior.AllowGet);
        }
        public List<ZtreeNode_ORG> GenOrgsTree(IEnumerable<SYS_ORGANIZE> all, string parid, string check, int? disabled, string searchNam)
        {
            return all.Where(obj => obj.PAR_ORG_ID == parid)
                .Select(obj => new ZtreeNode_ORG
                {
                    id = obj.ORG_ID.ToString(),
                    name = obj.ORG_NAM,
                    @checked = (check != null && check == obj.ORG_ID),
                    open = false,
                    Org_LV = obj.ORG_LEVEL,
                    isParent = all.Any(obj2 => obj2.PAR_ORG_ID == obj.ORG_ID),
                    chkDisabled = (disabled != null && disabled.ToString() == obj.ORG_ID),
                    highlight = (!string.IsNullOrEmpty(searchNam) && obj.ORG_NAM.IndexOf(searchNam) > -1),
                    children = GenOrgsTree(all, obj.ORG_ID, check, disabled, searchNam)
                }).ToList();
        }
        private void GenOrgsTreeOpen(ZtreeNode_ORG tree)
        {
            if (tree.children != null && tree.children.Any(obj => obj.highlight || obj.open))
            {
                tree.open = true;
            }
            foreach (var item in tree.children)
            {
                GenOrgsTreeOpen(item);
            }
        }
        #endregion

        #region 组织结构树-组织机构管理
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        //[UserAuth("AUTH_FUNC_VIW")]
        public ActionResult GetOrgsManagerTree(string id, string check, int? disabled, string searchNam)
        {
            var all = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            foreach (var org in all.Where(obj => obj.PAR_ORG_ID == ""))
            {
                org.PAR_ORG_ID = null;
            }
            bool rootHasAuth = true;
            if (!all.Any(o => o.ORG_LEVEL == 1))//处理没有省权限的数据
            {
                rootHasAuth = false;
                all.Where(o => o.ORG_LEVEL == (all.Min(o2 => o2.ORG_LEVEL)))
                    .ToList().ForEach(o => o.PAR_ORG_ID = null);
            }
            var tree = new ZtreeNode_ORG
            {
                id = "",
                name = "顶级机构",
                @checked = check == null,
                open = true,
                isParent = true,
                nocheck = true,
                hasauth = rootHasAuth,
                highlight = (!string.IsNullOrEmpty(searchNam) && "顶级机构".IndexOf(searchNam) > -1),
                children = GenOrgsManagerTree(all, null, check, disabled, searchNam)
            };
            var treelist = new List<ZtreeNode_ORG> { tree };
            if (!string.IsNullOrEmpty(id) || Request.Form["lv"] == "0")
            {
                if (id == "")
                    id = null;
                treelist = GenOrgsManagerTree(all, id, check, disabled, searchNam);
            }
            if (!string.IsNullOrEmpty(searchNam))
            {
                GenOrgsManagerTreeOpen(tree);
                GenOrgsManagerTreeOpen(tree);//递归达不到足够的深度，改为多次调用
                GenOrgsManagerTreeOpen(tree);
            }
            return Json(treelist, JsonRequestBehavior.AllowGet);
        }
        private List<ZtreeNode_ORG> GenOrgsManagerTree(IEnumerable<SYS_ORGANIZE> all, string parid, string check, int? disabled, string searchNam)
        {
            return all.Where(obj => obj.PAR_ORG_ID == parid)
                .Select(obj => new ZtreeNode_ORG
                {
                    id = obj.ORG_ID.ToString(),
                    name = obj.ORG_NAM,
                    @checked = (check != null && check == obj.ORG_ID),
                    open = false,
                    Org_LV = obj.ORG_LEVEL,
                    hasauth = true,
                    isParent = all.Any(obj2 => obj2.PAR_ORG_ID == obj.ORG_ID),
                    chkDisabled = (disabled != null && disabled.ToString() == obj.ORG_ID),
                    highlight = (!string.IsNullOrEmpty(searchNam) && obj.ORG_NAM.IndexOf(searchNam) > -1),
                    children = GenOrgsManagerTree(all, obj.ORG_ID, check, disabled, searchNam)
                }).ToList();
        }
        private void GenOrgsManagerTreeOpen(ZtreeNode_ORG tree)
        {
            if (tree.children != null && tree.children.Any(obj => obj.highlight || obj.open))
            {
                tree.open = true;
            }
            foreach (var item in tree.children)
            {
                GenOrgsManagerTreeOpen(item);
            }
        }
        #endregion

        #region 组织结构树2 - 只能叶子结点可选
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        //[UserAuth("AUTH_FUNC_VIW")]
        public ActionResult GetOrgsTreeLeafChk(string check, int? disabled, string searchNam)
        {
            var all = dao.FindList();
            var tree = new ZtreeNode
            {
                id = "",
                name = "顶级机构",
                @checked = check == null,
                open = true,
                isParent = true,
                nocheck = true,
                highlight = (!string.IsNullOrEmpty(searchNam) && "顶级机构".IndexOf(searchNam) > -1),
                children = GetOrgsTreeLeafChk(all, null, check, disabled, searchNam)
            };

            if (!string.IsNullOrEmpty(searchNam))
            {
                GenOrgsTreeLeafOpen(tree);
            }
            return Json(tree, JsonRequestBehavior.AllowGet);
        }
        private List<ZtreeNode> GetOrgsTreeLeafChk(IEnumerable<SYS_ORGANIZE> all, string parid, string check, int? disabled, string searchNam)
        {
            return all.Where(obj => obj.PAR_ORG_ID == parid)
                .Select(obj => new ZtreeNode
                {
                    id = obj.ORG_ID.ToString(),
                    name = obj.ORG_NAM,
                    @checked = (check != null && check == obj.ORG_ID),
                    open = false,
                    nocheck = all.Any(obj2 => obj2.PAR_ORG_ID == obj.ORG_ID),
                    isParent = all.Any(obj2 => obj2.PAR_ORG_ID == obj.ORG_ID),
                    chkDisabled = (disabled != null && disabled.ToString() == obj.ORG_ID),
                    highlight = (!string.IsNullOrEmpty(searchNam) && obj.ORG_NAM.IndexOf(searchNam) > -1),
                    children = GetOrgsTreeLeafChk(all, obj.ORG_ID, check, disabled, searchNam)
                }).ToList();
        }
        private void GenOrgsTreeLeafOpen(ZtreeNode tree)
        {
            if (tree.children != null && tree.children.Any(obj => obj.highlight))
            {
                tree.open = true;
            }
            foreach (var item in tree.children)
            {
                GenOrgsTreeLeafOpen(item);
            }
        }
        #endregion

        #region 组织结构树3-只有市，市下面挂服务厅
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        //[UserAuth("AUTH_FUNC_VIW")]
        public ActionResult GetOrgs2Tree(string id, string check, int? disabled, string searchNam)
        {
            var all = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            var hallall = DaoHall.FindList();
            foreach (var org in all.Where(obj => obj.PAR_ORG_ID == ""))
            {
                org.PAR_ORG_ID = null;
            }
            var treelist = all.Where(obj => obj.ORG_LEVEL == 2)//市级
                .Select(obj => new ZtreeNode_ORG
                {
                    id = obj.ORG_ID.ToString(),
                    name = obj.ORG_NAM,
                    @checked = (check != null && check == obj.ORG_ID),
                    open = true,
                    Org_LV = (byte)obj.ORG_LEVEL,
                    isParent = all.Any(obj2 => obj2.PAR_ORG_ID == obj.ORG_ID),
                    chkDisabled = (disabled != null && disabled.ToString() == obj.ORG_ID),
                    highlight = (!string.IsNullOrEmpty(searchNam) && obj.ORG_NAM.IndexOf(searchNam) > -1)
                }).ToList();
            if (null != treelist && treelist.Count > 0)
            {
                foreach (var leaf in treelist)
                {
                    List<ZtreeNode_ORG> leafchild = new List<ZtreeNode_ORG>();
                    GenOrgs2Tree(all, hallall, ref leafchild, leaf.id, check, disabled, searchNam);
                    leaf.children = leafchild;
                }
            }
            else
            {
                if (null != all && all.Count > 0)
                    foreach (var item in all.Where(o => o.ORG_LEVEL == 4))
                    {
                        List<ZtreeNode_ORG> leafchild = new List<ZtreeNode_ORG>();
                        GenOrgs2Tree(all, hallall, ref leafchild, item.PAR_ORG_ID, check, disabled, searchNam);
                        treelist.AddRange(leafchild);
                    }

            }
            if (!string.IsNullOrEmpty(searchNam))
            {
                GenOrgs2TreeOpen(treelist);
            }
            return Json(treelist, JsonRequestBehavior.AllowGet);
        }

        private void GenOrgs2Tree(IEnumerable<SYS_ORGANIZE> all, IEnumerable<SYS_HALL> hallall,
            ref List<ZtreeNode_ORG> leafchild,
            string parid, string check, int? disabled, string searchNam)
        {
            var l = all.Where(obj => obj.PAR_ORG_ID == parid);
            foreach (var obj in l)
            {
                var h_ = hallall.Where(obj2 => obj2.ORG_ID == obj.ORG_ID)
                 .Select(obj2 => new ZtreeNode_ORG
                 {
                     id = obj2.HALL_NO,
                     name = obj2.HALL_NAM,
                     open = false,
                     Org_LV = 5,
                     isParent = false,
                     highlight = (!string.IsNullOrEmpty(searchNam) && obj2.HALL_NAM.IndexOf(searchNam) > -1)
                 }).ToList();
                leafchild.AddRange(h_);
                GenOrgs2Tree(all, hallall, ref  leafchild, obj.ORG_ID, check, disabled, searchNam);
            }
        }
        private void GenOrgs2TreeOpen(List<ZtreeNode_ORG> tree)
        {
            foreach (var item in tree)
            {
                if (item.children != null && item.children.Any(obj => obj.highlight || obj.open))
                {
                    item.open = true;
                }
            }
        }
        #endregion

        #region 组织结构树4-只有省和市，用于查询条件
        /// <summary>
        /// 获取权限树
        /// </summary>
        /// <returns></returns>
        //[UserAuth("AUTH_FUNC_VIW")]
        public ActionResult SearchOrgsTree(string id)
        {
            var all = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            foreach (var org in all.Where(obj => obj.PAR_ORG_ID == ""))
            {
                org.PAR_ORG_ID = null;
            }
            var treelist = all.Where(obj => obj.ORG_LEVEL == all.Min(obj2 => obj2.ORG_LEVEL)
                && obj.ORG_LEVEL<=2)
                .Select(obj => new ZtreeNode_ORG
            {
                id = obj.ORG_ID.ToString(),
                name = obj.ORG_NAM,
                open = true,
                isParent = true,
                children = all.Where(o => o.PAR_ORG_ID == obj.ORG_ID && o.ORG_LEVEL == 2)//只显示市，没有市不显示
                    .Select(obj2 => new ZtreeNode_ORG
                    {
                        id = obj2.ORG_ID.ToString(),
                        name = obj2.ORG_NAM,
                        open = true,
                        isParent = true
                    }).ToList()
            }).ToList();

            return Json(treelist, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
