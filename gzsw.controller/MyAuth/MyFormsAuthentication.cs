﻿using gzsw.util;
using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace gzsw.controller.MyAuth
{
    //身份验证类
    public class MyFormsAuthentication<TUserData> where TUserData : class, new()
    {
        //Cookie保存是时间
        private const int CookieSaveMinutes = 1800;

        //用户登录成功时设置Cookie
        public static void SetAuthCookie(string username, TUserData userData, bool rememberMe)
        {
            if (userData == null)
                throw new ArgumentNullException("userData");

            var data = (new JavaScriptSerializer()).Serialize(userData);

            //创建ticket
            var ticket = new FormsAuthenticationTicket(
                2, username, DateTime.Now, DateTime.Now.AddMinutes(CookieSaveMinutes), rememberMe, data);

            //加密ticket
            var cookieValue = FormsAuthentication.Encrypt(ticket);

            //创建Cookie
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookieValue)
            {
                HttpOnly = true,
                Secure = FormsAuthentication.RequireSSL,
                Domain = FormsAuthentication.CookieDomain,
                Path = FormsAuthentication.FormsCookiePath,
            };
            if (rememberMe)
                cookie.Expires = DateTime.Now.AddDays(CookieSaveMinutes);

            //写入Cookie
            HttpContext.Current.Response.Cookies.Remove(cookie.Name);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        //从Request中解析出Ticket,UserData
        public static MyFormsPrincipal<TUserData> TryParsePrincipal(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            // 1. 读登录Cookie
            var cookie = request.Cookies[FormsAuthentication.FormsCookieName];
            if (cookie == null || string.IsNullOrEmpty(cookie.Value)) return null;

            try
            {
                // 2. 解密Cookie值，获取FormsAuthenticationTicket对象
                var ticket = FormsAuthentication.Decrypt(cookie.Value);
                if (ticket != null && !string.IsNullOrEmpty(ticket.UserData))
                {
                    var userData = (new JavaScriptSerializer()).Deserialize<TUserData>(ticket.UserData);
                    if (userData != null)
                    {
                        return new MyFormsPrincipal<TUserData>(ticket, userData);
                    }
                }
                return null;
            }
            catch(Exception ex)
            {
                LogHelper.ErrorLog("MyFormsPrincipal", ex);
                /* 有异常也不要抛出，防止攻击者试探。 */
                return null;
            }
        }
    }
}
