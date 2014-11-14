using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class DETAIL_WARN_DISPOSAL_DAL
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="warnType">警告类型</param>
        /// <param name="warnLevel">警告级别</param>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static object GetPager(DateTime? beginTime, DateTime? endTime, int? warnType, int? warnLevel, string orgId, int pageIndex, int pageSize)
        {

            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  WARN_INFO_DETAIL.WARN_INFO_DETAIL_ID ,
        WARN_INFO_DETAIL.REAL_VALUE ,
        WARN_INFO_DETAIL.WARN_TYP ,
        WARN_INFO_DETAIL.WARN_LEVEL ,
        WARN_INFO_DETAIL.HANDLE_USER ,
        WARN_INFO_DETAIL.HANDLE_TIME ,
        ( SELECT TOP 1
                    WARN_PARAM.CRITICAL_VALUE
          FROM      WARN_PARAM
          WHERE     ( WARN_PARAM.HALL_NO = WARN_INFO_DETAIL.HALL_NO
                      AND WARN_PARAM.WARN_LEVEL = WARN_INFO_DETAIL.WARN_LEVEL
                      AND WARN_PARAM.WARN_TYP = WARN_INFO_DETAIL.WARN_TYP
                    )
        ) AS CRITICAL_VALUE ,
        SYS_USER.USER_NAM AS HANDLE_USERNAME
FROM    WARN_INFO_DETAIL
        LEFT JOIN SYS_USER ON dbo.SYS_USER.[USER_ID] = WARN_INFO_DETAIL.HANDLE_USER
WHERE   1 = 1");

            if (beginTime.HasValue)
            {
                sql.Append(@" AND DATEDIFF(DAY, WARN_INFO_DETAIL.CREATE_DTIME, @0) <= 0 ", beginTime);
            }
            if (endTime.HasValue)
            {
                sql.Append(@" AND DATEDIFF(DAY, WARN_INFO_DETAIL.CREATE_DTIME, @0) >= 0 ", endTime);
            }
            if (warnType.HasValue)
            {
                sql.Append(@" AND WARN_INFO_DETAIL.WARN_TYP = @0 ", warnType);
            }
            if (warnLevel.HasValue)
            {
                sql.Append(@" AND WARN_INFO_DETAIL.WARN_LEVEL = @0 ", warnLevel);
            }
            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND WARN_INFO_DETAIL.HALL_NO = @0 ", orgId);
            }
            
            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;

            return data;
        }
    }
}
