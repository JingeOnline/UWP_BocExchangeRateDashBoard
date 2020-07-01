using LiteDB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWP_DashBoard.Boc
{
    public class BocDatabase
    {
        private string erDbPath = "ExchangeRates.db";
        private string erAuTable = "ExchangeRates";
        private string dhDbPath = "DailyHistory.db";
        private string dhAuTable = "DailyHistory";


        public BocDatabase()
        {
            erDbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, erDbPath);
            dhDbPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, dhDbPath);
            Debug.WriteLine(erDbPath);
        }

        /// <summary>
        /// 把ExchangeRateModel对象列表插入到数据库中，包含去重检验
        /// </summary>
        /// <param name="list"></param>
        public void InsertErTable(List<ExchangeRateModel> list)
        {
            if (list == null)
            {
                return;
            }

            using (var db = new LiteDatabase(erDbPath))
            {
                var table = db.GetCollection<ExchangeRateModel>(erAuTable);
                foreach (ExchangeRateModel er in list)
                {
                    var result = table.FindOne(Query.EQ("dateTime", er.dateTime));
                    if (result == null && er.dateTime != null)
                    {
                        table.Insert(er);
                    }
                }
            }
        }

        /// <summary>
        /// 把DailyHistoryModel对象插入到数据库中，包含去重检验
        /// </summary>
        /// <param name="dailyHistory"></param>
        public void InsertDhTable(DailyHistoryModel dailyHistory)
        {
            if (dailyHistory == null)
            {
                return;
            }
            using (var db = new LiteDatabase(dhDbPath))
            {
                var table = db.GetCollection<DailyHistoryModel>(dhAuTable);
                var result = table.FindOne(Query.EQ("dateTime", dailyHistory.dateTime));
                if (result == null)
                {
                    table.Insert(dailyHistory);
                }
            }
        }

        /// <summary>
        /// 根据日期查询某日的汇率，返回ExchangeRateModel列表
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns>ExchangeRateModel列表</returns>
        public List<ExchangeRateModel> queryErAuTable(DateTime dateTime)
        {
            DateTime day = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
            
            using (var db = new LiteDatabase(erDbPath))
            {
                DateTime dayEnd = day.AddDays(1);
                var table = db.GetCollection<ExchangeRateModel>(erAuTable);
                var results = table.FindAll().Where(x => x.dateTime >= day && x.dateTime < dayEnd);
                List<ExchangeRateModel> list = results.ToList();
                return list;
            }
        }
        /// <summary>
        /// 指定查询条数，返回DailyHistoryModel列表。
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        public List<DailyHistoryModel> queryDhAuTable(int num)
        {
            using (var db = new LiteDatabase(dhDbPath))
            {
                var table = db.GetCollection<DailyHistoryModel>(dhAuTable);
                var results = table.Find(Query.All(Query.Descending), limit: num);
                return results.ToList();
            }
        }

        /// <summary>
        /// 再DailyHistory数据库中查找指定日期的数据，如果找到返回True，找不到返回False
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public bool isDataExistInDhAuTable(DateTime dateTime)
        {
            using (var db=new LiteDatabase(dhDbPath))
            {
                var table = db.GetCollection<DailyHistoryModel>(dhAuTable);
                var result = table.FindOne(Query.EQ("dateTime", dateTime));
                return result != null;
            }
        }

    }

}
