using LoongEgg.LoongLog;
using System;

namespace UdpSender.Net45
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Logger.Enable(Loggers.ConsoleLogger | Loggers.DebugLogger);
            var sender = new LoongEgg.UdpCore.UdpSender(2233, true);
            bool stop = false;
            do
            {
                Console.WriteLine("Enter a message or stop/s to exit");
                string input = Console.ReadLine();
                stop = input.ToLower() == "stop" | input.ToLower() == "s";
                sender.SendAsync($"{input}");
            } while (!stop);
        }
    }
}
