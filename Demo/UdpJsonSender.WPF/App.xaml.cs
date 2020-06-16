using LoongEgg.DependencyInjection;
using LoongEgg.LoongLog;
using LoongEgg.UdpCore;
using System;
using System.Windows;

namespace UdpJsonSender.WPF
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            IoC = new Container();

            Logger.EnableDebugLogger();
            var config = UdpPack.DeserializeFromFile(AppDomain.CurrentDomain.BaseDirectory + "AltPack.json");
            IoC.AddOrUpdate(new AltPackViewModel( config, 2233)); // 端口号与接收器一致
        }

        /// <summary>
        /// 依赖注入容器
        /// </summary>
        public Container IoC { get; set; }
    }
}
