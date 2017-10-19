using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using WebBrowser = System.Windows.Forms.WebBrowser;

namespace KoalaDictDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Rect _normal;
        public MainWindow()
        {
            InitializeComponent();
            var borderBrowser = new WebBrowserOverlay(BdBrowser);
            var dicWeb = borderBrowser.WebBrowser;
            dicWeb.Navigate("http://html5test.com/");

        }

        private void TitleBar_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void DicBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //Dic.Visibility = Visibility.Visible;
            Tra.Visibility = Visibility.Hidden;
        }

        private void TraBtn_OnClick(object sender, RoutedEventArgs e)
        {
            //Dic.Visibility = Visibility.Hidden;
            Tra.Visibility = Visibility.Visible;
        }

        private void MinBtn_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MaxBtn_OnClick(object sender, RoutedEventArgs e)
        {

            if (Height != 620)
            {

                BorderThickness = new Thickness(10, 10, 10, 10);
                Left = _normal.Left;
                Top = _normal.Top;
                Width = _normal.Width;
                Height = _normal.Height;
                MaxBtn.Visibility = Visibility.Visible;
                BacBtn.Visibility = Visibility.Collapsed;

            }

            else
            {
                BorderThickness = new Thickness(0, 0, 0, 0);
                MaxBtn.Visibility = Visibility.Collapsed;
                BacBtn.Visibility = Visibility.Visible;
                _normal = new Rect(Left, Top, Width, Height);
                Left = 0;
                Top = 0;
                Rect rc = SystemParameters.WorkArea;
                Width = rc.Width;
                Height = rc.Height;

            }
        }
    }
}
