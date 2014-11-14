
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace gzsw.util.Extensions
{
    /// <summary>
    ///  Object 扩展
    /// </summary>
    /// <remark>
    ///     <para>    Creator：hcli </para>
    ///     <para>CreatedTime：2014/11/8 13:10:20</para>
    /// </remark>
   public  static class ObjectExtensions
    {
        /// <summary>
        /// 获取枚举标记中说明文字<br/>
        /// (枚举成员的[Description("说明文字")],使用说明:MyEnum.Test.GetDescription())
        /// </summary>
        /// <param name="en">枚举成员</param>
        /// <returns>返回说明文字,如果没有则返回英文的字段名</returns>
       public static DataTable ToDataTable(this Object list)
        {
            DataTable dt = null;
            Type listType = list.GetType();
            if (listType.IsGenericType)
            {
                //determine the underlying type the List<> contains  
                Type elementType = listType.GetGenericArguments()[0];

                //create empty table -- give it a name in case  
                //it needs to be serialized  
                dt = new DataTable(elementType.Name + "List");

                //define the table -- add a column for each public  
                //property or field  
                MemberInfo[] miArray = elementType.GetMembers(
                    BindingFlags.Public | BindingFlags.Instance);
                foreach (MemberInfo mi in miArray)
                {
                    if (mi.MemberType == MemberTypes.Property)
                    {
                        PropertyInfo pi = mi as PropertyInfo;
                        dt.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    else if (mi.MemberType == MemberTypes.Field)
                    {
                        FieldInfo fi = mi as FieldInfo;
                        dt.Columns.Add(fi.Name, fi.FieldType);
                    }
                }

                //populate the table  
                IList il = list as IList;
                foreach (object record in il)
                {
                    int i = 0;
                    object[] fieldValues = new object[dt.Columns.Count];
                    foreach (DataColumn c in dt.Columns)
                    {
                        MemberInfo mi = elementType.GetMember(c.ColumnName)[0];
                        if (mi.MemberType == MemberTypes.Property)
                        {
                            PropertyInfo pi = mi as PropertyInfo;
                            fieldValues[i] = pi.GetValue(record, null);
                        }
                        else if (mi.MemberType == MemberTypes.Field)
                        {
                            FieldInfo fi = mi as FieldInfo;
                            fieldValues[i] = fi.GetValue(record);
                        }
                        i++;
                    }
                    dt.Rows.Add(fieldValues);
                }
            }
            return dt;  
        }

        /// <summary>
        /// 转换简单类型的基类对象
        /// </summary>
        /// <param name="obj">基类对象</param>
        /// <returns></returns>
       public static string ToStringForSimple(this object obj)
        {
            if (obj == DBNull.Value || obj == null)
            {
                return string.Empty;
            }
            try
            {
                return Convert.ToString(obj);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
