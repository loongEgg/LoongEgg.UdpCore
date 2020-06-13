using System;
using System.Diagnostics;

namespace LoongEgg.UdpCore.Check
{
    class Program
    {
        static void Main(string[] args)
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

            Debugger.Break();
        }
    }
}
