using Microsoft.Extensions.Logging;

namespace MauiAppSample;

using MauiAppSample.Interfaces;
using MauiAppSample.Services;
using MauiAppSample.ViewModels;
using MauiAppSample.Views;

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
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // services
        builder.Services.AddSingleton<IGasMeterReadingService, GasMeterReadingService>();
        
        // view models
        builder.Services.AddSingleton<MainPageViewModel>();
        builder.Services.AddTransient<DetailPageViewModel>();

        // views
        builder.Services.AddSingleton<MainPage>();
        builder.Services.AddTransient<DetailPage>();

        return builder.Build();
    }
}