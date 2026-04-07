using ClinicaOdontologicaApp.Models;
using ClinicaOdontologicaApp.Services;
using ClinicaOdontologicaApp.Views.Cliente;
using ClinicaOdontologicaApp.Views.Consultorio;

namespace ClinicaOdontologicaApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly SessionService _session;

    public LoginPage(SessionService session)
    {
        InitializeComponent();
        _session = session;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var email = EmailEntry.Text?.Trim().ToLower() ?? "";

        if (email.Contains("admin"))
            _session.LoginAsConsultorio();
        else
            _session.LoginAsCliente("Matheus", "71993403860");

        if (_session.Role == UserRole.Consultorio)
            await Shell.Current.GoToAsync(nameof(ConsultorioHomePage));
        else
            await Shell.Current.GoToAsync(nameof(ClienteHomePage));
    }

    private async void OnForgotPasswordClicked(object sender, EventArgs e)
        => await DisplayAlert("Recuperação", "Função futura.", "OK");

    private async void OnContactClicked(object sender, EventArgs e)
        => await DisplayAlert("Contato", "Entrar em contato com a clínica.", "OK");
}
