using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.util
{
    public static class OutputHelper
    {
        /// <summary>
        /// 把字符转为时间输出 010101转01:01:01
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetTimeString(string time)
        {
            if (time.Length != 6) {
                return time;
            }
            var ary=  time.ToArray();
            var output = "";
            for (var i = 0; i < ary.Length; i++)
            {
                output += ary[i];
                if ((i + 1) % 2 == 0 && (i + 1) != ary.Length)
                {
                    output += ":";
                }
            }

            return output;
        }

        /// <summary>
        /// 把字符转为时间输出 010101转01:01:01
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetTimeChinaString(string time)
        {
            if (time.Length != 6)
            {
                return time;
            }
            var ary = time.ToArray();
            string output = ary[0].ToString() + ary[1].ToString()+"时";
            output += ary[2].ToString() + ary[3].ToString() + "分";

            return output;
        }
    }
}
