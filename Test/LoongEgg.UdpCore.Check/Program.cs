using LoongEgg.LoongLog;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace LoongEgg.UdpCore.Check
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Enable(Loggers.ConsoleLogger);
            //JsonPackConfig_Check();
            //UdpReceiverDefaulConfig_Check();
            UdpSenderCreatFromFile_Check();
        }

        private static void JsonPackConfig_Check()
        {
            try
            {
                UdpPack pack = UdpPack.DeserializeFromFile("AltPack.json");
                Console.WriteLine("Desializing...");
                Console.WriteLine(pack.ToString());

                Console.WriteLine("Serializing...");
                Console.WriteLine(pack.SerializeToJsonString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static bool isUdpWorking;
        private static void UdpReceiverDefaulConfig_Check()
        {
            if (isUdpWorking) return;
            isUdpWorking = true;

            var receiver = UdpReceiver.DefaultConsole(true);
            receiver.MessageRecieved += Receiver_MessageRecieved;
            receiver.ReceiveAsync().Wait();
        }

        private static void UdpSenderCreatFromFile_Check()
        {
            if (isUdpWorking) return;
            isUdpWorking = true;

            var sender = UdpSender.CreatFromConfig();
            sender.Init();
            bool stop = false;
            Console.WriteLine("Enter a message or stop/s to exit");
            do
            {
                string input = Console.ReadLine();
                stop = input.ToLower() == "stop" | input.ToLower() == "s";
                sender.SendAsync($"{input}");
            } while (!stop);
        }

        private static void Receiver_MessageRecieved(object sender, UdpReceivedEventArgs e)
        {
            Logger.Info($"buff: {String.Join(",", e.Buffer.Select(p => p.ToString()).ToArray())}");
        }
    }
}
