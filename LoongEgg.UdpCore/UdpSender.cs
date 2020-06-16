using LoongEgg.LoongLog;
using Newtonsoft.Json;
using System;
using System.IO;
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
 | 时间         版本		更改
 | 2020-06-15  2.1.1    将EndPoint属性改为字段，增加基本类型定义的端口信息  
 */

namespace LoongEgg.UdpCore
{
    /// <summary>
    /// Udp发送器
    /// </summary>
    public sealed class UdpSender : IDisposable
    {
        /*------------------------------------ Fields -------------------------------------*/
        /// <summary>
        /// IP端口
        /// </summary>
        private IPEndPoint EndPoint;
        /// <summary>
        /// Udp端口
        /// </summary>
        private UdpClient UdpClient;

        private const string DefaultConfigFile = "config.udpsender.json";

        /*---------------------------------- Properties -----------------------------------*/
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; } = 5566;

        /// <summary>
        /// 组播
        /// </summary>
        public bool IsBroadCast { get; set; }

        /// <summary>
        /// 组地址
        /// </summary>
        public string GroupAddress { get; set; }

        /// <summary>
        /// 主机名字
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// 小端在前？
        /// </summary>
        public bool LittleEndian { get; set; }

        /// <summary>
        /// IpV6模式？
        /// </summary>
        public bool IsIpV6 { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        public string Tag { get; set; }

        /*----------------------------------- Constructor ---------------------------------*/ 
        /// <summary>
        /// Udp发送器的构造器, 创建后不要忘记调用<see cref="Init()"/>进行初始化
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="isBroadcast">组播?</param> 
        /// <param name="groupAddress">组地址</param>
        /// <param name="isIpV6">IpV6模式?</param>
        [Obsolete("直接使用方法CreatFromConfig()更香，使用默认构造器，初始化时指定属性")]
        public UdpSender(
            int port,
            bool isBroadcast = true,
            string groupAddress = null,
            bool isIpV6 = false)
        {
            Port = port;
            IsBroadCast = isBroadcast;
            GroupAddress = groupAddress;
            IsIpV6 = IsIpV6; 
        }

        /// <summary>
        /// 默认构造器, 创建后不要忘记调用<see cref="Init()"/>进行初始化
        /// </summary>
        public UdpSender() { }

        /*--------------------------------- Public Methods --------------------------------*/
        /// <summary>
        /// 从指定的配置文件创建一个发送器
        /// </summary>
        /// <param name="path">配置文件路径</param>
        /// <returns></returns>
        public static UdpSender CreatFromConfig(string path = null)
        {
            if (path == null)
            {
                if (File.Exists(DefaultConfigFile))
                {
                    using (StreamReader reader = File.OpenText(DefaultConfigFile))
                    {
                        JsonSerializer serializer = JsonSerializer.Create();
                        if (serializer.Deserialize(reader, typeof(UdpSender)) is UdpSender sender)
                        {
                            Logger.Info($"Reading default [{DefaultConfigFile}] config OK.");
                            return sender;
                        }
                        else
                        {
                            string message = $"UdpSender creat from default config file [{DefaultConfigFile}] failed";
                            Logger.Warn(message);
                            throw new ArgumentException(message);
                        }
                    }
                }
                else
                {
                    Logger.Warn($"Reading default config file [{DefaultConfigFile}] failed.");
                    Logger.Warn("UdpSender creat with default property");
                    return new UdpSender();
                }
            }
            else
            {
                using (StreamReader reader = File.OpenText(path))
                {
                    JsonSerializer serializer = JsonSerializer.Create();
                    if (serializer.Deserialize(reader, typeof(UdpSender)) is UdpSender sender)
                    {
                        Logger.Info("Reading default config OK.");
                        return sender;
                    }
                    else
                    {
                        string message = $"UdpSender creat from specifit config file [{path}] failed";
                        Logger.Warn(message);
                        throw new ArgumentException(message);
                    }
                }
            }
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        /// <param name="message">待发送的信息</param>
        /// <returns></returns>
        public async void SendAsync(string message)
        {
            if (EndPoint == null || UdpClient == null)
                throw new InvalidOperationException("Init() before first sending");
            try
            {
                Logger.Info($"Send > {message}");
                byte[] datagram = Encoding.UTF8.GetBytes(message);
                await UdpClient.SendAsync(datagram, datagram.Length, EndPoint);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*--------------------------------- Private Methods -------------------------------*/
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
        /// 初始化端口
        /// </summary>
        public void Init()
        {
            try
            {
                EndPoint = GetIPEndPoint(Port, IsBroadCast, HostName, GroupAddress, IsIpV6).Result;
                UdpClient = new UdpClient
                {
                    EnableBroadcast = IsBroadCast
                };
                if (GroupAddress != null && GroupAddress.ToLower() != "null")
                {
                    UdpClient.JoinMulticastGroup(IPAddress.Parse(GroupAddress));
                }
                Logger.Info("Udp sender initialized");
                Logger.Info(this.ToString() );
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 将配置属性转为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            Environment.NewLine + $"UdpSender: "
            + Environment.NewLine + $"    {nameof(Port)}={Port}"
            + Environment.NewLine + $"    {nameof(IsBroadCast)}={IsBroadCast}"
            + Environment.NewLine + $"    {nameof(HostName)}={HostName}"
            + Environment.NewLine + $"    {nameof(GroupAddress)}={GroupAddress}"
            + Environment.NewLine + $"    {nameof(IsIpV6)}={IsIpV6}";

        /*------------------------------------ Destructor ----------------------------------*/
        private bool disposed;
        /// <summary>
        /// <see cref="IDisposable.Dispose"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                if (GroupAddress != null)
                {
                    UdpClient?.DropMulticastGroup(IPAddress.Parse(GroupAddress));
                }
                UdpClient = null;
            }
            disposed = true;
        }

        /// <summary>
        /// 类型终结器
        /// </summary>
        ~UdpSender()
        {
            Dispose(false);
        }
    }
}
