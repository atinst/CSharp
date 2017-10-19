using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;

namespace MojiWeather
{

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        //private String Url = "https://api.seniverse.com/v3/weather/now.json?key=9rvdrjggtlcsac1m&location=beijing&language=zh-Hans&unit=c";
        //private const string Url = "http://api.seniverse.com";

        public MainWindow()
        {
            InitializeComponent();
            //var client = new RestClient(Url);
            //var request = new RestRequest("/v3/weather/now.json", Method.GET);
            //request.AddHeader("Acception", "application/json");
            //request.AddParameter("key", "9rvdrjggtlcsac1m");
            //request.AddParameter("location", "shenyang");
            //request.AddParameter("language", "zh-Hans");
            //request.AddParameter("unit", "c");
            //var response = client.Execute(request);
            const string contrnt = "{\"results\":[{\"location\":{\"id\":\"WX4FBXXFKE4F\",\"name\":\"北京\",\"country\":\"CN\",\"path\":\"北京,北京,中国\",\"timezone\":\"Asia/Shanghai\",\"timezone_offset\":\"+08:00\"},\"now\":{\"text\":\"多云\",\"code\":\"4\",\"temperature\":\"28\",\"feels_like\":\"26\",\"pressure\":\"1007\",\"humidity\":\"48\",\"visibility\":\"15.8\",\"wind_direction\":\"北\",\"wind_direction_degree\":\"340\",\"wind_speed\":\"14.76\",\"wind_scale\":\"3\",\"clouds\":\"\",\"dew_point\":\"\"},\"last_update\":\"2017-09-05T10:05:00+08:00\"}]}";
            var type = new { results = new List<Result>() };
            var result = JsonConvert.DeserializeAnonymousType(contrnt, type);
            //var result = JsonConvert.DeserializeAnonymousType(response.Content, type);
            Diqu.Content = result.results[0].Location.Name;
            Wendu.Content = result.results[0].Now.Temperature + "°";
            Tianqi.Content = result.results[0].Now.Text;

        }

        private void Move_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MiniButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
        }
    }
}
