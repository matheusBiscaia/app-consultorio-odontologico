using ClinicaOdontologicaApp.Services;
using ClinicaOdontologicaApp.Models;
using System.Collections.ObjectModel;

namespace ClinicaOdontologicaApp.Views.Consultorio;

public partial class ConsultorioHomePage : ContentPage
{
    private readonly NotificationService _notifications;
    private readonly ConsultaService _consultas;
    private readonly ObservableCollection<Consulta> _consultasPendentes = new();

    public ConsultorioHomePage(NotificationService notifications, ConsultaService consultas)
    {
        InitializeComponent();
        _notifications = notifications;
        _consultas = consultas;
        NotifList.ItemsSource = _notifications.Items;
        ConsultasList.ItemsSource = _consultasPendentes;
        AtualizarEstadoTela();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarEstadoTela();
    }

    private void OnConfirmarClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || !TryGetConsultaId(button.CommandParameter, out var consultaId))
            return;

        _consultas.Confirmar(consultaId);
        AtualizarEstadoTela();
    }

    private void OnCancelarClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || !TryGetConsultaId(button.CommandParameter, out var consultaId))
            return;

        _consultas.Cancelar(consultaId);
        AtualizarEstadoTela();
    }

    private void AtualizarEstadoTela()
    {
        _consultasPendentes.Clear();

        foreach (var consulta in _consultas.Consultas
                     .Where(c => c.Status == ConsultaStatus.Pendente)
                     .OrderBy(c => c.DataHora))
        {
            _consultasPendentes.Add(consulta);
        }

        var totalConsultas = _consultasPendentes.Count;
        var totalNotificacoes = _notifications.Items.Count;

        EstadoVazioConsultasLabel.IsVisible = totalConsultas == 0;
        ConsultasList.IsVisible = totalConsultas > 0;
        ResumoConsultasLabel.Text = totalConsultas switch
        {
            0 => "Nenhuma",
            1 => "1 consulta",
            _ => $"{totalConsultas} consultas"
        };

        EstadoVazioNotificacoesLabel.IsVisible = totalNotificacoes == 0;
        NotifList.IsVisible = totalNotificacoes > 0;
    }

    private static bool TryGetConsultaId(object? commandParameter, out Guid consultaId)
    {
        switch (commandParameter)
        {
            case Guid id:
                consultaId = id;
                return true;
            case string idText when Guid.TryParse(idText, out var parsedId):
                consultaId = parsedId;
                return true;
            default:
                consultaId = Guid.Empty;
                return false;
        }
    }
}
