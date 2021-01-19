using System;
using System.IO;
using System.Net;

namespace Spider
{
    public static class Helper
    {
        public static string GetHtmlByURL(this string url)
        {
            var html = string.Empty;
            try
            {
                var request = WebRequest.Create(url);
                var response = request.GetResponse();
                var stream = new StreamReader(response.GetResponseStream());
                html = Nancy.Helpers.HttpUtility.HtmlDecode(stream.ReadToEnd());
                response.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{url} is failed.");
                Console.WriteLine(ex);
            }

            return html;
        }
    }
}