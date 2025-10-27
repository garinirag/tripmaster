using System.Diagnostics;

namespace TripMaster.Services
{
    public class ChronoService : IChronoService
    {
        private Stopwatch _stopwatch = new Stopwatch();
        private double _offsetSeconds = 0.0; // for partial resets

        public void Start() => _stopwatch.Start();
        public void Stop() => _stopwatch.Stop();
        public void ResetPartial()
        {
            _offsetSeconds = _stopwatch.ElapsedMilliseconds / 1000; // if you want to change semantics, adjust
            //_stopwatch.Restart();
        }
        public void ResetAll()
        {
            _stopwatch.Reset();
            _offsetSeconds = 0;
        }

        public double ElapsedSeconds => (_stopwatch.Elapsed.TotalSeconds);

        public double ElapsedSecondsPartial => (_stopwatch.Elapsed.TotalSeconds - _offsetSeconds);
    }
}