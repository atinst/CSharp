using System.Windows;
using System.Windows.Input;


namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            CamWeb.Navigate("https://baidu.com");
        }

        private void UrlIn_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var word = UrlIn.Text;
            var url = $"http://www.igimu.com/words/index.php?q={word}";
            CamWeb.Navigate(url);
        }

        private void UrlIn_GotFocus(object sender, RoutedEventArgs e)
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            UrlIn.Text = null;
        }
    }
}
