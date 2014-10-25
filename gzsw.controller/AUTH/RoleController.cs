using System;
using System.Linq;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using PetaPoco;
using System.Collections.Generic;
using gzsw.util;
using gzsw.util.Enum;
using gzsw.dal.dao;

namespace gzsw.controller.AUTH
{
    public class RoleController : BaseController<SYS_ROLE>
    {
        [Ninject.Inject]
        public IDao<SYS_ROLE> DaoRole { get; set; }
        [Ninject.Inject]
        public IDao<SYS_FUNCTION> DaoFunction { get; set; }
        [Ninject.Inject]
        public IDao<SYS_ORGANIZE> DaoOrg { get; set; }
        [Ninject.Inject]
        public IDao<SYS_ROLEFUNCTION> DaoRolefunction { get; set; }
        [Ninject.Inject]
        public IDao<SYS_USEROLE> DaoUserole { get; set; }

        [UserAuth("AUTH_ROLE_VIW")]
        public ActionResult Index(string rolenam, string orgid, string orgnam, int pageIndex = 1, int pageSize=20)
        {
            ViewBag.ORGID = orgid;
            ViewBag.ORGNAM = orgnam;
            ViewBag.ROLENAM = rolenam;

            var orgall = DaoOrg.FindList();
            var organdchild = new SYS_HALL_DAL().GetOrgAndChild(orgid);
            if (organdchild != null && organdchild.Count == 0)
                organdchild = null;

            Page<SYS_ROLE> data = dao.GetList(pageIndex, pageSize, "", "ROLE_NAM like", rolenam, "ORG_ID in", organdchild);
            foreach (var role in data.Items)
            {
                role.RoleORG = orgall.FirstOrDefault(obj => obj.ORG_ID == role.ORG_ID);
            }
            return View(data);
        }

