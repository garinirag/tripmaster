namespace TripMaster.Services
{
    public interface IGpsService
    {
        event Action<Location>? OnLocationChanged;
        event Action<double>? OnDistanceDelta;
        event Action<double>? OnSpeedChanged;
        Task StartAsync();
        void Stop();
        double GetInstantSpeedKmh();
    }
}