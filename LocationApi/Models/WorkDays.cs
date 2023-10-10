namespace LocationApi.Models
{
    public class WorkDays
    {
        public int WorkingMon { get; set; } = 1;
        public int WorkingTue { get; set; } = 1;
        public int WorkingWed { get; set; } = 1;
        public int WorkingThu { get; set; } = 1;
        public int WorkingFri { get; set; } = 1;
        public int WorkingSat { get; set; } = 0;
        public int WorkingSun { get; set; } = 0;
    }
}