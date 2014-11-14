using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web.UI.WebControls;

namespace gzsw.controller.MON.Models
{
    /// <summary>
    /// 饱和度
    /// </summary>
    public enum SaturationEnum
    {
        [Description("未饱和")]
        未饱和=0,
        [Description("饱和")]
        饱和=1,
        [Description("非常饱和")]
        非常饱和=2,
        [Description("极度饱和")]
        极度饱和=3
    }

    /// <summary>
    /// 警报级别
    /// </summary>
    public enum AlarmLevelEnum
    {
        [Description("正常")]
        正常 = 0,
        [Description("黄色预警")]
        黄色预警 = 1,
        [Description("橙色预警")]
        橙色预警 = 2,
        [Description("红色预警")]
        红色预警 = 3
    }

    /// <summary>
    /// 管理情况
    /// </summary>
    public class ManagementInfoModel
    {
        /// <summary>
        /// 总窗口数
        /// </summary>
        public int TotalNum { get; set; }

        /// <summary>
        /// 在线窗口数
        /// </summary>
        public int OnlineNum { get; set; }

        /// <summary>
        /// 开启率
        /// </summary>
        public decimal OpenRate { get; set; }

        /// <summary>
        /// 正在办理业务窗口数
        /// </summary>
        public int HandleNum { get; set; }

        /// <summary>
        /// 在办业务窗口比率
        /// </summary>
        public decimal HandleRate { get; set; }

        /// <summary>
        /// 窗口突发情况与报警
        /// </summary>
        public int PoliceNum { get; set; }

        /// <summary>
        /// 投诉次数
        /// </summary>
        public int ComplaintsNum { get; set; }

        /// <summary>
        /// 当月二次办理人次
        /// </summary>
        public int SecondaryHandleNum { get; set; }

        /// <summary>
        /// 同城通办人次
        /// </summary>
        public int CityNum { get; set; }
    }

    /// <summary>
    /// 排队情况
    /// </summary>
    public class QueuingSituationModel
    {
        /// <summary>
        /// 总出票数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 正在等候人次
        /// </summary>
        public int WaitingNum { get; set; }

        /// <summary>
        /// 等候达标率
        /// </summary>
        public decimal WaitingRate { get; set; }

        /// <summary>
        /// 窗口饱和度
        /// </summary>
        public SaturationEnum WindowPate { get; set; }

        /// <summary>
        /// 大厅饱和度
        /// </summary>
        public SaturationEnum HallPate { get; set; }

        /// <summary>
        /// 等候超时率
        /// </summary>
        public decimal WaitingTimeoutPate { get; set; }

        /// <summary>
        /// 等待超时人次预警
        /// </summary>
        public AlarmLevelEnum WaitingTimeout { get; set; }

        /// <summary>
        /// 当前平均等候时间 单位分钟
        /// </summary>
        public int AverageTime { get; set; }
    }

    /// <summary>
    /// 服务情况
    /// </summary>
    public class ServiceInfoModel
    {
        /// <summary>
        /// 已受理人次
        /// </summary>
        public int AcceptedNum { get; set; }

        /// <summary>
        /// 已受理业务笔数
        /// </summary>
        public int OnServiceNum { get; set; }

        /// <summary>
        /// 超时办结率
        /// </summary>
        public decimal TimeoutRate { get; set; }

        /// <summary>
        /// 按时办结率
        /// </summary>
        public decimal OnTimeRate { get; set; }

        /// <summary>
        /// 超时业务笔数预警
        /// </summary>
        public AlarmLevelEnum TimeoutServiceWarning { get; set; }

        /// <summary>
        /// 弃号数
        /// </summary>
        public int LeaveNum{ get; set; }

        /// <summary>
        /// 弃号率
        /// </summary>
        public decimal LeaveRate { get; set; }

        /// <summary>
        /// 弃号预警
        /// </summary>
        public AlarmLevelEnum LeaveWarning { get; set; }

        /// <summary>
        /// 一票多业务号数
        /// </summary>
        public int OneMoreServiceNum { get; set; }
    }

    /// <summary>
    /// 纳税人评价情况
    /// </summary>
    public class EvaluationInfoModel
    {
        /// <summary>
        /// 评价总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 未评价总数
        /// </summary>
        public int UnTotal { get; set; }

        /// <summary>
        /// 差评数
        /// </summary>
        public int PoorNum { get; set; }

        /// <summary>
        /// 未评价率
        /// </summary>
        public decimal UnEvaluationRate { get; set; }

        /// <summary>
        /// 满意率
        /// </summary>
        public decimal SatisfactionRate { get; set; }

        /// <summary>
        /// 差评率
        /// </summary>
        public decimal BadRate { get; set; }

        /// <summary>
        /// 差评预警
        /// </summary>
        public AlarmLevelEnum BadWarning { get; set; }
    }
}
