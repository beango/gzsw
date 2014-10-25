using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using PetaPoco;
using gzsw.util;
using gzsw.util.cache;
using gzsw.dal.dao;

namespace gzsw.controller.AUTH
{
    public class MenuController : BaseController<SYS_MENU>
    {
        [Ninject.Inject]
        public IDao<SYS_MENU> DaoMenu { get; set; }

        [Ninject.Inject]
        public IDao<SYS_FUNCTION> DaoFunction { get; set; }

        [Ninject.Inject]
        public ICacheProvider cacheProvider { get; set; }

        #region ActionResult
        [UserAuth("AUTH_MENU_VIW")]
        public ActionResult Index(string menunam, int page = 1)
        {
            ViewBag.MENUNAM = menunam;
            //ViewBag.PAGE = page;

            //Page<SYS_MENU> data = dao.GetList(page, PageSize, "", "MENU_NAM like", menunam);
            //var menuall = DaoMenu.FindList();
            //var funcall = DaoFunction.FindList();
            //foreach (var menu in data.Items)
            //{
            //    menu.Par_MenuList = menuall.Where(obj => obj.MENU_ID == menu.PAR_MENU_ID).ToList();
            //    menu.Func = funcall.FirstOrDefault(obj => obj.FUNCTION_ID == menu.FUNCTION_ID);
            //}
            return View();
        }

        #region 生成菜单树
        /// <summary>
        /// 生成菜单树
        /// </summary>
        /// <param name="id">parentid</param>
        /// <param name="check">选中的对象ID</param>
        /// <param name="disabled">不允许操作的ID</param>
        /// <param name="searchNam">搜索关键字</param>
        /// <returns></returns>
        [UserAuth("AUTH_MENU_VIW")]
        public ActionResult GetMenusTree(int? id,int? check,int? disabled, string searchNam)
        {
            var menusall = dao.FindList();
            
            var tree = new ZtreeNode
            {
                id = "",
                name = "顶级菜单",
                @checked = check == null,
                open = true,
                isParent = true,
                highlight = (!string.IsNullOrEmpty(searchNam) && "顶级菜单".IndexOf(searchNam) > -1),
                children = GenMenusTree(menusall,new int?(), check,disabled, searchNam)
            };
            var treelist = new List<ZtreeNode> { tree };
            if (null != id || Request.Form["lv"]=="0")
            {
                if (id == 0)
                    id = null;
                treelist = GenMenusTree(menusall, id, check, disabled, searchNam);
            }
            if (!string.IsNullOrEmpty(searchNam))
            {
                GenMenusTreeHighlight(tree);
                GenMenusTreeHighlight(tree);//递归达不到足够的树深度，改为多次调用
            }
            return Json(treelist, JsonRequestBehavior.AllowGet);
        }

        private List<ZtreeNode> GenMenusTree(IEnumerable<SYS_MENU> menuall, int? parid, int? check, int? disabled, string searchNam)
        {
            return menuall.Where(obj => obj.PAR_MENU_ID == parid)
                .Select(obj => new ZtreeNode
                {
                    id = obj.MENU_ID.ToString(),
                    name = obj.MENU_NAM,
                    @checked = (check != null && check == obj.MENU_ID),
                    open = false,
                    isParent = menuall.Any(obj2 => obj2.PAR_MENU_ID == obj.MENU_ID),
                    chkDisabled = (disabled != null && disabled == obj.MENU_ID),
                    highlight = (string.IsNullOrEmpty(searchNam) ? false : obj.MENU_NAM.IndexOf(searchNam) > -1),
                    children = GenMenusTree(menuall, obj.MENU_ID,check,disabled, searchNam)
                }).ToList();
        }

        private void GenMenusTreeHighlight(ZtreeNode tree)
        {
            if (tree.children != null && tree.children.Any(obj => obj.highlight || obj.open))
            {
                tree.open = true;
            }
            foreach (var item in tree.children)
            {
                GenMenusTreeHighlight(item);
            }
        }
        #endregion
        [HttpGet]
        [UserAuth("AUTH_MENU_ADD")]
        public ActionResult Create()
        {
            TempData["FUNCALL"] = DaoFunction.FindList();
            TempData["MENUALL"] = DaoMenu.FindList();
            return View();
        }

