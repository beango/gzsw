using System;
using System.Collections.Generic;
using PetaPoco;
using gzsw.util;

namespace gzsw.dal
{

    public class DaoTemplate<T> : IDao<T> where T : class
    {
        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <param name="obj">待添加的对象</param>
        public virtual T AddObject(T obj)
        {
            var db = new Database();
            db.Insert(obj);
            return obj;
        }

        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <param name="list">待添加的对象</param>
        public virtual void AddObject(List<T> list)
        {
            foreach (var obj in list)
                AddObject(obj);
        }

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="val">待删除的对象</param>
        /// <param name="priKey"></param>
        public void Delete(string priKey, object val)
        {
            var db = new Database();
            db.Delete(typeof(T).Name, priKey, null, val);
        }

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="obj">待删除的对象</param>
        public virtual void DeleteObject(T obj)
        {
            var db = new Database();
            db.Delete(obj);
        }

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="list">待删除的对象</param>
        public virtual void DeleteObject(List<T> list)
        {
            foreach (var obj in list)
            {
                DeleteObject(obj);
            }
        }

        /// <summary>
        /// 根据条件获取某个对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        public virtual T GetEntity(params object[] paras)
        {
            var db = new Database();

            var sql = Sql.Builder.Append("From dbo." + typeof(T).Name + " where 1=1 ");
            if (null != paras)
            {
                for (int i = 0; i < paras.Length; i += 2)
                {
                    sql.Append("and " + paras[i] + " =@0 ", paras[i + 1]);
                }
            }

            var data = db.FirstOrDefault<T>(sql);
            return data;
        }

        /// <summary>
        /// 根据实体的实体键修改实体
        /// </summary>
        /// <param name="obj">需要更新的实体</param>
        /// <param name="propertys">主键列名</param>
        public virtual int UpdateObject(T obj, params string[] primaryKey)
        {
            try
            {
                var db = new Database();
                if (null == primaryKey || primaryKey.Length == 0)
                {
                    db.Save(obj);
                }
                else
                {
                    db.Save(typeof(T).Name, primaryKey[0], obj);
                }
                return 1;
            }
            catch (Exception ex)
            {
                LogHelper.ErrorLog("UpdateObject", ex);
                return 0;
            }
        }

        /// <summary>
        /// 根据多个条件进行查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>符合条件的实体的集合</returns>
        public IList<T> FindList()
        {
            var db = new Database();

            var sql = Sql.Builder.Append("From dbo." + typeof(T).Name);

            var data = db.Fetch<T>(sql);
            return data;
        }

        #region PETAPOCO

        /// <summary>
        /// 根据条件进行查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="orderby"></param>
        /// <param name="paras"></param>
        /// <returns>符号条件的实体的集合</returns>
        public Page<T> GetList(int pageIndex, int pageSize, string orderby, params object[] paras)
        {
            var db = new Database();

            var sql = Sql.Builder.Append("From dbo." + typeof(T).Name + " where 1=1 ");
            if (null != paras)
            {
                for (int i = 0; i < paras.Length; i += 2)
                {
                    if (null == paras[i + 1])
                        continue;
                    if (paras[i + 1] is string && string.IsNullOrEmpty(paras[i + 1].ToString()))
                        continue;
                    if (paras[i].ToString().EndsWith(" like"))
                        sql.Append("and " + paras[i] + " @0 ", "%" + paras[i + 1] + "%");
                    else if (paras[i].ToString().EndsWith(" in"))
                    {
                        if (null != paras[i + 1])
                            sql.Append("and " + paras[i] + " (@0) ", paras[i + 1]);
                    }
                    else
                        sql.Append("and " + paras[i] + " =@0 ", paras[i + 1]);
                }
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                sql.Append("order by " + orderby);
            }

            var data = db.Page<T>(pageIndex, pageSize, sql);
            data.ItemsPerPage = pageSize;
            return data;
        }

        /// <summary>
        /// 根据条件进行查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>符号条件的实体的集合</returns>
        public IList<T> FindList(string orderby, params object[] paras)
        {
            var db = new Database();

            var sql = Sql.Builder.Append("From dbo." + typeof(T).Name + " where 1=1 ");
            if (null != paras)
            {
                for (int i = 0; i < paras.Length; i += 2)
                {
                    if (null == paras[i + 1])
                        continue;
                    if (paras[i + 1] is string && string.IsNullOrEmpty(paras[i + 1].ToString()))
                        continue;
                    if (paras[i].ToString().EndsWith(" like"))
                        sql.Append("and " + paras[i] + " @0 ", "%" + paras[i + 1] + "%");
                    else if (paras[i].ToString().EndsWith(" in"))
                    {
                        if (null != paras[i + 1])
                            sql.Append("and " + paras[i] + " (@0) ", paras[i + 1]);
                    }
                    else
                        sql.Append("and " + paras[i] + " =@0 ", paras[i + 1]);
                }
            }
            if (!string.IsNullOrEmpty(orderby))
            {
                sql.Append("order by " + orderby);
            }

            var data = db.Fetch<T>(sql);
            return data;
        }

        /// <summary>
        /// 检查记录是否存在
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public bool Exists(params object[] paras)
        {
            var db = new Database();

            var sql = Sql.Builder.Append("select count (1) From dbo." + typeof(T).Name + " where 1=1 ");
            if (null != paras)
            {
                for (int i = 0; i < paras.Length; i += 2)
                {
                    if (null == paras[i + 1])
                        continue;
                    if (paras[i + 1] is string && string.IsNullOrEmpty(paras[i + 1].ToString()))
                        continue;
                    if (paras[i].ToString().EndsWith(" like"))
                        sql.Append("and " + paras[i] + " @0 ", "%" + paras[i + 1] + "%");
                    else if (paras[i].ToString().EndsWith(" in"))
                    {
                        if (null != paras[i + 1])
                            sql.Append("and " + paras[i] + " (@0) ", paras[i + 1]);
                    }
                    else
                        sql.Append("and " + paras[i] + " =@0 ", paras[i + 1]);
                }
            }
            return db.ExecuteScalar<int>(sql)>0;
        }
        #endregion
    }
}
