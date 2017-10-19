using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace KoalaDictDemo
{
    /// <summary>
    /// Translate.xaml 的交互逻辑
    /// </summary>
    public partial class Translate : UserControl
    {
        public Translate()
        {
            InitializeComponent();           
        }

        private void TransBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var i = InBox.Text;
            const string appKey = "76f727cb09492e92";
            const string from = "ru";
            const string to = "zh-CHS";
            var salt = DateTime.Now.Millisecond.ToString();
            const string appSecret = "3Chbu5plRETDsjHfMZgbXIuiY2CCgp90";
            MD5 md5 = new MD5CryptoServiceProvider();
            var md5Str = appKey + i + salt + appSecret;
            var output = md5.ComputeHash(Encoding.UTF8.GetBytes(md5Str));
            var sign = BitConverter.ToString(output).Replace("-", "");
            var url =
                $"http://openapi.youdao.com/api?appKey={appKey}&q={System.Web.HttpUtility.UrlDecode(i, Encoding.GetEncoding("UTF-8"))}&from={from}&to={to}&sign={sign}&salt={salt}";
            var translationWebRequest = WebRequest.Create(url);
            var response = translationWebRequest.GetResponse();
            var stream = response.GetResponseStream();
            var encode = Encoding.GetEncoding("utf-8");
            if (stream == null) return;
            var reader = new StreamReader(stream, encode);
            var results = reader.ReadToEnd();
            var type = new {translation = new string[0]};
            var reType = JsonConvert.DeserializeAnonymousType(results, type);
            var temp = reType.translation[0];
            OutBox.Text = temp;

        }

        private void InBox_OnGotFocus(object sender, RoutedEventArgs e)
        {
            InBox.Text = "";
        }
    }
}
