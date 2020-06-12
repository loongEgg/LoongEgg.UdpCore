using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace LoongEgg.UdpCore.Check
{
    class Program
    {
        static void Main(string[] args)
        { 
            JsonPackConfig pack = JsonPackConfig.DeserializeFromFile("AltPack.json");
            Console.WriteLine("Desializing...");
            Console.WriteLine(pack.ToString());

            Console.WriteLine("Serializing...");
            Console.WriteLine(pack.SerializeToJsonString());
            Debugger.Break();
        }
    }
}
