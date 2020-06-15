using LoongEgg.LoongLog;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Linq;

/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/12 19:56:01
 | 主要用途：Udp接收器，并根据指定的json格式的数据包协议解析数据
 | 更改记录：
 | 时间         版本		更改
 | 2020-06-15  2.1.1    Udp接收器增加默认控制台，并且可以读取默认配置或者命令行交互配置        
 */
namespace LoongEgg.UdpCore
{
    /// <summary>
    /// Udp接收器
    /// </summary>
    public class UdpReceiver
    {
        /// <summary>
        /// 默认配置文件
        /// </summary>
        private static readonly string DefaultConfigFile = "config.udpreceiver.json";

        /// <summary>
        /// 使用说明
        /// </summary>
        private static readonly string Usage =
            Environment.NewLine + "    Usage: -p port  [-g groupaddress] [-t tag]" +
            Environment.NewLine + "           -p port number listen to" +
            Environment.NewLine + "           -g group address(224.0.0.0, 239.255.255.255)" +
            Environment.NewLine + "           -t tag" +
            Environment.NewLine + "     Info: config options will be saved as config.udpreceiver.json";

        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 组地址
        /// </summary>
        public string GroupAddress { get; set; }

        /// <summary>
        /// 接收器标识
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 创建一个新的接收器
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="groupAddress">组地址</param>
        public UdpReceiver(int port, string groupAddress = null)
        {
            GroupAddress = groupAddress;
            Port = port;
        }

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public UdpReceiver() { }

        /// <summary>
        /// 默认控制台程序实现
        /// </summary>
        /// <param name="useDefaultConfig">default=true, 使用默认的配置文件</param>
        public static void DefaultConsole(bool useDefaultConfig = true)
        {
            Logger.Info($"Try reading default UDP receiver config:{DefaultConfigFile}");

            UdpReceiver receiver = null;
            if (useDefaultConfig && File.Exists(DefaultConfigFile))
            {
                using (StreamReader reader = File.OpenText(DefaultConfigFile))
                {
                    JsonSerializer serializer = JsonSerializer.Create();
                    receiver = serializer.Deserialize(reader, typeof(UdpReceiver)) as UdpReceiver;
                    if (receiver != null)
                    {
                        Logger.Info(receiver.ToString());
                        Logger.Info("Reading default config OK.");
                    }
                }
            }
            else
            {
                Logger.Warn("Reading default UDP receiver config falied!");
                Logger.Info(Usage);
                bool unconfig = true;
                do
                {
                    Logger.Info("Input udp config options:");
                    string command = Console.ReadLine();
                    string[] args = command.Split(' ');
                    int port;
                    string group;
                    string tag;
                    ParseCommandOptions(args, out port, out group, out tag);
                    if (port != 0)
                    {
                        receiver = new UdpReceiver { Port = port, GroupAddress = group, Tag = tag };
                        Logger.Info("Udp initial as" + receiver.ToString() + "?");
                        Logger.Info($"Enter Y/y to confirm, and save as {DefaultConfigFile}");
                        unconfig = !(Console.ReadLine().ToLower() == "y");
                    }
                    else
                    {
                        Logger.Warn("option missed: -p [port number] ");
                    }
                } while (unconfig);
            }
            receiver?.ReceiveAsync().Wait();
        }

        /// <summary>
        /// 接收器工作
        /// </summary> 
        /// <returns></returns>
        [Obsolete("使用ReceiveAsync()")]
        public async Task ReaderAsync()
        {
            using (var client = new UdpClient(Port))
            {
                if (GroupAddress != null)
                {
                    Logger.Debug($"JoinMulticastGroup = {GroupAddress}");
                    client.JoinMulticastGroup(IPAddress.Parse(GroupAddress));
                }

                bool completed;
                do
                {
                    Logger.Debug("Listening...");
                    UdpReceiveResult result = await client.ReceiveAsync();
                    byte[] datagram = result.Buffer; ;
                    string received = Encoding.UTF8.GetString(datagram);
                    Logger.Info($"Received (from {result.RemoteEndPoint.Address})-> {received}");
                    completed = (received.ToLower() == "stop");
                } while (!completed);

                if (GroupAddress != null)
                {
                    client.DropMulticastGroup(IPAddress.Parse(GroupAddress));
                }

                Logger.Info("Receiver closed");
            }
        }

        /// <summary>
        /// 接收器工作
        /// </summary> 
        /// <returns></returns>
        public async Task ReceiveAsync()
        {
            using (var client = new UdpClient(Port))
            {
                if (GroupAddress != null)
                {
                    Logger.Debug($"Join Multicast Group = {GroupAddress}");
                    client.JoinMulticastGroup(IPAddress.Parse(GroupAddress));
                }

                bool completed;
                do
                {
                    Logger.Debug("Start Listening...Sending in stop to stop listening");
                    UdpReceiveResult result = await client.ReceiveAsync();
                    byte[] datagram = result.Buffer; ;
                    string received = Encoding.UTF8.GetString(datagram);
                    Logger.Info($"Received (from {result.RemoteEndPoint.Address}) > {received}");
                    completed = (received.ToLower() == "stop");
                } while (!completed);

                if (GroupAddress != null)
                {
                    client.DropMulticastGroup(IPAddress.Parse(GroupAddress));
                }

                Logger.Warn("Listening stop command received."); 
                Logger.Warn("Udp is stopping..."); 
            }
        }

        /// <summary>
        /// 显示Tag、Port、GroupAddress等详细信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{nameof(Port)}={Port}, {nameof(Tag)}={(Tag ?? "null")}, {nameof(GroupAddress)}={GroupAddress ?? "null"}";
        }

        /// <summary>
        /// 将数组转义到确切的UDP配置定义
        /// </summary>
        /// <param name="args"></param>
        /// <param name="port">端口号</param>
        /// <param name="group">组地址</param>
        /// <param name="tag">识别标签</param>
        private static void ParseCommandOptions(string[] args, out int port, out string group, out string tag)
        {
            int ip = args.Contains("-p") ? Array.IndexOf(args, "-p") + 1 : -1;
            int ig = args.Contains("-g") ? Array.IndexOf(args, "-g") + 1 : -1;
            int it = args.Contains("-t") ? Array.IndexOf(args, "-t") + 1 : -1;

            port = (ip > 0 && ip < args.Length) ? int.Parse(args[ip]) : 0;
            group = (ig > 0 && ig < args.Length) ? args[ig] : null;
            tag = (it > 0 && it < args.Length) ? args[it] : null;
        }

    }
}
