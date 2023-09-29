using LocationApi.Models;

namespace LocationApi.DTOs
{
    public class LocationParamSetDto
    {
        public List<string>? DevicesList { get; set; }
        public LocationParameters? LocationParameters { get; set; }
    }
}