namespace ClinicaOdontologicaApp.Models;

public class Profissional
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Nome { get; set; } = "";
    public string Especialidade { get; set; } = "";
    public bool Ativa { get; set; } = true;

    public string NomeExibicao => string.IsNullOrWhiteSpace(Especialidade)
        ? Nome
        : $"{Nome} • {Especialidade}";
}
