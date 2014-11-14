using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    public class CHK_STAFF_APPLYDETAIL_DAL
    {

        public Page<CHK_STAFF_APPLYDETAIL> GetPageList(string orgId,string staffNo, string staffName, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT APP.[APPLYDETAIL_ID]
                                            ,ST.[STAFF_NAM]
                                            ,APP.[CHK_STAFF_ID]
                                            ,APP.[APPLY_TIME]
                                            ,APP.[APPLY_USR_ID]
                                            ,APP.[BEGIN_TIME]
                                            ,APP.[END_TIME]
                                            ,APP.[HOLLI_TYP]
                                            ,APP.[APPLY_STATE]
                                            ,APP.[AUD_USR_ID]
                                            ,APP.[AUD_TIME]
                                            ,APP.APPLY_REASON
                                            FROM [CHK_STAFF_APPLYDETAIL] AS APP
                                            JOIN SYS_STAFF AS ST
                                            ON APP.[CHK_STAFF_ID] = ST.STAFF_ID WHERE 1=1 ");
            if (!string.IsNullOrEmpty(orgId))
            {
                sql.Append(@" AND ST.ORG_ID = @0 ", orgId);
            }
            if (!string.IsNullOrEmpty(staffNo))
            {
                sql.Append(@" AND APP.[CHK_STAFF_ID] like @0 ", "%" + staffNo + "%");
            }
            if (!string.IsNullOrEmpty(staffName))
            {
                sql.Append(@" AND ST.[STAFF_NAM] like @0 ", "%" + staffName + "%");
            }

            return db.Page<CHK_STAFF_APPLYDETAIL>(pageIndex, pageSize, sql);
        }

        public CHK_STAFF_APPLYDETAIL Get(int id)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT APP.[APPLYDETAIL_ID]
                                            ,ST.[STAFF_NAM]
                                            ,APP.[CHK_STAFF_ID]
                                            ,APP.[APPLY_TIME]
                                            ,APP.[APPLY_USR_ID]
                                            ,APP.[BEGIN_TIME]
                                            ,APP.[END_TIME]
                                            ,APP.[HOLLI_TYP]
                                            ,APP.[APPLY_STATE]
                                            ,APP.[AUD_USR_ID]
                                            ,APP.[AUD_TIME]
                                            ,APP.APPLY_REASON
                                            ,U.USER_NAM AS APPLY_USR_NAM
                                            ,ORG.ORG_ID
                                            ,ORG.ORG_NAM
                                            ,ST.STAFF_NAM
                                            FROM [CHK_STAFF_APPLYDETAIL] AS APP
                                            JOIN SYS_STAFF AS ST
                                            ON APP.[CHK_STAFF_ID] = ST.STAFF_ID 
                                            JOIN SYS_ORGANIZE ORG
                                            ON ST.ORG_ID=ORG.ORG_ID
                                            JOIN SYS_USER AS U 
                                            ON APP.APPLY_USR_ID = U.USER_ID 
                                            WHERE APP.[APPLYDETAIL_ID]=@0 ", id);

            return db.FirstOrDefault<CHK_STAFF_APPLYDETAIL>(sql);
        }

        /// <summary>
        /// 通过明细查询
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="staffNo"></param>
        /// <param name="staffName"></param>
        /// <param name="beginTime"></param>
        /// <param name="endTime"></param>
        /// <param name="type"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static Page<CHK_STAFF_APPLYDETAIL> GetMergeList(string orgId, string staffNo, string staffName,
                DateTime?beginTime,DateTime? endTime,int? type, int pageIndex,int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"SELECT APP.[APPLYDETAIL_ID]
                                            ,ST.[STAFF_NAM]
                                            ,APP.[CHK_STAFF_ID]
                                            ,APP.[APPLY_TIME]
                                            ,APP.[APPLY_USR_ID]
                                            ,APP.[BEGIN_TIME]
                                            ,APP.[END_TIME]
                                            ,APP.[HOLLI_TYP]
                                            ,APP.[APPLY_STATE]
                                            ,APP.[AUD_USR_ID]
                                            ,APP.[AUD_TIME]
                                            ,APP.APPLY_REASON
                                            FROM [CHK_STAFF_APPLYDETAIL] AS APP
                                            JOIN SYS_STAFF AS ST
                                            ON APP.[CHK_STAFF_ID] = ST.STAFF_ID WHERE 1=1 ");

            sql.Append(@" AND ST.[ORG_ID] = @0 ",orgId);
            if (!string.IsNullOrEmpty(staffNo))
            {
                sql.Append(@" AND APP.[CHK_STAFF_ID] like @0 ", "%" + staffNo + "%");
            }
            if (!string.IsNullOrEmpty(staffName))
            {
                sql.Append(@" AND ST.[STAFF_NAM] like @0 ", "%" + staffName + "%");
            }
            if (beginTime != null)
            {
                beginTime = beginTime.GetValueOrDefault().Date;
                sql.Append(@" AND APP.BEGIN_TIME >= @0 ", beginTime);
            }
            if (endTime != null)
            {
                endTime = endTime.GetValueOrDefault().Date.AddDays(1);
                sql.Append(@" AND APP.BEGIN_TIME < @0 ", endTime);
            }
            if (type != null)
            {
                sql.Append(@" AND APP.HOLLI_TYP = @0 ", type);
            }

            return db.Page<CHK_STAFF_APPLYDETAIL>(pageIndex, pageSize, sql);
        }
    }
}
