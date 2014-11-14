 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using gzsw.model.dto;
using PetaPoco;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 图表数据
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/10/18 11:47:48</para>
    /// </remark>
    public class Chart_DAL
    {
        /// <summary>
        /// 获取组织机构下的当前办税排队人数
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="orgId">组织机构ID</param>
        /// <param name="level">等级</param>
        /// <returns></returns>
        public IList<OrganizeQueueingDto> GetOrganizeQueueing(string userId,string orgId,int level)
        {
            var sql = PetaPoco.Sql.Builder.Append(@"select ORG_ID  , 
                ORG_NAM as Name,
                dbo.JGWaitPersons(ORG_ID) as WaitPersonsCount,
                dbo.JGWaitTimes(ORG_ID) as WaitPersonsTime,
                dbo.JGSaturation(ORG_ID,@2) as SaturationValue,
                dbo.FUN_GET_SATISFY_RT(ORG_ID) as SatisfactionValue
                from dbo.SYS_ORGANIZE where (dbo.isParent(ORG_ID,@0)=1)and(ORG_LEVEL=@1)
                and ORG_ID in ( select ORG_ID from SYS_USERORGANIZE where USER_ID =@2)
                group by Org_id,ORG_NAM
                 ", orgId, level, userId);
            var db = gzswDB.GetInstance();
            return db.Fetch<OrganizeQueueingDto>(sql);
        }


         
    }
}
