namespace TripMaster.Services
{
    public interface IDistanceService
    {
        void Start();
        void Pause();
        void ResetPartial();
        void ResetAll();
        double GetPartialDistanceMeters();
        double GetPartialDistanceKm();
        double GetDistanceMeters();
        double GetDistanceKm();
        double GetAverageKmh(double elapsedSeconds);
        void AddDeltaMeters(double meters, bool onlyTotal = false);
    }
}