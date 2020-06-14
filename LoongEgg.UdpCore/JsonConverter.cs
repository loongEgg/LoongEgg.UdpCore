using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/13 22:06:56
 | 主要用途：
 | 更改记录：
 |			 时间		版本		更改
 */
namespace LoongEgg.UdpCore
{
    /// <summary>
    /// Json序列化和反序列化工具
    /// </summary>
    [Obsolete("不可用", true)]
    public static class JsonConverter
    {
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T Desireialize<T>(string jsonString)
        {
            throw new NotImplementedException();
        }

        private static string Parse<T>(object obj, PropertyInfo p) => $"\"{p.Name}\":\"{p.GetValue(obj, null)}\""; 

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Serialize(object obj)
        {
            if (obj == null) return "{}";

            Type t = obj.GetType();

            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            PropertyInfo[] infos = t.GetProperties(flags);
            StringBuilder json = new StringBuilder("{" + Environment.NewLine);
            if (infos.Any())
            {
                int i = 0;
                int lastIndex = infos.Length - 1;

                foreach (PropertyInfo p in infos)
                {
                    // string
                    if (p.PropertyType.Equals(typeof(string)))
                    {
                        json.Append($"    \"{p.Name}\":\"{p.GetValue(obj, null)}\"");
                    }
                    // bool or data
                    else if (p.PropertyType.Equals(typeof(int))
                        || p.PropertyType.Equals(typeof(bool))
                        || p.PropertyType.Equals(typeof(double))
                        || p.PropertyType.Equals(typeof(byte))
                        || p.PropertyType.Equals(typeof(float))
                        || p.PropertyType.Equals(typeof(decimal)))
                    {
                        json.Append($"\"{p.Name}\":\"{p.GetValue(obj, null).ToString()}\"");
                    }
                    // array
                    else if (IsArray(p.PropertyType))
                    {
                        object o = p.GetValue(obj, null);
                        if (o == null)
                        {
                            json.Append($"\"{p.Name}\":{"null"}");
                        }
                        else
                        {
                            // Normal class type.
                            string subJsString = Serialize(p.GetValue(obj, null));
                            json.AppendFormat("\"{0}\":{1}", p.Name, subJsString);
                        }
                    }
                    else if (IsCustomClass(p.PropertyType))
                    {
                        object v = p.GetValue(obj, null);
                        if (v is IList)
                        {
                            IList list = v as IList;
                            string subJsString = GetIListValue(list);
                            json.AppendFormat("\"{0}\":{1}", p.Name, subJsString);
                        }
                        else
                        {
                            // Normal class type.
                            string subJsString = Serialize(p.GetValue(obj, null));
                            json.AppendFormat("\"{0}\":{1}", p.Name, subJsString);
                        }
                    }
                    else
                    {
                        Debugger.Break();

                    }
                    if (i >= 0 && i != lastIndex)
                    {
                        json.Append("," + Environment.NewLine);
                    }
                    ++i;
                }// end of foreach (PropertyInfo p in infos)
            }
            json.AppendLine("}");

            return json.ToString();
        }

        private static string GetIListValue(IList obj)
        {
            if (obj != null)
            {
                if (obj.Count == 0)
                {
                    return "[]";
                }
                object firstElement = obj[0];
                Type et = firstElement.GetType();
                bool quotable = et == typeof(string);
                StringBuilder sb = new StringBuilder("[" + Environment.NewLine);
                int index = 0;
                int lastIndex = obj.Count - 1;
                if (quotable)
                {
                    foreach (var item in obj)
                    {
                        sb.AppendFormat("\"{0}\"", item.ToString());
                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append("," + Environment.NewLine);
                        }
                        ++index;
                    }
                }
                else
                {
                    foreach (var item in obj)
                    {
                        sb.Append(item.ToString());
                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append("," + Environment.NewLine);
                        }
                        ++index;
                    }
                }
                sb.Append("]" + Environment.NewLine);
                return sb.ToString();
            }
            return "null";
        }

        private static object GetArrayValue(Array obj)
        {
            if (obj == null) return "null";
            if (obj.Length == 0) return "[]";

            object firstElement = obj.GetValue(0);
            Type et = firstElement.GetType();
            bool quotable = et == typeof(string);
            StringBuilder sb = new StringBuilder("[");
            int index = 0;
            int lastIndex = obj.Length - 1;
            if (quotable)
            {
                foreach (var item in obj)
                {
                    sb.AppendLine($"\"{item.ToString()}\"");
                    if (index >= 0 && index != lastIndex)
                    {
                        sb.Append("," + Environment.NewLine);
                    }
                    ++index;
                }
            }
            else
            {
                foreach (var item in obj)
                {
                    sb.AppendLine(item.ToString());
                    if (index >= 0 && index != lastIndex)
                    {
                        sb.Append("," + Environment.NewLine);
                    }
                    ++index;
                }
            }
            sb.Append("]" + Environment.NewLine);
            return sb.ToString();
        }

        private static bool IsArray(Type t)
        {
            if (t != null)
            {
                return t.IsArray;
            }
            return false;
        }

        private static bool IsCustomClass(Type t)
        {
            if (t != null)
            {
                return t.IsClass && t != typeof(string);
            }
            return false;
        }
    }
}
