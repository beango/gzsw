using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using gzsw.util.Extensions;

namespace gzsw.util
{
    public class CommonHelper
    {
        /// <summary>
        /// 深复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T DeepClone<T>(T obj) where T : class
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bFormatter.Serialize(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)bFormatter.Deserialize(stream);
        }

        /// <summary>
        /// 除法
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="de"></param>
        /// <returns></returns>
        public static string Division(int mo, int de)
        {
            if (de == 0)
            {
                return "";
            }
            return (mo / de).ToString("f2");
        }

        /// <summary>
        /// 除法
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="de"></param>
        /// <returns></returns>
        public static string DivisionOfTimeString(int mo, int de)
        {
            if (de == 0)
            {
                return 0.ToTimeString();
            }
            return (mo / de).ToTimeString();
        }

        /// <summary>
        /// 除法
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="de"></param>
        /// <returns></returns>
        public static string DivisionOfTimeString(double mo, double de)
        {
            if (de == 0)
            {
                return 0.ToTimeString();
            }
            return ((int)(mo / de)).ToTimeString();
        }

        /// <summary>
        /// 获取除法百分比
        /// </summary>
        /// <param name="mo"></param>
        /// <param name="de"></param>
        /// <returns></returns>
        public static string DivisionOfPercent(int mo, int de)
        {
            if (de == 0)
            {
                return "0.00%";
            }
            return (mo * 100.0d / de).ToString("f2") + "%";
        }
    }
}
