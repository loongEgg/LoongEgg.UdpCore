using LoongEgg.LoongLog;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/12 19:56:01
 | 主要用途：Udp接收器，并根据指定的json格式的数据包协议解析数据
 | 更改记录：
 |			 时间		版本		更改
 */
namespace LoongEgg.UdpCore
{
    /// <summary>
    /// Udp接收器
    /// </summary>
    public class UdpReceiver
    {
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// 组地址
        /// </summary>
        public string GroupAddress { get; private set; }

        /// <summary>
        /// 创建一个新的接收器
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="groupAddress">组地址</param>
        public UdpReceiver(int port, string groupAddress = null)
        {
            GroupAddress = groupAddress;
            Port = port;
            Logger.Info($"Listening to {nameof(Port)}={Port}, {nameof(GroupAddress)}={GroupAddress}");
        }

        /// <summary>
        /// 接收器工作
        /// </summary> 
        /// <returns></returns>
        public async Task ReaderAsync()
        {
            using (var client = new UdpClient(Port))
            {
                if (GroupAddress != null)
                {
                    Logger.Debug($"JoinMulticastGroup = {GroupAddress}");
                    client.JoinMulticastGroup(IPAddress.Parse(GroupAddress));
                }

                bool completed = false;
                do
                {
                    Logger.Debug("Listening...");
                    UdpReceiveResult result = await client.ReceiveAsync();
                    byte[] datagram = result.Buffer; ;
                    string received = Encoding.UTF8.GetString(datagram);
                    Logger.Info($"Received (from {result.RemoteEndPoint.Address.ToString()})-> {received}");
                    completed = (received == "stop");
                } while (!completed);
                 
                if (GroupAddress != null)
                {
                    client.DropMulticastGroup(IPAddress.Parse(GroupAddress));
                }
                
                Logger.Info("Receiver closing...s"); 
            }
        }
    }
}
