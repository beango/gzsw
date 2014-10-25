using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace gzsw.util
{
    /*
    * Http Cookie辅助类
    */
    public class CookieHelper
    {
        //设置Cookies
        public void SetCookie(string key, string value)
        {
            HttpCookie Cookie = new HttpCookie(key);
            Cookie.Value = value;
            //HttpContext.Current.Response.AppendCookie(Cookie);
            System.Web.HttpContext.Current.Response.Cookies.Add(Cookie);
        }

        //设置Cookies
        public void SetCookie(string key, string value, DateTime Expires)
        {
            HttpCookie Cookie = new HttpCookie(key);
            Cookie.Value = value;
            Cookie.Expires = Expires;
            HttpContext.Current.Response.Cookies.Add(Cookie);
        }

        //获取Cookie
        public string GetCookie(string key)
        {
            var httpCookie = HttpContext.Current.Request.Cookies[key];
            if (httpCookie != null) return httpCookie.Value;
            return null;
        }

        public void RemoveCookie(string key)
        {
            var httpCookie = HttpContext.Current.Request.Cookies[key];
            if (httpCookie != null)
            {
                httpCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.AppendCookie(httpCookie);
            }
        }
    }
}
