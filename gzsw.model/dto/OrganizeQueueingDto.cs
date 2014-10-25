 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.model.dto
{
    /// <summary>
    ///  
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/18 11:50:42</para>
    /// </remark>
   public  class OrganizeQueueingDto
    {
       /// <summary>
       /// 组织机构ID
       /// </summary>
       public string ORG_ID { get; set; }

      /// <summary>
      /// 组织结构名称
      /// </summary>
       public string Name { get; set; }

       /// <summary>
       /// 饱和度
       /// </summary>
       public double SaturationValue { get; set; }

       /// <summary>
       /// 满意度
       /// </summary>
       public double SatisfactionValue { get; set; }

       /// <summary>
       /// 排队人数
       /// </summary>
       public int WaitPersonsCount { get; set; }


       /// <summary>
       /// 排队时间 (总分钟)
       /// </summary>
       public int WaitPersonsTime { get; set; }
    }
}
