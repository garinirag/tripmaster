using System.Diagnostics;

namespace TripMaster.Services
{
    public class DistanceService : IDistanceService
    {
        private double _totalDistanceMeters = 0.0;
        private double _partialDistanceMeters = 0.0;
        private double _calibrationFactor = 1.0; // wheel coefficient
        private bool _running = false;

        // In real app, subscribe to GPS updates; here we'll expose methods for tests
        public void Start() => _running = true;
        public void Pause() => _running = false;

        public void ResetPartial()
        {
            _partialDistanceMeters = 0.0;
        }

        public void ResetAll()
        {
            _totalDistanceMeters = 0.0;
            _partialDistanceMeters = 0.0;
        }

        public void AddDeltaMeters(double meters, bool onlyTotal = false)
        {
            if (!_running) return;
            _totalDistanceMeters += meters * _calibrationFactor;
            if (!onlyTotal) _partialDistanceMeters += meters * _calibrationFactor;
        }

        public double GetDistanceMeters() => _totalDistanceMeters;
        public double GetDistanceKm() => _totalDistanceMeters / 1000.0;
        public double GetPartialDistanceMeters() => _partialDistanceMeters;
        public double GetPartialDistanceKm() => _partialDistanceMeters / 1000.0;
        public double GetAverageKmh(double elapsedSeconds)
        {
            if (elapsedSeconds <= 0) return 0.0;
            return (GetDistanceKm() / (elapsedSeconds / 3600.0));
        }

        public void SetCalibrationFactor(double factor) => _calibrationFactor = factor;
    }
}