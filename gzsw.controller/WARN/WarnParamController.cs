using gzsw.controller.MyAuth;
using gzsw.dal;
using gzsw.dal.dao;
using gzsw.model;
using gzsw.util;
using gzsw.util.Enum;
using Ninject;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace gzsw.controller.WARN
{
    public class WarnParamController : BaseController<WARN_PARAM>
    {
        [Inject]
        public IDao<SYS_HALL> DaoHall { get; set; }
        [Inject]
        public IDao<SYS_ORGANIZE> DaoOrg { get; set; }
        [Inject]
        public IDao<SYS_STAFF> DaoStaff { get; set; }
        [Inject]
        public IDao<WARN_PARAM_SEND_USER_CON> DaoParamUser { get; set; }
        #region 属性
        Dictionary<int, string> WARN_TYP_DICT = new Dictionary<int, string> { 
                {1,"等候超时"},
                {2,"等候超时率（高于）"},
                {3,"窗口饱和度（高于）"},
                {4,"大厅饱和度（高于）"},
                {5,"超时办结率（高于）"},
                {6,"超时业务笔数（高于）"},
                {7,"弃号率（高于）"},
                {8,"差评笔数预警（高于）"},
                {9,"连续工作时长超界 （高于）"}
            };
        Dictionary<int, string> WARN_LEVEL_DICT = new Dictionary<int, string> { 
                {1,"黄色预警"},
                {2,"橙色预警"},
                {3,"红色预警"}
            };
        #endregion

        [UserAuth("WARN_PARAM_VIW")]
        public ActionResult Index(int pageIndex = 1, int pageSize = 20)
        {
            return View();
        }

        [UserAuth("WARN_PARAM_VIW")]
        public ActionResult Details(string hallno)
        {
            var param = InitGetDT(hallno);
            if (null != param)
                return View(param);
            return RedirectToAction("Create", new { hallno = hallno });
        }

        private Model_WARN_PARAM InitGetDT(string hallno)
        {
            Model_WARN_PARAM param = null;
            var paralist = dao.FindList("", "HALL_NO", hallno);
            if (null != paralist && paralist.Count > 0)
            {
                param = new Model_WARN_PARAM();
                Type t = param.GetType();
                foreach (var t_dict in WARN_TYP_DICT)
                {
                    foreach (var l_dict in WARN_LEVEL_DICT)
                    {
                        string qkey1 = "CRITICAL_VALUE" + t_dict.Key + l_dict.Key;
                        var p = paralist.FirstOrDefault(obj => obj.WARN_TYP == t_dict.Key && obj.WARN_LEVEL == l_dict.Key);
                        var v = p.CRITICAL_VALUE;
                        if (t_dict.Key == 2 || t_dict.Key == 4 || t_dict.Key == 5 || t_dict.Key == 7)//等候超时率，大厅饱和度，超时办结率，弃号率
                        {
                            v = v * 100;
                        }
                        if (null != p)
                            t.GetProperty(qkey1).SetValue(param, v, null);

                        string qkey2 = "FRE_MIN" + t_dict.Key;
                        string qkey3 = "WARN_INFO_MODEL" + t_dict.Key;
                        var p2 = paralist.FirstOrDefault(obj => obj.WARN_TYP == t_dict.Key);
                        if (null != p2)
                        {
                            t.GetProperty(qkey2).SetValue(param, p2.FRE_MIN, null);
                            t.GetProperty(qkey3).SetValue(param, p2.WARN_INFO_MODEL, null);
                        }
                    }
                }
            }
            return param;
        }

        [UserAuth("WARN_PARAM_ADD")]
        public ActionResult Create(string hallno)
        {
            var param = InitGetDT(hallno);
            return View(param);
        }

        [HttpPost]
        [UserAuth("WARN_PARAM_ADD")]
        public ActionResult Create(string hallno, Model_WARN_PARAM model)
        {
            if (model.CRITICAL_VALUE11 >= model.CRITICAL_VALUE12)
            {
                ModelState.AddModelError("CRITICAL_VALUE12", "橙色预警值必须大于黄色预警值！");
            }
            if (model.CRITICAL_VALUE12 >= model.CRITICAL_VALUE13)
            {
                ModelState.AddModelError("CRITICAL_VALUE13", "红色预警值必须大于橙色预警值！");
            }
            if (model.CRITICAL_VALUE21 >= model.CRITICAL_VALUE22)
            {
                ModelState.AddModelError("CRITICAL_VALUE22", "橙色预警值必须大于黄色预警值！");
            }
            if (model.CRITICAL_VALUE22 >= model.CRITICAL_VALUE23)
            {
                ModelState.AddModelError("CRITICAL_VALUE23", "红色预警值必须大于橙色预警值！");
            }
            if (model.CRITICAL_VALUE31 >= model.CRITICAL_VALUE32)
            {
                ModelState.AddModelError("CRITICAL_VALUE32", "橙色预警值必须大于黄色预警值！");
            }
            if (model.CRITICAL_VALUE32 >= model.CRITICAL_VALUE33)
            {
                ModelState.AddModelError("CRITICAL_VALUE33", "红色预警值必须大于橙色预警值！");
            }
            if (model.CRITICAL_VALUE41 >= model.CRITICAL_VALUE42)
            {
                ModelState.AddModelError("CRITICAL_VALUE42", "橙色预警值必须大于黄色预警值！");
            }
            if (model.CRITICAL_VALUE42 >= model.CRITICAL_VALUE43)
            {
                ModelState.AddModelError("CRITICAL_VALUE43", "红色预警值必须大于橙色预警值！");
            }
            if (model.CRITICAL_VALUE51 >= model.CRITICAL_VALUE52)
            {
                ModelState.AddModelError("CRITICAL_VALUE52", "橙色预警值必须大于黄色预警值！");
            }
            if (model.CRITICAL_VALUE52 >= model.CRITICAL_VALUE53)
            {
                ModelState.AddModelError("CRITICAL_VALUE53", "红色预警值必须大于橙色预警值！");
            }
            if (model.CRITICAL_VALUE61 >= model.CRITICAL_VALUE62)
            {
                ModelState.AddModelError("CRITICAL_VALUE62", "橙色预警值必须大于黄色预警值！");
            }
            if (model.CRITICAL_VALUE62 >= model.CRITICAL_VALUE63)
            {
                ModelState.AddModelError("CRITICAL_VALUE63", "红色预警值必须大于橙色预警值！");
            }
            if (model.CRITICAL_VALUE71 >= model.CRITICAL_VALUE72)
            {
                ModelState.AddModelError("CRITICAL_VALUE72", "橙色预警值必须大于黄色预警值！");
            }
            if (model.CRITICAL_VALUE72 >= model.CRITICAL_VALUE73)
            {
                ModelState.AddModelError("CRITICAL_VALUE73", "红色预警值必须大于橙色预警值！");
            }
            if (model.CRITICAL_VALUE81 >= model.CRITICAL_VALUE82)
            {
                ModelState.AddModelError("CRITICAL_VALUE82", "橙色预警值必须大于黄色预警值！");
            }
            if (model.CRITICAL_VALUE82 >= model.CRITICAL_VALUE83)
            {
                ModelState.AddModelError("CRITICAL_VALUE83", "红色预警值必须大于橙色预警值！");
            }
            if (model.CRITICAL_VALUE91 >= model.CRITICAL_VALUE92)
            {
                ModelState.AddModelError("CRITICAL_VALUE92", "橙色预警值必须大于黄色预警值！");
            }
            if (model.CRITICAL_VALUE92 >= model.CRITICAL_VALUE93)
            {
                ModelState.AddModelError("CRITICAL_VALUE93", "红色预警值必须大于橙色预警值！");
            }
            if (!ModelState.IsValid)
            {
                Alter("提交失败！", AlterTypeEnum.Error, false, false);
                return View();
            }
            List<WARN_PARAM> paralist = new List<WARN_PARAM>();
            foreach (var t_dict in WARN_TYP_DICT)
            {
                foreach (var l_dict in WARN_LEVEL_DICT)
                {
                    WARN_PARAM param1 = new WARN_PARAM()
                    {
                        HALL_NO = hallno,
                        WARN_TYP = (byte)t_dict.Key,
                        WARN_LEVEL = (byte)l_dict.Key,
                        WARN_PARAM_NAM = t_dict.Value,
                        MODIFY_DTIME = DateTime.Now,
                        MODIFY_ID = UserState.UserID
                    };
                    string qkey1 = "CRITICAL_VALUE" + t_dict.Key + l_dict.Key;
                    if (!string.IsNullOrEmpty(Request[qkey1]))
                    {
                        var v = decimal.Parse(Request[qkey1]);
                        if (t_dict.Key == 2 || t_dict.Key == 4 || t_dict.Key == 5 || t_dict.Key == 7)//等候超时率，大厅饱和度，超时办结率
                        {
                            v = v / 100;
                        }
                        param1.CRITICAL_VALUE = v;
                    }
                    string qkey2 = "FRE_MIN" + t_dict.Key;
                    if (!string.IsNullOrEmpty(Request[qkey2]))
                        param1.FRE_MIN = int.Parse(Request[qkey2]);
                    string qkey3 = "WARN_INFO_MODEL" + t_dict.Key;
                    if (!string.IsNullOrEmpty(Request[qkey3]))
                        param1.WARN_INFO_MODEL = Request[qkey3];
                    paralist.Add(param1);
                }
            }
            var rst = new WARN_PARAM_DAL().AddList(paralist);
            if (!rst)
            {
                Alter("提交失败！", AlterTypeEnum.Error, false, false);
                return View();
            }
            Alter("提交成功！", AlterTypeEnum.Error, true, true);
            return RedirectToAction("Details", new { hallno = hallno });
        }

        [UserAuth("WARN_PARAM_ADD")]
        public ActionResult ParamSendUsr(string hallno, byte id)
        {
            var hall = DaoHall.GetEntity("HALL_NO", hallno);
            if (null != hall)
            {
                ViewBag.USERLIST = new SelectList(new SYS_USER_DAL().GetORGUser(hall.ORG_ID), "USER_ID", "USER_NAM");
                ViewBag.USERSELELIST = DaoParamUser.FindList("", "HALL_NO", hallno, "WARN_TYP", id).Select(obj => obj.USER_ID);
            }

            return View();
        }

        [HttpPost]
        [UserAuth("WARN_PARAM_ADD")]
        public ActionResult ParamSendUsr(string hallno, byte id, WARN_PARAM_SEND_USER_CON model)
        {
            if (!ModelState.IsValid)
            {
                Alter("提交失败！", AlterTypeEnum.Error, false, false);
                return View();
            }
            var USER_ID = Request.Form["USER_ID"];
            List<WARN_PARAM_SEND_USER_CON> models = new List<WARN_PARAM_SEND_USER_CON>();

            var ARR_USER_ID = USER_ID.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var hall = DaoHall.GetEntity("HALL_NO", hallno);
            if (null != hall)
            {
                var USERLIST = new SYS_USER_DAL().GetORGUser(hall.ORG_ID);
                foreach (var _USERID in ARR_USER_ID)
                {
                    model.WARN_LEVEL = 1;
                    model.USER_ID = _USERID;
                    model.MOB_NBR = USERLIST.FirstOrDefault(obj => obj.USER_ID == _USERID).TEL;
                    models.Add(model);

                    var model2 = CommonHelper.DeepClone(model);
                    model2.WARN_LEVEL = 2;
                    model.USER_ID = _USERID;
                    model.MOB_NBR = USERLIST.FirstOrDefault(obj => obj.USER_ID == _USERID).TEL;
                    models.Add(model2);

                    var model3 = CommonHelper.DeepClone(model);
                    model3.WARN_LEVEL = 3;
                    model.USER_ID = _USERID;
                    model.MOB_NBR = USERLIST.FirstOrDefault(obj => obj.USER_ID == _USERID).TEL;
                    models.Add(model3);
                }
            }
            new WARN_PARAM_DAL().AddParamUserList(models);
            Alter("提交成功！", AlterTypeEnum.Error, true, true);
            return RedirectToAction("ParamSendUsr", new { hallno = hallno,id = id});
        }
    }

    public class Model_WARN_PARAM
    {
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE11 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE12 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE13 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public int FRE_MIN1 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public string WARN_INFO_MODEL1 { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        [Range(0,100)]
        public decimal CRITICAL_VALUE21 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        [Range(0, 100)]
        public decimal CRITICAL_VALUE22 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        [Range(0, 100)]
        public decimal CRITICAL_VALUE23 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public int FRE_MIN2 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public string WARN_INFO_MODEL2 { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE31 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE32 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE33 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public int FRE_MIN3 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public string WARN_INFO_MODEL3 { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE41 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE42 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE43 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public int FRE_MIN4 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public string WARN_INFO_MODEL4 { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE51 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE52 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE53 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public int FRE_MIN5 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public string WARN_INFO_MODEL5 { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE61 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE62 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE63 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public int FRE_MIN6 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public string WARN_INFO_MODEL6 { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE71 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE72 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE73 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public int FRE_MIN7 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public string WARN_INFO_MODEL7 { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE81 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE82 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE83 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public int FRE_MIN8 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public string WARN_INFO_MODEL8 { get; set; }

        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE91 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE92 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public decimal CRITICAL_VALUE93 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public int FRE_MIN9 { get; set; }
        [Required(ErrorMessage = "不能为空！")]
        public string WARN_INFO_MODEL9 { get; set; }
    }
}
