using System;
using System.Net;
using System.Net.Http;

namespace Weather
{
    static class Helper
    {
        public static string GetNetContent(string url)
        {
            string content = null;

            using (HttpClientHandler handler = new())
            {

                if (handler.SupportsAutomaticDecompression) handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                                                             DecompressionMethods.Deflate;

                using (HttpClient client = new(handler))
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.102 Safari/537.36");
                    client.Timeout = TimeSpan.FromSeconds(6);
                    client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip");

                    try { content = client.GetStringAsync(url).Result; } catch { }
                }
            }

            return content;
        }
    }
}
