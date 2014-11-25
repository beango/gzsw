using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class DETAIL_BUSINESS_MISTAKE_DAL
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="orgId">服务厅编码</param>
        /// <param name="qualityId">质量类型编码</param>
        /// <param name="serialName">明细业务名称</param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static object GetPager(DateTime? beginTime, DateTime? endTime, string orgId, string qualityId, string serialName, int pageIndex, int pageSize)
        {

            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  CHK_STAFF_QUALITY_ALL.* ,
        SYS_USER.USER_NAM AS OUSERNAME ,
        SYS_DETAILSERIAL.SERIALNAME ,
        CHK_QUALITY_CON.QUALITY_NAM,
		SYS_STAFF.STAFF_NAM,
		(dbo.GetFwtNameForStaffno(CHK_STAFF_QUALITY_ALL.STAFF_ID)) AS HALL_NAM
FROM    ( SELECT    SEQ ,
					STAFF_ID,
                    QUALITY_CD ,
                    SERIALID ,
                    AMOUNT ,
                    OCCUR_DT ,
                    MODIFY_ID AS OUSER_ID ,
                    1 AS DATASOURCE
          FROM      CHK_STAFF_QUALITY_MARK
          WHERE     1 = 1
          UNION ALL
          SELECT    IMPORT_SEQ AS SEQ ,
					STAFF_ID,
                    QUALITY_CD ,
                    SERIALID ,
                    AMOUNT ,
                    OCCUR_DT ,
                    IMPORT_USER_ID AS OUSER_ID ,
                    2 AS DATASOURCE
          FROM      CHK_STAFF_QUALITY_IMPORT
          WHERE     1 = 1
                    AND IMPORT_STATE = 1
        ) AS CHK_STAFF_QUALITY_ALL
		JOIN SYS_STAFF ON SYS_STAFF.STAFF_ID = CHK_STAFF_QUALITY_ALL.STAFF_ID
        JOIN SYS_DETAILSERIAL ON SYS_DETAILSERIAL.SERIALID = CHK_STAFF_QUALITY_ALL.SERIALID
        JOIN CHK_QUALITY_CON ON CHK_QUALITY_CON.QUALITY_CD = CHK_STAFF_QUALITY_ALL.QUALITY_CD
        LEFT JOIN SYS_USER ON SYS_USER.[USER_ID] = CHK_STAFF_QUALITY_ALL.OUSER_ID
WHERE   1 = 1");


            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND ( dbo.GetFwtIdforStaffno(CHK_STAFF_QUALITY_ALL.STAFF_ID) = @0) ", orgId);
            }
            if (beginTime.HasValue)
            {
                sql.Append(@" AND DATEDIFF(DAY, CHK_STAFF_QUALITY_ALL.OCCUR_DT, @0) <= 0 ", beginTime);
            }
            if (endTime.HasValue)
            {
                sql.Append(@" AND DATEDIFF(DAY, CHK_STAFF_QUALITY_ALL.OCCUR_DT, @0) >= 0 ", endTime);
            }
            if (!string.IsNullOrEmpty(qualityId))
            {
                sql.Append(@" AND CHK_QUALITY_CON.QUALITY_CD = @0 ", qualityId);
            }
            if (!string.IsNullOrEmpty(serialName))
            {
                sql.Append(@" AND SYS_DETAILSERIAL.SERIALNAME LIKE @0 ", string.Format("%{0}%", serialName));
            }

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;

            return data;
        }

        /// <summary>
        /// 获取明细
        /// </summary>
        /// <returns></returns>
        public static dynamic GetOne(int dataSource, int seq)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"SELECT  CHK_STAFF_QUALITY_ALL.* ,
        SYS_USER.USER_NAM AS OUSERNAME ,
        SYS_DETAILSERIAL.SERIALNAME ,
        CHK_QUALITY_CON.QUALITY_NAM ,
        SYS_STAFF.STAFF_NAM,
		dbo.GetFwtIdforStaffno(CHK_STAFF_QUALITY_ALL.STAFF_ID) AS HALL_NO,
		(dbo.GetFwtNameForStaffno(CHK_STAFF_QUALITY_ALL.STAFF_ID)) AS HALL_NAM
FROM    ( SELECT    SEQ ,
                    STAFF_ID ,
                    SERIALID ,
                    QUALITY_CD ,
                    AMOUNT ,
                    OCCUR_DT ,
                    NULL AS FILE_URL ,
                    MODIFY_ID AS OUSER_ID ,
                    MODIFY_DTIME AS ODTIME ,
                    NULL AS OSTATE ,
                    1 AS DATASOURCE
          FROM      CHK_STAFF_QUALITY_MARK
          WHERE     1 = 1
          UNION ALL
          SELECT    IMPORT_SEQ AS SEQ ,
                    STAFF_ID ,
                    SERIALID ,
                    QUALITY_CD ,
                    AMOUNT ,
                    OCCUR_DT ,
                    FILE_URL ,
                    IMPORT_USER_ID AS OUSER_ID ,
                    IMPORT_DTIME AS ODTIME ,
                    IMPORT_STATE AS OSTATE ,
                    2 AS DATASOURCE
          FROM      CHK_STAFF_QUALITY_IMPORT
          WHERE     1 = 1
        ) AS CHK_STAFF_QUALITY_ALL
        JOIN SYS_DETAILSERIAL ON SYS_DETAILSERIAL.SERIALID = CHK_STAFF_QUALITY_ALL.SERIALID
        JOIN CHK_QUALITY_CON ON CHK_QUALITY_CON.QUALITY_CD = CHK_STAFF_QUALITY_ALL.QUALITY_CD
        JOIN SYS_STAFF ON SYS_STAFF.STAFF_ID = CHK_STAFF_QUALITY_ALL.STAFF_ID		
        LEFT JOIN SYS_USER ON SYS_USER.[USER_ID] = CHK_STAFF_QUALITY_ALL.OUSER_ID
WHERE   1 = 1");

            sql.Append(@" AND CHK_STAFF_QUALITY_ALL.DATASOURCE = @0 AND CHK_STAFF_QUALITY_ALL.SEQ = @1 ", dataSource, seq);

            return db.FirstOrDefault<dynamic>(sql);
        }
    }
}
