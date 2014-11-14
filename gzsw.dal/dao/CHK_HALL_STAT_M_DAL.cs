using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;
using gzsw.model;

namespace gzsw.dal.dao
{
    public class CHK_HALL_STAT_M_DAL
    {
        public Page<dynamic> GetPersonnelAttendanceList(int page, int PageSize, string hallno, int id)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,  COR_LAT_DAY_CNT,COR_EAR_DAY_CNT,COR_ABSENT_DAY_CNT,COR_ATTEND_PER_CNT,COR_WORK_DAY_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO
WHERE CH.STAT_MO=" + id+"   and ch.HALL_NO='"+hallno+"'");
 
            var data = db.Page<dynamic>(page, PageSize, sql);
            data.ItemsPerPage = PageSize;
            return data;
        }



        public static CHK_HALL_STAT_M GetPersonnelAttendance(int STAT_MO, string staffId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,  COR_LAT_DAY_CNT,COR_EAR_DAY_CNT,COR_ABSENT_DAY_CNT,COR_ATTEND_PER_CNT,COR_WORK_DAY_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO 
	                                        WHERE 1=1 ");

            sql.Append(@" AND CH.STAT_MO=@0 AND ch.HALL_NO=@1  ", STAT_MO, staffId);

            return db.FirstOrDefault<CHK_HALL_STAT_M>(sql);
        }

        public Page<dynamic> GetQueuingDetentionList(int pageIndex, int pageSize, string orgId, int stat)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,  COR_OVERTIME_CNT,COR_VALID_QUEUE_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO
WHERE CH.STAT_MO=" + stat + "   and ch.HALL_NO='" + orgId + "'");

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        public static CHK_HALL_STAT_M GetQueuingDetention(int STAT_MO, string staffId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,   COR_OVERTIME_CNT,COR_VALID_QUEUE_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO 
	                                        WHERE 1=1 ");

            sql.Append(@" AND CH.STAT_MO=@0 AND ch.HALL_NO=@1  ", STAT_MO, staffId);

            return db.FirstOrDefault<CHK_HALL_STAT_M>(sql);
        }

        public Page<dynamic> GetRateCompletionList(int pageIndex, int pageSize, string orgId, int stat)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,  COR_OVERTIME_SVR_CNT,COR_VALID_SVR_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO
WHERE CH.STAT_MO=" + stat + "   and ch.HALL_NO='" + orgId + "'");

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        public static CHK_HALL_STAT_M GetRateCompletion(int STAT_MO, string staffId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,    COR_OVERTIME_SVR_CNT,COR_VALID_SVR_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO 
	                                        WHERE 1=1 ");

            sql.Append(@" AND CH.STAT_MO=@0 AND ch.HALL_NO=@1  ", STAT_MO, staffId);

            return db.FirstOrDefault<CHK_HALL_STAT_M>(sql);
        }

        public object GetRateErrorList(int pageIndex, int pageSize, string orgId, int stat)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,   COR_MISTAKE_SVR_CNT,COR_VALID_SVR_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO
WHERE CH.STAT_MO=" + stat + "   and ch.HALL_NO='" + orgId + "'");

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        public static CHK_HALL_STAT_M GetRateError(int STAT_MO, string staffId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,    COR_MISTAKE_SVR_CNT,COR_VALID_SVR_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO 
	                                        WHERE 1=1 ");

            sql.Append(@" AND CH.STAT_MO=@0 AND ch.HALL_NO=@1  ", STAT_MO, staffId);

            return db.FirstOrDefault<CHK_HALL_STAT_M>(sql);
        }

        public object GetRateSatisfactionList(int pageIndex, int pageSize, string orgId, int stat)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,   COR_EVAL_DISSATISFY_CNT,COR_VALID_SVR_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO
WHERE CH.STAT_MO=" + stat + "   and ch.HALL_NO='" + orgId + "'");

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        public static CHK_HALL_STAT_M GetRateSatisfaction(int STAT_MO, string staffId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,    COR_EVAL_DISSATISFY_CNT,COR_VALID_SVR_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO 
	                                        WHERE 1=1 ");

            sql.Append(@" AND CH.STAT_MO=@0 AND ch.HALL_NO=@1  ", STAT_MO, staffId);

            return db.FirstOrDefault<CHK_HALL_STAT_M>(sql);
        }


        public object GetRateComplaintList(int pageIndex, int pageSize, string orgId, int stat)
        {
            var db = gzswDB.GetInstance();

