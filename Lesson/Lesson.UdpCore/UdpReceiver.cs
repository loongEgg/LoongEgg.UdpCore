using LoongEgg.LoongLog;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/16 20:40:50
 | 主要用途：
 | 更改记录：
 | 时间	       版本        更改
 */
namespace Lesson.UdpCore
{

    public class UdpReceiver
    {
        /*------------------------------------ Events -------------------------------------*/
        public event UdpReceivedEvent MessageReceived;

        /*------------------------------------ Fields -------------------------------------*/

        /*---------------------------------- Properties -----------------------------------*/
        public int Port { get; set; }
        public string GroupAddress { get; set; }

        /*----------------------------------- Constructor ---------------------------------*/
        public UdpReceiver()
        {
            Port = 5566;
        }

        /*--------------------------------- Public Methods --------------------------------*/
        public async Task ReceiveAsync()
        {
            using (var client = new UdpClient(Port))
            {
                bool stop = false ;
                do
                {
                    UdpReceiveResult result = await client.ReceiveAsync();
                    byte[] buff = result.Buffer;
                    MessageReceived?.Invoke(this, new UdpReceivedEventArgs(buff));
                    string rec = Encoding.UTF8.GetString(buff);
                    Logger.Info(rec);
                    stop = rec == "s";
                } while (!stop);
            }
        }

        /*--------------------------------- Private Methods -------------------------------*/
    }
}
