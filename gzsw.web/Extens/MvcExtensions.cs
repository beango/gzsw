using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using gzsw.controller.MyAuth;
using gzsw.util;
using gzsw.model;
using System.Web.Routing;

namespace System.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        //private static bool Visible(HtmlHelper helper, SYS_FUNCTION accountType)
        //{
        //    return new UserAuthAttribute().PerformAuthorizeCore(helper.ViewContext.HttpContext);
        //}

        /// <summary>
        /// Returns an anchor element (a element) that contains the virtual path of the specified action.
        /// </summary>
        /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
        /// <param name="html"></param>
        /// <param name="linkText">The inner text of the anchor element.</param>
        /// <param name="actionName">The name of the action.</param>
        /// <param name="controllerName">The name of the controller.</param>
        /// <param name="accountTypeRequired">The required account type.</param>
        /// <param name="funcs"></param>
        /// <returns>The anchor element (a element) that contains the virtual path of the specified action.</returns>
        //public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper,
        //    string linkText,
        //    string actionName,
        //    string controllerName,
        //    SYS_FUNCTION accountTypeRequired)
        //{
        //    MvcHtmlString link = MvcHtmlString.Empty;
        //    if (Visible(htmlHelper, accountTypeRequired))
        //        link = htmlHelper.ActionLink(linkText, actionName);
        //    return link;
        //}

        public static MvcHtmlString AuthActionLink(this HtmlHelper html, string linkText, string actionName, string funcs)
        {
            return ChkAuthorize(html, funcs)
                   ? html.ActionLink(linkText, actionName, new object())
                   : MvcHtmlString.Empty;
        }


        public static MvcHtmlString AuthActionLink(this HtmlHelper html, string linkText, string actionName, string funcs, object routeValues)
        {
            return ChkAuthorize(html, funcs)
                   ? html.ActionLink(linkText, actionName, routeValues)
                   : MvcHtmlString.Empty;
        }

        public static MvcHtmlString AuthActionLink(
            this HtmlHelper html,
            string linkText,
            string actionName,
            string funcs,
            object routeValues,
            object htmlattribute)
        {
            return ChkAuthorize(html, funcs)
                   ? html.ActionLink(linkText, actionName, routeValues, htmlattribute)
                   : MvcHtmlString.Empty;
        }

        public static MvcHtmlString AuthActionLink(
           this HtmlHelper html,
           string linkText,
           string actionName,
           string controllerName,
           string funcs,
           object routeValues,
           object htmlattribute)
        {
            return ChkAuthorize(html, funcs)
                   ? html.ActionLink(linkText, actionName, controllerName, routeValues, htmlattribute)
                   : MvcHtmlString.Empty;
        }

        public static MvcHtmlString AuthViewButton(
           this HtmlHelper html, string funcs, string url, string text = "查看", int width = 450, int height = 360)
        {
            if (!ChkAuthorize(html, funcs))
                return MvcHtmlString.Empty;
            return new MvcHtmlString(@"{
                text: '" + text + @"',
                click: function (item) {
                    if ($('.tablelist').getSelCount() != 1) {
                        gzsw.dialog.alert('请选择需要操作的单个选项!');
                    } else {
                        var url = '" + url + (url.IndexOf("?") > -1 ? "&" : "?") + @"id=' + $('.tablelist').getSelValue();
                        gzsw.dialog.open({
                            url: url.urlstamp(),
                            title : '" +text+@"',
                            width: " + width + @",
                            height: " + height + @",
                            isReload: false
                        });
                    }
                },
                icon: 'archives'
            }");
        }

        public static MvcHtmlString AuthAddButton(
          this HtmlHelper html, string funcs, string url, string text = "新增", int width = 450, int height = 360)
        {
            if (!ChkAuthorize(html, funcs))
                return MvcHtmlString.Empty;
            return new MvcHtmlString(@"{
                        text: '" + text + @"',
                        click: function (item) {
                            gzsw.dialog.open({
                                url: '" + url + @"',
                                title : '" + text + @"',
                                width: " + width + @",
                                height: " + height + @",
                                isReload: true
                            });
                        },
                        icon: 'add'
                    }");
        }

        public static MvcHtmlString AuthEditButton(
          this HtmlHelper html, string funcs, string url, string text = "修改", int width = 450, int height = 360)
        {
            if (!ChkAuthorize(html, funcs))
                return MvcHtmlString.Empty;
            return new MvcHtmlString(@"{
                        text: '" + text + @"',
                        click: function () {
                            if ($('.tablelist').getSelCount() != 1) {
                                gzsw.dialog.alert('请选择需要操作的单个选项!');
                            } else {
                                var url = '" + url + @"?id=' + $('.tablelist').getSelValue();
                                gzsw.dialog.open({
                                    url: url.urlstamp(),
                                    title : '" + text + @"',
                                    width: " + width + @",
                                    height: " + height + @",
                                });
                            }
                        },
                        icon: 'modify'
                    }");
        }

        public static MvcHtmlString AuthDelButton(
          this HtmlHelper html, string funcs, string url, int width = 450, int height = 360)
        {
            if (!ChkAuthorize(html, funcs))
                return new MvcHtmlString("{line:''}");

            return new MvcHtmlString(@"{
                        text: '删除',
                        click: function () {
                            if ($('.tablelist').getSelCount() <= 0) {
                                gzsw.dialog.alert('请选择需要删除的项!');
                            } else {
                                gzsw.dialog.confirm({
                                    msg: '是否删除选中项？',
                                    yesFun: function () {
                                        var url = '" + url + (url.IndexOf("?") > -1 ? "&" : "?") + @"id=' + $('.tablelist').getSelValue();
                                        location.href = url.urlstamp();
                                    }
                                });
                            }
                        },
                        icon: 'delete'
                    }");
        }

        /// <summary>
        /// Auth LigerUIButton
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url">请求的地址</param>
        /// <param name="linkText">显示的文字</param>
        /// <param name="funcs">权限编码</param>
        /// <param name="icon">图标</param>
        /// <param name="whereType">点击时的操作类型，0不做判断，1判断只能选一条，2最少要选一条，默认0</param>
        /// <param name="isReload">是否在关闭弹窗刷新页面</param>
        /// <param name="isOpen">是否打开弹窗</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="primaryKey">URL参数,如果以@:开头就是JS参数</param>
        /// <returns></returns>
        public static MvcHtmlString LigerUIButton(this HtmlHelper helper, string url, string linkText, string funcs, string icon,
            int whereType = 0, bool isReload = true, bool isOpen = true, int width = 450, int height = 360, params string[] primaryKey)
        {
            if (!ChkAuthorize(helper, funcs))
            {
                return MvcHtmlString.Empty;
            }
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append("text: '" + linkText + "',");
            sb.Append("click: function(item) {");
            //条件类型，用于判断是否要条件
            if (whereType == 0)
            {
                //不判断
                sb.Append(openDiaog(url,linkText, isOpen, isReload, width, height, primaryKey));
            }
            else if (whereType == 1)
            {
                //判断是否只选了一条
                sb.Append("if ($(\".tablelist\").getSelCount() != 1) { gzsw.dialog.alert(\"请选择需要操作的单个选项!\"); return false; }");
                sb.Append(openDiaog(url, linkText, isOpen, isReload, width, height, primaryKey));
            }
            else if (whereType == 2)
            {
                //最少选择一项
                sb.Append("if ($(\".tablelist\").getSelCount() <= 0) { gzsw.dialog.alert(\"请选择需要操作的项!\");return false; }");
                sb.Append(confirm(url, primaryKey));
            }
            else if (whereType==3)
            {
                //撤销
                sb.Append("if ($(\".tablelist\").getSelCount() <= 0) { gzsw.dialog.alert(\"请选择需要操作的项!\");return false; }");
                sb.Append(confirmtype2(url, primaryKey)); 
            }
            sb.Append("}");

            if (!string.IsNullOrEmpty(icon))
            {
                sb.Append(",icon:'" + icon + "'");
            }

            sb.Append("}");
            return MvcHtmlString.Create(sb.ToString());
        }


        public static MvcHtmlString LigerUIViewButton(this HtmlHelper helper, string url, string linkText, string funcs,
            int width = 450, int height = 360, params string[] primaryKey)
        {
            if (primaryKey == null)
            {
                primaryKey = new string[] { "id", "@:$('.tablelist').getSelValue()" };
            }
            else
            {
                var list = primaryKey.ToList();
                list.Add("id");
                list.Add("@:$('.tablelist').getSelValue()");
                primaryKey = list.ToArray();
            }
            return LigerUIButton(helper, url, linkText, funcs, "archives", 1, false, true, width, height, primaryKey);
        }

        public static MvcHtmlString LigerUIAddButton(this HtmlHelper helper, string url, string linkText, string funcs,
            int width = 450, int height = 360, params string[] primaryKey)
        {
            return LigerUIButton(helper, url, linkText, funcs, "add", 0, true, true, width, height, primaryKey);
        }

        public static MvcHtmlString LigerUIEditButton(this HtmlHelper helper, string url, string linkText, string funcs,
            int width = 450, int height = 360, params string[] primaryKey)
        {
            if (primaryKey == null)
            {
                primaryKey = new string[] { "id", "@:$('.tablelist').getSelValue()" };
            }
            else
            {
                var list = primaryKey.ToList();
                list.Add("id");
                list.Add("@:$('.tablelist').getSelValue()");
                primaryKey = list.ToArray();
            }
            return LigerUIButton(helper, url, linkText, funcs, "modify", 1, true, true, width, height, primaryKey);
        }

        public static MvcHtmlString LigerUIDeleteButton(this HtmlHelper helper, string url, string linkText, string funcs,
             params string[] primaryKey)
        {
            if (primaryKey == null)
            {
                primaryKey = new string[] { "id", "@:$('.tablelist').getSelValue()" };
            }
            else
            {
                var list = primaryKey.ToList();
                list.Add("id");
                list.Add("@:$('.tablelist').getSelValue()");
                primaryKey = list.ToArray();
            }
            return LigerUIButton(helper, url, linkText, funcs, "delete", 2, true, true, 0, 0, primaryKey);
        }

        public static MvcHtmlString LigerUIcancelButton(this HtmlHelper helper, string url, string linkText, string funcs,
             params string[] primaryKey)
        {
            if (primaryKey == null)
            {
                primaryKey = new string[] { "id", "@:$('.tablelist').getSelValue()" };
            }
            else
            {
                var list = primaryKey.ToList();
                list.Add("id");
                list.Add("@:$('.tablelist').getSelValue()");
                primaryKey = list.ToArray();
            }
            return LigerUIButton(helper, url, linkText, funcs, "delete", 3, true, true, 0, 0, primaryKey);
        }
        private static string confirm(string url, params string[] primaryKey)
        {
            var sb = new StringBuilder();
            var param = getUrl(url, primaryKey);
            sb.Append("gzsw.dialog.confirm({ \n");
            sb.Append("msg: '是否删除选中项？', \n");
            sb.Append("yesFun: function () { \n");
            sb.Append("var url='" + url + "'" + param + ";");
            sb.Append("location.href = url.urlstamp();");
            sb.Append("}");
            sb.Append("});");
            return sb.ToString();
        }
        private static string confirmtype2(string url, params string[] primaryKey)
        {
            var sb = new StringBuilder();
            var param = getUrl(url, primaryKey);
            sb.Append("gzsw.dialog.confirm({ \n");
            sb.Append("msg: '是否撤销选中项？', \n");
            sb.Append("yesFun: function () { \n");
            sb.Append("var url='" + url + "'" + param + ";");
            sb.Append("location.href = url.urlstamp();");
            sb.Append("}");
            sb.Append("});");
            return sb.ToString();
        }
        private static string openDiaog(string url,string title, bool isOpen = true, bool isReload = true, int width = 450, int height = 360, params string[] primaryKey)
        {
            var sb = new StringBuilder();
            var param = getUrl(url, primaryKey);
            if (isOpen)
            {
                sb.Append("var url='" + url + "'" + param + ";");
                sb.Append("gzsw.dialog.open({");
                sb.Append("url: url.urlstamp(),");
                sb.Append("title: '" + title + "',");
                sb.Append("width: " + width + ",");
                sb.Append("height:" + height);
                if (!isReload)
                {
                    sb.Append(",isReload: false");
                }
                sb.Append("});");
            }
            else
            {
                sb.Append("var url='" + url + "'" + param + ";\n location.href=url.urlstamp();");
            }

            return sb.ToString();
        }

        private static string getUrl(string url, params string[] paras)
        {
            var sql = new StringBuilder();
            if (null == paras || paras.Length < 1) return "";
            var isUrl = url.Contains("?");
            for (var i = 0; i < paras.Length; i += 2)
            {
                if (isUrl)
                {
                    if (paras[i + 1].IndexOf("@:", StringComparison.Ordinal) == 0)
                    {
                        sql.Append("+'&" + paras[i] + "='+" + paras[i + 1].Replace("@:", ""));
                    }
                    else
                    {
                        sql.Append("+'&" + paras[i] + "=" + paras[i + 1] + "'");
                    }
                }
                else
                {
                    isUrl = true;
                    if (paras[i + 1].IndexOf("@:", StringComparison.Ordinal) == 0)
                    {
                        sql.Append("+'?" + paras[i] + "='+" + paras[i + 1].Replace("@:", ""));
                    }
                    else
                    {
                        sql.Append("+'?" + paras[i] + "=" + paras[i + 1] + "'");
                    }
                }
            }
            return sql.ToString();
        }

        public static MvcHtmlString AuthActionLink2(this HtmlHelper html, string linkText, string actionName, string funcs, object routeValues, object htmlattribute)
        {
            string str = String.Format("<li class=\"click\">{0}</li>", linkText);
            return ChkAuthorize(html, funcs)
                   ? new MvcHtmlString(str)
                   : MvcHtmlString.Empty;
        }

        public static bool ChkAuth(this HtmlHelper html, string funcs)
        {
            return ChkAuthorize(html, funcs);
        }

        private static bool ChkAuthorize(HtmlHelper html, string funcs)
        {
            var user = html.ViewContext.HttpContext.User as MyFormsPrincipal<MyUserDataPrincipal>;
            if (user != null)
                return user.IsInFunc(funcs);

            return false;
        }

        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> items
            , string[] selected = null)
        {
            var str = new StringBuilder();
            str.Append(@"<div class=""checkboxlist"">");

            foreach (var item in items)
            {
                str.Append(@"<div class=""list""><input type=""checkbox"" name=""");
                str.Append(name);
                str.Append("\" value=\"");
                str.Append(item.Value);
                str.Append("\"");

                if (item.Selected || (null != selected && selected.Contains(item.Value)))
                    str.Append(@" checked=""chekced""");

                str.Append(" />");
                str.Append(item.Text);
                str.Append("</div>");
            }

            str.Append("</div>");

            return MvcHtmlString.Create(str.ToString());
        }

        public static MvcHtmlString RadioButtonList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> items
            , string[] selected = null)
        {
            var str = new StringBuilder();
            str.Append(@"<div class=""radiobuttonlist"">");

            foreach (var item in items)
            {
                str.Append(@"<div class=""item""><input type=""radio"" name=""");
                str.Append(name);
                str.Append("\" value=\"");
                str.Append(item.Value);
                str.Append("\"");

                if (item.Selected || (null != selected && selected.Contains(item.Value)))
                    str.Append(@" checked=""chekced""");

                str.Append(" />");
                str.Append(item.Text);
                str.Append("</div>");
            }

            str.Append("</div>");

            return MvcHtmlString.Create(str.ToString());
        }
    }
}
