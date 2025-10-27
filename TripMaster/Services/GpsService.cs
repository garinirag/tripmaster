namespace TripMaster.Services
{
    public class GpsService : IGpsService
    {
        public event Action<Location>? OnLocationChanged;
        public event Action<double>? OnDistanceDelta;
        public event Action<double>? OnSpeedChanged;


        private System.Timers.Timer? _timer;
        private Location? _lastLocation;
        private double _totalDistanceMeters = 0.0;
        private double _instantSpeedKmh = 0.0;

        public async Task StartAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                    throw new UnauthorizedAccessException("Location permission not granted.");
            }

            _timer = new System.Timers.Timer(1000); // Update every second
            _timer.Elapsed += async (s, e) => await FetchLocationAsync();
            _timer.Start();
        }


        public void Stop()
        {
            _timer?.Stop();
        }


        private async Task FetchLocationAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);
                if (location != null)
                {
                    if (_lastLocation != null)
                    {
                        double distance = Location.CalculateDistance(_lastLocation, location, DistanceUnits.Kilometers) * 1000.0;
                        double timeDeltaSeconds = (location.Timestamp - _lastLocation.Timestamp).TotalSeconds;

                        if (location.Accuracy > 20 || distance < 0.1 || timeDeltaSeconds <= 0)
                            return;

                        _totalDistanceMeters += distance;
                        _instantSpeedKmh = (distance / timeDeltaSeconds) * 3.6; // m/s -> km/h

                        OnDistanceDelta?.Invoke(distance);
                        OnSpeedChanged?.Invoke(_instantSpeedKmh);
                    }


                    OnLocationChanged?.Invoke(location);
                    _lastLocation = location;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GpsService] Error: {ex.Message}");
            }
        }

        public double GetInstantSpeedKmh() => _instantSpeedKmh;
        public double GetTotalDistanceMeters() => _totalDistanceMeters;
        public void ResetDistance() => _totalDistanceMeters = 0.0;
    }
}