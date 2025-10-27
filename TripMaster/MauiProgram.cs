using Microsoft.Extensions.Logging;
using TripMaster.Services;

namespace TripMaster
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialDesign.ttf", "MaterialDesignIcons");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Services
            builder.Services.AddSingleton<IMainThreadDispatcher, MainThreadDispatcher>();
            builder.Services.AddSingleton<IDistanceService, DistanceService>();
            builder.Services.AddSingleton<IChronoService, ChronoService>();
            builder.Services.AddSingleton<IAverageTargetService, AverageTargetService>();
            builder.Services.AddSingleton<IBluetoothCommandService, BluetoothCommandService>();
            builder.Services.AddSingleton<IDataLoggerService, DataLoggerService>();

#if DEBUG
            builder.Services.AddSingleton<IGpsService, MockGpsService>();
            //builder.Services.AddSingleton<IGpsService, GpsService>();
#else
            builder.Services.AddSingleton<IGpsService, GpsService>();
#endif

            // ViewModels & Views
            builder.Services.AddSingleton<MainPageViewModel>();
            builder.Services.AddSingleton<MainPage>();

            return builder.Build();
        }
    }
}
