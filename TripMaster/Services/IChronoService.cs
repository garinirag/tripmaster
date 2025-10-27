namespace TripMaster.Services
{
    public interface IChronoService
    {
        void Start();
        void Stop();
        void ResetPartial();
        void ResetAll();
        double ElapsedSeconds { get; }
        double ElapsedSecondsPartial { get; }
    }
}