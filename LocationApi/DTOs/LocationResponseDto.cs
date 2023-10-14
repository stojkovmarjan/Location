using LocationApi.Models;

namespace LocationApi.DTOs
{
    public class LocationResponseDto
    {
        // TODO: remove Location
        //public Location? LocationResponse { get; set;}
        public LocationParameters? ParametersResponse { get; set;}
        public string? Message { get; set; }
        public TrackingProfile? TrackingProfile { get; set; }
        public WorkDays? WorkDays { get; set; }
        public WorkTime? WorkTime { get; set; }
    }
}