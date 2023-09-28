namespace LocationApi.Models
{
    public class LocationParameters
    {
        public int UpdateInterval { get; set; } = 60;
        public int MinUpdateInterval { get; set; } = 55;
        public float UpdateDistance { get; set; } = 0f;
    }
}