        [HttpGet]
        [UserAuth("AUTH_MENU_VIW")]
        public ActionResult Details(int id)
        {
            try
            {
                var menuall = DaoMenu.FindList();
                var menu = DaoMenu.GetEntity("MENU_ID", id);
                if (null != menu)
                {
                    menu.Par_MenuList = menuall.Where(obj => obj.MENU_ID == menu.PAR_MENU_ID).ToList();
                    menu.Func = DaoFunction.GetEntity("FUNCTION_ID", menu.FUNCTION_ID);
                }

                return View(menu);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("查看菜单出错", ex);
                return Json(new { Result = false, Text = "系统出错！" + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [UserAuth("AUTH_MENU_ADD")]
        public ActionResult Create(SYS_MENU menu)
        {
            try
            {
                if (string.IsNullOrEmpty(menu.MENU_NAM))
                {
                    ModelState.AddModelError("MENU_NAM", "菜单名称不能为空！");
                }
                if (menu.FUNCTION_ID == null || menu.FUNCTION_ID == 0)
                {
                    ModelState.AddModelError("FUNCTION_ID", "权限不能为空！");
                }
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "增加菜单失败！");
                    return JsonResult(false, "增加失败", "AUTH");
                }
                DaoMenu.AddObject(menu);
                //RefleshMenuCache();
                return JsonResult(true, "新增成功", "AUTH","/",false);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("增加菜单出错", ex);
                Alter("增加菜单出错！", util.Enum.AlterTypeEnum.Error, false, false);
                return JsonResult(false, "增加菜单出错", "AUTH");
            }
        }

        [UserAuth("AUTH_MENU_DEL")]
        public ActionResult Delete(int id)
        {
            try
            {
                new SYS_MENU_DAL().DeleteAndChildren(id);
                return JsonResult(true,"删除成功！");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpGet]
        [UserAuth("AUTH_MENU_EDT")]
        public ActionResult Edit(int id)
        {
            try
            {
                TempData["MENUALL"] = DaoMenu.FindList();
                TempData["FUNCALL"] = DaoFunction.FindList();
                var role = DaoMenu.GetEntity("MENU_ID", id);
                return View(role);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改菜单出错", ex);
                return Json(new { result = false, desc = "系统错误，请重试" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [UserAuth("AUTH_MENU_EDT")]
        public ActionResult Edit(SYS_MENU menu)
        {
            var old = DaoMenu.GetEntity("MENU_ID", menu.MENU_ID);
            try
            {
                if (string.IsNullOrEmpty(menu.MENU_NAM))
                {
                    ModelState.AddModelError("MENU_NAM", "菜单名称不能为空！");
                }
                if (menu.FUNCTION_ID == null || menu.FUNCTION_ID == 0)
                {
                    ModelState.AddModelError("FUNCTION_ID", "权限不能为空！");
                }
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "修改失败！");
                    return JsonResult(false, "修改失败", "AUTH");
                }
                var rst = dao.UpdateObject(menu, "MENU_ID");
                if (rst > 0)
                {
                    return JsonResult(true, "修改成功", "AUTH",",",false);
                }
                else
                {
                    ModelState.AddModelError("", "修改失败。");
                    return JsonResult(false, "修改失败", "AUTH");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改菜单出错", ex);
                ModelState.AddModelError("", "修改菜单失败。" + ex.Message);
                return JsonResult(false, "修改失败", "AUTH");
            }
        }
        #endregion

        #region 辅助方法
        internal void RefleshMenuCache()
        {
            try
            {
                string key = "MENUCONTROLLLER_CACHEKEY_MENULIST";
                cacheProvider.Bust(key);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("RefleshMenuCache", ex);
            }
        }
        internal ICollection<SYS_MENU> GetMenuCache()
        {
            try
            {
                string key = "MENUCONTROLLLER_CACHEKEY_MENULIST";
                return new MemoryCacheProvider().Get(key, () => DaoMenu.FindList("MENU_ID"));
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("GetMenuCache", ex);
                return null;
            }
        }
        #endregion
    }
}
