using ClinicaOdontologicaApp.Views.Cliente;
using ClinicaOdontologicaApp.Views.Consultorio;

namespace ClinicaOdontologicaApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(ClienteHomePage), typeof(ClienteHomePage));
        Routing.RegisterRoute(nameof(ConsultorioHomePage), typeof(ConsultorioHomePage));
    }
}
