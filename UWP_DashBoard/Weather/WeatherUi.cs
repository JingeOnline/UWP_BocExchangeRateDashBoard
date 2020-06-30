using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace UWP_DashBoard.Weather
{
    public class WeatherUi
    {
        public string Icon { get; set; }
        public string Main { get; set; }
        public string Temperature { get; set; }
        public DateTime Time { get; set; }
        private string _description;
        public string Description 
        { 
            get { return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(_description); }
            set { _description = value; } 
        }
    }
}
