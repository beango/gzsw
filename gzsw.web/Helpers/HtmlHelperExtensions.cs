
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using gzsw.util;

namespace gzsw.web
{
    /// <summary>
    /// HtmlHelper 扩展
    /// </summary>
    /// <remarks>
    ///     <para>    Creator：hcli</para>
    ///     <para>CreatedTime：2013/7/29 18:17:52</para>
    /// </remarks>
    public static class HtmlHelperExtensions
    {

        #region AppSettings Config

        /// <summary>
        /// 根节点
        /// </summary>
        private const string BaseRootPath = "~/";

        /// <summary>
        /// 版本号
        /// </summary>
        private static readonly string Version = ConfigurationManager.AppSettings["Version"];

        /// <summary>
        /// 地图核心CODE JS
        /// </summary>
        private static readonly string MapCodeJs = ConfigurationManager.AppSettings["MapCodeJs"];

        /// <summary>
        /// 地图扩展 JS
        /// </summary>
        private static readonly string MapExJs = ConfigurationManager.AppSettings["MapExJs"];

        /// <summary>
        /// 地图样式
        /// </summary>
        private static readonly string MapCss = ConfigurationManager.AppSettings["MapCss"];


        #endregion


        /// <summary>
        /// 生成CSS的链接
        /// </summary>
        /// <param name="html"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static MvcHtmlString StyleSheet(this HtmlHelper html, string fileName)
        {
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            return StyleSheetForFullPath(html,
                urlHelper.Content(BaseRootPath + "content/" + fileName));
        }


        /// <summary>
        /// 生成CSS的链接
        /// </summary>
        /// <param name="html"></param>
        /// <param name="path">全路径</param>
        /// <returns></returns>
        public static MvcHtmlString StyleSheetForFullPath(this HtmlHelper html, string path)
        {

            var tagBuilder = new TagBuilder("link");
            tagBuilder.MergeAttribute("href", path.GetVersionPar(Version));
            tagBuilder.MergeAttribute("rel", "stylesheet");
            tagBuilder.MergeAttribute("type", "text/css");
            return MvcHtmlString.Create(HttpUtility.HtmlDecode(tagBuilder.ToString()));
        }



        /// <summary>
        /// 生成Scrip的链接
        /// </summary>
        /// <param name="html"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static MvcHtmlString JavaScript(this HtmlHelper html, string fileName)
        {
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            return JavaScriptForFullPath(html,
                urlHelper.Content(BaseRootPath + "content/" + fileName));
        }


        /// <summary>
        /// 生成Scrip的链接
        /// </summary>
        /// <param name="html"></param>
        /// <param name="path">全路径</param>
        /// <returns></returns>
        public static MvcHtmlString JavaScriptForFullPath(this HtmlHelper html, string path)
        {

            var tagBuilder = new TagBuilder("script");
            tagBuilder.MergeAttribute("src", path.GetVersionPar(Version));
            tagBuilder.MergeAttribute("type", "text/javascript");
            return MvcHtmlString.Create(HttpUtility.HtmlDecode(tagBuilder.ToString()));
        }


