namespace LocationApi.Models
{
    public class TrackingProfile
    {
        public string? Message { get; set; }
        public string? EmployeeId { get; set; }
        public string? CompanyId { get; set; }
        public string? CompanyName { get; set; }
        public string? GeozoneId { get; set; }
        public string? GeozoneName { get; set; }
        public int StartBtnEnabled { get; set; }
        public int StopBtnEnabled { get; set; }

    }
}