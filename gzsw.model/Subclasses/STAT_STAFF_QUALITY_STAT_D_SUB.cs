using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.model.Subclasses
{
    /// <summary>
    /// 业务差错  数据
    /// </summary>
    public  class STAT_STAFF_QUALITY_STAT_D_SUB : STAT_STAFF_QUALITY_STAT_D
    {
        /// <summary>
        /// 员工名称
        /// </summary>
         [ResultColumn]
        public string STAFF_NAM { get; set; }

        [ResultColumn]
         public string DLS_SERIALID { get; set; }
    }
}
