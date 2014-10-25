using gzsw.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.controller.MyAuth
{
    public class UserState
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public List<UserFuncs> UserFuncs { get; set; }
        public List<UserOrgs> UserOrgs { get; set; }
    }
}
