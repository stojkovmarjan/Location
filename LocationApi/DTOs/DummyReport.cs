using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocationApi.DTOs
{
    public class DummyReport
    {
        public string? device_id { get; set; } = "test device";
        public string? employee_id { get; set; } = "test employee id";
        public string? employee_name { get; set; } = "test employee name";
        public string? company_id { get; set; } = "test company id";
        public string? company_name { get; set; } = "test company name";
        public string? geozone_id { get; set; } = "test geozone id";
        public string? geozone_name { get; set; } = "test geozone name";
        public string? inzone_time { get; set; } = "122:00:00";
        public string? excuse_time { get; set; } = "101:00:00";
    }
}