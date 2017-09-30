using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfApplication2
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UserInfo user = new UserInfo() { ImgPath = "/Image/0.png",UserName="张三" };
            List<UserInfo> list = new List<UserInfo>();
            list.Add(user);
            CustomList.ItemsSource = null;
            CustomList.ItemsSource = list;
        }

        private void MinBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
                System.Windows.Application.Current.Shutdown();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
