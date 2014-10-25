

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Cotide.Framework.ActionResult;

namespace gzsw.controller.SYS
{
    /// <summary>
    ///  通用功能控制器
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/12 17:18:23</para>
    /// </remark>
    public class UtilityController : BaseController
    {

        [HttpGet]
        public ImageActionResult GetCode()
        {
            #region///资源访问事件
            return new ImageActionResult();
            #endregion
        } 
    }
}
