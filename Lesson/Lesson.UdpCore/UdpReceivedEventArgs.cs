using System;

namespace Lesson.UdpCore
{
    public class UdpReceivedEventArgs : EventArgs
    {
        public byte[] Buffer { get; set; }

        public UdpReceivedEventArgs(byte[] buffer)
        {
            Buffer = buffer;
        }
    }

    public delegate void UdpReceivedEvent(object sender, UdpReceivedEventArgs args);
}
