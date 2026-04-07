using Microsoft.Extensions.Logging;

using ClinicaOdontologicaApp.Services;
using ClinicaOdontologicaApp.Views;               // ✅ LoginPage
using ClinicaOdontologicaApp.Views.Cliente;       // ✅ ClienteHomePage
using ClinicaOdontologicaApp.Views.Consultorio;   // ✅ ConsultorioHomePage

namespace ClinicaOdontologicaApp;

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

        // Services
        builder.Services.AddSingleton<SessionService>();
        builder.Services.AddSingleton<NotificationService>();
        builder.Services.AddSingleton<ProfissionalService>();
        builder.Services.AddSingleton<ConsultaService>();

        // Pages
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ClienteHomePage>();
        builder.Services.AddTransient<ConsultorioHomePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
