using LoongEgg.LoongLog;
using System;

namespace UdpSender.Net45
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Enable(Loggers.ConsoleLogger | Loggers.DebugLogger, false);
            var sender = new LoongEgg.UdpCore.UdpSender(2233, true); 
            bool stop = false;
            Console.WriteLine("Enter a message or stop/s to exit");
            do
            {
                string input = Console.ReadLine();
                stop = input.ToLower() == "stop" | input.ToLower() == "s";
                sender.SendAsync($"{input}");
            } while (!stop);
        }
    }
}
