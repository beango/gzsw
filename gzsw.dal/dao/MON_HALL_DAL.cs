using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using gzsw.model;
using PetaPoco;

namespace gzsw.dal.dao
{
    /// <summary>
    /// 服务大厅的配置
    /// </summary>
    public class MON_HALL_DAL
    {
        public List<MON_HALL_TAB_DEF> GetHallTabDefs(string hallNo)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"FROM [MON_HALL_TAB_DEF] 
                                          WHERE [HALL_NO]=@0",hallNo);

            return db.Fetch<MON_HALL_TAB_DEF>(sql);
        }

        public List<MON_HALL_CAMERA_DEF> GetHallCameraDefs(string hallNo)
        {
            var db = gzswDB.GetInstance();

            var sql = Sql.Builder.Append(@"FROM [MON_HALL_CAMERA_DEF] 
                                          WHERE [HALL_NO]=@0", hallNo);

            return db.Fetch<MON_HALL_CAMERA_DEF>(sql);
        }

        public void SaveHallTabDef(MON_HALL_TAB_DEF item)
        {
            var db = gzswDB.GetInstance();
            var tab = db.FirstOrDefault<MON_HALL_TAB_DEF>(Sql.Builder.Append("SELECT * FROM MON_HALL_TAB_DEF WHERE [HALL_NO]=@0 AND [COUNTER_ID]=@1", item.HALL_NO, item.COUNTER_ID));
            if (tab == null)
            {
                db.Insert(item);
            }
            else
            {
                db.Execute(Sql.Builder.Append(@"UPDATE [MON_HALL_TAB_DEF]
                                               SET 
                                                  [HORIZ_SIGN] = @0
                                                  ,[VERTI_SIGN] = @1
                                                  ,[ICON_URL] = @2
                                             WHERE [HALL_NO] = @3 AND [COUNTER_ID]=@4",
                                            item.HORIZ_SIGN,item.VERTI_SIGN,item.ICON_URL,item.HALL_NO,item.COUNTER_ID));
            }
        }

        public void DeleteHallTabDef(string hallNo)
        {
            var db = gzswDB.GetInstance();
            var sql = Sql.Builder.Append(@"DELETE FROM [MON_HALL_TAB_DEF] WHERE [HALL_NO] = @0 ", hallNo);
            db.Execute(sql);
        }


        public void SaveHallCameraDef(MON_HALL_CAMERA_DEF item)
        {
            var db = gzswDB.GetInstance();
            if (item.SEQ > 0)
            {
                db.Update(item);
            }
            else
            {
                db.Insert(item);
            }
        }

        public void RemoveHallTabDef(string hallNo, int counterId)
        {
            var db = gzswDB.GetInstance();
            db.Execute(Sql.Builder.Append(@"DELETE FROM [MON_HALL_TAB_DEF]
                                             WHERE [HALL_NO] = @0 AND [COUNTER_ID]=@1",
                                            hallNo,counterId));
        }

        public void RemoveHallCameraDef(int seq)
        {
            var db = gzswDB.GetInstance();

            db.Execute(Sql.Builder.Append(@"DELETE FROM [MON_HALL_CAMERA_DEF]
                                             WHERE [SEQ] = @0 ",
                                            seq));
        }
    }
}
