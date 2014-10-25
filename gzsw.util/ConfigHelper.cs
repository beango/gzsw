using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace gzsw.util
{
    public class ConfigHelper
    {
        #region 配置文件
        /// <summary>
        /// 获取配置文件中的设置
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetSettingByKey(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
        #endregion
    }
}
