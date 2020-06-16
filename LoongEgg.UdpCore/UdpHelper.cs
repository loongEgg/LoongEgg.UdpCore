using System;
using System.Linq;

/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/16 11:35:24
 | 主要用途：
 | 更改记录：
 | 时间	       版本        更改
 */
namespace LoongEgg.UdpCore
{
    /// <summary>
    /// Udp工具类
    /// </summary>
    public static class UdpHelper
    {
        /// <summary>
        /// int类型转义失败标志
        /// </summary>
        public const int IntFailFlag = -111000;

        /*--------------------------------- Public Methods --------------------------------*/
        /// <summary>
        /// 根据key值获取响应的param -key param
        /// </summary>
        /// <param name="args">命令行参数数组</param>
        /// <param name="key">目标key</param>
        /// <param name="value">转义结果</param>
        /// <returns> 
        ///     [false]:转义失败
        /// </returns>
        public static bool TryParseCommandParam(string[] args, string key, out string value)
        {
            int index = args.Contains(key.ToLower()) ? Array.IndexOf(args, key) + 1 : -1;
            bool success = (index > 0 && index < args.Length);
            value = success ? args[index] : null; ;
            return success;
        }

        /// <summary>
        /// 根据key值获取响应的param -key param
        /// </summary>
        /// <param name="args">命令行参数数组</param>
        /// <param name="key">目标key</param>
        /// <param name="value">转义结果, defalut=[-111000]转义失败</param>
        /// <returns> 
        ///     [false]:转义失败
        /// </returns>
        public static bool TryParseCommandParam(string[] args, string key, out int value)
        {
            value = IntFailFlag;
            string strvalue;
            if (UdpHelper.TryParseCommandParam(args, key, out strvalue))
            {
                if (!int.TryParse(strvalue, out value))
                {
                    return false;
                }
                return true;
            }

            return false;
        }
        /*--------------------------------- Private Methods -------------------------------*/
    }
}
