using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using PetaPoco;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;

namespace gzsw.controller.WARN
{
    public class WarnComplainController : BaseController<WARN_COMPLAIN_TYP_CON>
    {

        [Ninject.Inject]
        public IDao<WARN_COMPLAIN_TYP_CON> WarncomplaintypedaoDao { get; set; }

        [UserAuth("WARN_COMPLAIN_TYP_CON_VIW")]
        public ActionResult Index(string dlnam, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.TITL = dlnam;

            ViewBag.WarnComplainlist = new SelectList(WarncomplaintypedaoDao.FindList().Select(x => x.COMPLAIN_NAM));

            var data = WarncomplaintypedaoDao.GetList(pageIndex, pageSize, "", "COMPLAIN_NAM like", dlnam); 
            return View(data);
        }
         

        [UserAuth("WARN_COMPLAIN_TYP_CON_ADD")]
        public ActionResult Create()
        {
            var temp = new WARN_COMPLAIN_TYP_CON(); 
            temp.MODIFY_DTIME = DateTime.Now;
            temp.MODIFY_ID = UserState.UserID;
            temp.CREATE_DTIME = DateTime.Now;
            temp.CREATE_ID = UserState.UserID;
            return View(temp);
        }


        [UserAuth("WARN_COMPLAIN_TYP_CON_EDT")]
        public ActionResult Edit(int id)
        {


            var data = WarncomplaintypedaoDao.GetEntity("COMPLAIN_TYP_ID", id);
            return View(data);
        }

        [HttpPost]
        [UserAuth("WARN_COMPLAIN_TYP_CON_EDT")]
        public ActionResult Edit(WARN_COMPLAIN_TYP_CON info)
        {
            try
            {
                if (string.IsNullOrEmpty(info.COMPLAIN_NAM))
                {
                    ModelState.AddModelError("", "类型名称不能为空！");
                    return RedirectToAction("Error", "Home");
     
                }
                var data = WarncomplaintypedaoDao.GetEntity("COMPLAIN_TYP_ID", info.COMPLAIN_TYP_ID);
                data.COMPLAIN_NAM = info.COMPLAIN_NAM;
                WarncomplaintypedaoDao.UpdateObject(data); 
                Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                return Redirect("/Home/Blank");
                
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("修改出错！", ex);
                ModelState.AddModelError("", "修改失败！" + ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }


        [UserAuth("WARN_COMPLAIN_TYP_CON_ADD")]
        [HttpPost]
        public ActionResult Create(WARN_COMPLAIN_TYP_CON info)
        {
            try
            {
                //校验
                CHKVALID(info);


             

                if (!ModelState.IsValid)
                { 
                    ModelState.AddModelError("", "新增出错。");
                    return View(info);
                }
                WarncomplaintypedaoDao.AddObject(info); 
                //return JsonResult(true, "新增成功！", "WARN");


                Alter("新增成功！", util.Enum.AlterTypeEnum.Success, false, true);
                return Redirect("/Home/Blank");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误", ex);
                ModelState.AddModelError("", "系统错误！");
                return Redirect("/Home/Error");
            }
        }

        private void CHKVALID(WARN_COMPLAIN_TYP_CON info)
        {
            if (string.IsNullOrEmpty(info.COMPLAIN_NAM))
            {
                ModelState.AddModelError("COMPLAIN_NAM", "标题不能为空!");
            } 
        }

        [UserAuth("WARN_COMPLAIN_TYP_CON_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                { 
                    WarncomplaintypedaoDao.Delete("COMPLAIN_TYP_ID", int.Parse(_id.Split('-')[0])); 
                }

             
                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            }
        }
    }
}
