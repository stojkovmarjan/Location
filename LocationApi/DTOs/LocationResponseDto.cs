using LocationApi.Models;

namespace LocationApi.DTOs
{
    public class LocationResponseDto
    {
        public Location? LocationResponse { get; set;}
        public LocationParameters? LocationParameters { get; set;}
    }
}