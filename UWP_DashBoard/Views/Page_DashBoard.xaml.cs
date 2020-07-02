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
using LiveCharts;
using LiveCharts.Uwp;
using System.Drawing;
using Windows.UI;
using LiveCharts.Configurations;

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

        //public Windows.UI.Color color=Window.

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

        private SeriesCollection _AUDExchangeRateSeries;

        public SeriesCollection AUDExchangeRateSeries
        {
            get { return _AUDExchangeRateSeries; }
            set { _AUDExchangeRateSeries = value; OnPropertyChanged(); }
        }

        private SeriesCollection _AUDDailyHistorySeries;

        public SeriesCollection AUDDailyHistorySeries
        {
            get { return _AUDDailyHistorySeries; }
            set { _AUDDailyHistorySeries = value; OnPropertyChanged(); }
        }

        private ExchangeRateModel _latestRate;
        public ExchangeRateModel LatestRate
        {
            get { return _latestRate; }
            set { _latestRate = value;OnPropertyChanged(); }
        }

        private Windows.UI.Color fillColor = new Windows.UI.Color() {A=40,R=0,G=154,B=151 };
        private Windows.UI.Color lineColor = Colors.Orange;

        public Page_DashBoard()
        {
            this.InitializeComponent();
            showTime();
            updateWeather();
            updateBocExchangeRate();
            setChartUi();
        }

        /// <summary>
        /// 初始化并显示时钟，每秒钟更新一次
        /// </summary>
        private void showTime()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += (object sender, object e) =>
            {
                CurrentDateTime = DateTime.Now;
            };
            timer.Start();
            //启动timer之后，需要过一个周期才执行，所以这里手动执行该方法一次。
            CurrentDateTime = DateTime.Now;
        }
        /// <summary>
        /// 获取并更新天气到UI
        /// </summary>
        /// <returns></returns>
        private async Task updateWeather()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 3, 0);
            timer.Tick += async (object sender, object e) =>
            {
                try
                {
                    CurrentWeather = await new WeatherProxy().GetCurrentWeatherUi();
                    WeatherForecast = await new WeatherProxy().GetForecastWeatherUiList();
                    Debug.WriteLine("Weather获取数据完成");
                }catch(Exception ex)
                {
                    Debug.WriteLine("Weather获取数据失败"+ex.Message);
                }

            };
            timer.Start();
            //启动timer之后，需要过一个周期才执行，所以这里手动执行该方法一次。
            try
            {
                CurrentWeather = await new WeatherProxy().GetCurrentWeatherUi();
                WeatherForecast = await new WeatherProxy().GetForecastWeatherUiList();
                Debug.WriteLine("Weather获取数据完成");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Weather获取数据失败" + ex.Message);
            }
        }


        /// <summary>
        /// 更新数据库并获取最新的汇率
        /// </summary>
        /// <returns></returns>
        private async Task updateBocExchangeRate()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 3, 0);
            timer.Tick += async (object sender, object e) =>
            {
                await new BocService().updateDb();
                setExchangeRateSeries();
                setDailyHistorySeries();
            };
            timer.Start();
            //启动timer之后，需要过一个周期才执行，所以这里手动执行该方法一次。
            await new BocService().updateDb();
            //new BocService().updateDb();
            setExchangeRateSeries();
            setDailyHistorySeries();
        }

        private void setExchangeRateSeries()
        {
            BocService boc = new BocService();

            List<ExchangeRateModel> exchangeRateList = boc.GetExchangeRate();
            Debug.WriteLine($"从数据库中成功取出{exchangeRateList.Count()}条当日汇率，准备显示到UI上");
            LatestRate = boc.GetLatestExchangeRate();
            Debug.WriteLine($"从数据库中成功取出最新汇率{LatestRate.xhmcj}/{LatestRate.dateTime},准备显示到UI上");
            //LatestRate.xhmcj = LatestRate.xhmcj / 100;
            ChartValues<double> exchangeRateValues = new ChartValues<double>();
            foreach (ExchangeRateModel er in exchangeRateList)
            {
                exchangeRateValues.Add(er.xhmcj);
            }
            AUDExchangeRateSeries = new SeriesCollection
            {
                new LineSeries
                {
                    Values=exchangeRateValues,
                    StrokeThickness=4,
                    Stroke=new SolidColorBrush(lineColor),
                    Fill=new SolidColorBrush(fillColor),
                    PointGeometry=null,
                    Title="现汇卖出价",
                },
            };
        }


        private void setDailyHistorySeries()
        {
            BocService boc = new BocService();
            List<DailyHistoryModel> dailyHistory = boc.GetDailyHistory();
            ChartValues<double> dailyHistoryValues = new ChartValues<double>();
            foreach (DailyHistoryModel dh in dailyHistory)
            {
                dailyHistoryValues.Add(dh.avgXhmcj);
            }
            AUDDailyHistorySeries = new SeriesCollection
            {
                new LineSeries
                {
                    Values=dailyHistoryValues,
                    StrokeThickness=4,
                    Stroke=new SolidColorBrush(lineColor),
                    Fill=new SolidColorBrush(fillColor),
                    PointGeometry=null,
                    Title="现汇卖出价",
                },
            };
        }


        //设置图表的UI（因为LiveChart UWP有bug，xmal代码老报错）
        private void setChartUi()
        {
            ExchangeRateChart.AxisX = new AxesCollection()
            {
                new Axis()
                {
                    IsEnabled=false,
                    ShowLabels=false,
                },
            };

            DailyHistoryChart.AxisX = new AxesCollection()
            {
                new Axis()
                {
                    IsEnabled=true,
                    ShowLabels=false,
                },
            };

            ExchangeRateChart.AxisY = new AxesCollection()
            {
                new Axis()
                {
                    FontSize=16,
                    Separator=new Separator{ Step=10},
                    //MinValue=550,
                    //MaxValue=450,
                },
            };

            DailyHistoryChart.AxisY = new AxesCollection()
            {
                new Axis()
                {
                    FontSize=16,
                    Separator=new Separator{ Step=10},
                },
            };
            ExchangeRateChart.DataTooltip = new DefaultTooltip()
            {
                FontSize=18,
                SelectionMode=TooltipSelectionMode.SharedXValues,
                
            };
            DailyHistoryChart.DataTooltip = new DefaultTooltip()
            {
                FontSize = 18,
                SelectionMode = TooltipSelectionMode.SharedXValues,
            };
        }


    }
}
