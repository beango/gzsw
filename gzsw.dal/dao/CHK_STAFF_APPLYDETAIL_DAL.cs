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

        private double getDiffDays(DateTime being, DateTime end)
        {
            var ts = new TimeSpan();
            ts = end-being;
            return ts.TotalDays;
        }

        public Page<CHK_STAFF_APPLYDETAIL> GetPageList(string staffNo, string staffName, int pageIndex, int pageSize)
        {
            var db = new Database();
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
            var db = new Database();
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
                                            ON APP.[CHK_STAFF_ID] = ST.STAFF_ID WHERE APP.[APPLYDETAIL_ID]=@0 ",id);

            return db.FirstOrDefault<CHK_STAFF_APPLYDETAIL>(sql);
        }
    }
}
