using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace gzsw.util.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举标记中说明文字<br/>
        /// (枚举成员的[Description("说明文字")],使用说明:MyEnum.Test.GetDescription())
        /// </summary>
        /// <param name="en">枚举成员</param>
        /// <returns>返回说明文字,如果没有则返回英文的字段名</returns>
        public static string GetDescription(this System.Enum en)
        {
            var type = en.GetType();
            var memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                var attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }
    }
}
