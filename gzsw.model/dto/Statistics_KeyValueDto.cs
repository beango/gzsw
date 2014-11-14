 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.dto
{ 
    public class Statistics_KeyValueDto
    {
        /// <summary>
        /// 名字
        /// </summary>
        [ResultColumn]
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary> 
        [ResultColumn]
        public string Value { get; set; }
    }
}
