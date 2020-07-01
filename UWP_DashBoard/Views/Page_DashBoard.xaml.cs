using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWP_DashBoard.Weather;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using UWP_DashBoard.Boc;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace UWP_DashBoard.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Page_DashBoard : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            if (this.PropertyChanged != null)
            {
                var handler = PropertyChanged;
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        /// <summary>
        /// 当前时间，用于显示在UI上
        /// </summary>
        private DateTime _currentDateTime;
        public DateTime CurrentDateTime
        {
            get { return _currentDateTime; }
            set { _currentDateTime = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// 当前天气对象，用于显示在UI上
        /// </summary>
        private WeatherUi _currentWeather;
        public WeatherUi CurrentWeather
        {
            get { return _currentWeather; }
            set { _currentWeather = value; OnPropertyChanged(); }
        }
        /// <summary>
        /// 天气预报列表,用于显示在UI上
        /// </summary>
        private ObservableCollection<WeatherUi> _weatherForecast;
        public ObservableCollection<WeatherUi> WeatherForecast
        {
            get { return _weatherForecast; }
            set { _weatherForecast = value; OnPropertyChanged(); }
        }

        public Page_DashBoard()
        {
            this.InitializeComponent();
            showTime();
            updateWeather();
            //updateExchangeRate();
            test();
        }

        /// <summary>
        /// 初始化并显示时钟，每秒钟更新一次
        /// </summary>
        private void showTime()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += (object sender,object e) =>
            {
                CurrentDateTime = DateTime.Now;
            };
            timer.Start();
            //启动timer之后，需要过一个周期才执行，所以这里手动执行该方法一次。
            CurrentDateTime = DateTime.Now;
        }

        private async Task updateWeather()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 1, 0);
            timer.Tick += async (object sender, object e)=>
            {
                CurrentWeather = await new WeatherProxy().GetCurrentWeatherUi();
                WeatherForecast = await new WeatherProxy().GetForecastWeatherUiList();
            };
            timer.Start();
            //启动timer之后，需要过一个周期才执行，所以这里手动执行该方法一次。
            CurrentWeather = await new WeatherProxy().GetCurrentWeatherUi();
            WeatherForecast = await new WeatherProxy().GetForecastWeatherUiList();
        }

        //private async Task updateExchangeRate()
        //{
        //    List<ExchangeRateModel> rateList = await new BocDataRetrieval().GetRateFromWebPageAsync();
        //    int count = rateList.Count();
        //}

        public async Task show(string s)
        {
            var md = new MessageDialog(s);
            await md.ShowAsync();
        }

        private async Task test()
        {
            BocService boc = new BocService();
            await boc.updateDb();
            List<ExchangeRateModel> exchangeRate=boc.getExchangeRate();
            List<DailyHistoryModel> dailyHistory = boc.GetDailyHistory();
        }




        
    }
}
