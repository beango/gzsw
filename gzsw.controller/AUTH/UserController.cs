using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using gzsw.util.Enum;
using PetaPoco;
using gzsw.util;
using gzsw.dal.dao;
using gzsw.model.Enums;

namespace gzsw.controller.AUTH
{
    public class UserController : BaseController<SYS_USER>
    {
        [Ninject.Inject]
        public IDao<SYS_USER> DaoUser { get; set; }

        [Ninject.Inject]
        public IDao<SYS_ROLE> DaoRole { get; set; }

        [Ninject.Inject]
        public IDao<SYS_USEROLE> DaoUserole { get; set; }

        [Ninject.Inject]
        public IDao<SYS_USERORGANIZE> DaoUSERORGANIZE { get; set; }

        [Ninject.Inject]
        public IDao<SYS_ORGANIZE> DaoORGANIZE { get; set; }

        [UserAuth("AUTH_USER_VIW")]
        public ActionResult Index(
            string userid,
            string usernam,
            string orgid,
            string orgnam,
            int pageIndex = 1,
            int pageSize = 20)
        {
            ViewBag.USERID = userid;
            ViewBag.USERNAM = usernam;
            ViewBag.PAGE = pageIndex;
            ViewBag.ORGID = orgid;
            ViewBag.ORGNAM = orgnam;

            //var orgsandchild = new SYS_USER_DAL().GetUserORG(UserState.UserID);//获取组织机构及其所有下级
            Page<SYS_USER> data = new Page<SYS_USER>() { Items = new List<SYS_USER>(), ItemsPerPage = pageSize };
            //if (orgsandchild != null && orgsandchild.Count > 0)
            {
                

                //if (null != orgsuser && orgsuser.Count > 0)
                {
                    var param = new List<object>
                    {
                        "USER_ID like", userid,
                        
                        "USER_NAM like", usernam
                    };
                    if (!string.IsNullOrEmpty(orgid))
                    {
                        var orgsuser = DaoUSERORGANIZE.FindList("", "ORG_ID", orgid);
                        if (orgsuser!=null&&orgsuser.Count>0)
                        {
                            param.Add("USER_ID in");
                            param.Add(orgsuser.Select(o => o.USER_ID)); 
                        }
                        else
                        {
                             param.Add("USER_ID");
                             param.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                        }
                           

                    }
                    if (!isAdmin)
                    {
                        param.Add("CREATE_ID");
                        param.Add(UserState.UserID);
                    }

                    data = dao.GetList(
                        pageIndex, pageSize,
                        "CREATE_DTIME desc",
                        param.ToArray());
                }
            }
            return View(data);
        }

        [UserAuth("AUTH_USER_VIW")]
        public ActionResult LoadIndex(string userid, string usernam, int pageIndex = 1, int pageSize = 20)
        {
            Page<SYS_USER> data = dao.GetList(pageIndex, pageSize, "CREATE_DTIME desc", "USER_ID like", userid, "USER_NAM like", usernam);

            var gridData = new
            {
                Rows = data.Items,
                Total = data.TotalItems
            };
            return Json(gridData);
        }

        [HttpGet]
        [UserAuth("AUTH_USER_EDT")]
        public ActionResult Edit(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = DaoUser.GetEntity("USER_ID", id);
                return View(user);
            }
            return Redirect("/Home/Error");
        }

