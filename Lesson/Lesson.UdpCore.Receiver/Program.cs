using LoongEgg.LoongLog;
using System;
using System.Linq;

namespace Lesson.UdpCore.Receiver
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Enable(Loggers.ConsoleLogger);
            var receiver = new UdpReceiver();
            receiver.MessageReceived += Receiver_MessageReceived;
            receiver.ReceiveAsync().Wait();
        }

        private static void Receiver_MessageReceived(object sender, UdpReceivedEventArgs args)
        {
            byte[] buff = args.Buffer;
            // 一行将byte[]转为字符串
            string rec = String.Join(",", buff.Select(b => b.ToString()).ToArray());
            Console.WriteLine(rec);
        }
    }
}
