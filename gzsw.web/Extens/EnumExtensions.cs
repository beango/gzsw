using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gzsw.web
{
    public static class EnumExtensions
    {
        /// <summary>
        /// DayOfWeek类的星期转换方法
        /// </summary>
        /// <param name="en">DayOfWeek</param>
        /// <returns>返回星期几</returns>
        public static string ToWeekString(this DayOfWeek en)
        {
            switch (en)
            {
                case DayOfWeek.Sunday:
                    return "星期日";
                case DayOfWeek.Monday:
                    return "星期一";
                case DayOfWeek.Tuesday:
                    return "星期二";
                case DayOfWeek.Wednesday:
                    return "星期三";
                case DayOfWeek.Thursday:
                    return "星期四";
                case DayOfWeek.Friday:
                    return "星期五";
                case DayOfWeek.Saturday:
                    return "星期六";
                default:
                    return null;
            }
        }
    }
}