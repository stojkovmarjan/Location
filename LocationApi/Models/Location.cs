using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationApi.Models
{
    public class Location
    {
        public DateTime Time { get; set; }
        public string? DeviceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public string? Provider { get; set; }
        public string? Message { get; set; }
        public int BatteryLevel { get; set; }
        public string? TimeZone { get; set; }
        public int TZoneOffset { get; set; }
    }
}