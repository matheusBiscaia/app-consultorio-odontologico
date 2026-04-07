using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClinicaOdontologicaApp.Models;

public class Consulta : INotifyPropertyChanged
{
    private DateTime _dataHora;
    private ConsultaStatus _status = ConsultaStatus.Pendente;
    private DateTime _atualizadoEm = DateTime.Now;

    public event PropertyChangedEventHandler? PropertyChanged;

    public Guid Id { get; set; } = Guid.NewGuid();
    public string ClienteNome { get; set; } = "";
    public string ClienteTelefone { get; set; } = "";
    public Guid ProfissionalId { get; set; }
    public string ProfissionalNome { get; set; } = "";

    public DateTime DataHora
    {
        get => _dataHora;
        set
        {
            if (_dataHora == value) return;
            _dataHora = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DataHoraFormatada));
        }
    }

    public ConsultaStatus Status
    {
        get => _status;
        set
        {
            if (_status == value) return;
            _status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusTexto));
            OnPropertyChanged(nameof(StatusCor));
            OnPropertyChanged(nameof(PodeCancelarComoCliente));
            OnPropertyChanged(nameof(MensagemCliente));
            OnPropertyChanged(nameof(MostrarMensagemCliente));
        }
    }

    public DateTime CriadoEm { get; set; } = DateTime.Now;

    public DateTime AtualizadoEm
    {
        get => _atualizadoEm;
        set
        {
            if (_atualizadoEm == value) return;
            _atualizadoEm = value;
            OnPropertyChanged();
        }
    }

    public string DataHoraFormatada => DataHora == default ? "-" : DataHora.ToString("dd/MM/yyyy 'às' HH:mm");

    public string ProfissionalResumo => string.IsNullOrWhiteSpace(ProfissionalNome)
        ? "Profissional nao informada"
        : $"Profissional: {ProfissionalNome}";

    public string StatusTexto => Status switch
    {
        ConsultaStatus.Confirmada => "Confirmada",
        ConsultaStatus.Cancelada => "Cancelada",
        _ => "Pendente"
    };

    public string StatusCor => Status switch
    {
        ConsultaStatus.Confirmada => "#2F855A",
        ConsultaStatus.Cancelada => "#C53030",
        _ => "#B08D57"
    };

    public bool PodeCancelarComoCliente => Status == ConsultaStatus.Pendente;

    public bool MostrarMensagemCliente => Status != ConsultaStatus.Pendente;

    public string MensagemCliente => Status switch
    {
        ConsultaStatus.Confirmada => "Consulta confirmada. Alterações devem ser feitas com o consultório.",
        ConsultaStatus.Cancelada => "Consulta cancelada.",
        _ => string.Empty
    };

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
