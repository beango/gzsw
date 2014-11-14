using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.ext
{
    public partial class WARN_COMPLAIN_TYP_CON
    {
        /// <summary>
        /// 创建人
        /// </summary>
            [ResultColumn]
        private string CREATE_User{ get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
            [ResultColumn]
        private string MODIFY_User { get; set; } 
    }
}
