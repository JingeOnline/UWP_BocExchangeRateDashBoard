using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_DashBoard.Boc
{
    public class DailyHistoryModel
    {
        public DateTime dateTime { get; set; }
        public string name { get; set; }
        /// <summary>
        /// 现汇买入价
        /// </summary>
        public float avgXhmrj { get; set; }
        /// <summary>
        /// 现钞买入价
        /// </summary>
        public float avgXcmrj { get; set; }
        /// <summary>
        /// 现汇卖出价
        /// </summary>
        public float avgXhmcj { get; set; }
        /// <summary>
        /// 现钞卖出价
        /// </summary>
        public float avgXcmcj { get; set; }
        /// <summary>
        /// 中行折算价
        /// </summary>
        public float avgZhzsj { get; set; }
    }
}
