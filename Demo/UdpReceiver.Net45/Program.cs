using LoongEgg.LoongLog;
using System;

namespace UdpReceiver.Net45
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Logger.Enable(Loggers.ConsoleLogger);
            var receiver = new LoongEgg.UdpCore.UdpReceiver(2233);
            receiver.ReaderAsync().Wait();

            Console.WriteLine("Good bye~");
        }
    }
}
