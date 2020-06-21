using Lesson.UdpCore;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Lesson.JsonDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            using (StreamReader reader = File.OpenText("demo.json"))
            {
                JsonSerializer serializer = JsonSerializer.Create();
                var pack = serializer.Deserialize(reader, typeof(UdpPack)) as UdpPack;
                Console.WriteLine(JsonConvert.SerializeObject(pack, Formatting.Indented)); 
            }


        }
    }
}
