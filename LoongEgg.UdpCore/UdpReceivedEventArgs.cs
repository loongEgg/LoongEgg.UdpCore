using System;

/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/12 19:56:01
 | 主要用途：Udp接收器，并根据指定的json格式的数据包协议解析数据
 | 更改记录：
 | 时间         版本		更改
 | 2020-06-15  2.1.1    Udp接收器增加默认控制台DefaultConsole()，并且可以读取默认配置或者命令行交互配置
 | 2020-06-15  2.1.1    Udp接收器增加 UdpReceivedEvent 事件 和 UdpReceivedEventArgs 参数，来处理新接收到的消息
 */
namespace LoongEgg.UdpCore
{
    /// <summary>
    /// Udp接收事件
    /// </summary>
    public delegate void UdpReceivedEvent(object sender, UdpReceivedEventArgs args);

    /// <summary>
    /// Udp接收事件参数
    /// </summary>
    public class UdpReceivedEventArgs: EventArgs
    {
        /// <summary>
        /// 接收到的缓存信息
        /// </summary>
        public byte[] Buffer { get; set; }

        /// <summary>
        /// 默认构造器
        /// </summary>
        /// <param name="buffer"></param>
        public UdpReceivedEventArgs(byte[] buffer)
        {
            Buffer = buffer;
        }

    }
}
