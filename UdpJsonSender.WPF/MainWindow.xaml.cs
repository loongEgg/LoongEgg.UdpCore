using System.Windows;

namespace UdpJsonSender.WPF
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = (Application.Current as App).IoC.Get<AltPackViewModel>();
        }
    }
}
