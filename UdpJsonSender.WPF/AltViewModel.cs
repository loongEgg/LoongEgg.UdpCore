using LoongEgg.MvvmCore;

/* 
 | 个人微信：InnerGeeker
 | 联系邮箱：LoongEgg@163.com 
 | 创建时间：2020/6/13 17:00:07
 | 主要用途：
 | 更改记录：
 |			 时间		版本		更改
 */
namespace UdpJsonSender.WPF
{

    public class AltViewModel : ViewModel
    { 
        public string Name
        {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }
        private string _Name; 

        public string Unit
        {
            get { return _Unit; }
            set { SetProperty(ref _Unit, value); }
        }
        private string _Unit;
         
        public double Maximum
        {
            get { return _Maximum; }
            set { SetProperty(ref _Maximum, value); }
        }
        private double _Maximum;

        public double Minimum
        {
            get { return _Minimum; }
            set { SetProperty(ref _Minimum, value); }
        }
        private double _Minimum;

        public double Readout
        {
            get { return _Readout; }
            set { SetProperty(ref _Readout, value); }
        }
        private double _Readout;
    }
}
