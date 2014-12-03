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

namespace gzsw.controller.WARN
{
    public class SenduserconController : BaseController<WARN_ALARM_SEND_USER_CON>
    {

        [Ninject.Inject]
        public IDao<WARN_ALARM_SEND_USER_CON> SenduserconDao { get; set; }


        [Ninject.Inject]
        public IDao<SYS_USER> Userdao { get; set; }

        [Ninject.Inject]
        public IDao<SYS_HALL> Halldao { get; set; }


        [Ninject.Inject]
        public IDao<SYS_USERORGANIZE> DaoUSERORGANIZE { get; set; }

        [Ninject.Inject]
        public IDao<SYS_ORGANIZE> DaoORGANIZE { get; set; }

        [UserAuth("WARN_ALARM_SEND_USER_CON_VIW")]
        public ActionResult Index(string hallno, int pageIndex = 1, int pageSize = 20)
        {
       
            var orgall = new SYS_USER_DAL().GetUserORG(UserState.UserID);
            ViewBag.UserORG = new SelectList(orgall.Where(obj => obj.ORG_LEVEL == 4), "ORG_ID", "ORG_NAM", true);

            ViewBag.hallno = hallno;
            var data = new WARN_ALARM_SEND_USER_CON_DAL().GetList(pageIndex, pageSize, hallno);
            return View(data);
        }


        [UserAuth("WARN_ALARM_SEND_USER_CON_VIW")]
        public ActionResult Details(int id)
        {
            var data = SenduserconDao.GetEntity("SEQ", id);
            var hall= Halldao.GetEntity("HALL_NO", data.HALL_NO);
            ViewBag.HallName = "";
            if (hall != null)
                ViewBag.HallName = hall.HALL_NAM;

            ViewBag.UserName = "";
            if (!string.IsNullOrEmpty(data.USER_ID))
            {
                var user = Userdao.GetEntity("USER_ID", data.USER_ID);
                ViewBag.UserName = "";
                if (user!=null)
                    ViewBag.UserName =  user.USER_NAM;
            }
            return View(data);
        }

          [UserAuth("WARN_ALARM_SEND_USER_CON_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    SenduserconDao.Delete("SEQ", int.Parse(_id.Split('-')[0]));
                }


                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("WARN_ALARM_SEND_USER_CON_ADD")]
        public ActionResult Create()
        {
            GetCreateDt();
            var temp = new WARN_ALARM_SEND_USER_CON();
            return View(temp);
        }

        private void GetCreateDt(string org = null)
        {
            ViewBag.ALARM_TYP_ENUM = EnumHelper.GetCategorySelectList(typeof(gzsw.model.ext.WARN_ALARM_SEND_USER_CON.ALARM_TYP_ENUM));
            var organDao = new SYS_ORGANIZE_DAL();
            var orgs = organDao.GetListForUserId(UserState.UserID, "4");

            ViewBag.UserORG = new SelectList(orgs, "ORG_ID", "ORG_NAM", org);
        }

        [HttpPost]
        [UserAuth("WARN_ALARM_SEND_USER_CON_ADD")]
        public ActionResult Create(WARN_ALARM_SEND_USER_CON info)
        {
            
            if (ModelState.IsValid)
            {
                try
                {
                    var item = new WARN_ALARM_SEND_USER_CON()
                    {
                         ALARM_TYP=info.ALARM_TYP,
                         HALL_NO = info.HALL_NO,
                         MOB_NBR = info.MOB_NBR,
                         USER_ID = info.USER_ID
                    };

                    var rst = SenduserconDao.AddObject(item);
                    if (null != rst)
                    {
                        Alter("新增成功！", util.Enum.AlterTypeEnum.Success, false, true);
                        return Redirect("/Home/Blank");
                    }

                    ModelState.AddModelError("", "新增失败。");
                    return View(info);
                }
                catch (Exception ex)
                {
                    LogHelper.ErrorLog("新增出错。", ex);
                    ModelState.AddModelError("", "新增出错。");
                    return View(info);
                }
            }
            return View(info); 
        }
         
        public ActionResult UserOrgTree(string userid)
        {
            var orgall = DaoORGANIZE.FindList();
            var exists = DaoUSERORGANIZE.FindList();
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


        /// <summary>
        /// 用户信息
        /// </summary>
        /// <param name="hallNo">服务厅编号</param>
        /// <returns></returns>
        public ActionResult GetUsers(string hallNo)
        {
            var staffDal = new SYS_USER_DAL();
            var list = staffDal.GetUserOByHall(hallNo);
            return Json(list.Select(m => new SelectListItem()
            {
                Text = m.USER_NAM,
                Value = m.USER_ID
            }), JsonRequestBehavior.AllowGet);
        }
    }
}
