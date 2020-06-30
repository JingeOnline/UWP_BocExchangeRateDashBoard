using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UWP_DashBoard.Weather
{
    [DataContract]
    class WeatherForecastRoot
    {
        [DataMember]
        public string cod { get; set; }
        [DataMember]
        public int message { get; set; }
        [DataMember]
        public int cnt { get; set; }
        [DataMember]
        public List<List> list { get; set; }
        [DataMember]
        public City city { get; set; }

        [DataContract]
        public class Main
        {
            [DataMember]
            public double temp { get; set; }
            [DataMember]
            public double feels_like { get; set; }
            [DataMember]
            public double temp_min { get; set; }
            [DataMember]
            public double temp_max { get; set; }
            [DataMember]
            public int pressure { get; set; }
            [DataMember]
            public int sea_level { get; set; }
            [DataMember]
            public int grnd_level { get; set; }
            [DataMember]
            public int humidity { get; set; }
            [DataMember]
            public double temp_kf { get; set; }
        }
        [DataContract]
        public class Weather
        {
            [DataMember]
            public int id { get; set; }
            [DataMember]
            public string main { get; set; }
            [DataMember]
            public string description { get; set; }
            [DataMember]
            public string icon { get; set; }
        }
        [DataContract]
        public class Clouds
        {
            [DataMember]
            public int all { get; set; }
        }
        [DataContract]
        public class Wind
        {
            [DataMember]
            public double speed { get; set; }
            [DataMember]
            public int deg { get; set; }
        }
        [DataContract]
        public class Sys
        {
            [DataMember]
            public string pod { get; set; }
        }
        [DataContract]
        public class Rain
        {
            [DataMember]
            public double __invalid_name__3h { get; set; }
        }
        [DataContract]
        public class List
        {
            [DataMember]
            public int dt { get; set; }
            [DataMember]
            public Main main { get; set; }
            [DataMember]
            public List<Weather> weather { get; set; }
            [DataMember]
            public Clouds clouds { get; set; }
            [DataMember]
            public Wind wind { get; set; }
            [DataMember]
            public Sys sys { get; set; }
            [DataMember]
            public string dt_txt { get; set; }
            [DataMember]
            public Rain rain { get; set; }
        }
        [DataContract]
        public class Coord
        {
            [DataMember]
            public double lat { get; set; }
            [DataMember]
            public double lon { get; set; }
        }
        [DataContract]
        public class City
        {
            [DataMember]
            public int id { get; set; }
            [DataMember]
            public string name { get; set; }
            [DataMember]
            public Coord coord { get; set; }
            [DataMember]
            public string country { get; set; }
            [DataMember]
            public int population { get; set; }
            [DataMember]
            public int timezone { get; set; }
            [DataMember]
            public int sunrise { get; set; }
            [DataMember]
            public int sunset { get; set; }
        }
    }
}
