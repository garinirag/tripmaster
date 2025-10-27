namespace TripMaster.Models
{
    public class RunRecord
    {
        public DateTime Date { get; set; }
        public double DistanceMeters { get; set; }
        public double DurationSeconds { get; set; }
        public double AverageKmh { get; set; }
    }
}