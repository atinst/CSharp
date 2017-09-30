using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json.Linq;
using YaService.Helper;
using YaService.Interface;
using YaService.Model;

namespace YaService.Api.Music
{
    public class MgMusic : IMusic
    {
        public static List<SongResult> Search(string key, int page)
        {
            var url = "http://a.vip.migu.cn/rdp2/v5.5/search.do?pageno="+page+"&title="+key+"&type=1&ua=Iphone_Sst&version=4.24413";
            var html = CommonHelper.GetHtmlContent(url);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var json = JObject.Parse(html);
            if (json["totalcount"].ToString() == "0")
            {
                return null;
            }
            var datas = json["songs"];
            return GetListByJson(datas);
        }

        private static List<SongResult> GetListByJson(JToken datas)
        {
            var list = new List<SongResult>();
            foreach (JToken j in datas)
            {
                if (j["control"].ToString() == "00000")
                {
                    continue;
                }
                var song = new SongResult
                {
                    SongId = j["contentid"].ToString(),
                    SongName = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate(j["title"].ToString())),
                    SubTitle = "",
                    Artist = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate((j["singer"]?.ToString() ?? "").Replace("+", ";"))),
                    ArtistSubTitle = "",
                    AlbumId = j["albumid"]?.ToString() ?? "0",
                    Album = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate(j["album"]?.ToString() ?? "")),
                    AlbumSubTitle = "",
                    AlbumArtist = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate((j["singer"]?.ToString() ?? "").Replace("+", ";"))),
                    LrcUrl = CommonHelper.GetSignUrl("mg", "320", j["contentid"].ToString(), "lrc"),
                    KlokLrc = "",
                    TrackNum = "",
                    PicUrl = j["albumImg"]?.ToString() ?? "http://yyfm.oss-cn-qingdao.aliyuncs.com/img/mspy.jpg",
                    Year = "",
                    Length = "",
                    CollectName = "",
                    BitRate = "",
                    Disc = "1",
                    MvId = j["mvid"]?.ToString() ?? "",
                    Type = "mg",
                    AacUrl = "",
                    Company = "",
                    CopyUrl = "",
                    FlacUrl = "",
                    HqUrl = "",
                    Language = "",
                    LqUrl = "",
                    MvUrl = "",
                    Size = "",
                    SqUrl = "",
                    VideoUrl = "",
                    ListenUrl = ""
                };
                if (song.AlbumId == "-1")
                {
                    song.AlbumId = "";
                }
                var flag = j["toneControl"].ToString();
                if (!string.IsNullOrEmpty(song.MvId))
                {
                    song.MvUrl = CommonHelper.GetSignUrl("mg", "hd", song.MvId, "mp4");
                    song.VideoUrl = CommonHelper.GetSignUrl("mg", "ld", song.MvId, "mp4");
                }
                if (flag[0].ToString() == "1")
                {
                    song.BitRate = "128K";
                    song.CopyUrl = song.ListenUrl = song.LqUrl = CommonHelper.GetSignUrl("mg", "128", song.SongId, "mp3");
                }
                if (flag[1].ToString() == "1")
                {
                    song.BitRate = "320K";
                    song.CopyUrl = song.ListenUrl = song.SqUrl= song.HqUrl = CommonHelper.GetSignUrl("mg", "320", song.SongId, "mp3");
                }
                if (flag[2].ToString() == "1")
                {
                    song.BitRate = "无损";
                    song.FlacUrl = CommonHelper.GetSignUrl("mg", "1000", song.SongId, "flac");
                }
                if (string.IsNullOrEmpty(song.BitRate))
                {
                    continue;
                }
                list.Add(song);
            }
            return list;
        }

        private static List<SongResult> SearchAlbum(string id)
        {
            var url = "http://a.vip.migu.cn/rdp2/v5.5/albuminfo.do?albumid=" + id + "&ua=Iphone_Sst&version=4.243&groupcode=1418/93061&pageno=1&ua=Iphone_Sst&version=4.243";
            var html = CommonHelper.GetHtmlContent(url);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var json = JObject.Parse(html);
            if (json["totalcount"].ToString() == "0")
            {
                return null;
            }
            var datas = json["songs"];
            var date = json["publishTime"].ToString();
            var ar = json["singerName"].ToString();
            var list = GetListByJson(datas);
            for (int i = 0; i < list.Count; i++)
            {
                list[i].TrackNum = (i + 1).ToString();
                list[i].Year = date;
                list[i].AlbumArtist = ar;
            }
            return list;
        }

        private static List<SongResult> SearchArtist(string id)
        {
            var url = "http://a.vip.migu.cn/rdp2/v5.5/singer_songs.do?ua=Iphone_Sst&version=4.243&singerid=" + id + "&pageno=1";
            var html = CommonHelper.GetHtmlContent(url);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var json = JObject.Parse(html);
            if (json["totalcount"].ToString() == "0")
            {
                return null;
            }
            var datas = json["songs"];
            return GetListByJson(datas);
        }

        private static List<SongResult> SearchCollect(string id)
        {
            var url = "http://music.migu.cn/music-mobile/playListContent/playList.do?groupcode=10000&id=" + id + "&pageno=&ua=Iphone_Sst&version=4.24413";
            var html = CommonHelper.GetHtmlContent(url);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var json = JObject.Parse(html);
            if (json["totalcount"].ToString() == "0")
            {
                return null;
            }
            var datas = json["songs"];
            var list = GetListByJson(datas);
            var ar = json["shareTitle"].ToString();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].TrackNum = (i + 1).ToString();
                list[i].CollectName = ar;
            }
            return list;
        }

        public static SongResult SearchSingle(string id)
        {
            var html =
                    CommonHelper.GetHtmlContent("http://music.migu.cn/music-mobile/songContent/getByContentId.do?contentid=" + id + "&ua=Iphone_Sst&version=4.24413");
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var json = JObject.Parse(html);
            var key = json["singerName"] + "-" + json["songName"];
            var list = Search(key, 1);
            return list?[0];
        }

        private static string GetUrl(string id, string quality, string format)
        {
            if (format == "lrc" || format == "jpg" || format == "mp3")
            {
                var html =
                    CommonHelper.GetHtmlContent("http://music.migu.cn/music-mobile/songContent/getByContentId.do?contentid=" + id + "&ua=Iphone_Sst&version=4.24413");
                if (string.IsNullOrEmpty(html))
                {
                    return "";
                }
                var json = JObject.Parse(html);
                if (format=="lrc")
                {
                    var lrcText = "[By:雅音FM] " + json["dynamicLyric"];
                    return lrcText;
                }
                if (format == "jpg")
                {
                    var value = json["albumPicm"].ToString();
                    return string.IsNullOrEmpty(value) ? "http://yyfm.oss-cn-qingdao.aliyuncs.com/img/mspy.jpg" : value;
                }
                return json["urlResourcePath"].ToString().Replace("toneFlag=1", quality == "320" ? "toneFlag=3" : "toneFlag=2");
            }
            if (format == "flac")
            {
                var html = GetData("http://218.200.160.29/rdp2/v5.5/download_biz.do?bizType=3&contentid="+id+"&groupcode=1418%2F93061&ua=Android_sst&version=4.2300", id);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }
                var paramsStr = Regex.Match(html, @"(?<=params"":"")[^""]+").Value;
                var data = "contentid="+id+"&groupcode=1418%2F93061&params="+paramsStr+"&payType=01";
                html = PostData("http://218.200.160.29/rdp2/v5.5/download_url.do?ua=Android_sst&version=4.2300", data, id);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }
                var json = JObject.Parse(html);
                return json["url"].ToString();
            }
            if (format == "mp4")
            {
                var url =
                    "http://a.vip.migu.cn/rdp2/v5.5/mv/playinfo.do?formatid="+(quality == "ld"? "050014" : "050013") +"&mvid="+id+"&ua=Iphone_Sst&version=4.24413";
                var html = GetData(url, id);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }
                var json = JObject.Parse(html);
                return json["playUrl"].ToString();
            }
            return null;
        }

        private static string PostData(string url, string data,string id)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers.Add("uid", "1d93a24f-771a-4566-9d3b-0abcd112eef8");
                request.Headers.Add("randomsessionkey", "c353a2cb40104a35837227e6a8506403");
                request.Headers.Add("mode", "android");
                request.Headers.Add("channel", "014002C");
                var time = DateTime.Now.ToString("yyyyMMddHHmmss");
                request.Headers.Add("TimeStep", time);
                request.Headers.Add("randkey", CommonHelper.Md5(id+time+ "a974ac0a-7617-4338-bab6-fc65012db253"));
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
                var postdatabyte = Encoding.UTF8.GetBytes(data);
                request.ContentLength = postdatabyte.Length;
                request.KeepAlive = true;
                //提交请求
                var stream = request.GetRequestStream();
                stream.Write(postdatabyte, 0, postdatabyte.Length);
                stream.Close();
                //接收响应
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK) return null;
                // ReSharper disable once AssignNullToNotNullAttribute
                var responseReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                var result = responseReader.ReadToEnd();
                return result;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return null;
            }
        }

        private static string GetData(string url, string id)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.Headers.Add("uid", "1d93a24f-771a-4566-9d3b-0abcd112eef8");
                request.Headers.Add("randomsessionkey", "c353a2cb40104a35837227e6a8506403");
                request.Headers.Add("mode", "android");
                request.Headers.Add("channel", "014002C");
                var time = DateTime.Now.ToString("yyyyMMddHHmmss");
                request.Headers.Add("TimeStep", time);
                request.Headers.Add("randkey", CommonHelper.Md5(id + time + "a974ac0a-7617-4338-bab6-fc65012db253"));
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)";
                request.KeepAlive = true;
                var response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode != HttpStatusCode.OK) return null;
                // ReSharper disable once AssignNullToNotNullAttribute
                var responseReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                var result = responseReader.ReadToEnd();
                return result;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                return null;
            }
        }

        public List<SongResult> SongSearch(string key, int page)
        {
            return Search(key, page);
        }

        public List<SongResult> AlbumSearch(string id)
        {
            return SearchAlbum(id);
        }

        public List<SongResult> ArtistSearch(string id)
        {
            return SearchArtist(id);
        }

        public List<SongResult> CollectSearch(string id)
        {
            return SearchCollect(id);
        }

        public List<SongResult> GetSingleSong(string id, bool isDetials = false)
        {
            return new List<SongResult> { SearchSingle(id) };
        }

        public string GetSongUrl(string id, string quality, string format)
        {
            return GetUrl(id, quality, format);
        }
    }
}