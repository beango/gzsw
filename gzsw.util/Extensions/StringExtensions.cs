 

using System;
using System.Data;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace gzsw.util
{
    /// <summary>
    /// String 的扩展类
    /// </summary>
    /// <remarks>
    ///     <para>    Creator：LHC</para>
    ///     <para>CreatedTime：2013/7/26 11:32:46</para>
    /// </remarks>
    public static class StringExtensions
    {
        /// <summary>
        /// 去除SQL非法字符扩展
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetSqlCheckStr(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            string strBadChar, tempChar;
            string[] arrBadChar;
            strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\"";
            arrBadChar = strBadChar.Split( ',');
            tempChar = str;
            for (int i = 0; i < arrBadChar.Length; i++)
            {
                if (arrBadChar[i].Length > 0)
                    tempChar = tempChar.Replace(arrBadChar[i], "");
            }
            return tempChar;
        }

        /// <summary>
        ///  获取版本号
        /// </summary>
        /// <param name="str">当前字符串</param>
        /// <param name="versionNo">版本号</param>
        /// <param name="versionName">版本号名车</param>
        /// <returns>是否是数值类型</returns>
        public static string GetVersionPar(
            this string str,
            string versionNo,
            string versionName = "Version")
        {
            return string.Format(IsHaveUrlParameter(str)
                ? "{0}&{1}={2}"
                : "{0}?{1}={2}",
                str,
                versionName,
                versionNo);
        }
 



        /// <summary>
        ///  获取地址参数
        /// </summary>
        /// <param name="str">当前字符串</param> 
        /// <returns>是否是数值类型</returns>
        public static string GetParStr( this string str)
        {
            
            return string.Format(IsHaveUrlParameter(str)
                ? "{0}&{1}={2}"
                : "{0}?{1}={2}",
                str,
                "t",
                DateTime.Now.ToString("yyyyMMddHHmmssff"));
        }

        /// <summary>
        /// Url是否有参数
        /// </summary>
        /// <param name="str">当前字符串</param>
        /// <returns></returns>
        public static bool IsHaveUrlParameter(this string str)
        {
            return str.IndexOf('?') > 0;
        }


        /// <summary>
        /// Long类型转换
        /// </summary>
        /// <param name="str">需要转换的字符串</param>
        /// <returns></returns>
        public static long ToTryLong(this string str)
        {
            long i;
            long.TryParse(str, out i);
            return i;
        }
 
        /// <summary>
        /// 转换价格格式类型
        /// </summary>
        /// <param name="price">需要转换的金额</param>
        /// <returns></returns>
        public static string TryToPrice(this decimal price)
        {
            return string.Format("{0:C}", price);
        }

        /// <summary>
        /// 转换时间格式类型
        /// </summary>
        /// <param name="dateTime">需要转换的时间</param>
        /// <returns></returns>
        public static string TryToDateTime(this DateTime dateTime)
        {
            return dateTime.Year < 1910 ? "" : dateTime.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 转换时间格式类型
        /// </summary>
        /// <param name="dateTime">需要转换的时间</param>
        /// <returns></returns>
        public static string TryToDateTimeCanBeNull(this DateTime? dateTime)
        {
            return !dateTime.HasValue || Convert.ToDateTime(dateTime).Year < 1910
                       ? " "
                       : ((DateTime)dateTime).ToString("yyyy-MM-dd");
        }


        /// <summary>
        /// Decimal 扩展
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static string ToExString(this
            decimal? value,string format="")
        {
            if (value != null)
            {
                return ((decimal) value).ToString(format);
            }
            return "";
        }


        /// <summary>
        /// int 扩展
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static string ToExString(this
            int? value, string format = "")
        {
            if (value != null)
            {
                return ((int)value).ToString(format);
            }
            return "";
        }

        /// <summary>
        /// dateTime 扩展
        /// </summary>
        /// <param name="value">值</param>
        /// <param name="format">格式</param>
        /// <returns></returns>
        public static string ToExString(this
            DateTime? value, string format = "")
        {
            if (value != null && value!=DateTime.MinValue)
            {
                return ((DateTime)value).ToString(format);
            }
            return "";
        }

        /// <summary>
        /// 转换成百分比
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToPercent(this
            double value)
        { 
           return string.Format("{0:0.00%}", value);//得到5.88%
        }
    }
}
