using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace gzsw.dal
{
    /// <summary>
    /// 分页信息类
    /// </summary>
    /// <typeparam name="T">分页的实体类型</typeparam>
    /// <typeparam name="TKey">排序的属性类型</typeparam>
    /// <typeparam name="TResult">投影结果类型</typeparam>
    [Serializable]
    public class PageInfo<T, TKey, TResult>
    {
        /// <summary>
        /// 条件表达式树
        /// </summary>
        public Expression<Func<T, bool>> Where { get; set; }
        /// <summary>
        /// 排序表达式树
        /// </summary>
        public Expression<Func<T,TKey>> Order { get; set; }

        private String _include=string.Empty;
        /// <summary>
        /// 要抓取的导航属性
        /// </summary>
        public string Include
        {
            get
            {
                return _include;
            }
            set
            {
                _include = value;
            }
        }

        /// <summary>
        /// 投影表达式树
        /// </summary>
        public Expression<Func<T,TResult>> Select { get; set; }

        private int _pageSize = 10;
        /// <summary>
        /// 页大小
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set { _pageSize = value; }
        }

        private int _pageIndex = 1;
        /// <summary>
        /// 页号
        /// </summary>
        public int PageIndex
        {
            get { return _pageIndex; }
            set { _pageIndex = value; }
        }

        /// <summary>
        /// 总条数
        /// </summary>
        public long RecordCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public long PageCount { get; set; }

        /// <summary>
        /// 分页结果
        /// </summary>
        public ICollection<TResult> List { get; set; }

    }
}
