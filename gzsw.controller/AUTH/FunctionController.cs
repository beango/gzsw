using System;
using System.Linq;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using gzsw.util.cache;
using System.Collections.Generic;
using gzsw.util;
using gzsw.dal.dao;
using gzsw.util.Enum;

namespace gzsw.controller.AUTH
{
    public class FunctionController : BaseController<SYS_FUNCTION>
    {
        [Ninject.Inject]
        public IDao<SYS_FUNCTION> DaoFunction { get; set; }
        [Ninject.Inject]
        public IDao<SYS_ROLEFUNCTION> DaoRolefunction { get; set; }
        [Ninject.Inject]
        public ICacheProvider cacheProvider { get; set; }

        [UserAuth("AUTH_FUNC_VIW")]
        public ActionResult Index(string funnam, string funcode, string parfun, int page = 1)
        {
            ViewBag.FUNNAM = funnam;
            ViewBag.FUNCODE = funcode;
            ViewBag.PARFUN = parfun;
            ViewBag.PAGE = page;
            return View();
        }

        #region 生成权限树
        [UserAuth("AUTH_FUNC_VIW")]
        public ActionResult GetRoleFuncsTree(int roleid, int? check, int? disabled, string searchNam)
        {
            var rolefunclist = DaoRolefunction.FindList("", "role_id", roleid);
            var funcsall = DaoFunction.FindList();
            var tree = new ZtreeNode
            {
                id = "",
                name = "顶级权限",
                @checked = check == null,
                open = true,
                isParent = true,
                nocheck = true,
                highlight = (string.IsNullOrEmpty(searchNam) ? false : "顶级权限".IndexOf(searchNam) > -1),
                children = GenRoleFuncsTree(funcsall, rolefunclist, null, null
                , disabled, searchNam)
            };

            if (!string.IsNullOrEmpty(searchNam))
            {
                GenFuncsTreeHighlight(tree);
            }
            GenFuncsTreeHighlight2(tree, rolefunclist);
            return Json(tree, JsonRequestBehavior.AllowGet);
        }
        private List<ZtreeNode> GenRoleFuncsTree(IEnumerable<SYS_FUNCTION> funcsall, IEnumerable<SYS_ROLEFUNCTION> rolefunclist
            , int? parid, int? check, int? disabled, string searchNam)
        {
            return funcsall.Where(obj => obj.PAR_FUNCTION_ID == parid)
                .Select(obj => new ZtreeNode
                {
                    id = obj.FUNCTION_ID.ToString(),
                    name = obj.FUNCTION_NAM,
                    @checked = (check != null && check == obj.FUNCTION_ID) || rolefunclist.Any(obj2=>obj2.FUNCTION_ID==obj.FUNCTION_ID),
                    open = false,
                    nocheck = false,
                    isParent = funcsall.Any(obj2 => obj2.PAR_FUNCTION_ID == obj.FUNCTION_ID),
                    chkDisabled = (disabled != null && disabled == obj.FUNCTION_ID),
                    highlight = (!string.IsNullOrEmpty(searchNam) && obj.FUNCTION_NAM.IndexOf(searchNam) > -1),
                    children = GenRoleFuncsTree(funcsall,rolefunclist, obj.FUNCTION_ID, check, disabled, searchNam)
                }).ToList();
        }
        private void GenFuncsTreeHighlight2(ZtreeNode tree, IList<SYS_ROLEFUNCTION> rolefunclist)
        {
            if (tree.children == null || tree.children.Count == 0)
            {
                tree.children = new List<ZtreeNode>{ new ZtreeNode
                {
                    id = tree.id+"_1",
                    name = "查看",
                    @checked = rolefunclist.Any(obj=>obj.FUNCTION_ID.ToString()==tree.id && obj.FUNCTION_TYP==1),
                    open = true,
                    isParent = false
                },new ZtreeNode
                {
                    id = tree.id+"_2",
                    name = "增加",
                    @checked = rolefunclist.Any(obj=>obj.FUNCTION_ID.ToString()==tree.id && obj.FUNCTION_TYP==2),
                    open = true,
                    isParent = false
                },
                new ZtreeNode
                {
                    id = tree.id+"_3",
                    name = "修改",
                    @checked = rolefunclist.Any(obj=>obj.FUNCTION_ID.ToString()==tree.id && obj.FUNCTION_TYP==3),
                    open = true,
                    isParent = false
                },new ZtreeNode
                {
                    id = tree.id+"_4",
                    name = "删除",
                    @checked = rolefunclist.Any(obj=>obj.FUNCTION_ID.ToString()==tree.id && obj.FUNCTION_TYP==4),
                    open = true,
                    isParent = false
                }};
                tree.@checked = tree.children.Any(obj => obj.@checked);
                return;
            }
            foreach (var item in tree.children)
            {
                if (item.id.IndexOf("_1") > -1)
                    return;
                GenFuncsTreeHighlight2(item, rolefunclist);
            }
        }

