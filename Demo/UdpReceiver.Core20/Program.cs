using System;
using LoongEgg.LoongLog;

namespace UdpReceiver.Core20
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Logger.Enable(Loggers.ConsoleLogger | Loggers.DebugLogger);
            var receiver = new LoongEgg.UdpCore.UdpReceiver(2233);
            receiver.ReaderAsync().Wait();

            Console.WriteLine("Good bye~");
        }
    }
}
