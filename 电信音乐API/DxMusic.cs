using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json.Linq;
using YaService.Helper;
using YaService.Interface;
using YaService.Model;
using YaService.Properties;

namespace YaService.Api.Music
{
    public class DxMusic : IMusic
    {
        private static string GetSign(string time, string sid = "240236110", string imsi = "460028616837013")
        {
            var hmac = HMAC.Create("HmacSHA1");
            var data = Encoding.Default.GetBytes("xe3FwAT2ZiDxOhah_" + sid + "_" + time + "_" + imsi);
            var key = Encoding.Default.GetBytes("eNy61dEM7oogZUqONUI56OHoirJzuRFM5rmM58B25gQOxjQ4JGQRYe6e8DA3zMY8");
            var signedData = Bytes2HexUpperCaseString(SignData(key, data, hmac));
            return Md5Encode(signedData);
        }

        //public String getSign(Context paramContext, String paramString)
        //{
        //    int i = NetConfig.getIntConfig("sid", 0);
        //    paramContext = NetConfig.getIMSI(paramContext);
        //    return SecurityUtil.getMD5String(SecurityUtil.hash_hmac("HmacSHA1", "xe3FwAT2ZiDxOhah_" + i + "_" + paramString + "_" + paramContext, "eNy61dEM7oogZUqONUI56OHoirJzuRFM5rmM58B25gQOxjQ4JGQRYe6e8DA3zMY8"));
        //}

        private static byte[] SignData(byte[] key, byte[] data, HMAC alg)
        {
            alg.Key = key;
            var hash = alg.ComputeHash(data);
            return hash.Concat(data).ToArray();
        }

        private static string Bytes2HexUpperCaseString(byte[] paramArrayOfByte)
        {
            return paramArrayOfByte.Aggregate("", (current, b) => current + $"{Convert.ToInt32(b & 0xFF):X}".PadLeft(2, '0').ToUpper()).Substring(0, 40);
        }

        private static string Md5Encode(string text)
        {
            var result = Encoding.Default.GetBytes(text);
            var md5 = new MD5CryptoServiceProvider();
            var output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", "").ToLower().Trim();
        }

