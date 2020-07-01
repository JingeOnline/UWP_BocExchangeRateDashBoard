using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UWP_DashBoard.Tools;


namespace UWP_DashBoard.Boc
{
    public class BocDataRetrieval
    {

        private int startPageNum=25;

        /// <summary>
        /// 从中国银行的汇率历史页面获取汇率数据（所有）
        /// </summary>
        /// <returns>返回中国银行历史汇率所有数据</returns>
        public async Task<List<ExchangeRateModel>> GetRateFromWebPageAsync()
        {
            List<ExchangeRateModel> listAll = new List<ExchangeRateModel>();
            //每次开启5个Task同时获取数据
            for (int i = startPageNum; i >= 1; i-=5)
            {
                Task<List<ExchangeRateModel>> task1 = GetRateFromWebPageAsync(i);
                Task<List<ExchangeRateModel>> task2 = GetRateFromWebPageAsync(i-1);
                Task<List<ExchangeRateModel>> task3 = GetRateFromWebPageAsync(i-2);
                Task<List<ExchangeRateModel>> task4 = GetRateFromWebPageAsync(i - 3);
                Task<List<ExchangeRateModel>> task5 = GetRateFromWebPageAsync(i - 4);

                listAll.AddRange(await task1);
                listAll.AddRange(await task2);
                listAll.AddRange(await task3);
                listAll.AddRange(await task4);
                listAll.AddRange(await task5);
                Debug.WriteLine("下载完成Page" + i);
                Debug.WriteLine("数据总条数" + listAll.Count());
            }
            Debug.WriteLine("下载完成，数据总条数" + listAll.Count());
            return listAll;
        }

        /// <summary>
        /// 从中国银行的汇率历史页面获取汇率数据（单页）
        /// </summary>
        /// <param name="pageNum">网页的页码</param>
        /// <returns>返回中国银行历史汇率当页数据</returns>
        public async Task<List<ExchangeRateModel>> GetRateFromWebPageAsync(int pageNum)
        {
            List<ExchangeRateModel> rateList = new List<ExchangeRateModel>();

            string url = "https://srh.bankofchina.com/search/whpj/search_cn.jsp" +
                "?erectDate&nothing&pjname=%E6%BE%B3%E5%A4%A7%E5%88%A9%E4%BA%9A%E5%85%83&page=" + pageNum;
            String html = await DataScraper.HtmlDownloadAsync(url);
            string pattern1 = "澳大利亚元</td>(.|\n)*?</tr>";
            MatchCollection matchs = Regex.Matches(html, pattern1);

            foreach (Match match in matchs)
            {
                string pattern2 = @"<td>[\d.]*</td>";
                MatchCollection matches = Regex.Matches(match.Value, pattern2);
                string valuePattern = "[0-9]+[.]?[0-9]+";
                ExchangeRateModel er = new ExchangeRateModel();
                er.name = "AUD";
                er.xhmrj = float.Parse(Regex.Match(matches[0].Value, valuePattern).Value);
                er.xcmrj = float.Parse(Regex.Match(matches[1].Value, valuePattern).Value);
                er.xhmcj = float.Parse(Regex.Match(matches[2].Value, valuePattern).Value);
                er.xcmcj = float.Parse(Regex.Match(matches[3].Value, valuePattern).Value);
                er.zhzsj = float.Parse(Regex.Match(matches[4].Value, valuePattern).Value);


                string datePattern = "[0-9]{4}.[0-9]{2}.[0-9]{2}";
                Match m_date = Regex.Match(match.Value, datePattern);
                string[] date = m_date.Value.Split(".");
                string timePattern = "[0-9]{2}:[0-9]{2}:[0-9]{2}";
                Match m_time = Regex.Match(match.Value, timePattern);
                string[] time = m_time.Value.Split(":");
                er.dateTime = new DateTime
                    (
                    Convert.ToInt32(date[0]),
                    Convert.ToInt32(date[1]),
                    Convert.ToInt32(date[2]),
                    Convert.ToInt32(time[0]),
                    Convert.ToInt32(time[1]),
                    Convert.ToInt32(time[2])
                    );
                rateList.Add(er);
            }
            rateList.Reverse();
            return rateList;
        }

    }
}
