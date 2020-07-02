using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_DashBoard.Tools
{
    public class DataScraper
    {
        /// <summary>
        /// 传入URL下载网页的HTML文本
        /// </summary>
        /// <param name="url">网址链接</param>
        /// <returns>返回string类型的HTML文本</returns>
        public static async Task<string> HtmlDownloadAsync(string url)
        {

            //HTTP下载
            var uri = new System.Uri(url);
            using (var httpClient = new Windows.Web.Http.HttpClient())
            {
                //Always catch network exceptions for async methods
                try
                {
                    string result = await httpClient.GetStringAsync(uri);
                    return result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("HTML页面下载出现错误,URL=" + url + "。错误消息：" + ex.Message);
                    throw ex;
                }
            }
        }
    }
}
