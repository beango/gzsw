using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model
{
    public partial class CLI_COUNTERSTATE
    {
        /// <summary>
        /// 员工信息
        /// </summary>
       [ResultColumn]
        public string STAFF_NAM { get; set; }

    }
}
