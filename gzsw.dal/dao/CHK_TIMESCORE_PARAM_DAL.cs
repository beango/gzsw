using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 考勤参数
    /// </summary>
    public class CHK_TIMESCORE_PARAM_DAL
    {
        public List<CHK_TIMESCORE_PARAM> GetPageList(string orgId, string userId)
        {
            var db = new Database();

            var whereStr = " PAR_ORG_ID is null ";
            if (!string.IsNullOrEmpty(orgId))
            {
                whereStr = " ORG_ID = @0 ";
            }

            var sql = Sql.Builder.Append(@";WITH locs(ORG_ID,ORG_NAM,PAR_ORG_ID,ORG_LEVEL,loclevel)     
                                        AS     
                                        (   
                                          SELECT ORG_ID,ORG_NAM,PAR_ORG_ID,ORG_LEVEL,0 AS loclevel FROM SYS_ORGANIZE       
                                          WHERE  "+ whereStr +
                                          @" UNION ALL          
                                          SELECT l.ORG_ID,l.ORG_NAM,l.PAR_ORG_ID,l.ORG_LEVEL,loclevel+1 FROM SYS_ORGANIZE l              
                                          INNER JOIN locs p ON l.PAR_ORG_ID=p.ORG_ID    
                                         ) 
                                        SELECT ti.HALL_NO,
			                                        ti.HALL_NAM,
                                                    p.ORG_ID,
			                                        p.MODIFY_ID,
			                                        p.MODIFY_DTIME,
			                                        p.A_BEGIN_TIME,
			                                        p.P_BEGIN_TIME,
			                                        p.A_END_TIME,
			                                        p.P_END_TIME,
			                                        p.LAT_LAST_MIN,
			                                        p.EAR_LAST_MIN,
			                                        p.EAR_SCORE,
			                                        p.NEG_SCORE,
			                                        p.ILL_SCORE,
			                                        p.ABS_SCORE,
			                                        p.LAT_SCORE,
			                                        p.NONSIGN_SCORE 
		                                        from locs as l
		                                        join SYS_HALL as ti
		                                        on ti.ORG_ID=l.ORG_ID
		                                        join CHK_TIMESCORE_PARAM as p
		                                        on ti.ORG_ID=p.ORG_ID
		                                        join SYS_USERORGANIZE as u
		                                        on ti.ORG_ID = u.ORG_ID
		                                        where u.[USER_ID]=@1", orgId, userId);

            var list = db.Fetch<CHK_TIMESCORE_PARAM>(sql);
            return list;
        }

        private List<string> getOrgList(string orgId, string userId)
        {
            var db = new Database();

            var whereStr = " PAR_ORG_ID is null ";
            if (!string.IsNullOrEmpty(orgId))
            {
                whereStr = " ORG_ID = @0 ";
            }
            var sql = Sql.Builder.Append(@";WITH locs(ORG_ID,ORG_NAM,PAR_ORG_ID,ORG_LEVEL,loclevel)     
                                        AS     
                                        (   
                                          SELECT ORG_ID,ORG_NAM,PAR_ORG_ID,ORG_LEVEL,0 AS loclevel FROM SYS_ORGANIZE       
                                          WHERE  " + whereStr +
                                         @" UNION ALL          
                                          SELECT l.ORG_ID,l.ORG_NAM,l.PAR_ORG_ID,l.ORG_LEVEL,loclevel+1 FROM SYS_ORGANIZE l              
                                          INNER JOIN locs p ON l.PAR_ORG_ID=p.ORG_ID    
                                         ) 
                                        SELECT  l.ORG_ID
		                                FROM locs as l
		                                join SYS_USERORGANIZE as u
		                                on l.ORG_ID = u.ORG_ID
		                                where u.[USER_ID]=@1 ",orgId,userId);


            return db.Fetch<string>(sql);
        }

        public void Save(CHK_TIMESCORE_PARAM param,string userId)
        {
            var db = new Database();
            var list = getOrgList(param.ORG_ID, userId);
            if (list == null || list.Count <= 0) return;
            foreach (var orgId in list)
            {
                var obj= db.FirstOrDefault<CHK_TIMESCORE_PARAM>(Sql.Builder.Append(@"FROM CHK_TIMESCORE_PARAM WHERE ORG_ID=@0",
                    orgId));

                param.ORG_ID = orgId;
                if (obj == null)
                {
                    db.Insert(param);
                }
                else
                {
                    db.Update(param);
                }
            }
        }

        public CHK_TIMESCORE_PARAM Get(string orgId)
        {
            var db = new Database();
            var sql = Sql.Builder.Append(@"SELECT HA.HALL_NO,
			                                        HA.HALL_NAM,
                                                    PA.ORG_ID,
			                                        PA.MODIFY_ID,
			                                        PA.MODIFY_DTIME,
			                                        PA.A_BEGIN_TIME,
			                                        PA.P_BEGIN_TIME,
			                                        PA.A_END_TIME,
			                                        PA.P_END_TIME,
			                                        PA.LAT_LAST_MIN,
			                                        PA.EAR_LAST_MIN,
			                                        PA.EAR_SCORE,
			                                        PA.NEG_SCORE,
			                                        PA.ILL_SCORE,
			                                        PA.ABS_SCORE,
			                                        PA.LAT_SCORE,
			                                        PA.NONSIGN_SCORE  
                                            FROM CHK_TIMESCORE_PARAM AS PA
                                            JOIN SYS_HALL as HA
                                            ON PA.ORG_ID=HA.ORG_ID 
                                            WHERE PA.ORG_ID = @0 ", orgId);

            return db.FirstOrDefault<CHK_TIMESCORE_PARAM>(sql);
        }
    }
}
