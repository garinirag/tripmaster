namespace TripMaster.Services
{
    public class AverageTargetService : IAverageTargetService
    {
        private double _targetKmh = 0.0;

        public void SetTarget(double kmh) => _targetKmh = kmh;

        // returns elapsed - expected (positive => behind schedule)
        public double GetDeltaSeconds(double distanceMeters, double elapsedSeconds)
        {
            if (_targetKmh <= 0) return 0.0;
            var expectedSeconds = (distanceMeters / 1000.0) / _targetKmh * 3600.0;
            return elapsedSeconds - expectedSeconds;
        }

        public void Reset() => _targetKmh = 0.0;
    }
}