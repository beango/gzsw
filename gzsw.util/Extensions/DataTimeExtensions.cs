 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.util.Extensions
{
    /// <summary>
    ///  
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/8 17:55:36</para>
    /// </remark>
    public static class DataTimeExtensions
    {

        /// <summary>
        /// 默认开始时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime? DefaultBeginDateTime(this DateTime dt)
        {
            var data =  dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d"))); 
            return new DateTime(data.Year, data.Month, data.Day);
        }

        /// <summary>
        /// 默认结束时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime? DefaultEndDateTime(this DateTime dt)
        {
            DateTime startWeek = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")));
            startWeek = startWeek.AddDays(6);
            return new DateTime(startWeek.Year, startWeek.Month, startWeek.Day);
        }



        /// <summary>
        /// 获取周开始时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToWeekForBeginTime(this DateTime dt)
        {
            DateTime startWeek = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d"))); 
            return startWeek.ToString("yyyy-MM-dd"); 
        }



        /// <summary>
        ///  获取周结束时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToWeekForEndTime(this DateTime dt)
        {
            DateTime startWeek = dt.AddDays(1 - Convert.ToInt32(dt.DayOfWeek.ToString("d")));
            startWeek =  startWeek.AddDays(6);
            return startWeek.ToString("yyyy-MM-dd"); 
             
        }


        /// <summary>
        /// 获取月开始时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToMonthForBeginTime(this DateTime dt)
        {
            //本月第一天时间      
            return new DateTime(dt.Year, dt.Month, 1).ToString("yyyy-MM-dd");
        }


        /// <summary>
        /// 获取月结束时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToMonthForEndTime(this DateTime dt)
        { 
           return new DateTime(dt.Year, dt.Month, 1).AddMonths(+1).AddDays(-1).ToString("yyyy-MM-dd");
        }


        /// <summary>
        /// 获取年开始时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToYearForBeginTime(this DateTime dt)
        {
            string datebegin = dt.ToString("yyyy-1-1");
            return datebegin;
        }


        /// <summary>
        ///  获取年结束时间
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToYearForEndTime(this DateTime dt)
        {
            string datebegin = dt.ToString("yyyy-12-31");
            return datebegin;
        }
    }
}
