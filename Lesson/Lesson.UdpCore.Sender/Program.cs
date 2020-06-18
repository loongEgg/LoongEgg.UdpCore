using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoongEgg.LoongLog;
using LoongEgg.UdpCore;

namespace Lesson.UdpCore.Sender
{
    class Program
    {
        static void Main(string[] args)
        {
            var sender = UdpSender.CreatFromConfig();
            sender.Init();
            Logger.Enable(Loggers.ConsoleLogger);
            bool stop = false;
            do
            {
                string input = Console.ReadLine();
                sender.SendAsync(input);
                stop = input == "s";
            } while (!stop);
        }
    }
}
