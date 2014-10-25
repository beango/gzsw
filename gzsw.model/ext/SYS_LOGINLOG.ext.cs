using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace gzsw.model
{
    public partial class SYS_LOGINLOG
    {
        public enum STATE_ENUM
        {
            [Description("登录成功")]
            登录成功 = 10,

            [Description("登录失败，不存在的用户")]
            用户不存在 = 11,

            [Description("登录失败，密码错误")]
            密码错误 = 12,

            [Description("退出成功")]
            退出成功 = 20,

            [Description("退出失败")]
            退出失败 = 21
        }
    }
}
