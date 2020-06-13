using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/*
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com
 | 创建时间：2020/6/12 20:41:39
 | 主要用途：从json文件读取数据包协议
 | 更改记录：
 |			 时间		版本		更改
 */

namespace LoongEgg.UdpCore
{
    /// <summary>
    /// 数据包，可以直接从json文件反序列化
    /// </summary>
    public class JsonPackConfig
    {
        /// <summary>
        /// 当前包的名字
        /// </summary> 
        public string PackName { get; set; } = "[Undefined]";

        /// <summary>
        /// 当前包的ID号, 注意为单字节无符号数
        /// </summary>
        public byte PackID { get; set; }

        /// <summary>
        /// 包的字节总长度, 注意最大长度为255
        /// </summary>
        public byte Length
        {
            get
            {
                int count = 0;
                if (Items.Any())
                {
                    Items.ForEach(i => count += i.Length);
                    return (byte)count;
                }

                return 0;
            }
        }
        
        /// <summary>
        /// 数据对象定义的集合
        /// </summary>
        public List<ItemConfig> Items { get; set; }

        /// <summary>
        /// 从指定的文件反序列化数据包的定义
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>反序列化后的数据包</returns>
        public static JsonPackConfig DeserializeFromFile(string path)
        {
            using (StreamReader reader = File.OpenText(path))
            {
                JsonSerializer serializer = JsonSerializer.Create();
                var pack = serializer.Deserialize(reader, typeof(JsonPackConfig)) as JsonPackConfig;
                return pack;
            } 
        }

        /// <summary>
        /// 将协议转换为json字符串形式
        /// </summary>
        /// <returns></returns>
        public string SerializeToJsonString() => JsonConvert.SerializeObject(this, Formatting.Indented);

        /// <summary>
        /// 将所有的数据协议项转换为一个字符串
        /// </summary> 
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"PackName={PackName}, PackID={PackID}:");
            if (Items.Any())
            {
                Items.ForEach(
                    i =>
                    {
                        builder.AppendLine(i.ToString());
                    });
                return builder.ToString();
            }
            return "JsonPackDefinition is undefined";
        }
    }
}