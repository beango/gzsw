 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    ///  质量类型
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/9 12:57:03</para>
    /// </remark>
    public  class QualityViewModel
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public QualityViewModel()
        {
            Items = new List<ItemBigTypeViewModel>();
        }

        /// <summary>
        /// 质量类型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 质量类型编码
        /// </summary>
        public string No { get; set; }


        /// <summary>
        /// 事项大类
        /// </summary>
        public IList<ItemBigTypeViewModel> Items { get; set; }

    }
}
