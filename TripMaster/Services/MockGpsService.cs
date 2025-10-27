namespace TripMaster.Services
{
    public class MockGpsService : IGpsService
    {
        public event Action<Location>? OnLocationChanged;
        public event Action<double>? OnDistanceDelta;
        public event Action<double>? OnSpeedChanged;
        private System.Timers.Timer? _timer;
        private Random _rnd = new();

        public async Task StartAsync()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += (s, e) => Simulate();
            _timer.Start();
        }

        public void Stop() => _timer?.Stop();

        private void Simulate()
        {
            // Simulate small movement; call handler
            OnDistanceDelta?.Invoke(_rnd.NextDouble() * 50);
        }

        public double GetInstantSpeedKmh()
        {
            return _rnd.NextDouble() * 30;
        }
    }
}