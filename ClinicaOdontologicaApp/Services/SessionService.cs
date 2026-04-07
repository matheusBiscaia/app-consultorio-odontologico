using ClinicaOdontologicaApp.Models;

namespace ClinicaOdontologicaApp.Services;

public class SessionService
{
    public bool IsLoggedIn { get; private set; }
    public UserRole Role { get; private set; } = UserRole.Cliente;

    // Dados simples do cliente (pra fase 1)
    public string ClienteNome { get; private set; } = "Cliente";
    public string ClienteTelefone { get; private set; } = "000000000";

    public void LoginAsCliente(string nome, string telefone)
    {
        IsLoggedIn = true;
        Role = UserRole.Cliente;
        ClienteNome = string.IsNullOrWhiteSpace(nome) ? "Cliente" : nome.Trim();
        ClienteTelefone = string.IsNullOrWhiteSpace(telefone) ? "71993403860" : telefone.Trim();
    }

    public void LoginAsConsultorio()
    {
        IsLoggedIn = true;
        Role = UserRole.Consultorio;
    }

    public void Logout()
    {
        IsLoggedIn = false;
        Role = UserRole.Cliente;
    }
}
