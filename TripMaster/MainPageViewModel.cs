using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TripMaster.Services;
using Timer = System.Timers.Timer;

namespace TripMaster
{
    public partial class MainPageViewModel : ObservableObject
    {
        private readonly IDistanceService _distanceService;
        private readonly IChronoService _chronoService;
        private readonly IAverageTargetService _averageTargetService;
        private readonly IDataLoggerService _dataLogger;
        private readonly IGpsService _gpsService;

        private readonly Timer _uiTimer;

        public MainPageViewModel(IDistanceService distanceService, IChronoService chronoService, IAverageTargetService averageTargetService, IDataLoggerService dataLogger, IGpsService gpsService)
        {
            _distanceService = distanceService;
            _chronoService = chronoService;
            _averageTargetService = averageTargetService;
            _gpsService = gpsService;
            _dataLogger = dataLogger;

            _uiTimer = new Timer(200); // 5 Hz UI refresh
            _uiTimer.Elapsed += (s, e) => UpdateBindings();

            StartCommand = new RelayCommand(Start);
            StopCommand = new RelayCommand(Stop);
            PauseCommand = new RelayCommand(Pause);
            NextStepCommand = new RelayCommand(NextStep);
            IncrementStepCommand = new RelayCommand(IncrementStep);
            DecrementStepCommand = new RelayCommand(DecrementStep);
            IncrementDistanceCommand = new RelayCommand(IncrementDistance);
            DecrementDistanceCommand = new RelayCommand(DecrementDistance);

            _gpsService.OnDistanceDelta += meters => _distanceService.AddDeltaMeters(meters);
        }

        public IRelayCommand StartCommand { get; }
        public IRelayCommand StopCommand { get; }
        public IRelayCommand PauseCommand { get; }
        public IRelayCommand NextStepCommand { get; }
        public IRelayCommand IncrementStepCommand { get; }
        public IRelayCommand DecrementStepCommand { get; }
        public IRelayCommand IncrementDistanceCommand { get; }
        public IRelayCommand DecrementDistanceCommand { get; }

        [ObservableProperty]
        double totalDistance = 0.0;

        [ObservableProperty]
        double partialDistance = 0.0;

        [ObservableProperty]
        TimeSpan totalElapsedTime = new TimeSpan();

        [ObservableProperty]
        TimeSpan partialElapsedTime = new TimeSpan();

        [ObservableProperty]
        int currentStep = 0;

        [ObservableProperty]
        bool isRunning;

        public bool IsNotRunning => !IsRunning;

        private void UpdateBindings()
        {
            // Run on main thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var avg = _distanceService.GetAverageKmh(_chronoService.ElapsedSeconds);
                var delta = _averageTargetService.GetDeltaSeconds(_distanceService.GetDistanceMeters(), _chronoService.ElapsedSeconds);

                TotalDistance = _distanceService.GetDistanceKm();
                PartialDistance = _distanceService.GetPartialDistanceKm();
                TotalElapsedTime = TimeSpan.FromSeconds(_chronoService.ElapsedSeconds);
                PartialElapsedTime = TimeSpan.FromSeconds(_chronoService.ElapsedSecondsPartial);
            });
        }

        private async void Start()
        {
            IsRunning = true;
            if (CurrentStep == 0) CurrentStep++;
            _chronoService.Start();
            _distanceService.Start();
            await _gpsService.StartAsync();
            _uiTimer.Start();
        }

        private void Pause()
        {
            IsRunning = false;
            _uiTimer.Stop();
            _distanceService.Pause();
            _chronoService.Stop();
            _gpsService.Stop();
            _dataLogger.SaveRun(new Models.RunRecord
            {
                Date = DateTime.UtcNow,
                DistanceMeters = _distanceService.GetDistanceMeters(),
                AverageKmh = _distanceService.GetAverageKmh(_chronoService.ElapsedSeconds),
                DurationSeconds = _chronoService.ElapsedSeconds
            });
        }

        private void NextStep()
        {
            CurrentStep++;
            _distanceService.ResetPartial();
            _chronoService.ResetPartial();
        }

        private void Stop()
        {
            Pause();
            _distanceService.ResetAll();
            _chronoService.ResetAll();
            _averageTargetService.Reset();
            CurrentStep = 0;
            TotalElapsedTime = new TimeSpan();
            PartialElapsedTime = new TimeSpan();
            TotalDistance = 0.0;
            PartialDistance = 0.0;
        }

        private void IncrementStep()
        {
            CurrentStep++;
        }

        private void DecrementStep()
        {
            if (CurrentStep > 0)
                CurrentStep--;
        }

        private void IncrementDistance()
        {
            _distanceService.AddDeltaMeters(10, true);
        }

        private void DecrementDistance()
        {
            _distanceService.AddDeltaMeters(-10, true);
        }
    }
}