        [UserAuth("AUTH_FUNC_VIW")]
        public ActionResult GetFuncsTree(int? id, int? check, int? disabled, string searchNam)
        {
            var funcsall = DaoFunction.FindList();
            var tree = new ZtreeNode
            {
                id = "",
                name = "顶级权限",
                @checked = check == null,
                open = true,
                isParent = true,
                nocheck = true,
                highlight = (!string.IsNullOrEmpty(searchNam) && "顶级权限".IndexOf(searchNam) > -1),
                children = GenFuncsTree(funcsall, null, check, disabled, searchNam)
            };
            var treelist = new List<ZtreeNode> { tree };
            if (null != id || Request.Form["lv"] == "0")
            {
                if (id == 0)
                    id = null;
                treelist = GenFuncsTree(funcsall, id, check, disabled, searchNam);
            }
            if (!string.IsNullOrEmpty(searchNam))
            {
                GenFuncsTreeHighlight(tree);
            }
            return Json(treelist, JsonRequestBehavior.AllowGet);
        }

        private List<ZtreeNode> GenFuncsTree(IEnumerable<SYS_FUNCTION> funcsall, int? parid, int? check, int? disabled, string searchNam)
        {
            return funcsall.Where(obj => obj.PAR_FUNCTION_ID == parid)
                .Select(obj => new ZtreeNode
                {
                    id = obj.FUNCTION_ID.ToString(),
                    name = obj.FUNCTION_NAM,
                    @checked = (check != null && check == obj.FUNCTION_ID),
                    open = false,
                    nocheck = false,
                    isParent = funcsall.Any(obj2 => obj2.PAR_FUNCTION_ID == obj.FUNCTION_ID),
                    chkDisabled = (disabled != null && disabled == obj.FUNCTION_ID),
                    highlight = (string.IsNullOrEmpty(searchNam) ? false : obj.FUNCTION_NAM.IndexOf(searchNam) > -1),
                    children = GenFuncsTree(funcsall, obj.FUNCTION_ID, check, disabled, searchNam)
                }).ToList();
        }

        private void GenFuncsTreeHighlight(ZtreeNode tree)
        {
            if (tree.children != null && tree.children.Any(obj => obj.highlight))
            {
                tree.open = true;
            }
            foreach (var item in tree.children)
            {
                GenFuncsTreeHighlight(item);
            }
        }
        #endregion

