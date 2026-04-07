using System.Collections.ObjectModel;
using ClinicaOdontologicaApp.Models;

namespace ClinicaOdontologicaApp.Services;

public class ConsultaService
{
    private readonly ObservableCollection<Consulta> _consultas = new();
    public ReadOnlyObservableCollection<Consulta> Consultas { get; }

    private readonly NotificationService _notifications;

    public ConsultaService(NotificationService notifications)
    {
        _notifications = notifications;
        Consultas = new ReadOnlyObservableCollection<Consulta>(_consultas);
    }

    public bool Marcar(string nome, string telefone, Guid profissionalId, string profissionalNome, DateTime dataHora)
    {
        if (!HorarioDisponivel(profissionalId, dataHora))
            return false;

        var consulta = new Consulta
        {
            ClienteNome = nome,
            ClienteTelefone = telefone,
            ProfissionalId = profissionalId,
            ProfissionalNome = profissionalNome,
            DataHora = dataHora,
            Status = ConsultaStatus.Pendente
        };

        _consultas.Add(consulta);

        _notifications.Add(
            "Nova consulta",
            $"{consulta.ClienteNome} marcou com {consulta.ProfissionalNome} em {consulta.DataHora:dd/MM HH:mm}."
        );

        return true;
    }

    public void Confirmar(Guid consultaId)
    {
        var c = _consultas.FirstOrDefault(x => x.Id == consultaId);
        if (c is null) return;

        c.Status = ConsultaStatus.Confirmada;
        c.AtualizadoEm = DateTime.Now;

        _notifications.Add(
            "Consulta confirmada",
            $"{c.ClienteNome} teve a consulta com {c.ProfissionalNome} de {c.DataHora:dd/MM HH:mm} confirmada."
        );
    }

    public void Cancelar(Guid consultaId)
    {
        var c = _consultas.FirstOrDefault(x => x.Id == consultaId);
        if (c is null) return;
        if (c.Status != ConsultaStatus.Pendente) return;

        c.Status = ConsultaStatus.Cancelada;
        c.AtualizadoEm = DateTime.Now;

        _notifications.Add(
            "Consulta desmarcada",
            $"{c.ClienteNome} desmarcou com {c.ProfissionalNome} em {c.DataHora:dd/MM HH:mm}."
        );
    }

    public bool HorarioDisponivel(Guid profissionalId, DateTime dataHora)
        => !_consultas.Any(c =>
            c.Status != ConsultaStatus.Cancelada &&
            c.ProfissionalId == profissionalId &&
            c.DataHora == dataHora);
}
