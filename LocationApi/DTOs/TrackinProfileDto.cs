using LocationApi.Models;

namespace LocationApi.DTOs
{
    public class TrackingProfileDto
    {
        public TrackingProfile? TrackingProfile { get; set; }
        public WorkDays? WorkDays { get; set; }
        public WorkTime? WorkTime { get; set; }
    }
}