        [HttpGet]
        [UserAuth("AUTH_FUNC_EDT")]
        public ActionResult Edit(int id, int? pid)
        {
            try
            {
                TempData["FUNCALL"] = DaoFunction.FindList();
                var user = DaoFunction.GetEntity("FUNCTION_ID", id);

                if (null != pid)
                {
                    if (pid != 0)
                        TempData["PARFUN"] = DaoFunction.GetEntity("FUNCTION_ID", pid);
                    else
                        ViewBag.pid = pid;
                }

                return View(user);
            }
            catch (Exception e)
            {
                LogHelper.ErrorLog("修改权限出错", e);

                return Json(new { result = false, desc = "系统错误，请重试" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [UserAuth("AUTH_FUNC_EDT")]
        public ActionResult Edit(SYS_FUNCTION function, int id, int? pid)
        {
            TempData["FUNCALL"] = DaoFunction.FindList();
            var exituser = DaoFunction.GetEntity("FUNCTION_ID", id);
            var old = DaoFunction.GetEntity("FUNCTION_ID", function.FUNCTION_ID);
            try
            {
                if (string.IsNullOrEmpty(function.FUNCTION_NAM))
                {
                    ModelState.AddModelError("FUNCTION_NAM", "权限名称不能为空！");
                }
                if (string.IsNullOrEmpty(function.FUNCTION_COD))
                {
                    ModelState.AddModelError("FUNCTION_COD", "权限编码不能为空！");
                }else
                {
                    var exist = DaoFunction.GetEntity("FUNCTION_COD", function.FUNCTION_COD);
                    if (null != exist && exist.FUNCTION_ID != function.FUNCTION_ID)
                    {
                        ModelState.AddModelError("FUNCTION_COD", "权限编码已经存在！");
                    }
                }
                
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "修改失败");
                    return JsonResult(false, "修改失败", "AUTH");
                }
                function.MODIFY_ID = UserState.UserID;
                function.MODIFY_DTIME = DateTime.Now;

                var rst = dao.UpdateObject(function, "FUNCTION_ID");
                if (rst > 0)
                {
                    //RefleshFuncCache();
                    return JsonResult(true, "修改成功", "AUTH","/",false);
                }
                else
                {
                    ModelState.AddModelError("ModelState", "修改失败。");
                    return JsonResult(false, "修改失败", "AUTH");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改失败", ex);
                ModelState.AddModelError("", "修改失败。");
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("AUTH_FUNC_VIW")]
        public ActionResult Details(int id)
        {
            var funcall = DaoFunction.FindList();
            var model = DaoFunction.GetEntity("FUNCTION_ID", id);
            if (null != model)
                model.Par_FuncList = funcall.Where(obj => obj.FUNCTION_ID == model.PAR_FUNCTION_ID).ToList();

            return View(model);
        }

        [HttpGet]
        [UserAuth("AUTH_FUNC_ADD")]
        public ActionResult Create(int? pid)
        {
            TempData["FUNCALL"] = DaoFunction.FindList();
            if (null != pid)
            {
                if (pid != 0)
                    TempData["PARFUN"] = DaoFunction.GetEntity("FUNCTION_ID", pid);
                else
                    ViewBag.pid = pid;
            }
            return View();
        }

        [HttpPost]
        [UserAuth("AUTH_FUNC_ADD")]
        public ActionResult Create(SYS_FUNCTION function)
        {
            try
            {
                if (string.IsNullOrEmpty(function.FUNCTION_NAM))
                {
                    ModelState.AddModelError("FUNCTION_NAM", "权限名称不能为空！");
                }
                if (string.IsNullOrEmpty(function.FUNCTION_COD))
                {
                    ModelState.AddModelError("FUNCTION_COD", "权限编码不能为空！");
                }
                else if (DaoFunction.GetEntity("FUNCTION_COD", function.FUNCTION_COD) != null)
                {
                    ModelState.AddModelError("FUNCTION_COD", "权限编码已经存在！");
                }

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "数据验证失败！");
                    return JsonResult(false, "数据验证失败！");
                }

                function.CREATE_ID = UserState.UserID;
                function.CREATE_DTIME = DateTime.Now;
                DaoFunction.AddObject(function);
                //RefleshFuncCache();
                return JsonResult(true,"新增成功","AUTH","/",false);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                return JsonResult(false, "系统错误！");
            }
        }

        [UserAuth("AUTH_FUNC_DEL")]
        public ActionResult Delete(int id)
        {
            try
            {
                new SYS_FUNCTION_DAL().DeleteAndChildren(id);
                //RefleshFuncCache();
                return JsonResult(true,"删除成功");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除失败", ex);
                return Redirect("/Home/Error");
            }
        }

        #region 辅助方法
        internal void RefleshFuncCache()
        {
            try
            {
                string key = "FUNCTIONCONTROLLLER_CACHEKEY_FUNCLIST";
                cacheProvider.Bust(key);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("RefleshFuncCache", ex);
            }
        }
        internal ICollection<SYS_FUNCTION> GetFuncCache()
        {
            try
            {
                string key = "FUNCTIONCONTROLLLER_CACHEKEY_FUNCLIST";
                return new MemoryCacheProvider().Get(key, () => DaoFunction.FindList());
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("GetFuncCache", ex);
                return null;
            }
        }
        #endregion
    }
}
