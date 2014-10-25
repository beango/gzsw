 
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.ajax
{
    /// <summary>
    /// WebApi接口返回数据
    /// </summary>
    /// <remarks>
    ///     <para>    Creator：LHC</para>
    ///     <para>CreatedTime：2013/7/29 14:56:28</para>
    /// </remarks>
    public class ApiJsonResult<T> where T : class
    {
        /// <summary>
        /// 请求结果 是否成功
        /// </summary>
        public bool IsSuccess;

        /// <summary>
        /// 请求提示消息
        /// </summary>
        public string Msg;

        /// <summary>
        /// 请求的数据
        /// </summary>
        public T Data;

        /// <summary>
        /// ActionResult的构造函数
        /// </summary>
        public ApiJsonResult(string msg, bool isSuccess=true)
        {
            this.Msg = msg;
            this.IsSuccess = isSuccess;
        }

    }
}