        [HttpPost]
        [UserAuth("AUTH_USER_EDT")]
        public ActionResult Edit(SYS_USER user, string id)
        {
            try
            {
                var exituser = DaoUser.GetEntity("USER_ID", id);
                if (string.IsNullOrEmpty(user.USER_NAM))
                {
                    ModelState.AddModelError("USER_NAM", "用户名称不能为空！");
                }
                if (!ModelState.IsValidField("USER_PASSWORD"))
                {
                    ModelState.Remove("USER_PASSWORD");//不验证密码
                }
                if (!ModelState.IsValid)
                {
                    //return JsonResult(false, "数据验证失败！");
                    Alter("数据验证失败！", AlterTypeEnum.Error, false, false);
                    return View(user);
                }
                if (!string.IsNullOrEmpty(user.USER_PASSWORD))
                    user.USER_PASSWORD = CryptTools.Md5(user.USER_PASSWORD);
                else
                {
                    var olduser = DaoUser.GetEntity("USER_ID", user.USER_ID);
                    user.USER_PASSWORD = olduser.USER_PASSWORD;
                }

                user.MODIFY_ID = UserState.UserID;
                user.MODIFY_DTIME = DateTime.Now;

                var rst = dao.UpdateObject(user, "USER_ID");
                if (rst > 0)
                {
                    Alter("修改成功！", AlterTypeEnum.Success, true, true);
                    return View(user);
                }
                else
                {
                    ModelState.AddModelError("", "修改失败！");
                    Alter("修改失败！", AlterTypeEnum.Error, false, false);
                    return View(user);
                }
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                ModelState.AddModelError("", "系统错误！");
                Alter("系统错误！", AlterTypeEnum.Error, false, false);
                return View(user);
            }
        }

        [HttpGet]
        [UserAuth("AUTH_USER_VIW")]
        public ActionResult Details(string id)
        {
            return View(DaoUser.GetEntity("USER_ID", id));
        }

