using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_DashBoard.Boc
{
    public class ExchangeRateModel
    {
        public string name { get; set; }
        /// <summary>
        /// 现汇买入价
        /// </summary>
        public float xhmrj { get; set; }
        /// <summary>
        /// 现钞买入价
        /// </summary>
        public float xcmrj { get; set; }
        /// <summary>
        /// 现汇卖出价
        /// </summary>
        public float xhmcj { get; set; }
        /// <summary>
        /// 现钞卖出价
        /// </summary>
        public float xcmcj { get; set; }
        /// <summary>
        /// 中行折算价
        /// </summary>
        public float zhzsj { get; set; }
        public DateTime dateTime { get; set; }
    }
}
