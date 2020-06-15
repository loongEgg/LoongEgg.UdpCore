using LoongEgg.LoongLog;
using System;
using System.Diagnostics;

namespace LoongEgg.UdpCore.Check
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.Enable(Loggers.ConsoleLogger);
            //JsonPackConfig_Check();
            DefaulConfig_Check();

            Debugger.Break();
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
            UdpReceiver.DefaultConsole(false);
        }
    }
}
