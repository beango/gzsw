using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web.Security;

namespace gzsw.controller.MyAuth
{
    public class MyFormsPrincipal<T> : IPrincipal
        where T : class, new()
    {
        //当前用户实例
        public IIdentity Identity { get; private set; }
        //用户数据
        public T UserState { get; private set; }

        public MyFormsPrincipal(FormsAuthenticationTicket ticket, T userState)
        {
            if (ticket == null)
                throw new ArgumentNullException("ticket");
            if (userState == null)
                throw new ArgumentNullException("userState");

            Identity = new FormsIdentity(ticket);
            UserState = userState;
        }

        //角色验证
        public bool IsInRole(string role)
        {
            var userData = UserState as MyUserDataPrincipal;
            if (userData == null)
                throw new NotImplementedException();

            return userData.IsInRole(role);
        }

        //权限验证
        public bool IsInFunc(string func)
        {
            var userData = UserState as MyUserDataPrincipal;
            if (userData == null)
                throw new NotImplementedException();

            return userData.IsInFunc(func);
        }
    }
}
