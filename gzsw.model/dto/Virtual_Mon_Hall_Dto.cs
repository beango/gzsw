using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PetaPoco;

namespace gzsw.model.dto
{
    /// <summary>
    /// 监控预警服务厅信息
    /// </summary>
    public class Virtual_Mon_Hall_Dto
    {
        [ResultColumn]
        public string HALL_NO { get; set; }

        [ResultColumn]
        public string HALL_NAM { get; set; }

        /// <summary>
        /// 总窗口数
        /// </summary>
        [ResultColumn]
        public int TOT_COUNTER_CNT { get; set; }

        /// <summary>
        /// 在线窗口数
        /// </summary>
        [ResultColumn]
        public int ONLINE_COUNTER_CNT { get; set; }

        /// <summary>
        /// 开启率，为小数的形式，如0.2323，在前端页面需转化为 23.23%的百分比形式,以下的率的类似
        /// </summary>
        [ResultColumn]
        public decimal COUNTER_OPEN_RT { get; set; }

        /// <summary>
        /// 正在办理业务窗口数
        /// </summary>
        [ResultColumn]
        public int SERVICE_COUNTER_CNT { get; set; }

        /// <summary>
        /// 在办业务窗口比率
        /// </summary>
        [ResultColumn]
        public decimal SERVICE_COUNTER_RT { get; set; }

        /// <summary>
        /// 窗口突发情况与报警数量
        /// </summary>
        [ResultColumn]
        public int COUNTER_ALARM_CNT { get; set; }

        /// <summary>
        /// 投诉次数
        /// </summary>
        [ResultColumn]
        public int COMPLAIN_CNT { get; set; }

        /// <summary>
        /// 当月二次办理人次
        /// </summary>
        [ResultColumn]
        public int SECOND_SERVICE_CNT { get; set; }

        /// <summary>
        /// 同城通办人次
        /// </summary>
        [ResultColumn]
        public int LOCAL_SERVICE_CNT { get; set; }

        /// <summary>
        /// 总出票数
        /// </summary>
        [ResultColumn]
        public int TOT_TICKET_CNT { get; set; }

        /// <summary>
        /// 正在等候人次
        /// </summary>
        [ResultColumn]
        public int CUR_WAITING_CNT { get; set; }

        /// <summary>
        /// 等候达标率
        /// </summary>
        [ResultColumn]
        public decimal WAITING_ONTIME_RT { get; set; }

        /// <summary>
        /// 窗口饱和度预警 0：未饱和，1：饱和,2:非常饱和，3：极度饱和(如果返回为NULL，则为0未饱和)
        /// </summary>
        [ResultColumn]
        public int COUNTER_SATUR_RT_WARN { get; set; }

        /// <summary>
        /// 大厅饱和度预警 0：未饱和，1：饱和,2:非常饱和，3：极度饱和(如果返回为NULL，则为0未饱和)
        /// </summary>
        [ResultColumn]
        public int HALL_SATUR_RT_WARN { get; set; }

        /// <summary>
        /// 等候超时率
        /// </summary>
        [ResultColumn]
        public decimal WAITING_OVERTIME_RT { get; set; }

        /// <summary>
        /// 等候超时率预警 
        /// </summary>
        [ResultColumn]
        public int WAITING_OVERTIME_RT_WARN { get; set; }

        /// <summary>
        /// 当前平均等候时间,单位：分钟 
        /// </summary>
        [ResultColumn]
        public int CUR_AVG_WAIT_DUR { get; set; }

        /// <summary>
        /// 已受理人次
        /// </summary>
        [ResultColumn]
        public int SERVICE_TICKET_CNT { get; set; }

        /// <summary>
        /// 已受理业务笔数
        /// </summary>
        [ResultColumn]
        public int SERVICE_CNT { get; set; }

        /// <summary>
        /// 超时办结率
        /// </summary>
        [ResultColumn]
        public decimal OVERTIME_SERVICE_RT { get; set; }

        /// <summary>
        /// 按时办结率
        /// </summary>
        [ResultColumn]
        public decimal ONTIME_SERVICE_RT { get; set; }

        /// <summary>
        /// 超时业务笔数预警
        /// </summary>
        [ResultColumn]
        public int OVERTIME_SERVICE_CNT_WARN { get; set; }

        /// <summary>
        /// 弃号数
        /// </summary>
        [ResultColumn]
        public int ABANDON_CNT { get; set; }

        /// <summary>
        /// 弃号率
        /// </summary>
        [ResultColumn]
        public decimal ABANDON_RT { get; set; }

        /// <summary>
        /// 弃号预警
        /// </summary>
        [ResultColumn]
        public int ABANDON_WARN { get; set; }

        /// <summary>
        /// 一票多业务号数
        /// </summary>
        [ResultColumn]
        public int ONE_TICKET_MUTSERVICE_CNT { get; set; }

        /// <summary>
        /// 评价总数
        /// </summary>
        [ResultColumn]
        public int EVAL_CNT { get; set; }

        /// <summary>
        /// 未评价总数
        /// </summary>
        [ResultColumn]
        public int NON_EVAL_CNT { get; set; }

        /// <summary>
        /// 差评数
        /// </summary>
        [ResultColumn]
        public int UNSATISFY_CNT { get; set; }

        /// <summary>
        /// 未评价率
        /// </summary>
        [ResultColumn]
        public decimal NON_EVAL_RT { get; set; }

        /// <summary>
        /// 满意率
        /// </summary>
        [ResultColumn]
        public decimal SATISFY_RT { get; set; }

        /// <summary>
        /// 差评率
        /// </summary>
        [ResultColumn]
        public decimal UNSATISFY_RT { get; set; }

        /// <summary>
        /// 差评预警
        /// </summary>
        [ResultColumn]
        public int UNSATISFY_WARN { get; set; }

        /// <summary>
        /// 大厅容量--出 大厅饱和度
        /// </summary>
        [ResultColumn]
        public int HALL_LIMIT_CNT { get; set; }

        /// <summary>
        /// 超时业务笔数,--出超时办结率,按时办结率,超时业务笔数预警
        /// </summary>
        [ResultColumn]
        public int OVERTIME_SERVICE_CNT { get; set; }

        /// <summary>
        /// 差窗口饱和度评率
        /// </summary>
        [ResultColumn]
        public decimal COUNTER_SATUR_RT { get; set; }

        /// <summary>
        /// 大厅饱和度
        /// </summary>
        [ResultColumn]
        public decimal HALL_SATUR_RT { get; set; }
    }
}