        [HttpGet]
        [UserAuth("AUTH_USER_ADD")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [UserAuth("AUTH_USER_ADD")]
        public ActionResult Create(SYS_USER user)
        {
            try
            {
                if (string.IsNullOrEmpty(user.USER_ID))
                {
                    ModelState.AddModelError("USER_ID", "用户账号不能为空！");
                }
                if (!string.IsNullOrEmpty(user.USER_ID) && DaoUser.GetEntity("USER_ID", user.USER_ID) != null)
                {
                    ModelState.AddModelError("USER_ID", "用户账号已经存在！");
                }
                if (string.IsNullOrEmpty(user.USER_NAM))
                {
                    ModelState.AddModelError("USER_NAM", "用户名称不能为空！");
                }
                if (string.IsNullOrEmpty(user.USER_PASSWORD))
                {
                    ModelState.AddModelError("USER_PASSWORD", "用户密码不能为空！");
                }
                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "数据验证失败！");
                    //return JsonResult(false, "数据验证失败！");
                    Alter("数据验证失败！", AlterTypeEnum.Error, false, false);
                    return View();
                }
                user.USER_PASSWORD = CryptTools.Md5(user.USER_PASSWORD);
                user.CREATE_ID = UserState.UserID;
                user.CREATE_DTIME = DateTime.Now;
                DaoUser.AddObject(user);
                Alter("新增成功！", AlterTypeEnum.Success, true, true);
                return View();
                //return JsonResult(true, "新增成功！","AUTH","",false);
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误！", ex);
                ModelState.AddModelError("", "系统错误！");
                Alter("系统错误！", AlterTypeEnum.Error, false, false);
                return View();
            }
        }

        [UserAuth("AUTH_USER_DEL")]
        public ActionResult Delete(string id)
        {
            var deleteId = id.Split(',');
            try
            {
                if (deleteId.Contains("admin"))
                {
                    Alter("管理员账号不允许删除!", AlterTypeEnum.Error);
                    return Redirect(Request.UrlReferrer.PathAndQuery);
                }
                new SYS_USER_DAL().DeleteUser(deleteId);
                Alter("删除成功!", AlterTypeEnum.Success);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除用户出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("AUTH_ROLE_VIW")]
        public ActionResult UserOrgTree(string userid)
        {
            var orgall = DaoORGANIZE.FindList();
            var exists = DaoUSERORGANIZE.FindList("", "USER_ID", userid);
            return Json(GenOrgTree(orgall, exists, string.Empty), JsonRequestBehavior.AllowGet);
        }

        private List<ZtreeNode> GenOrgTree(IEnumerable<SYS_ORGANIZE> orgall, IEnumerable<SYS_USERORGANIZE> exists, string orgid)
        {
            return orgall.Where(obj => (orgid == string.Empty && string.IsNullOrEmpty(obj.PAR_ORG_ID)) || obj.PAR_ORG_ID == orgid)
                .Select(obj => new ZtreeNode
                {
                    id = obj.ORG_ID,
                    name = obj.ORG_NAM,
                    @checked = exists.Any(obj2 => obj2.ORG_ID == obj.ORG_ID),
                    open = true,
                    isParent = orgall.Any(obj2 => obj2.PAR_ORG_ID == obj.ORG_ID),
                    children = GenOrgTree(orgall, exists, obj.ORG_ID)
                }).ToList();

        }

        [UserAuth("AUTH_ROLE_VIW")]
        public ActionResult UserRole(string id)
        {
            GetViewData(id);
            return View("UserRole", DaoUserole.FindList("USER_ID", "USER_ID", id));
        }

        private void GetViewData(string id)
        {
            TempData["user"] = DaoUser.GetEntity("USER_ID", id);
            TempData["roles"] = DaoRole.FindList("ROLE_ID");
        }

        [HttpPost]
        [UserAuth("AUTH_ROLE_VIW")]
        public ActionResult UserRole(FormCollection formCollection)
        {
            string userid = formCollection["userid"];
            var roles = DaoRole.FindList();
            var exists = DaoUserole.FindList("", "USER_ID", userid);
            List<SYS_USEROLE> addroleauths = new List<SYS_USEROLE>(),
                           delroleauths = new List<SYS_USEROLE>();
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var formitem = formCollection["chkChecked" + role.ROLE_ID];
                    if (null != formitem)
                    {
                        if (formitem.Contains("true"))
                        {
                            if (exists.All(obj => obj.ROLE_ID != role.ROLE_ID)) //原权限中不存在
                                addroleauths.Add(new SYS_USEROLE { USER_ID = userid, ROLE_ID = role.ROLE_ID, CREATE_DTIME = DateTime.Now, CREATE_ID = UserState.UserID });
                        }
                        else
                        {
                            var e = exists.FirstOrDefault(obj => obj.ROLE_ID == role.ROLE_ID);
                            if (null != e)
                            {
                                delroleauths.Add(e);
                            }
                        }
                    }
                }
            }
            if (addroleauths.Count > 0)
            {
                DaoUserole.AddObject(addroleauths);
            }
            if (delroleauths.Count > 0)
            {
                DaoUserole.DeleteObject(delroleauths);
            }
            Alter("修改成功！", AlterTypeEnum.Success, true, true);
            GetViewData(userid);
            return View("UserRole", DaoUserole.FindList("USER_ID", "USER_ID", userid));
        }

        [UserAuth("SYS_ORG_VIW")]
        public ActionResult UserOrg(string id)
        {
            TempData["user"] = DaoUser.GetEntity("USER_ID", id);
            return View(DaoUSERORGANIZE.FindList("", "USER_ID", id));
        }

        [HttpPost]
        [UserAuth("SYS_ORG_VIW")]
        public ActionResult UserOrg(FormCollection formCollection)
        {
            string userid = formCollection["userid"];
            string orgid = formCollection["hidOrg"];
            DaoUSERORGANIZE.DeleteObject(DaoUSERORGANIZE.FindList("", "USER_ID", userid).ToList());

            var orgs = DaoORGANIZE.FindList();
            List<SYS_USERORGANIZE> addroleauths = new List<SYS_USERORGANIZE>();
            if (!string.IsNullOrEmpty(orgid))
            {
                var orgarr = orgid.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var seleorg in orgarr)
                {
                    var org = orgs.FirstOrDefault(obj => obj.ORG_ID == seleorg);
                    addroleauths.Add(new SYS_USERORGANIZE { USER_ID = userid, ORG_ID = org.ORG_ID, CREATE_DTIME = DateTime.Now, CREATE_ID = UserState.UserID });
                }
            }
            if (addroleauths.Count > 0)
            {
                DaoUSERORGANIZE.AddObject(addroleauths);
            }
            TempData["user"] = DaoUser.GetEntity("USER_ID", userid);
            Alter("修改成功！", AlterTypeEnum.Success, true, true);
            return View(DaoUSERORGANIZE.FindList("", "USER_ID", userid));
        }
    }
}
