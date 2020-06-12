/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/12 20:24:28
 | 主要用途：数据包中单个数据项的定义
 | 更改记录：
 |			 时间		版本		更改
 */
namespace LoongEgg.UdpCore
{
    /// <summary>
    /// 单个数据项定义
    /// </summary>
    public class ItemConfig
    {
        /// <summary>
        /// 数据名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 字节长度
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// 数据精度，精度乘以传进来的数据=实际的数值
        /// </summary>
        public float Resolution { get; set; }

        /// <summary>
        /// 有无符号？
        /// </summary>
        public bool Signed { get; set; }

        /// <summary>
        /// 数据的最大值
        /// </summary>
        public float Maximum { get; set; }

        /// <summary>
        /// 数据的最小值
        /// </summary>
        public float Minimum { get; set; }

        /// <summary>
        /// 打印数据定义信息到字符串
        /// </summary>
        /// <returns>格式化后的定义信息</returns>
        public override string ToString()
        {
            return $"Name={Name}, Unit={Unit}, Resolution={Resolution}, Signed=" 
                + (Signed ? "true" : "false") 
                + $", Maximum={Maximum}, Minimum={Minimum}";
        }
    }
}
