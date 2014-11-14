using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.controller.AUTH;
using gzsw.dal;
using gzsw.model;
using gzsw.dal.dao;

namespace gzsw.controller
{
    public class HomeController : BaseController<SYS_USER>
    {
        [Ninject.Inject]
        public IDao<SYS_MENU> DaoMenu { get; set; }

        /// <summary>
        /// 用户组织机构
        /// </summary>
        [Ninject.Inject]
        public IDao<SYS_USERORGANIZE> Userorganize { get; set; }

        /// <summary>
        /// 组织机构
        /// </summary>
        [Ninject.Inject]
        public IDao<SYS_ORGANIZE> Organize { get; set; }

        [UserAuth]
        public ActionResult Default()
        {
            return RedirectToAction("Index", "Default");
        }

        /// <summary>
        /// 底部
        /// </summary>
        /// <returns></returns>
        public ActionResult Bottom()
        {
            return View();
        }

        /// <summary>
        /// 顶部菜单
        /// </summary>
        /// <returns></returns>
        public ActionResult Top()
        {
            var menuList = DaoMenu.FindList("MENU_ORD asc"); 
            var menuall = menuList.Where(obj =>
                              (obj.PAR_MENU_ID == 0 || obj.PAR_MENU_ID == null));
            if (!isAdmin)
            {
                menuall = menuall.Where(obj =>
                    UserState.UserFuncs != null && UserState.UserFuncs.Any(obj2 => obj2.FUNCTION_ID == obj.FUNCTION_ID)
                );
            }
            menuall = menuall.OrderBy(obj => obj.MENU_ORD).ThenBy(obj => obj.MENU_ID);
            ViewBag.UserID = UserState.UserID;
            ViewBag.UserName = UserState.UserName;
            return View(menuall);
        }

        /// <summary>
        /// 中间栏
        /// </summary>
        /// <returns></returns>
        public ActionResult Middle()
        {
            return View();
        }

        /// <summary>
        /// 左菜单
        /// </summary>
        /// <returns></returns>
        [UserAuth]
        public ActionResult Left(int? nid)
        {
            if (nid == null)
                return View();
            var sysMenu =    DaoMenu.GetEntity("MENU_ID", nid.Value);
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
                var listTree = new List<MenuTree>();
                var organizeList =   new SYS_ORGANIZE_DAL().GetListForUserId(UserState.UserID); 
                listTree.AddRange(organizeList.Where(x=>x.PAR_ORG_ID==null).Select(node => new MenuTree()
                {
                    id = node.ORG_ID,
                    text = node.ORG_NAM,
                    url = Url.Action("Index", "MapServer", new { @orgId = node.ORG_ID }),
                    children = GetMenuTreeForOrganize(organizeList, node.ORG_ID)
                })); 
                 
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
        /// 内容
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Blank()
        {
            return View();
        }
        [UserAuth]
        public JsonResult LoadMenu(int? id = null)
        {
            if (id == null)
                return Json("", JsonRequestBehavior.AllowGet);
            var menuall = DaoMenu.FindList("MENU_ORD asc");

            if (!isAdmin)
            {
                menuall = menuall.Where(obj =>
                        UserState.UserFuncs != null && UserState.UserFuncs.Any(obj2 => obj2.FUNCTION_ID == obj.FUNCTION_ID)
                    ).ToList();
            }

            menuall = menuall.OrderBy(obj => obj.MENU_ORD)
                .ThenBy(obj => obj.MENU_ID).ToList();

            return Json(GenMenuTree(menuall, id), JsonRequestBehavior.AllowGet);
        }
        [UserAuth]
        public ActionResult LoadMenu2(int? id = null)
        {
            if (id == null)
                return Json("", JsonRequestBehavior.AllowGet);
            var menuall = DaoMenu.FindList("MENU_ORD asc");

            if (!isAdmin)
            {
                menuall = menuall.Where(obj =>
                        UserState.UserFuncs != null && UserState.UserFuncs.Any(obj2 => obj2.FUNCTION_ID == obj.FUNCTION_ID)
                    ).ToList();
            }

            menuall = menuall.OrderBy(obj => obj.MENU_ORD)
                .ThenBy(obj => obj.MENU_ID).ToList();

            string str = "";

            var menu = menuall.FirstOrDefault(obj => obj.MENU_ID == id.Value);
            str += @"<dd>";
            str += @"<ul class='menuson'>";
            foreach (var menu2 in menuall.Where(obj => obj.PAR_MENU_ID == id.Value))
            {
                str += "<li><a href='#" + menu2.NAVIGATEURL.Replace("/", "") + "' nid='" + menu2.NAVIGATEURL + "'>" + menu2.MENU_NAM + "</a><i></i></li>";//<cite></cite>
            }
            str += @"</ul></dd>";
            return Content(str);
        }
         

        /// <summary>
        /// 菜单树
        /// </summary>
        public class MenuTree
        {
            public MenuTree()
            {
                children = new List<MenuTree>();
            }

            public string id { get; set; }
             
            public string text { get; set; }
            public string url { get; set; }
            public IList<MenuTree> children { get; set; }

        }

        #region Helper

        /// <summary>
        /// 根据组织结构获取树结构
        /// </summary>
        /// <param name="soure"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private List<MenuTree> GetMenuTreeForOrganize(IEnumerable<SYS_ORGANIZE> soure,string id )
        {
            if (soure != null && soure.Any()&&!string.IsNullOrEmpty(id))
            { 
                return soure.Where(obj => obj.PAR_ORG_ID == id).Select(obj => new MenuTree()
                {
                    id = obj.ORG_ID,
                    text = obj.ORG_NAM,
                    url = Url.Action("Index", "MapServer", new { @orgId = obj.ORG_ID }),
                    children = GetMenuTreeForOrganize(soure,obj.ORG_ID)
                }).ToList();
            }
            return new List<MenuTree>();
        }


        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="menuall"></param>
        /// <param name="menuid"></param>
        /// <returns></returns>
        private List<MenuTree> GenMenuTree(IEnumerable<SYS_MENU> menuall, int? menuid)
        {
            return menuall.Where(obj => menuid != null && obj.PAR_MENU_ID == menuid)
                .Select(obj => new MenuTree
                {
                    id = obj.MENU_ID.ToString(),
                    text = obj.MENU_NAM,
                    url = obj.NAVIGATEURL
                ,
                    children = GenMenuTree(menuall, obj.MENU_ID)
                }).ToList(); 
        }

        #endregion 
    }
}
