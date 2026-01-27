using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurisGo.Destinos
{
    public class CityDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        public string Region { get; set;}
        public string RegionCode { get; set; }  
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Population { get; set; }
        public string WikiDataId { get; set; }
        public string TimeZone { get; set; }
        public int? ElevationMeters {  get; set; }
        public string Type { get; set; }

    }
}
