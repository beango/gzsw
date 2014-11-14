using log4net;
using System;

namespace gzsw.util
{
    public class LogHelper
    {
        public static ILog log = LogManager.GetLogger(typeof(LogHelper));

        public static void WriteLog(string info)
        {
            log.Info(info);
        }

        public static void ErrorLog(string info)
        {
            log.Error(info);
        }

        /// <summary>
        /// 错误记录
        /// </summary>
        /// <param name="info">附加信息</param>
        /// <param name="ex">错误</param>
        public static void ErrorLog(string info, Exception ex)
        {
            log.Error(info,ex);
        }
    }
}
