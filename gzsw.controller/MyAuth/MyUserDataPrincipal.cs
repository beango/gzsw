using gzsw.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Script.Serialization;
using gzsw.util;

namespace gzsw.controller.MyAuth
{
    //存放数据的用户实体
    public class MyUserDataPrincipal : IPrincipal
    {
        public UserState UserState { get; set; }

        //当使用Authorize特性时，会调用改方法验证角色 
        public bool IsInRole(string role)
        {
            return false;
        }

        //验证用户信息
        public bool IsInFunc(string func)
        {
            if (string.IsNullOrEmpty(func))
                return true;
            if (UserState.UserID == "admin")
                return true;
            var funcs = func.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return funcs.Any(r => UserState.UserFuncs.Any(obj => obj.FUNCTION_COD + "_" + ((ActionEnum)obj.FUNCTION_TYP).ToString() == r));
        }

        [ScriptIgnore]    //在序列化的时候忽略该属性
        public IIdentity Identity { get { return null; } }
    }
}
