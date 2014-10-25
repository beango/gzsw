using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 实时窗口
    /// </summary>
    public class CLI_COUNTERSTATE_DAL
    {
        /// <summary>
        /// 获取服务厅的所有窗口状态信息
        /// </summary>
        /// <param name="hallNo"></param>
        /// <returns></returns>
        public List<CLI_COUNTERSTATE> GetListByHallNo(string hallNo)
        {
            var db= new Database();

            var sql = Sql.Builder.Append(@"SELECT CC.[HALL_NO]
                                              ,CC.[COUNTER_ID]
                                              ,CC.[LOGIN_STATE]
                                              ,CC.[LST_LOGIN_TIME]
                                              ,CC.[LST_LOGOUT_TIME]
                                              ,CC.[LST_OPT_TIME]
                                              ,CC.[STATE]
                                              ,CC.[LST_QUEUE_NUMBER]
                                              ,CC.[LST_NSR_SBM]
                                              ,CC.[NSR_NAME]
                                              ,CC.[SERIALID]
                                              ,CC.[SERIALNAME]
                                              ,CC.[Q_SERIALNAME]
                                              ,CC.[STAFF_ID]
                                              ,CC.[QUEUE_TRANSCODEID]
                                              ,SS.STAFF_NAM
                                          FROM [CLI_COUNTERSTATE] AS CC 
                                            JOIN SYS_STAFF AS SS
                                            ON CC.STAFF_ID=SS.STAFF_ID 
                                            WHERE CC.HALL_NO=@0 ", hallNo);

            return db.Fetch<CLI_COUNTERSTATE>(sql);
        }

        /// <summary>
        /// 获取窗口的排队人数
        /// </summary>
        /// <param name="hallNo">服务厅编号</param>
        /// <param name="counterId">窗口ID</param>
        public int GetTabNumber(string hallNo, string counterId)
        {
            var db = new Database();
            var sql = Sql.Builder.Append(@"SELECT dbo.CounterQueueWaitPersons(HALL_NO,COUNTER_ID) AS Number
                                        FROM  CLI_COUNTERSTATE 
                                        WHERE HALL_NO=@0 AND COUNTER_ID=@1 ",
                                        hallNo, counterId);
            return db.FirstOrDefault<int>(sql);
        }

        /// <summary>
        /// 业务排队人数
        /// </summary>
        /// <param name="hallNo"></param>
        /// <returns></returns>
        public List<HallBusinessNum> GetOrganizeNumber(string hallNo)
        {
            var db = new Database();
            var sql = Sql.Builder.Append(@"SELECT COUNT(*) AS NUMBER,QL.Q_SERIALNAME 
                                        FROM SYS_QUEUEING AS Q
                                        JOIN SYS_QUEUESERIAL AS QL 
                                        ON Q.QUEUE_QSERIALID=QL.Q_SERIALID 
                                        WHERE Q.QUEUE_SYSNO=@0
                                        GROUP BY QL.Q_SERIALNAME ", hallNo);
            return db.Fetch<HallBusinessNum>(sql);
        }
    }
}