        [HttpGet]
        [UserAuth("AUTH_ROLE_EDT")]
        public ActionResult Edit(int id)
        {
            try
            {
                var role = DaoRole.GetEntity("ROLE_ID", id);
                return View(role);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改角色出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [HttpPost]
        [UserAuth("AUTH_ROLE_EDT")]
        public ActionResult Edit(SYS_ROLE role, int id)
        {
            var extrole = DaoRole.GetEntity("ROLE_ID", id);
            var old = DaoRole.GetEntity("ROLE_ID", role.ROLE_ID);
            try
            {
                if (string.IsNullOrEmpty(role.ROLE_NAM))
                {
                    ModelState.AddModelError("ROLE_NAM", "角色名称不能为空！");
                }
                if (string.IsNullOrEmpty(role.ORG_ID))
                {
                    ModelState.AddModelError("ORG_ID", "组织机构不能为空！");
                }
                if (!ModelState.IsValid)
                {
                    return JsonResult(false, "修改失败！");
                }
                role.MODIFY_ID = UserState.UserID;
                role.MODIFY_DTIME = DateTime.Now;

                var rst = dao.UpdateObject(role, "ROLE_ID");
                if (rst > 0)
                {
                    return JsonResult(true, "修改成功！", "AUTH", "", false);
                }
                else
                {
                    return JsonResult(false, "修改失败！");
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改角色出错", ex);
                ModelState.AddModelError("", "修改角色失败。" + ex.Message);
                return JsonResult(false, "系统错误！");
            }
        }

        [HttpGet]
        [UserAuth("AUTH_ROLE_VIW")]
        public ActionResult Details(int id)
        {
            var model = DaoRole.GetEntity("ROLE_ID", id);
            var orgall = DaoOrg.FindList();
            model.RoleORG = orgall.FirstOrDefault(obj => obj.ORG_ID == model.ORG_ID);
            return View(model);
        }

        [HttpGet]
        [UserAuth("AUTH_ROLE_ADD")]
        public ActionResult Create()
        {
            TempData["ORGALL"] = DaoOrg.FindList();
            return View();
        }

        [HttpPost]
        [UserAuth("AUTH_ROLE_ADD")]
        public ActionResult Create(SYS_ROLE role)
        {
            try
            {
                if (string.IsNullOrEmpty(role.ROLE_NAM))
                {
                    ModelState.AddModelError("ROLE_NAM", "角色名称不能为空！");
                }
                if (string.IsNullOrEmpty(role.ORG_ID))
                {
                    ModelState.AddModelError("ORG_ID", "组织机构不能为空！");
                }
                if (!ModelState.IsValid)
                {
                    return JsonResult(false, "数据验证失败！");
                }
                role.CREATE_ID = UserState.UserID;
                role.CREATE_DTIME = DateTime.Now;
                DaoRole.AddObject(role);
                return JsonResult(true, "新增成功！", "AUTH", "", false);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统出错！", ex);
                return JsonResult(false, "系统出错！");
            }
        }

        [UserAuth("AUTH_ROLE_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(',');
                foreach (var _id in arrid)
                {
                    dao.DeleteObject(dao.GetEntity("ROLE_ID", int.Parse(_id)));
                    DaoRolefunction.Delete("ROLE_ID", int.Parse(_id));
                    DaoUserole.Delete("ROLE_ID", int.Parse(_id));
                }
                
                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除角色出错", ex);
                return Redirect("/Home/Error");
            }
        }

        #region 角色权限

        [UserAuth("AUTH_FUNC_VIW")]
        public ActionResult RoleFunc(int id)
        {
            TempData["role"] = DaoRole.GetEntity("ROLE_ID", id);
            return View();
        }

        [HttpPost]
        [UserAuth("AUTH_FUNC_VIW")]
        public ActionResult RoleFuncSubmit(FormCollection formCollection)
        {
            int roleid = Convert.ToInt32(formCollection["roleid"]);
            var allfuncs = DaoFunction.FindList();
            var exists = DaoRolefunction.FindList("", "ROLE_ID", roleid);
            DaoRolefunction.DeleteObject(exists.ToList());
            var rolefuncs = formCollection["hidRoleFunc"];

            var addroleauths = new List<SYS_ROLEFUNCTION>();

            if (allfuncs != null && !string.IsNullOrEmpty(rolefuncs))
            {
                rolefuncs = "," + rolefuncs + ",";
                foreach (var func in allfuncs)
                {
                    if (rolefuncs.IndexOf("," + func.FUNCTION_ID + ",") > -1)
                        addroleauths.Add(new SYS_ROLEFUNCTION
                        {
                            ROLE_ID = roleid,
                            FUNCTION_ID = func.FUNCTION_ID,
                            CREATE_DTIME = DateTime.Now,
                            CREATE_ID = UserState.UserID
                        });
                    if (rolefuncs.IndexOf("," + func.FUNCTION_ID + "_1,") > -1)
                        addroleauths.Add(new SYS_ROLEFUNCTION
                        {
                            ROLE_ID = roleid,
                            FUNCTION_ID = func.FUNCTION_ID,
                            FUNCTION_TYP = 1,
                            CREATE_DTIME = DateTime.Now,
                            CREATE_ID = UserState.UserID
                        });
                    if (rolefuncs.IndexOf("," + func.FUNCTION_ID + "_2,") > -1)
                        addroleauths.Add(new SYS_ROLEFUNCTION
                        {
                            ROLE_ID = roleid,
                            FUNCTION_ID = func.FUNCTION_ID,
                            FUNCTION_TYP = 2,
                            CREATE_DTIME = DateTime.Now,
                            CREATE_ID = UserState.UserID
                        });
                    if (rolefuncs.IndexOf("," + func.FUNCTION_ID + "_3,") > -1)
                        addroleauths.Add(new SYS_ROLEFUNCTION
                        {
                            ROLE_ID = roleid,
                            FUNCTION_ID = func.FUNCTION_ID,
                            FUNCTION_TYP = 3,
                            CREATE_DTIME = DateTime.Now,
                            CREATE_ID = UserState.UserID
                        });
                    if (rolefuncs.IndexOf("," + func.FUNCTION_ID + "_4,") > -1)
                        addroleauths.Add(new SYS_ROLEFUNCTION
                        {
                            ROLE_ID = roleid,
                            FUNCTION_ID = func.FUNCTION_ID,
                            FUNCTION_TYP = 4,
                            CREATE_DTIME = DateTime.Now,
                            CREATE_ID = UserState.UserID
                        });
                }
            }
            if (addroleauths.Count > 0)
            {
                DaoRolefunction.AddObject(addroleauths);
            }

            return JsonResult(true,"提交成功");
        }
        #endregion

        [UserAuth("AUTH_ROLE_VIW")]
        public ActionResult RoleOrgTree(int? roleid)
        {
            var orgall = DaoOrg.FindList();
            SYS_ROLE role = null;
            if (roleid != null)
                role = DaoRole.GetEntity("ROLE_ID", roleid);
            return Json(GenRoleOrgTree(orgall, role, string.Empty), JsonRequestBehavior.AllowGet);
        }

        private List<ZtreeNode> GenRoleOrgTree(IEnumerable<SYS_ORGANIZE> orgall, SYS_ROLE exists, string orgid)
        {
            return orgall.Where(obj => (orgid == string.Empty && string.IsNullOrEmpty(obj.PAR_ORG_ID)) || obj.PAR_ORG_ID == orgid)
                .Select(obj => new ZtreeNode
                {
                    id = obj.ORG_ID,
                    name = obj.ORG_NAM,
                    @checked = (exists != null && obj.ORG_ID == exists.ORG_ID),
                    open = true,
                    isParent = orgall.Any(obj2 => obj2.PAR_ORG_ID == obj.ORG_ID),
                    children = GenRoleOrgTree(orgall, exists, obj.ORG_ID)
                }).ToList();

        }

        [UserAuth("AUTH_ROLE_VIW")]
        public ActionResult RoleFuncsTree(int? roleid)
        {
            var funcsall = DaoFunction.FindList();
            List<SYS_ROLEFUNCTION> exists = null;
            if (roleid != null)
                exists = DaoRolefunction.FindList("", "ROLE_ID", roleid).ToList();
            return Json(GenRoleFuncsTree(funcsall, exists, null), JsonRequestBehavior.AllowGet);
        }

        private List<ZtreeNode> GenRoleFuncsTree(IEnumerable<SYS_FUNCTION> funcsall, List<SYS_ROLEFUNCTION> exists, int? funcid)
        {
            return funcsall.Where(obj => funcid == null || obj.PAR_FUNCTION_ID == funcid)
                .Select(obj => new ZtreeNode
                {
                    id = obj.FUNCTION_ID.ToString(),
                    name = obj.FUNCTION_NAM,
                    @checked = (null != exists && exists.Any(obj2 => obj2.FUNCTION_ID == obj.FUNCTION_ID)),
                    open = true,
                    isParent = funcsall.Any(obj2 => obj2.PAR_FUNCTION_ID == obj.FUNCTION_ID),
                    children = GenRoleFuncsTree(funcsall, exists, obj.FUNCTION_ID)
                }).ToList();

        }
    }
}
