 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.AUTH;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using Newtonsoft.Json;

namespace gzsw.controller.Default
{
    /// <summary>
    ///  
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/19 22:17:33</para>
    /// </remark>
    public class DefaultController : BaseController
    {
        [Ninject.Inject]
        public IDao<SYS_MENU> DaoMenu { get; set; }

         

        public ActionResult LeftMenu(int? nid)
        {
            if (nid == null)
                return View();
            var sysMenu = DaoMenu.GetEntity("MENU_ID", nid.Value);
            TempData["topMenu"] = sysMenu;
            var menuall = DaoMenu.FindList("MENU_ORD asc");

            if (!isAdmin)
            {
                menuall = menuall.Where(obj =>
                        UserState.UserFuncs != null && UserState.UserFuncs.Any(obj2 => obj2.FUNCTION_ID == obj.FUNCTION_ID)
                    ).ToList();
            }

            // 实时监控处理
            if (sysMenu != null
                && sysMenu.MENU_NAM.Trim() == "实时监控")
            {
                // 读取当前用户所属的用户组织机构
                var listTree = new List<HomeController.MenuTree>();
                var organizeList = new SYS_ORGANIZE_DAL().GetListForUserId(UserState.UserID);

                // 如果只有营业厅权限只显示营业厅
                if (organizeList.Count == organizeList.Count(x => x.ORG_LEVEL == 4))
                {
                    // 读取省的菜单
                    var level4List = organizeList.Where(x => x.ORG_LEVEL == 4).ToList();
                    if (level4List != null && level4List.Count()>0)
                    {
                        foreach (var item in level4List)
                        {
                            listTree.Add(new HomeController.MenuTree()
                            {
                                id = item.ORG_ID,
                                text = item.ORG_NAM,
                                url = Url.Action("Index", "MapServer", new { @orgId = item.ORG_ID }),
                                children = GetMenuTreeForOrganize(organizeList, item.ORG_ID)
                            });
                        } 
                       
                    }
                }
                else
                {
                    // 读取省的菜单
                    var level1 = organizeList.FirstOrDefault(x => x.PAR_ORG_ID == null && x.ORG_LEVEL == 1);
                    if (level1 != null)
                    {
                        listTree.Add(new HomeController.MenuTree()
                        {
                            id = level1.ORG_ID,
                            text = level1.ORG_NAM,
                            url = Url.Action("Index", "MapServer", new { @orgId = level1.ORG_ID }),
                            children = GetMenuTreeForOrganize(organizeList, level1.ORG_ID)
                        });
                    }
                    // 读取市菜单
                    var level2 = organizeList.Where(x => x.ORG_LEVEL == 2).ToList();
                    foreach (var node in level2)
                    {
                        listTree.Add(new HomeController.MenuTree()
                        {
                            id = node.ORG_ID,
                            text = node.ORG_NAM,
                            url = Url.Action("Index", "MapServer", new { @orgId = node.ORG_ID }),
                            children = GetMenuTreeForOrganize(organizeList, node.ORG_ID)
                        });
                    } 
                }

                
                ViewData["MapServer"] = true;

               /* listTree.AddRange(organizeList.Where(x => x.ORG_LEVEL == null).Select(node => new HomeController.MenuTree()
                {
                    id = node.ORG_ID,
                    text = node.ORG_NAM,
                    url = Url.Action("Index", "MapServer", new { @orgId = node.ORG_ID }),
                    children = GetMenuTreeForOrganize(organizeList, node.ORG_ID)
                })); */
                return View(listTree);
            }
            else
            {
                menuall = menuall.OrderBy(obj => obj.MENU_ORD)
                    .ThenBy(obj => obj.MENU_ID).ToList();

            } 
            return View(GenMenuTree(menuall, nid)); 
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [UserAuth]
        public ActionResult Index()
        {
            // 获取
            var menuList = DaoMenu.FindList("MENU_ORD asc");
            var menuall = menuList.Where(obj =>
                              (obj.PAR_MENU_ID == 0 || obj.PAR_MENU_ID == null));
            if (!isAdmin)
            {
                menuall = menuall.Where(obj =>
                    UserState.UserFuncs != null && UserState.UserFuncs.Any(obj2 => obj2.FUNCTION_ID == obj.FUNCTION_ID)
                ).ToList();
            }
            menuall = menuall.OrderBy(obj => obj.MENU_ORD).ThenBy(obj => obj.MENU_ID);
            ViewBag.UserID = UserState.UserID;
            ViewBag.UserName = UserState.UserName; 
            return View(menuall);
        }

        public ActionResult Welcome()
        {
             return View();
        }

        #region Helper

        /// <summary>
        /// 根据组织结构获取树结构
        /// </summary>
        /// <param name="soure"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<HomeController.MenuTree> GetMenuTreeForOrganize(
            IEnumerable<SYS_ORGANIZE> soure,
            string id)
        {
            if (soure != null && soure.Any() && !string.IsNullOrEmpty(id))
            {
                return soure.Where(obj => obj.PAR_ORG_ID == id && obj.ORG_LEVEL==3).Select(obj => new HomeController.MenuTree()
                {
                    id = obj.ORG_ID,
                    text = obj.ORG_NAM,
                    url = Url.Action("Index", "MapServer", new { @orgId = obj.ORG_ID })
                    //,children = GetMenuTreeForOrganize(soure, obj.ORG_ID)
                }).ToList();
            }
            return new List<HomeController.MenuTree>();
        }


        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="menuall"></param>
        /// <param name="menuid"></param>
        /// <returns></returns>
        private List<HomeController.MenuTree> GenMenuTree(IEnumerable<SYS_MENU> menuall, int? menuid)
        {
            return menuall.Where(obj => menuid != null && obj.PAR_MENU_ID == menuid)
                .Select(obj => new HomeController.MenuTree
                {
                    id = obj.MENU_ID.ToString(),
                    text = obj.MENU_NAM,
                    url = obj.NAVIGATEURL
                ,
                    children = GenMenuTree(menuall, obj.MENU_ID)
                }).ToList();
        }

      /*  /// <summary>
        /// 1省 2市 3县 4营业厅
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private string GetTreeIcon(int level)
        {
            icon
        }*/

        #endregion 
    }
}
