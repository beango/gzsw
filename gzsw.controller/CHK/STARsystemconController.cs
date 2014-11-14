using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.CHK.Models;
using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.model;
using gzsw.util;

namespace gzsw.controller.CHK
{
    public class STARsystemconController : BaseController<CHK_HALL_STAR_SYSTEM_CON>
    {



        [Ninject.Inject]
        public IDao<CHK_HALL_STAR_SYSTEM_CON> HallstarsystemconDao { get; set; }




        [UserAuth("CHK_HALL_CHKITEM_CON_VIW")]
        public ActionResult Index(string Name, int pageIndex = 1, int pageSize = 20)
        {
            ViewBag.Name = Name;

            var list = HallstarsystemconDao.FindList("STAR_LEVEL");
            return View(list);
        }


        [UserAuth("CHK_HALL_CHKITEM_CON_ADD")]
        public ActionResult Create()
        {
            var temp = new CHK_HALL_STAR_SYSTEM_CON_Model();
            return View(temp);
        }
        [HttpPost]
        [UserAuth("CHK_HALL_CHKITEM_CON_ADD")]
        public ActionResult Create(CHK_HALL_STAR_SYSTEM_CON_Model info)
        { 
            try
            {

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "新增出错。");
                    return View(info);
                }
                var temp = new CHK_HALL_STAR_SYSTEM_CON();
                temp.ATTEND_SCORE = info.ATTEND_SCORE / 100;
                temp.COMPLAIN_SCORE = info.COMPLAIN_SCORE / 100;
                temp.ENVIRON_SCORE = info.ENVIRON_SCORE;
                temp.EVAL_SATISFY_SCORE = info.EVAL_SATISFY_SCORE / 100;
                temp.HANDLE_ONTIME_SCORE = info.HANDLE_ONTIME_SCORE / 100;
                temp.NORM_SCORE = info.NORM_SCORE;
                temp.OTHER_SCORE = info.OTHER_SCORE;
                temp.PROFESS_SCORE = info.PROFESS_SCORE;
                temp.QUALITY_SCORE = info.QUALITY_SCORE / 100;
                temp.QUEUE_DETAIN_SCORE = info.QUEUE_DETAIN_SCORE / 100;
                temp.STAR_LEVEL = info.STAR_LEVEL;
                temp.SYSTEM_SCORE = info.SYSTEM_SCORE;
                temp.THIRD_SURVEY_SCORE = info.THIRD_SURVEY_SCORE;

                HallstarsystemconDao.AddObject(temp);

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

        [UserAuth("CHK_HALL_CHKITEM_CON_EDT")]
        public ActionResult Edit(int id)
        {
            var data = HallstarsystemconDao.GetEntity("STAR_LEVEL", id);
            var model = new CHK_HALL_STAR_SYSTEM_CON_Model();
            model.ATTEND_SCORE = (int)(data.ATTEND_SCORE*100);
            model.COMPLAIN_SCORE = (int)(data.COMPLAIN_SCORE * 100);
            model.ENVIRON_SCORE = data.ENVIRON_SCORE;
            model.EVAL_SATISFY_SCORE = (int)(data.EVAL_SATISFY_SCORE * 100);
            model.HANDLE_ONTIME_SCORE = (int)(data.HANDLE_ONTIME_SCORE*100);
            model.NORM_SCORE =data.NORM_SCORE;
            model.OTHER_SCORE = data.OTHER_SCORE;
            model.PROFESS_SCORE = data.PROFESS_SCORE;
            model.QUALITY_SCORE = (int)(data.QUALITY_SCORE*100);
            model.QUEUE_DETAIN_SCORE = (int)(data.QUEUE_DETAIN_SCORE*100);
            model.STAR_LEVEL = data.STAR_LEVEL;
            model.SYSTEM_SCORE = data.SYSTEM_SCORE;
            model.THIRD_SURVEY_SCORE = data.THIRD_SURVEY_SCORE; ;
            return View(model);
        }
        [HttpPost]
        [UserAuth("CHK_HALL_CHKITEM_CON_EDT")]
        public ActionResult Edit(CHK_HALL_STAR_SYSTEM_CON_Model info)
        { 
           
            try
            {

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "修改出错。");
                    return View(info);
                }
                var temp = HallstarsystemconDao.GetEntity("STAR_LEVEL", info.STAR_LEVEL);
                temp.ATTEND_SCORE = info.ATTEND_SCORE/100;
                temp.COMPLAIN_SCORE = info.COMPLAIN_SCORE/100;
                temp.ENVIRON_SCORE = info.ENVIRON_SCORE;
                temp.EVAL_SATISFY_SCORE = info.EVAL_SATISFY_SCORE/100;
                temp.HANDLE_ONTIME_SCORE = info.HANDLE_ONTIME_SCORE/100;
                temp.NORM_SCORE = info.NORM_SCORE;
                temp.OTHER_SCORE = info.OTHER_SCORE;
                temp.PROFESS_SCORE = info.PROFESS_SCORE;
                temp.QUALITY_SCORE = info.QUALITY_SCORE/100;
                temp.QUEUE_DETAIN_SCORE = info.QUEUE_DETAIN_SCORE/100;
                temp.STAR_LEVEL = info.STAR_LEVEL;
                temp.SYSTEM_SCORE = info.SYSTEM_SCORE;
                temp.THIRD_SURVEY_SCORE = info.THIRD_SURVEY_SCORE;

                HallstarsystemconDao.UpdateObject(temp);

                Alter("修改成功！", util.Enum.AlterTypeEnum.Success, false, true);
                return Redirect("/Home/Blank");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("系统错误", ex);
                ModelState.AddModelError("", "系统错误！");
                return Redirect("/Home/Error");
            }
        }

        [UserAuth("CHK_HALL_CHKITEM_CON_DEL")]
        public ActionResult Delete(string id)
        {
            try
            {
                var arrid = id.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var _id in arrid)
                {
                    HallstarsystemconDao.Delete("STAR_LEVEL", int.Parse(_id.Split('-')[0]));
                }


                return RedirectToAction("/");
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("删除出错", ex);
                return Redirect("/Home/Error");
            }
        }

        public ActionResult CheckLevel(int star_level)
        {
            var date = HallstarsystemconDao.GetEntity("STAR_LEVEL", star_level);
            return Json(date == null, JsonRequestBehavior.AllowGet);
        }
    }
}
