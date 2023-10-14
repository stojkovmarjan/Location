using LocationApi.Models;

namespace LocationApi.DTOs
{
    public class TrackingProfileDto
    {
        public string? DeviceId { get; set; }
        public string? Message { get; set; }
        public TrackingProfile? TrackingProfile { get; set; }
        public WorkDays? WorkDays { get; set; }
        public WorkTime? WorkTime { get; set; }
        
    }
}