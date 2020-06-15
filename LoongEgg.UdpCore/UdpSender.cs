using LoongEgg.LoongLog;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/12 19:56:01
 | 主要用途：Udp发送器，并根据指定的json格式的数据包协议解析数据
 | 更改记录：
 |			 时间		版本		更改
 */

namespace LoongEgg.UdpCore
{
    /// <summary>
    /// Udp发送器
    /// </summary>
    public class UdpSender
    {
        /// <summary>
        /// IP端口
        /// </summary>
        public IPEndPoint EndPoint { get; private set; }

        /// <summary>
        /// 组地址
        /// </summary>
        public string GroupAddress { get; private set; }

        /// <summary>
        /// Udp发送器的构造器
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="isBroadcast">组播?</param> 
        /// <param name="groupAddress">组地址</param>
        /// <param name="isIpV6">IpV6模式?</param>
        public UdpSender(
            int port,
            bool isBroadcast,
            string groupAddress = null,
            bool isIpV6 = false)
        {
            GroupAddress = groupAddress;
            string hostName = Dns.GetHostName();
            EndPoint = GetIPEndPoint(port, isBroadcast, hostName, groupAddress, isIpV6).Result;
            Logger.Info("Udp sender initialized");
            Logger.Info(
                $"IPEndPoint: "
                + $"{nameof(port)}={port}, i{nameof(isBroadcast)}={isBroadcast}, "
                + $"{nameof(hostName)}={hostName}, {nameof(groupAddress)}={groupAddress}, "
                + $"{nameof(isIpV6)}={isIpV6}");
        }

        /// <summary>
        /// 获取指定的IP端口
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="isBroadcast">组播?</param>
        /// <param name="hostName">主机名称</param>
        /// <param name="groupAddress">组地址</param>
        /// <param name="isIpV6">IpV6模式?</param>
        /// <returns></returns>
        private static async Task<IPEndPoint> GetIPEndPoint(
            int port,
            bool isBroadcast,
            string hostName,
            string groupAddress,
            bool isIpV6)
        {
            IPEndPoint endpoint;
            try
            {
                if (isBroadcast)
                {
                    endpoint = new IPEndPoint(IPAddress.Broadcast, port);
                    Logger.Info($"{nameof(isBroadcast)}={isBroadcast}, {nameof(port)}={port} ");
                }
                else if (hostName != null)
                {
                    IPHostEntry hostEntry = await Dns.GetHostEntryAsync(hostName);
                    IPAddress address;
                    if (isIpV6)
                    {
                        address = hostEntry.AddressList.Where(
                            a => a.AddressFamily == AddressFamily.InterNetworkV6
                        ).FirstOrDefault();
                    }
                    else
                    {
                        address = hostEntry.AddressList.Where(
                            a => a.AddressFamily == AddressFamily.InterNetwork
                        ).FirstOrDefault();
                    }
                    endpoint = new IPEndPoint(address, port);
                    Logger.Info($"{nameof(hostName)}={hostName}, {nameof(address)}={address}, {nameof(isIpV6)}={isIpV6}");
                }
                else if (groupAddress != null)
                {
                    endpoint = new IPEndPoint(IPAddress.Parse(groupAddress), port);
                    Logger.Info($"{nameof(groupAddress)}={groupAddress}, {nameof(port)}={port} ");

                }
                else
                {
                    throw new InvalidOperationException($"{nameof(hostName)}, {nameof(isBroadcast)}, or {nameof(groupAddress)} must be set");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return endpoint;
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="message">待发送的信息</param>
        /// <returns></returns>
        public async void SendAsync(string message)
        {
            try
            {
                string localhost = Dns.GetHostName();
                using (var client = new UdpClient())
                {
                    if (GroupAddress != null)
                    {
                        client.JoinMulticastGroup(IPAddress.Parse(GroupAddress));
                    }
                    Logger.Info($"Sending > {message}");
                    byte[] datagram = Encoding.UTF8.GetBytes(message);
                    await client.SendAsync(datagram, datagram.Length, EndPoint);
                    Logger.Debug($"Sending finished");

                    if (GroupAddress != null)
                    {
                        client.DropMulticastGroup(IPAddress.Parse(GroupAddress));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
