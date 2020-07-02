using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_DashBoard.Boc
{
    public class BocService
    {
        private static bool IsFirstRun = true;
        private ExchangeRateModel latestExchangeRate;

        /// <summary>
        /// 获取数据库中的ExchangeRage
        /// </summary>
        /// <returns></returns>
        public List<ExchangeRateModel> GetExchangeRate()
        {
            //调试的时候如果当天没有数据，就用昨天的数据代替。
            //List<ExchangeRateModel> list = new BocDatabase().queryErAuTable(getChinaTimeNow().AddDays(-1));
            List<ExchangeRateModel> list = new BocDatabase().queryErAuTable(getChinaTimeNow());
            if (list.Count==0)
            {
           latestExchangeRate = new BocDatabase().getLatestExchangeRateInDb();
            }
            else
            {
            latestExchangeRate = list[list.Count - 1];
            }
            return list;
        }

        /// <summary>
        /// 获取数据库中的DailyHistory
        /// </summary>
        /// <returns></returns>
        public List<DailyHistoryModel> GetDailyHistory()
        {
            return new BocDatabase().queryDhAuTable(365);
        }


        public ExchangeRateModel GetLatestExchangeRate()
        {
            return latestExchangeRate; 
        }
        /// <summary>
        /// 执行所有的更新数据库工作
        /// </summary>
        /// <returns></returns>
        public async Task updateDb()
        {
            try
            {
                if (IsFirstRun)
                {

                    List<ExchangeRateModel> rateList = await new BocDataRetrieval().GetRateFromWebPageAsync();
                    new BocDatabase().InsertErTable(rateList);
                    createDailyHistoryObjectAndInsertDatabase();
                    IsFirstRun = false;
                }
                else
                {
                    List<ExchangeRateModel> rateList = await new BocDataRetrieval().GetRateFromWebPageAsync(1);
                    new BocDatabase().InsertErTable(rateList);
                    createDailyHistoryObjectAndInsertDatabase();
                }
                Debug.WriteLine("更新数据库完成");
            }
            catch(Exception e)
            {
                Debug.WriteLine("更新数据库因为异常而失败,可能是由于下载网页出现问题，或者数据库操作出现问题。" + e.Message);
                return;
            }

        }

        /// <summary>
        /// 从数据库中取出昨天的所有汇率数据，计算平均值统计成一个“每日数据”，并插入到DailyHistory数据库中
        /// </summary>
        private void createDailyHistoryObjectAndInsertDatabase()
        {
            BocDatabase database = new BocDatabase();
            DateTime yesterdayChinaTime = getChinaTimeNow().AddDays(-1).Date;
            //如果昨天数据已经存在，就不要再创建了
            if (database.isDataExistInDhAuTable(yesterdayChinaTime))
            {
                Debug.WriteLine("昨日历史已经存在，不需要创建‘昨日历史’。");
                return;
            }
            
            List<ExchangeRateModel> list = database.queryErAuTable(yesterdayChinaTime);
            if (list != null)
            {
                DailyHistoryModel dh = new DailyHistoryModel
                {
                    dateTime = yesterdayChinaTime,
                    name = "AUD",
                    avgXcmcj = list.Average(x => x.xcmcj),
                    avgXcmrj = list.Average(x => x.xcmrj),
                    avgXhmcj = list.Average(x => x.xhmcj),
                    avgXhmrj = list.Average(x => x.xhmrj),
                    avgZhzsj = list.Average(x => x.zhzsj),
                };
                database.InsertDhTable(dh);
            }
        }

        /// <summary>
        /// 获取系统当前时间对应的中国时间
        /// </summary>
        /// <returns></returns>
        private DateTime getChinaTimeNow()
        {
            DateTime chinaTime = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
            return chinaTime;
        }
    }
}
