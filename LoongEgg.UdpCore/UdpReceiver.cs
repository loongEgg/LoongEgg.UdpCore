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
 | 主要用途：Udp接收器，并根据指定的json格式的数据包协议解析数据
 | 更改记录：
 | 时间         版本		更改
 | 2020-06-15  2.1.1    Udp接收器增加默认控制台DefaultConsole()，并且可以读取默认配置或者命令行交互配置
 | 2020-06-15  2.1.1    Udp接收器增加 UdpReceivedEvent 事件 和 UdpReceivedEventArgs 参数，来处理新接收到的消息
 */
namespace LoongEgg.UdpCore
{
   

    /// <summary>
    /// Udp接收器
    /// </summary>
    public class UdpReceiver
    {
        /*------------------------------------ Events -------------------------------------*/
        /// <summary>
        /// 接收到新消息事件
        /// </summary>
        public event UdpReceivedEvent MessageRecieved;

        /*------------------------------------ Fields -------------------------------------*/
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

        /*---------------------------------- Properties -----------------------------------*/
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

        /*----------------------------------- Constructor ---------------------------------*/
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public UdpReceiver() { }

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

        /*--------------------------------- Public Methods --------------------------------*/
        /// <summary>
        /// 默认控制台程序实现
        /// </summary>
        /// <param name="useDefaultConfig">default=true, 使用默认的配置文件</param>
        public static UdpReceiver DefaultConsole(bool useDefaultConfig = true)
        {
            string hostName = Dns.GetHostName();
            IPAddress[] IPs = Dns.GetHostAddresses(hostName);
            Logger.Info($"HostName: {hostName}, Local IP(s):");
            if (IPs.Any())
            {
                IPs.ToList().ForEach(ip =>
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                        Logger.Info($"    {ip.ToString()}");
                });
            }
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
                        Logger.Info("Udp initial as: " + receiver.ToString() + "?");
                        Logger.Info($"Enter Y/y to confirm, and save as [{DefaultConfigFile}]. OR any other keys to reinput.");
                        unconfig = !(Console.ReadLine().ToLower() == "y");
                    }
                    else
                    {
                        Logger.Warn("option missed: -p [port number] ");
                    }
                } while (unconfig);
                string json = JsonConvert.SerializeObject(receiver, Formatting.Indented);
                using (StreamWriter writer = File.CreateText(DefaultConfigFile))
                {
                    writer.Write(json);
                    writer.Flush();
                    writer.Close();
                }
            }
            return receiver;
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

                Logger.Debug("Start Listening...Sending in [stop] to stop listening");
                bool completed;
                do
                {
                    UdpReceiveResult result = await client.ReceiveAsync();
                    byte[] datagram = result.Buffer;
                    MessageRecieved?.Invoke(this, new UdpReceivedEventArgs(datagram));
                    string received = Encoding.UTF8.GetString(datagram);
                    Logger.Info($"Received (from {result.RemoteEndPoint.Address}) < {received}");
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
            return $"{nameof(Port)}={Port}, {nameof(GroupAddress)}={GroupAddress ?? "null"}, {nameof(Tag)}={(Tag ?? "null")}";
        }

        /*--------------------------------- Private Methods -------------------------------*/
        /// <summary>
        /// 将数组转义到确切的UDP配置定义
        /// </summary>
        /// <param name="args">命令参数</param>
        /// <param name="port">端口号</param>
        /// <param name="group">组地址</param>
        /// <param name="tag">识别标签</param>
        private static void ParseCommandOptions(string[] args, out int port, out string group, out string tag)
        {
            UdpHelper.TryParseCommandParam(args, "-p", out port);
            UdpHelper.TryParseCommandParam(args, "-g", out group);
            UdpHelper.TryParseCommandParam(args, "-t", out tag);
        }

        #region V3.0废除
        /// <summary>
        /// 接收器工作
        /// </summary> 
        /// <returns></returns>
        [Obsolete("V3.0, 废弃，使用ReceiveAsync()")]
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
        #endregion

    }
}
