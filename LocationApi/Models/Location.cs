using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationApi.Models
{
    public class Location
    {
        public DateTime DeviceTime { get; set; }
        public string? DeviceId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Accuracy { get; set; }
        public string? Provider { get; set; }
        public string? Message { get; set; }
    }
}