        public static List<SongResult> Search(string key, int page)
        {
            //var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            //var sing = GetSign(time);
            //var data =
            //    Resources.DxSearch.Replace("**TIME**", time)
            //        .Replace("**SIGN**", sing)
            //        .Replace("**PAGE**", page.ToString())
            //        .Replace("**KEY**", key);
            //var html = CommonHelper.PostData("http://iting.music.189.cn:9101/iting2/imusic/V2", data,1);
            //var html =
            //    CommonHelper.GetHtmlContent(
            //        "http://iting.music.189.cn:9101/iting2/imusic/V2?method=search&maxRows=30&networkType=wifi&serviceCode=800&type=1&pageNum="+page+"&timestamp=20160527215409&imei=C94C6CA0-8979-4D2B-B583-F81F3065CAEB&key=" +
            //        key +
            //        "&appChannelCode=10000000&responseType=Ring&sid=242040610&company=uiwap&appVerCode=V3.000.002&protocolCode=2.0&sign=6f53bd7a35d662437433155fe5dda59d&imsi=551803512369514&sessionId=403427288025D6FAF0E7C414092A5B78&format=json");

            //{"sid":240236110,"networkType":"wifi","format":"json","type":1,"protocolCode":"2.0","pageNum":**PAGE**,"timestamp":"**TIME**","sign":"**SIGN**","sessionId":"043CF1B4B66FA413AE67757C58734E10","appChannelCode":"20000027","responseType":"Ring","appVerCode":"V3.002.001.696ctch1","maxRows":50,"method":"search","imsi":"460028616837013","parentPath":"400","key":"**KEY**"}{"sid":244679252,"networkType":"wifi","format":"json","type":1,"protocolCode":"2.0","pageNum":1,"timestamp":"20160528123336","sign":"0f687aca150f5ee736953dc72d741b38","sessionId":"386205452FE8A30A7F8723C0E38B6482","appChannelCode":"20000028","responseType":"Ring","appVerCode":"V3.002.001.696ctch1","maxRows":20,"method":"search","imsi":"460071608869773","parentPath":"400","key":"许嵩"}
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var sing = GetSign(time);
            var data =
                Resources.DxSearch.Replace("**TIME**", time).Replace("**SIGN**", sing).Replace("**PAGE**", page.ToString()).Replace("**KEY**", key);
            var html = CommonHelper.PostData("http://iting.music.189.cn:9101/iting2/imusic/V2", data, 1,"",false,"",5000);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var json = JObject.Parse(html);
            if (json["totalRows"].ToString() == "0")
            {
                return null;
            }
            var datas = json["resList"];
            var list = new List<SongResult>();
            foreach (JToken j in datas)
            {
                var song = new SongResult
                {
                    SongId = j["resId"].ToString(),
                    SongName = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate(j["resName"].ToString())),
                    SubTitle = "",
                    Artist = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate((j["singer"]?.ToString() ?? "").Replace("+", ";"))),
                    ArtistSubTitle = "",
                    AlbumId = j["albumId"]?.ToString() ?? "0",
                    Album = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate(j["album"]?.ToString() ?? "")),
                    AlbumSubTitle = "",
                    AlbumArtist = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate((j["singer"]?.ToString() ?? "").Replace("+", ";"))),
                    LrcUrl = CommonHelper.GetSignUrl("dx", "320", j["resId"].ToString(), "lrc"),
                    KlokLrc = "",
                    TrackNum = "",
                    PicUrl = CommonHelper.GetSignUrl("dx", "320", j["resId"].ToString(), "jpg"),
                    Year = "",
                    Length = "",
                    CollectName = "",
                    BitRate = "",
                    Disc = "1",
                    MvId = "",
                    Type = "dx",
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
                var flag = j["flag"];
                if (flag["mvFlag"].ToString() == "1")
                {
                    song.MvUrl = CommonHelper.GetSignUrl("dx", "hd", song.SongId, "mp4");
                    song.VideoUrl = CommonHelper.GetSignUrl("dx", "ld", song.SongId, "mp4");
                }
                if (flag["listenFlag"].ToString() == "1")
                {
                    song.BitRate = "128K";
                    song.CopyUrl = song.ListenUrl = song.LqUrl = CommonHelper.GetSignUrl("dx", "128", song.SongId, "mp3");
                }
                if (flag["hqFlag"].ToString() == "1")
                {
                    song.BitRate = "192K";
                    song.CopyUrl = song.ListenUrl = song.HqUrl = CommonHelper.GetSignUrl("dx", "192", song.SongId, "mp3");
                }
                if (flag["sqFlag"].ToString() == "1")
                {
                    song.BitRate = "320K";
                    song.CopyUrl = song.ListenUrl = song.SqUrl = CommonHelper.GetSignUrl("dx", "320", song.SongId, "mp3");
                }
                if (flag["hifiFlag"].ToString() == "1")
                {
                    song.BitRate = "无损";
                    song.FlacUrl = CommonHelper.GetSignUrl("dx", "1000", song.SongId, "flac");
                }
                list.Add(song);
            }
            return list;
        }

        private static SongResult GetDxSong(string id,out string lrc)
        {
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var sing = GetSign(time);
            var data = Resources.DxSingle.Replace("**TIME**", time)
                    .Replace("**SIGN**", sing)
                    .Replace("**SONGID**", id);
            var html = CommonHelper.PostData("http://iting.music.189.cn:9101/iting2/imusic/V2", data, 1);
            if (string.IsNullOrEmpty(html))
            {
                lrc = "";
                return null;
            }
            var j = JObject.Parse(html);
            var song = new SongResult
            {
                SongId = j["resId"].ToString(),
                SongName = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate(j["resName"].ToString())),
                SubTitle = "",
                Artist = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate((j["singger"]?.ToString() ?? "").Replace("+", ";"))),
                ArtistSubTitle = "",
                AlbumId = j["albumId"]?.ToString() ?? "0",
                Album = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate(j["album"]?.ToString() ?? "")),
                AlbumSubTitle = "",
                AlbumArtist = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate((j["singger"]?.ToString() ?? "").Replace("+", ";"))),
                LrcUrl = CommonHelper.GetSignUrl("dx", "320", j["resId"].ToString(), "lrc"),
                KlokLrc = "",
                TrackNum = "",
                PicUrl = j["albumPic"].ToString(),
                Year = "",
                Length = CommonHelper.NumToTime(j["fullDownloadPlayTime"].ToString()),
                CollectName = "",
                BitRate = "",
                Disc = "1",
                MvId = "",
                Type = "dx",
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
            if (string.IsNullOrEmpty(song.PicUrl))
            {
                try
                {
                    song.PicUrl = j["pics"].First["picUrl"].ToString();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            var flag = j["flag"];
            if (flag["mvFlag"].ToString() == "1")
            {
                song.MvUrl = CommonHelper.GetSignUrl("dx", "hd", song.SongId, "mp4");
                song.VideoUrl = CommonHelper.GetSignUrl("dx", "ld", song.SongId, "mp4");
            }
            var baseUrl = Regex.Match(j["fullDownloadUrl"].ToString(), @"http[^""]+?00(?=\d{4}.mp3)").Value;
            
            if (flag["listenFlag"].ToString() == "1")
            {
                song.BitRate = "128K";
                if (string.IsNullOrEmpty(baseUrl))
                {
                    song.ListenUrl = song.LqUrl = j["fullDownloadUrl"].ToString();
                }
                else
                {
                    song.ListenUrl = song.LqUrl = baseUrl + "0800.mp3";
                }
                
            }
            if (flag["hqFlag"].ToString() == "1")
            {
                song.BitRate = "192K";
                if (string.IsNullOrEmpty(baseUrl))
                {
                    song.ListenUrl = song.LqUrl = j["fullDownloadUrl"].ToString();
                }
                else
                {
                    song.ListenUrl = song.HqUrl = baseUrl + "1600.mp3";
                }
                
            }
            if (flag["sqFlag"].ToString() == "1")
            {
                if (string.IsNullOrEmpty(baseUrl))
                {
                    song.ListenUrl = song.LqUrl = j["fullDownloadUrl"].ToString();
                }
                else
                {
                    song.ListenUrl = song.SqUrl = baseUrl + "3000.mp3";
                }
                song.BitRate = "320K";
                
            }
            if (flag["hifiFlag"].ToString() == "1")
            {
                song.BitRate = "无损";
                if (baseUrl.Contains("res/V"))
                {
                    song.ListenUrl = song.FlacUrl = baseUrl.Replace("mp3/", "flac/") + "0000.flac";
                }
                else
                {
                    song.ListenUrl = song.FlacUrl = baseUrl.Replace("res/", "res/V/").Replace("mp3/", "flac/") + "0000.flac";
                }
            }
            song.CopyUrl = CommonHelper.GetSignUrl("dx", "320", song.SongId, "mp3");
            lrc = j["lyric"]?.ToString() ?? "";
            return song;
        }

        private static string GetUrl(string id, string quality, string format)
        {
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var sing = GetSign(time);
            if (format == "mp4" || format == "flv")
            {
                var data = Resources.DxMv.Replace("**TIME**", time)
                    .Replace("**SIGN**", sing)
                    .Replace("**QUALITY**", quality=="ld"?"1":"2")
                    .Replace("**MVID**", id);
                var html = CommonHelper.PostData("http://iting.music.189.cn:9101/iting2/imusic/V2", data, 1);
                if (string.IsNullOrEmpty(html))
                {
                    return null;
                }
                return Regex.Match(html, @"(?<=mvUrl"":"")[^""]+").Value;
            }
            string lrc;
            var song = GetDxSong(id,out lrc);
            if (format == "lrc")
            {
                if (string.IsNullOrEmpty(lrc))
                {
                    return "";
                }
                lrc = "[ti:" + song.SongName + "]\n[ar: " + song.Artist + "]\n[by: 雅音FM]\n" + lrc;
                return lrc;
            }
            if (format == "jpg")
            {
                if (string.IsNullOrEmpty(song.PicUrl))
                {
                    return "http://yyfm.oss-cn-qingdao.aliyuncs.com/img/mspy.jpg";
                }
                return song.PicUrl;
            }
            if (format == "flac")
            {
                if (!string.IsNullOrEmpty(song.FlacUrl))
                {
                    return song.FlacUrl;
                }

                if (song.SqUrl.Contains("res/V"))
                {
                    song.ListenUrl = song.FlacUrl = Regex.Match(song.SqUrl, @"http[^""]+?00(?=\d{4}.mp3)").Value.Replace("mp3/", "flac/") + "0000.flac";
                }
                else
                {
                    song.ListenUrl = song.FlacUrl = Regex.Match(song.SqUrl, @"http[^""]+?00(?=\d{4}.mp3)").Value.Replace("res/", "res/V/").Replace("mp3/", "flac/") + "0000.flac";
                }
                return song.FlacUrl;
            }
            var link = "";
            if (format == "mp3")
            {
                switch (quality)
                {
                    case "320":
                        link = song.SqUrl;
                        break;
                    case "192":
                        link = song.HqUrl;
                        break;
                    default:
                        link = song.LqUrl;
                        break;
                }
            }
            if (string.IsNullOrEmpty(link))
            {
                if (quality == "320")
                {
                    link = song.HqUrl;
                }
                if (string.IsNullOrEmpty(link))
                {
                    link = song.LqUrl;
                }
            }
            return link;
        }

        private static List<SongResult> SearchAlbum(string id)
        {
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var sing = GetSign(time);
            var data =
                Resources.DxAlbum.Replace("**TIME**", time)
                    .Replace("**SIGN**", sing)
                    .Replace("**ALBUMID**", id);
            var html = CommonHelper.PostData("http://iting.music.189.cn:9101/iting2/imusic/V2", data, 1);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var json = JObject.Parse(html);
            if (json["resid"].ToString() == "0")
            {
                return null;
            }
            var datas = json["songlist"];
            var year = json["public_time"].ToString().Substring(0, 10);
            var ar = json["singer_name"].ToString();
            var al = json["title"].ToString();
            var list = GetListByJson(datas);
            var index = 0;
            foreach (var result in list)
            {
                index++;
                result.Year = year;
                result.AlbumArtist = ar;
                result.Album = al;
                result.AlbumId = id;
                result.TrackNum = index.ToString();
            }
            return list;
        }

        private static List<SongResult> SearchCollect(string id)
        {
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var sing = GetSign(time);
            var data =
                Resources.DxCollet.Replace("**TIME**", time)
                    .Replace("**SIGN**", sing)
                    .Replace("**ID**", id);
            var html = CommonHelper.PostData("http://iting.music.189.cn:9101/iting2/imusic/V2", data, 1);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }
            var json = JObject.Parse(html);
            if (json["data"]["count"].ToString() == "0")
            {
                return null;
            }
            var datas = json["data"]["songlist"];
            var collectName = json["data"]["songlistname"].ToString();

            List<SongResult> list = GetListByJson(datas);
            foreach (var songResult in list)
            {
                songResult.CollectName = collectName;
            }
            return list;
        }

        private static List<SongResult> GetListByJson(JToken datas)
        {
            var list = new List<SongResult>();
            foreach (JToken j in datas)
            {
                var song = new SongResult
                {
                    SongId = j["song_id"].ToString(),
                    SongName = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate(j["song_name"].ToString())),
                    SubTitle = "",
                    Artist = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate((j["singer_name"]?.ToString() ?? "").Replace("+", ";"))),
                    ArtistSubTitle = "",
                    AlbumId = "0",
                    Album = "",
                    AlbumSubTitle = "",
                    AlbumArtist = HttpUtility.HtmlDecode(CommonHelper.CheckIsInValidate((j["singer_name"]?.ToString() ?? "").Replace("+", ";"))),
                    LrcUrl = CommonHelper.GetSignUrl("dx", "320", j["song_id"].ToString(), "lrc"),
                    KlokLrc = "",
                    TrackNum = "",
                    PicUrl = CommonHelper.GetSignUrl("dx", "320", j["song_id"].ToString(), "jpg"),
                    Year = "",
                    Length = "",
                    CollectName = "",
                    BitRate = "",
                    Disc = "1",
                    MvId = "",
                    Type = "dx",
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
                var urls = j["qqInfo"]["qq_url_list"];
                var max = 0;
                foreach (JToken jToken in urls)
                {
                    switch (jToken["type_des"].ToString())
                    {
                        case "标准音质":
                            if (Convert.ToInt32(jToken["bitrate"]) > max)
                            {
                                max = Convert.ToInt32(jToken["bitrate"]);
                            }
                            song.ListenUrl = song.LqUrl = jToken["url"].ToString();
                            break;
                        case "高品音质":
                            if (Convert.ToInt32(jToken["bitrate"]) > max)
                            {
                                max = Convert.ToInt32(jToken["bitrate"]);
                            }
                            song.ListenUrl = song.HqUrl = jToken["url"].ToString();
                            break;
                        case "超清音质":
                            if (Convert.ToInt32(jToken["bitrate"]) > max)
                            {
                                max = Convert.ToInt32(jToken["bitrate"]);
                            }
                            song.ListenUrl = song.SqUrl = jToken["url"].ToString();
                            break;
                        case "无损HIFI":
                            if (Convert.ToInt32(jToken["bitrate"]) > max)
                            {
                                max = Convert.ToInt32(jToken["bitrate"]);
                            }
                            song.FlacUrl = jToken["url"].ToString();
                            break;
                    }
                }
                song.CopyUrl = CommonHelper.GetSignUrl("dx", "320", j["song_id"].ToString(), "mp3");
                if (max > 320)
                {
                    song.BitRate = "无损";
                }
                else
                {
                    song.BitRate = max + "K";
                }
                var mvs = j["mvPlayUrls"];
                var count = 0;
                foreach (JToken mv in mvs)
                {
                    if (count >= 2)
                    {
                        break;
                    }
                    if (count == 0)
                    {
                        song.MvUrl = song.VideoUrl = mv["mv_url"].ToString();
                    }
                    else
                    {
                        song.VideoUrl = mv["mv_url"].ToString();
                    }
                }
                list.Add(song);
            }
            return list;
        }


        public List<SongResult> SongSearch(string key, int page)
        {
            return Search(key,page);
        }

        public List<SongResult> AlbumSearch(string id)
        {
            return SearchAlbum(id);
        }

        public List<SongResult> ArtistSearch(string id)
        {
            return null;
        }

        public List<SongResult> CollectSearch(string id)
        {
            return SearchCollect(id);
        }

        public List<SongResult> GetSingleSong(string id, bool isDetials = false)
        {
            string lrc;
            var song = GetDxSong(id, out lrc);
            return new List<SongResult>() {song};
        }

        public string GetSongUrl(string id, string quality, string format)
        {
            return GetUrl(id, quality, format);
        }
    }
}