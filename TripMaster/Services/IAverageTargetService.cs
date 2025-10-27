namespace TripMaster.Services
{
    public interface IAverageTargetService
    {
        void SetTarget(double kmh);
        double GetDeltaSeconds(double distanceMeters, double elapsedSeconds);
        void Reset();
    }
}