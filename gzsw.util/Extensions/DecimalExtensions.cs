using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.util.Extensions
{
    public static class DecimalExtensions
    {
        /// <summary>
        /// 转换Int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this decimal value)
        {
            return Convert.ToInt32(value);
        }
    }
}
