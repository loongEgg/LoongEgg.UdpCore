using LoongEgg.LoongLog;
using System;
using System.Diagnostics;
using System.Linq;

namespace LoongEgg.UdpCore.Check
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Enable(Loggers.ConsoleLogger);
            //JsonPackConfig_Check();
            DefaulConfig_Check();
        }

        private static void JsonPackConfig_Check()
        {
            try
            {
                JsonPackConfig pack = JsonPackConfig.DeserializeFromFile("AltPack.json");
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

        static void DefaulConfig_Check()
        {
            var receiver = UdpReceiver.DefaultConsole(false);
            receiver.MessageRecieved += Receiver_MessageRecieved;
            receiver.ReceiveAsync().Wait();
        }

        private static void Receiver_MessageRecieved(object sender, UdpReceivedEventArgs e)
        {
            Logger.Info($"buff: {String.Join(",", e.Buffer.Select(p => p.ToString()).ToArray())}");
        }
    }
}
