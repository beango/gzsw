using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.util.Extensions
{
    public static class IntExtensions
    {
        /// <summary>
        /// 把201409转成2014年09月
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToYearsMonthString(this int value)
        {
            var s = value.ToString();
            if (s.Length < 4)
            {
                return s;
            }
            return s.Insert(4, "年") + "月";
        }

        /// <summary>
        /// 把秒数转为时间格式输出
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToTimeString(this int value)
        {
            if (value < 60)
            {
                return value + "秒";
            }
            var str = "";
            var h = value / 3600;
            var hm = value % 3600;
            if (h > 0)
            {
                str = h + "时";
            }
            var m = hm / 60;
            var s = hm % 60;
            if (m > 0)
            {
                str += m + "分";
            }
            if (s > 0)
            {
                str += s + "秒";
            }
            return str;
        }
    }
}
