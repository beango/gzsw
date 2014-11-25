using gzsw.model;
using PetaPoco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gzsw.dal.dao
{
    public class WARN_SENDINFO_DETAIL_DAL
    {
        /// <summary>
        /// 修改为已发送
        /// </summary>
        /// <param name="id"></param>
        public void UPDATE_WARN_SENDINFO_DETAIL(IEnumerable<long> id)
        {
            if (null!=id&&id.Count()>0)
            {
                var sql = Sql.Builder.Append("update WARN_SENDINFO_DETAIL set CLI_SEND_STATE=1 where SENDINFO_DETAIL_ID in(@0)", id);
                gzswDB.GetInstance().Execute(sql);
            }
        }
    }
}
