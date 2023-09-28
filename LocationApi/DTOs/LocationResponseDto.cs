using LocationApi.Models;

namespace LocationApi.DTOs
{
    public class LocationResponseDto
    {
        public Location? Location { get; set;}
        public LocationParameters? LocationParameters { get; set;}
    }
}