        /// <summary>
        /// 生成Url的地址脚本
        /// </summary>
        /// <param name="html"></param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static MvcHtmlString Url(this HtmlHelper html, string filePath)
        {
            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            return MvcHtmlString.Create(urlHelper.Content(BaseRootPath + filePath));
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString GetVersionNo(this HtmlHelper html)
        {
            return new MvcHtmlString(Version);
        }

        /*  public static MvcHtmlString GetMenu(this HtmlHelper html){ 
           
          }*/


        /// <summary>
        /// 加载地图JS
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString GetMapJS(this HtmlHelper html)
        {
            var styleTagBuilder = new TagBuilder("link");
            styleTagBuilder.MergeAttribute("href", Url(html, MapCss.GetVersionPar(Version)).ToString());
            styleTagBuilder.MergeAttribute("rel", "stylesheet");
            styleTagBuilder.MergeAttribute("type", "text/css");

            var scriptTagBuilder1 = new TagBuilder("script");
            scriptTagBuilder1.MergeAttribute("src", Url(html, MapCodeJs.GetVersionPar(Version)).ToString());
            scriptTagBuilder1.MergeAttribute("type", "text/javascript");

            var scriptTagBuilder2 = new TagBuilder("script");
            scriptTagBuilder2.MergeAttribute("src", Url(html, MapExJs.GetVersionPar(Version)).ToString());
            scriptTagBuilder2.MergeAttribute("type", "text/javascript");

            var allPath = styleTagBuilder.ToString() + scriptTagBuilder1.ToString() + scriptTagBuilder2.ToString();
            return MvcHtmlString.Create(HttpUtility.HtmlDecode(allPath));
        }


        /// <summary>  
        /// 分页Pager显示  
        /// </summary>   
        /// <param name="html"></param>  
        /// <param name="currentPageStr">标识当前页码的QueryStringKey</param>    
        /// <param name="pageSize">每页显示</param>  
        /// <param name="totalCount">总数据量</param>
        /// <param name="isUseEncrypt">是否使用加密</param>
        /// <param name="requestUrl">请求URL</param>
        /// <param name="isUpDownPage">是否显示上一页下一页</param>
        /// <returns></returns> 
        public static MvcHtmlString Pager(
            this HtmlHelper html,
            string currentPageStr,
            long pageSize,
            long totalCount,
            bool isUseEncrypt = false,
            string requestUrl = null,
            bool isUpDownPage = true)
        {

            // 当前请求
            var request = HttpContext.Current.Request;

            var requestQueryStr = request.QueryString;
            var parameter = requestQueryStr.AllKeys.
                                            Where(
                                                str =>
                                                !string.IsNullOrEmpty(str) && str.ToLower() != currentPageStr.ToLower())
                                           .
                                            Aggregate("",
                                                      (current, str) =>
                                                      current + string.Format("&{0}={1}", str, requestQueryStr[str]));

            // Url地址
            var url = request.CurrentExecutionFilePath;


            // 当前页码
            var currentPage = 1;

            if (isUseEncrypt)
            {
                currentPage = CryptTools.PagerNumDecrypt(request[currentPageStr]);
            }
            else
            {
                if (request[currentPageStr] != null)
                {

                    int.TryParse(request[currentPageStr], out currentPage);
                }
            }

            // 总页数
            var totalPages = Math.Max((totalCount + pageSize - 1) / pageSize, 1);


            if (currentPage <= 0)
            {
                currentPage = 1;
            }

            var output = new StringBuilder("<div class=\"l-panel-bar\"> <div class=\"l-panel-bbar-inner\">");

            output.Append("<div class=\"l-bar-group  l-bar-message\">");
            output.Append(string.Format("<span class=\"l-bar-text\">当前第{0}页/共{1}页 (共:{2}条记录/每页显示:{3})</span>",
                currentPage,
                totalPages,
                totalCount,
                pageSize));
            output.Append("</div>");

            output.Append("<div class=\"l-bar-group\">");
            output.Append("<select name=\"rp\">");
            output.Append("<option value=\"20\">20</option>");
            output.Append("<option value=\"30\">30</option>");
            output.Append("<option value=\"40\">40</option>");
            output.Append("<option value=\"50\">50</option>");
            output.Append("</select>");
            output.Append("</div>");

            output.Append("<div class=\"l-bar-separator\"></div>");

            output.Append("<div class=\"l-bar-group\">");
            output.Append("<div class=\"l-bar-button l-bar-btnfirst\">");
            output.Append(string.Format("<span tag=\"firstBtn\" title=\"{0}\"  class=\"{1}\"></span>", "首页", currentPage == 1 ? "l-disabled" : ""));
            output.Append("</div>");
            output.Append(string.Format("<div class=\"l-bar-button l-bar-btnprev\"><span title=\"上一页\"  class=\"{0}\" tag=\"upBtn\"></span></div>", currentPage == 1?"l-disabled":""));
            output.Append("</div>");
            output.Append("<div class=\"l-bar-separator\"></div>");

            output.Append("<div class=\"l-bar-group\">");
            output.Append("<span class=\"pcontrol\">");
            output.Append(string.Format("<input type=\"text\" tag=\"num\" maxvalue=\"{0}\" size=\"4\" value=\"{1}\" style=\"width:30px;height:14px;\"> / <span>{2}</span>",
                totalPages, currentPage, totalPages));
            output.Append("</span></div>");

            output.Append("<div class=\"l-bar-separator\"></div>");

            output.Append("<div class=\"l-bar-group\">");

            output.Append(
                string.Format(
                    "<div class=\"l-bar-button l-bar-btnnext\"><span tag=\"nextBtn\" class=\"{0}\" title=\"下一页\" ></span></div>",
                    currentPage >= totalPages ? "l-disabled" : ""));
            output.Append(string.Format("<div class=\"l-bar-button l-bar-btnlast\"><span class=\"{0}\" title=\"尾页\" tag=\"endBtn\"></span></div>",currentPage >= totalPages ? "l-disabled" : ""));
            output.Append("</div>");

            output.Append("<div class=\"l-bar-group\">");
            output.Append(string.Format("<div class=\"l-bar-group\"><div class=\"l-bar-button l-bar-btnload\"><span tag=\"refresh\" title=\"刷新\"></span></div></div>"));
            output.Append("<div class=\"l-bar-group\"><div class=\"l-clear\"></div>");
            output.Append("</div></div>");

            output.Append("</div></div>");

            // 新增处理脚本依赖jqueryHelper.js
            output.Append("<script type=\"text/javascript\">$(function () { $(\".l-panel-bar\").pager(); });</script>");
               
            return new MvcHtmlString(output.ToString());
            /* 
                        var output = new StringBuilder("<div pagerId='pageNav' class='pageNav'>");

                            //处理首页连接    
                            output.AppendFormat("<a href='{0}?{1}={2}{3}'>{4}</a>", url, currentPageStr,
                                                (isUseEncrypt ? HttpUtility.UrlEncode(CryptTools.PagerNumEncrypt(1)) : "1"),
                                                parameter, "首页");
                            output.Append(" ");
                            int currint = 5;
                            var startNum = currentPage - currint <= 1 ? 1 : currentPage - currint;
                            var endNum = startNum + (currint * 2) > totalPages ? totalPages : (startNum + (currint * 2));

                             output.Append(
                                 string.Format("<div style=\"float: left;font-weight: normal;font-size: 10pt;color: #097cae;padding-left: 5px;\">当前第{0}页/共{1}页 (共{2}条记录)</div>",
                                 currentPage, totalPages, totalCount));

                            for (int i = startNum; i <= endNum; i++)
                            {
                                if (i == currentPage)
                                {
                                    output.Append(string.Format("<strong>{0}</strong>", currentPage));
                                }
                                else
                                {
                                    if (isUseEncrypt)
                                    {
                                        //需要加密 
                                        //一般页处理  
                                        output.AppendFormat("<a href='{0}?{1}={2}{3}'>{4}</a>",
                                                            url,
                                                            currentPageStr,
                                                            HttpUtility.UrlEncode(CryptTools.PagerNumEncrypt(i)),
                                                            parameter,
                                                            i);
                                    }
                                    else
                                    {

                                        //一般页处理  
                                        output.AppendFormat("<a href='{0}?{1}={2}{3}'>{4}</a>",
                                                            url,
                                                            currentPageStr,
                                                            i,
                                                            parameter,
                                                            i);
                                    }

                                }

                            }
                            output.Append(" ");
                            if (isUseEncrypt)
                            {
                                output.AppendFormat("<a href='{0}?{1}={2}{3}'>{4}</a>",
                                                    url,
                                                    currentPageStr,
                                                    HttpUtility.UrlEncode(CryptTools.PagerNumEncrypt(totalPages)),
                                                    parameter,
                                                    "末页");
                            }
                            else
                            {
                                output.AppendFormat("<a href='{0}?{1}={2}{3}'>{4}</a>",
                                         url,
                                         currentPageStr,
                                         totalPages,
                                         parameter,
                                        "末页");
                            }

                            output.Append(" ");
                            output.Append("</div>");
            return new MvcHtmlString(output.ToString());*/
        }

        /// <summary>
        /// 生成面包屑
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString TitleBaner(
            this HtmlHelper html, string titles)
        {
            StringBuilder str = new StringBuilder("<div class=\"place\">");
            str.Append("    <span>位置：</span>");
            str.Append("    <ul class=\"placeul\">");
            str.Append("        <li><span  >首页</span></li>");
            foreach (var title in titles.Split(','))
            {
                str.AppendFormat("        <li><span>{0}</span></li>", title);
            }
            str.Append("    </ul>");
            str.Append("</div>");
            return new MvcHtmlString(str.ToString());
        }


    }



}