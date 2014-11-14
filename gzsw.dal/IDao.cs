using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PetaPoco;

namespace gzsw.dal
{
    public interface IDao<T> where T : class
    {
        Page<T> GetList(int pageIndex, int pageSize, string orderby, params object[] paras);

        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <param name="obj">待添加的对象</param>
        T AddObject(T obj);

        /// <summary>
        /// 添加一个对象
        /// </summary>
        /// <param name="list">待添加的对象</param>
        void AddObject(List<T> list);

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="priKey"></param>
        /// <param name="val"></param>
        void Delete(string priKey,object val);

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="obj">待删除的对象</param>
        void DeleteObject(T obj);

        /// <summary>
        /// 删除一个对象
        /// </summary>
        /// <param name="list">待删除的对象</param>
        void DeleteObject(List<T> list);

        /// <summary>
        /// 根据条件获取某个对象
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>实体</returns>
        T GetEntity(params object[] paras);

        /// <summary>
        /// 根据实体的实体键修改实体
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertys"></param>
        int UpdateObject(T obj, params string[] propertys);

        /// <summary>
        /// 根据多个条件进行查询
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="where">查询条件</param>
        /// <returns>符合条件的实体的集合</returns>
        IList<T> FindList();

        /// <summary>
        /// 查询记录
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        IList<T> FindList(string orderby, params object[] paras);

        /// <summary>
        /// 检查记录是否存在
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        bool Exists(params object[] paras);

        Page<T> GetPage(int pageIndex, int pageSize, params object[] paras);
    }
}
