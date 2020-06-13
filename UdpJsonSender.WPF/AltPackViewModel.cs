using LoongEgg.MvvmCore;
using LoongEgg.UdpCore;
using System.ComponentModel;

/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/13 17:11:16
 | 主要用途：
 | 更改记录：
 |			 时间		版本		更改
 */
namespace UdpJsonSender.WPF
{

    public class AltPackViewModel : ViewModel
    {
        internal static AltPackViewModel DesignInstance { get; }
        = new AltPackViewModel(
            JsonPackConfig.DeserializeFromFile(
                @"E:\Published\LoongEgg.UdpCore\UdpJsonSender.WPF\AltPack.json")); // 根据你自己实际的文件路径写

        public string PackName
        {
            get { return _PackName; }
            set { SetProperty(ref _PackName, value); }
        }
        private string _PackName;

        public byte PackId
        {
            get { return _PackId; }
            set { SetProperty(ref _PackId, value); }
        }
        private byte _PackId;

        public AltViewModel Gps { get; set; }
        public AltViewModel Baro { get; set; }

        public AltPackViewModel(JsonPackConfig config)
        {
            Gps = new AltViewModel
            {
                Name = config.Items[0].Name,
                Unit = config.Items[0].Unit,
                Minimum = config.Items[0].Minimum,
                Maximum = config.Items[0].Maximum
            };

            Baro = new AltViewModel
            {
                Name = config.Items[1].Name,
                Unit = config.Items[1].Unit,
                Minimum = config.Items[1].Minimum,
                Maximum = config.Items[1].Maximum
            };

            PackName = config.PackName;
            PackId = config.PackID;

        }

        readonly UdpSender Sender;
        /// <summary>
        /// 会启动Udp发送器的实例，默认采用组播模式，只需设置端口号一致即可接收到
        /// </summary>
        /// <param name="config">json配置实例</param>
        /// <param name="port">端口号</param>
        public AltPackViewModel(JsonPackConfig config, int port) : this(config)
        {
            Gps.PropertyChanged += OnPropertyChanged;
            Sender = new UdpSender(port, true);
        }

        /// <summary>
        /// 当属性改变时，调用Udp发送功能
        /// </summary> 
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AltViewModel.Readout))
            {
                Sender?.SendAsync($"Gps={Gps.Readout}");
            }
        }
    }
}
