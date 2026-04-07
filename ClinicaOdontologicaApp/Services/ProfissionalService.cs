using System.Collections.ObjectModel;
using ClinicaOdontologicaApp.Models;

namespace ClinicaOdontologicaApp.Services;

public class ProfissionalService
{
    private readonly ObservableCollection<Profissional> _profissionais = new();
    public ReadOnlyObservableCollection<Profissional> Profissionais { get; }

    public ProfissionalService()
    {
        Profissionais = new ReadOnlyObservableCollection<Profissional>(_profissionais);

        _profissionais.Add(new Profissional
        {
            Nome = "Dra. Thay",
            Especialidade = "Clinica Geral"
        });

        _profissionais.Add(new Profissional
        {
            Nome = "Dra. Theu",
            Especialidade = "Ortodontia"
        });
    }
}
