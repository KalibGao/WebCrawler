using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using QiubaiCrawler.Models;

namespace QiubaiCrawler.Services
{
    public class QiubaiCrawlerService
    {
        private const string USER_AGENT = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.139 Safari/537.36";
        /// <summary>
        /// .*?<div\s*class="content">\s*(?<content>.*?)\s*<\/div>\s*?(<div\s*class="thumb">\s*<img\s*src="\/\/(?<imageurl>.*?)"[^>]*>\s*<\/div>)?\s*<\/div>
        /// </summary> 
        private const string Content_Pattern = @".*?<div\s*class=""content"">\s*(?<content>.*?)\s*<\/div>\s*?(<div\s*class=""thumb"">\s*<img\s*src=""\/\/(?<imageurl>.*?)""[^>]*>\s*<\/div>)?\s*<\/div>";
        /// <summary>
        /// 没有格式化的正则表达式
        /// .*?<div[\s]*class="article[^>]*>\s*<div\s*class="author[^>]*>\s*<a\s*href="\/users\/(?<userid>.*?)\/"[^>]*>\s*<img\s*src="\/\/(?<usericon>.*?)"\s*alt="(?<username>.*?)"[^>]*>\s*<\/a>\s*<a[^>]*>\s*<h2>.*<\/h2>\s*<\/a>\s*<div\s*class="articleGender\s*(?<gender>.*?)Icon">\s*(?<userage>.*?)\s*<\/div>
        /// </summary>      
        private const string User_Pattern = @".*?<div[\s]*class=""article[^>]*>\s*<div\s*class=""author[^>]*>\s*<a\s*href=""\/users\/(?<userid>.*?)\/""[^>]*>\s*<img\s*src=""\/\/(?<usericon>.*?)""\s*alt=""(?<username>.*?)""[^>]*>\s*<\/a>\s*<a[^>]*>\s*<h2>.*<\/h2>\s*<\/a>\s*<div\s*class=""articleGender\s*(?<gender>.*?)Icon"">\s*(?<userage>.*?)\s*<\/div>";
        private HttpClient HttpClient { get; set; }
        private HttpClientHandler HttpClientHandler { get; set; }
        private CookieContainer CustomCookieContainer { get; set; }

        public QiubaiCrawlerService()
        {

            CustomCookieContainer = new CookieContainer();
            HttpClientHandler = new HttpClientHandler()
            {
                CookieContainer = CustomCookieContainer,
                AllowAutoRedirect = false
            };
            HttpClient = new HttpClient(HttpClientHandler);
        }


        public async Task<HttpResponseMessage> HttpRequestAsync(string requestUri)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            httpRequestMessage.Version = HttpVersion.Version11;
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
            httpRequestMessage.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("image/apng", 0.8));
            httpRequestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            httpRequestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            httpRequestMessage.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("br"));
            httpRequestMessage.Headers.TryAddWithoutValidation("User-Agent", USER_AGENT);
            httpRequestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh-CN"));
            httpRequestMessage.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("zh", 0.9));

            return await HttpClient.SendAsync(httpRequestMessage);
        }

        public async Task<QiuBaiModel> ProcessPageAsync(string requestUri)
        {
            var responseMessage = await HttpRequestAsync(requestUri);
            try
            {
                responseMessage.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                return null;
            }

            string html = string.Empty;
            using (GZipStream zipStream = new GZipStream(await responseMessage.Content.ReadAsStreamAsync(), CompressionMode.Decompress))
            using (StreamReader reader = new StreamReader(zipStream, encoding: Encoding.UTF8))
            {
                html = await reader.ReadToEndAsync();
            }
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            var matches = Regex.Matches(html, Content_Pattern);
            var qiubaiModel = new QiuBaiModel();
            foreach (Match contentMatch in matches)
            {
                qiubaiModel.Content = contentMatch.Groups["content"]?.Value;
                qiubaiModel.ImageUrl = contentMatch.Groups["imageurl"]?.Value;

                if (!string.IsNullOrEmpty(qiubaiModel.Content))
                {
                    foreach (Match userMatch in Regex.Matches(html, User_Pattern))
                    {
                        qiubaiModel.UserId = userMatch.Groups["userid"]?.Value;
                        qiubaiModel.UserIcon = userMatch.Groups["usericon"]?.Value;
                        qiubaiModel.UserName = userMatch.Groups["username"]?.Value;
                        qiubaiModel.Gender = userMatch.Groups["gender"]?.Value;
                        qiubaiModel.Age = userMatch.Groups["userage"]?.Value;
                    }
                }
            }
            return qiubaiModel;
        }


    }
}