            var sql = PetaPoco.Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,  COR_COMPLAIN_CNT,COR_HALL_STAFF_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO
WHERE CH.STAT_MO=" + stat + "   and ch.HALL_NO='" + orgId + "'");

            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        public static CHK_HALL_STAT_M GetRateComplaint(int STAT_MO, string staffId)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
select  ch.STAT_MO,
h.HALL_NO,   COR_COMPLAIN_CNT,COR_HALL_STAFF_CNT
 from  
CHK_HALL_STAT_M ch
left join SYS_HALL h on  ch.HALL_NO=H.HALL_NO 
	                                        WHERE 1=1 ");

            sql.Append(@" AND CH.STAT_MO=@0 AND ch.HALL_NO=@1  ", STAT_MO, staffId);

            return db.FirstOrDefault<CHK_HALL_STAT_M>(sql);
        }

        public static object GetListSub(int STAT_MO, string orgId, int pageIndex, int pageSize)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
                    select 
                    STAT_MO,
                    CH.HALL_NO,
                    ATTEND_SCORE,
                    QUEUE_DETAIN_SCORE,
                    HANDLE_ONTIME_SCORE,
                    QUALITY_SCORE,
                    EVAL_SATISFY_SCORE,
                    COMPLAIN_SCORE,
                    ENVIRON_SCORE,
                    SYSTEM_SCORE,
                    NORM_SCORE,
                    PROFESS_SCORE,
                    THIRD_SURVEY_SCORE,
                    OTHER_SCORE,
                    STAR_LEVEL,
                    HALL_NAM
                    from CHK_HALL_STAT_M ch,SYS_HALL h
                    where ch.HALL_NO=h.HALL_NO  ");

            sql.Append(@" AND  CH.HALL_NO=@0 ", orgId);

            var beginMo = STAT_MO;
            var endMo = STAT_MO;
            if (STAT_MO.ToString().Length == 4)
            {
                beginMo = int.Parse(STAT_MO.ToString() + "01");
                endMo = int.Parse(STAT_MO.ToString() + "12");
                sql.Append(@" AND (( ch.STAT_MO>=@0 AND ch.STAT_MO<=@1 ) OR ch.STAT_MO=@2 ) ", beginMo, endMo, STAT_MO);
            }
            else
            {
                sql.Append(@" AND ch.STAT_MO=@0 ", STAT_MO);
            }


            //return db.Page<dynamic>(pageIndex, pageSize, sql);
            var data = db.Page<dynamic>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        public static void UpdateObject(CHK_HALL_STAT_M item)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"
UPDATE [CHK_HALL_STAT_M]
   SET  [MODIFY_ID] = @0
      ,[MODIFY_DTIME] = @1 
      ,[COR_LAT_DAY_CNT] = @2
      ,[COR_EAR_DAY_CNT] = @3
      ,[COR_ABSENT_DAY_CNT] = @4
      ,[COR_ATTEND_PER_CNT] = @5
      ,[COR_WORK_DAY_CNT] = @6  
      ,[COR_OVERTIME_CNT] = @7
      ,[COR_VALID_QUEUE_CNT] = @8 
      ,[COR_OVERTIME_SVR_CNT] = @9
      ,[COR_VALID_SVR_CNT] =@10 
      ,[COR_MISTAKE_SVR_CNT] =@11 
      ,[COR_EVAL_DISSATISFY_CNT] =@12
      ,[COR_VALID_EVAL_CNT] = @13 
      ,[COR_COMPLAIN_CNT] = @14
      ,[COR_HALL_STAFF_CNT] =@15 
 WHERE 1=1 ",item.MODIFY_ID,item.MODIFY_DTIME,item.COR_LAT_DAY_CNT,item.COR_EAR_DAY_CNT,item.COR_ABSENT_DAY_CNT,item.COR_ATTEND_PER_CNT
            , item.COR_WORK_DAY_CNT, item.COR_OVERTIME_CNT, item.COR_VALID_QUEUE_CNT, item.COR_OVERTIME_SVR_CNT, item.COR_VALID_SVR_CNT, item.COR_MISTAKE_SVR_CNT,
            item.COR_EVAL_DISSATISFY_CNT,item.COR_VALID_EVAL_CNT,item.COR_COMPLAIN_CNT,item.COR_HALL_STAFF_CNT);

            sql.Append(@" AND  HALL_NO=@0 and  STAT_MO=@1", item.HALL_NO,item.STAT_MO);
            db.Execute(sql);

        }
    }
}
