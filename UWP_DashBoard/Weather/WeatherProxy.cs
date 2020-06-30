using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace UWP_DashBoard.Weather
{
    class WeatherProxy
    {
        static string city_name = "Melbourne";
        static string country_code = "AU";
        static string api_key = "97066f70dbb3d235a5ce9bf33da71f20";
        static string units = "metric";
        static string apiUrlCurrent = $"http://api.openweathermap.org/data/2.5/weather?q={city_name},{country_code}&units={units}&appid={api_key}";
        static string apiUrlForecast = $"http://api.openweathermap.org/data/2.5/forecast?q={city_name},{country_code}&units={units}&appid={api_key}";

        async private Task<WeatherCurrentRoot> getWeatherCurrent()
        {
            var http = new HttpClient();
            var response = await http.GetAsync(apiUrlCurrent);
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(WeatherCurrentRoot));
            var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (WeatherCurrentRoot)serializer.ReadObject(ms);
            return data;
        }

        async private Task<WeatherForecastRoot> getWeatherForecast()
        {
            var http = new HttpClient();
            var response = await http.GetAsync(apiUrlForecast);
            var result = await response.Content.ReadAsStringAsync();
            var serializer = new DataContractJsonSerializer(typeof(WeatherForecastRoot));
            var ms = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(result));
            var data = (WeatherForecastRoot)serializer.ReadObject(ms);
            return data;
        }

        async public Task<WeatherUi> GetCurrentWeatherUi()
        {
            WeatherCurrentRoot weatherCurrent = await getWeatherCurrent();
            WeatherUi weatherUi = new WeatherUi();
            weatherUi.Main = weatherCurrent.weather[0].main;
            weatherUi.Time = unixTimeStampToDateTime(weatherCurrent.dt);
            weatherUi.Temperature = weatherCurrent.main.temp.ToString("0") + "°C";
            weatherUi.Icon = getIcon(weatherCurrent.weather[0].icon);
            weatherUi.Description = weatherCurrent.weather[0].description;
            return weatherUi;
        }

        async public Task<ObservableCollection<WeatherUi>> GetForecastWeatherUiList()
        {
            WeatherForecastRoot weatherForecast = await getWeatherForecast();
            ObservableCollection<WeatherUi> weatherList = new ObservableCollection<WeatherUi>();
            foreach (var weatherData in weatherForecast.list.Take(30))
            {
                WeatherUi weatherUi = new WeatherUi();
                weatherUi.Main = weatherData.weather[0].main;
                weatherUi.Time = unixTimeStampToDateTime(weatherData.dt);
                weatherUi.Temperature = weatherData.main.temp.ToString("0") + "°C";
                weatherUi.Icon = getIcon(weatherData.weather[0].icon);
                weatherUi.Description = weatherData.weather[0].description;
                weatherList.Add(weatherUi);
            }
            return weatherList;
        }

        private DateTime unixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        private string getIcon(string index)
        {
            switch (index)
            {
                case "01d": return "\U0001F31E";
                case "01n": return "\U0001F31D";
                case "02d": return "\U0001F324";
                case "02n": return "\U0001F324";
                case "03d": return "\U0001F325";
                case "03n": return "\U0001F325";
                case "04d": return "\u2601";
                case "04n": return "\u2601";
                case "09d": return "\U0001F327";
                case "09n": return "\U0001F327";
                case "10d": return "\U0001F326";
                case "10n": return "\U0001F326";
                case "11d": return "\U0001F329";
                case "11n": return "\U0001F329";
                case "13d": return "\U0001F328";
                case "13n": return "\U0001F328";
                case "50d": return "\U0001F32B";
                case "50n": return "\U0001F32B";
            }
            throw new Exception($"获得天气图标异常，异常图标index={index}");
        }
    }
}
