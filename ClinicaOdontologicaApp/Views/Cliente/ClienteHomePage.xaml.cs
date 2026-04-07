using System.Collections.ObjectModel;
using ClinicaOdontologicaApp.Services;
using ClinicaOdontologicaApp.Models;
using Microsoft.Maui.Controls;

namespace ClinicaOdontologicaApp.Views.Cliente;

public partial class ClienteHomePage : ContentPage
{
    private static readonly DateTime MinDataAgendamento = DateTime.Today.AddDays(1);
    private static readonly DateTime MaxDataAgendamento = DateTime.Today.AddYears(3);
    private readonly SessionService _session;
    private readonly ConsultaService _consultas;
    private readonly ProfissionalService _profissionais;
    private readonly ObservableCollection<Consulta> _consultasCliente = new();
    private readonly List<TimeSpan> _horariosDisponiveis = new();
    private readonly string[] _nomesMeses =
    [
        "Janeiro", "Fevereiro", "Marco", "Abril", "Maio", "Junho",
        "Julho", "Agosto", "Setembro", "Outubro", "Novembro", "Dezembro"
    ];
    private DateTime _dataSelecionada;
    private int _horarioSelecionadoIndex;
    private DateTime _mesAtualExibido;
    private int _anoSeletorMesAno;
    private int _profissionalSelecionadoIndex;

    public ClienteHomePage(SessionService session, ConsultaService consultas, ProfissionalService profissionais)
    {
        InitializeComponent();
        _session = session;
        _consultas = consultas;
        _profissionais = profissionais;

        InfoLabel.Text = $"Logado como: {_session.ClienteNome} ({_session.ClienteTelefone})";
        ConfigurarSeletores();
        ConsultasList.ItemsSource = _consultasCliente;
        AtualizarLista();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        AtualizarLista();
    }

    private async void OnMarcarClicked(object sender, EventArgs e)
    {
        var profissionalSelecionado = _profissionais.Profissionais[_profissionalSelecionadoIndex];
        var horarioSelecionado = _horariosDisponiveis[_horarioSelecionadoIndex];
        var dataHora = _dataSelecionada.Date.Add(horarioSelecionado);

        if (dataHora <= DateTime.Now)
        {
            await DisplayAlert("Horário inválido", "Escolha uma data e um horário no futuro.", "OK");
            return;
        }

        if (!_consultas.HorarioDisponivel(profissionalSelecionado.Id, dataHora))
        {
            await DisplayAlert("Horário indisponível", $"Esse horário já foi reservado para {profissionalSelecionado.Nome}. Escolha outro horário.", "OK");
            RenderizarHorarios();
            return;
        }

        var agendou = _consultas.Marcar(
            _session.ClienteNome,
            _session.ClienteTelefone,
            profissionalSelecionado.Id,
            profissionalSelecionado.Nome,
            dataHora);
        if (!agendou)
        {
            await DisplayAlert("Horário indisponível", $"Esse horário acabou de ser reservado para {profissionalSelecionado.Nome}. Escolha outro horário.", "OK");
            RenderizarHorarios();
            return;
        }

        AtualizarLista();
        RenderizarHorarios();

        await DisplayAlert("Consulta marcada", $"Seu horário foi agendado para {dataHora:dd/MM/yyyy 'às' HH:mm}.", "OK");
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        if (sender is not Button button || !TryGetConsultaId(button.CommandParameter, out var consultaId))
            return;

        var confirmar = await DisplayAlert("Cancelar consulta", "Deseja mesmo cancelar esta consulta?", "Sim", "Não");
        if (!confirmar)
            return;

        _consultas.Cancelar(consultaId);
        AtualizarLista();
        RenderizarHorarios();
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

    private void AtualizarLista()
    {
        _consultasCliente.Clear();

        foreach (var consulta in _consultas.Consultas
                     .Where(c => c.ClienteTelefone == _session.ClienteTelefone)
                     .OrderBy(c => c.DataHora))
        {
            _consultasCliente.Add(consulta);
        }

        var total = _consultasCliente.Count;
        EstadoVazioLabel.IsVisible = total == 0;
        ConsultasList.IsVisible = total > 0;
        ResumoLabel.Text = total switch
        {
            0 => "Nenhuma",
            1 => "1 consulta",
            _ => $"{total} consultas"
        };
    }

    private void ConfigurarSeletores()
    {
        _horariosDisponiveis.Clear();

        for (var hora = 8; hora <= 18; hora++)
        {
            _horariosDisponiveis.Add(new TimeSpan(hora, 0, 0));
        }

        _dataSelecionada = MinDataAgendamento;
        _horarioSelecionadoIndex = 1;
        _mesAtualExibido = new DateTime(_dataSelecionada.Year, _dataSelecionada.Month, 1);
        _anoSeletorMesAno = _mesAtualExibido.Year;
        _profissionalSelecionadoIndex = 0;
        RenderizarProfissionais();
        AtualizarCabecalhoMes();
        RenderizarCalendario();
        RenderizarSeletorMesAno();
        RenderizarHorarios();
        AtualizarResumoSelecao();
    }

    private void AtualizarResumoSelecao()
    {
        var profissionalSelecionado = _profissionais.Profissionais[_profissionalSelecionadoIndex];
        var horarioSelecionado = _horariosDisponiveis[_horarioSelecionadoIndex];
        SelecaoResumoLabel.Text = $"Selecionado: {profissionalSelecionado.Nome} em {_dataSelecionada:dd/MM/yyyy} às {horarioSelecionado:hh\\:mm}";
    }

    private void RenderizarCalendario()
    {
        CalendarioGrid.Children.Clear();
        CalendarioGrid.RowDefinitions.Clear();

        var primeiroDiaDoMes = new DateTime(_mesAtualExibido.Year, _mesAtualExibido.Month, 1);
        var diasNoMes = DateTime.DaysInMonth(_mesAtualExibido.Year, _mesAtualExibido.Month);
        var colunaInicial = ((int)primeiroDiaDoMes.DayOfWeek + 6) % 7;
        var totalCelulas = colunaInicial + diasNoMes;
        var totalLinhas = (int)Math.Ceiling(totalCelulas / 7d);

        for (var linha = 0; linha < totalLinhas; linha++)
        {
            CalendarioGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        for (var i = 0; i < colunaInicial; i++)
        {
            var espacoVazio = new Border
            {
                StrokeThickness = 0,
                BackgroundColor = Colors.Transparent,
                HeightRequest = 72
            };

            Grid.SetRow(espacoVazio, 0);
            Grid.SetColumn(espacoVazio, i);
            CalendarioGrid.Children.Add(espacoVazio);
        }

        for (var dia = 1; dia <= diasNoMes; dia++)
        {
            var data = new DateTime(_mesAtualExibido.Year, _mesAtualExibido.Month, dia);
            var posicao = colunaInicial + dia - 1;
            var row = posicao / 7;
            var column = posicao % 7;
            var disponivel = data.Date >= MinDataAgendamento.Date && data.Date <= MaxDataAgendamento.Date;

            var button = new Button
            {
                Text = dia.ToString("00"),
                CornerRadius = 14,
                Padding = new Thickness(8, 10),
                HeightRequest = 72,
                FontSize = 16,
                CommandParameter = data
            };

            if (disponivel)
            {
                button.Clicked += OnDataButtonClicked;
                AplicarEstiloData(button, data.Date == _dataSelecionada.Date, true);
            }
            else
            {
                button.IsEnabled = false;
                AplicarEstiloData(button, false, false);
            }

            Grid.SetRow(button, row);
            Grid.SetColumn(button, column);
            CalendarioGrid.Children.Add(button);
        }
    }

    private void RenderizarHorarios()
    {
        GarantirHorarioSelecionadoValido();
        HorariosContainer.Children.Clear();
        var profissionalSelecionado = _profissionais.Profissionais[_profissionalSelecionadoIndex];

        for (var i = 0; i < _horariosDisponiveis.Count; i++)
        {
            var horario = _horariosDisponiveis[i];
            var dataHora = _dataSelecionada.Date.Add(horario);
            var disponivel = _consultas.HorarioDisponivel(profissionalSelecionado.Id, dataHora);
            var button = new Button
            {
                Text = horario.ToString(@"hh\:mm"),
                CornerRadius = 12,
                Padding = new Thickness(16, 10),
                Margin = new Thickness(0, 0, 10, 10),
                FontSize = 14,
                CommandParameter = i,
                IsEnabled = disponivel
            };

            if (disponivel)
                button.Clicked += OnHorarioButtonClicked;

            AplicarEstiloHorario(button, i == _horarioSelecionadoIndex, disponivel);
            HorariosContainer.Children.Add(button);
        }
    }

    private void RenderizarProfissionais()
    {
        ProfissionaisContainer.Children.Clear();

        for (var i = 0; i < _profissionais.Profissionais.Count; i++)
        {
            var profissional = _profissionais.Profissionais[i];
            var button = new Button
            {
                Text = profissional.NomeExibicao,
                CornerRadius = 14,
                Padding = new Thickness(16, 10),
                Margin = new Thickness(0, 0, 10, 10),
                FontSize = 14,
                CommandParameter = i
            };

            button.Clicked += OnProfissionalClicked;
            AplicarEstiloProfissional(button, i == _profissionalSelecionadoIndex);
            ProfissionaisContainer.Children.Add(button);
        }
    }

    private void OnDataButtonClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not DateTime dataSelecionada)
            return;

        _dataSelecionada = dataSelecionada;
        _mesAtualExibido = new DateTime(dataSelecionada.Year, dataSelecionada.Month, 1);
        AtualizarCabecalhoMes();
        RenderizarCalendario();
        RenderizarHorarios();
        AtualizarResumoSelecao();
    }

    private void OnHorarioButtonClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int index)
            return;

        _horarioSelecionadoIndex = index;
        RenderizarHorarios();
        AtualizarResumoSelecao();
    }

    private void OnProfissionalClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int index)
            return;

        _profissionalSelecionadoIndex = index;
        RenderizarProfissionais();
        RenderizarHorarios();
        AtualizarResumoSelecao();
    }

    private static void AplicarEstiloData(Button button, bool selecionado, bool disponivel)
    {
        if (!disponivel)
        {
            button.BackgroundColor = Color.FromArgb("#F7F3ED");
            button.TextColor = Color.FromArgb("#C9BFB1");
            button.BorderColor = Color.FromArgb("#EFE7DC");
            button.BorderWidth = 1;
            return;
        }

        button.BackgroundColor = selecionado ? Color.FromArgb("#B08D57") : Colors.White;
        button.TextColor = selecionado ? Colors.White : Color.FromArgb("#2C2C2C");
        button.BorderColor = selecionado ? Color.FromArgb("#B08D57") : Color.FromArgb("#E7DED2");
        button.BorderWidth = 1;
    }

    private static void AplicarEstiloHorario(Button button, bool selecionado, bool disponivel)
    {
        if (!disponivel)
        {
            button.BackgroundColor = Color.FromArgb("#F2F2F2");
            button.TextColor = Color.FromArgb("#B9B9B9");
            button.BorderColor = Color.FromArgb("#E8E8E8");
            button.BorderWidth = 1;
            return;
        }

        button.BackgroundColor = selecionado ? Color.FromArgb("#2C2C2C") : Color.FromArgb("#F2F2F2");
        button.TextColor = selecionado ? Colors.White : Color.FromArgb("#2C2C2C");
        button.BorderColor = selecionado ? Color.FromArgb("#2C2C2C") : Color.FromArgb("#E0E0E0");
        button.BorderWidth = 1;
    }

    private static void AplicarEstiloProfissional(Button button, bool selecionado)
    {
        button.BackgroundColor = selecionado ? Color.FromArgb("#B08D57") : Color.FromArgb("#F2F2F2");
        button.TextColor = selecionado ? Colors.White : Color.FromArgb("#2C2C2C");
        button.BorderColor = selecionado ? Color.FromArgb("#B08D57") : Color.FromArgb("#E0E0E0");
        button.BorderWidth = 1;
    }

    private void OnMesAnteriorClicked(object? sender, EventArgs e)
    {
        var primeiroMesDisponivel = new DateTime(MinDataAgendamento.Year, MinDataAgendamento.Month, 1);
        var mesAnterior = _mesAtualExibido.AddMonths(-1);

        if (mesAnterior < primeiroMesDisponivel)
            return;

        _mesAtualExibido = mesAnterior;
        AtualizarCabecalhoMes();
        RenderizarCalendario();
    }

    private void OnMesProximoClicked(object? sender, EventArgs e)
    {
        var ultimoMesDisponivel = new DateTime(MaxDataAgendamento.Year, MaxDataAgendamento.Month, 1);
        var proximoMes = _mesAtualExibido.AddMonths(1);

        if (proximoMes > ultimoMesDisponivel)
            return;

        _mesAtualExibido = proximoMes;
        AtualizarCabecalhoMes();
        RenderizarCalendario();
    }

    private void OnSelecionarMesClicked(object? sender, EventArgs e)
    {
        _anoSeletorMesAno = _mesAtualExibido.Year;
        RenderizarSeletorMesAno();
        SeletorMesAnoFrame.IsVisible = true;
    }

    private async void OnSelecionarAnoClicked(object? sender, EventArgs e)
    {
        var anosDisponiveis = Enumerable.Range(MinDataAgendamento.Year, MaxDataAgendamento.Year - MinDataAgendamento.Year + 1)
            .ToArray();

        var opcoes = anosDisponiveis
            .Select(ano => ano.ToString())
            .ToArray();

        var escolha = await DisplayActionSheet("Escolha o ano", "Cancelar", null, opcoes);
        if (string.IsNullOrWhiteSpace(escolha) || escolha == "Cancelar")
            return;

        if (!int.TryParse(escolha, out var anoSelecionado))
            return;

        var mesDesejado = _mesAtualExibido.Month;
        if (anoSelecionado == MinDataAgendamento.Year && mesDesejado < MinDataAgendamento.Month)
            mesDesejado = MinDataAgendamento.Month;
        if (anoSelecionado == MaxDataAgendamento.Year && mesDesejado > MaxDataAgendamento.Month)
            mesDesejado = MaxDataAgendamento.Month;

        _mesAtualExibido = new DateTime(anoSelecionado, mesDesejado, 1);
        AjustarDataSelecionadaAoMesVisivel();
        AtualizarCabecalhoMes();
        RenderizarCalendario();
        AtualizarResumoSelecao();
    }

    private void OnAnoAnteriorClicked(object? sender, EventArgs e)
    {
        if (_anoSeletorMesAno <= MinDataAgendamento.Year)
            return;

        _anoSeletorMesAno--;
        RenderizarSeletorMesAno();
    }

    private void OnAnoProximoClicked(object? sender, EventArgs e)
    {
        if (_anoSeletorMesAno >= MaxDataAgendamento.Year)
            return;

        _anoSeletorMesAno++;
        RenderizarSeletorMesAno();
    }

    private void OnMesSeletorClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not int mes)
            return;

        var dataMes = new DateTime(_anoSeletorMesAno, mes, 1);
        var primeiroMesDisponivel = new DateTime(MinDataAgendamento.Year, MinDataAgendamento.Month, 1);
        var ultimoMesDisponivel = new DateTime(MaxDataAgendamento.Year, MaxDataAgendamento.Month, 1);

        if (dataMes < primeiroMesDisponivel || dataMes > ultimoMesDisponivel)
            return;

        _mesAtualExibido = dataMes;
        AjustarDataSelecionadaAoMesVisivel();
        AtualizarCabecalhoMes();
        RenderizarCalendario();
        RenderizarHorarios();
        AtualizarResumoSelecao();
        SeletorMesAnoFrame.IsVisible = false;
    }

    private void OnFecharSeletorMesAnoClicked(object? sender, EventArgs e)
    {
        SeletorMesAnoFrame.IsVisible = false;
    }

    private void AjustarDataSelecionadaAoMesVisivel()
    {
        if (_dataSelecionada.Year == _mesAtualExibido.Year && _dataSelecionada.Month == _mesAtualExibido.Month)
            return;

        var primeiraDataValidaDoMes = new DateTime(_mesAtualExibido.Year, _mesAtualExibido.Month, 1);
        if (primeiraDataValidaDoMes < MinDataAgendamento.Date)
            primeiraDataValidaDoMes = MinDataAgendamento.Date;

        var ultimaDataValidaDoMes = new DateTime(_mesAtualExibido.Year, _mesAtualExibido.Month, DateTime.DaysInMonth(_mesAtualExibido.Year, _mesAtualExibido.Month));
        if (ultimaDataValidaDoMes > MaxDataAgendamento.Date)
            ultimaDataValidaDoMes = MaxDataAgendamento.Date;

        if (primeiraDataValidaDoMes <= ultimaDataValidaDoMes)
            _dataSelecionada = primeiraDataValidaDoMes;
    }

    private void GarantirHorarioSelecionadoValido()
    {
        if (_horarioSelecionadoIndex < 0 || _horarioSelecionadoIndex >= _horariosDisponiveis.Count)
            _horarioSelecionadoIndex = 0;

        var profissionalSelecionado = _profissionais.Profissionais[_profissionalSelecionadoIndex];
        var horarioAtual = _horariosDisponiveis[_horarioSelecionadoIndex];
        if (_consultas.HorarioDisponivel(profissionalSelecionado.Id, _dataSelecionada.Date.Add(horarioAtual)))
            return;

        var novoIndice = _horariosDisponiveis.FindIndex(h => _consultas.HorarioDisponivel(profissionalSelecionado.Id, _dataSelecionada.Date.Add(h)));
        if (novoIndice >= 0)
            _horarioSelecionadoIndex = novoIndice;
    }

    private void AtualizarCabecalhoMes()
    {
        var textoMes = _mesAtualExibido.ToString("MMMM");
        MesReferenciaButton.Text = char.ToUpper(textoMes[0]) + textoMes[1..];
        AnoReferenciaButton.Text = _mesAtualExibido.Year.ToString();

        var primeiroMesDisponivel = new DateTime(MinDataAgendamento.Year, MinDataAgendamento.Month, 1);
        var ultimoMesDisponivel = new DateTime(MaxDataAgendamento.Year, MaxDataAgendamento.Month, 1);

        MesAnteriorButton.IsEnabled = _mesAtualExibido > primeiroMesDisponivel;
        MesProximoButton.IsEnabled = _mesAtualExibido < ultimoMesDisponivel;
        MesAnteriorButton.Opacity = MesAnteriorButton.IsEnabled ? 1 : 0.45;
        MesProximoButton.Opacity = MesProximoButton.IsEnabled ? 1 : 0.45;
    }

    private void RenderizarSeletorMesAno()
    {
        MesesGrid.Children.Clear();
        MesesGrid.RowDefinitions.Clear();

        for (var row = 0; row < 4; row++)
        {
            MesesGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        SeletorAnoLabel.Text = _anoSeletorMesAno.ToString();

        var primeiroMesDisponivel = new DateTime(MinDataAgendamento.Year, MinDataAgendamento.Month, 1);
        var ultimoMesDisponivel = new DateTime(MaxDataAgendamento.Year, MaxDataAgendamento.Month, 1);

        for (var i = 0; i < _nomesMeses.Length; i++)
        {
            var mes = i + 1;
            var dataMes = new DateTime(_anoSeletorMesAno, mes, 1);
            var disponivel = dataMes >= primeiroMesDisponivel && dataMes <= ultimoMesDisponivel;
            var selecionado = dataMes.Year == _mesAtualExibido.Year && dataMes.Month == _mesAtualExibido.Month;

            var button = new Button
            {
                Text = _nomesMeses[i],
                CornerRadius = 14,
                HeightRequest = 52,
                FontSize = 14,
                CommandParameter = mes,
                IsEnabled = disponivel
            };

            button.Clicked += OnMesSeletorClicked;
            AplicarEstiloMesSeletor(button, selecionado, disponivel);

            Grid.SetRow(button, i / 3);
            Grid.SetColumn(button, i % 3);
            MesesGrid.Children.Add(button);
        }

        AnoAnteriorButton.IsEnabled = _anoSeletorMesAno > MinDataAgendamento.Year;
        AnoProximoButton.IsEnabled = _anoSeletorMesAno < MaxDataAgendamento.Year;
        AnoAnteriorButton.Opacity = AnoAnteriorButton.IsEnabled ? 1 : 0.45;
        AnoProximoButton.Opacity = AnoProximoButton.IsEnabled ? 1 : 0.45;
    }

    private static void AplicarEstiloMesSeletor(Button button, bool selecionado, bool disponivel)
    {
        if (!disponivel)
        {
            button.BackgroundColor = Color.FromArgb("#F7F3ED");
            button.TextColor = Color.FromArgb("#C9BFB1");
            button.BorderColor = Color.FromArgb("#EFE7DC");
            button.BorderWidth = 1;
            return;
        }

        button.BackgroundColor = selecionado ? Color.FromArgb("#B08D57") : Colors.White;
        button.TextColor = selecionado ? Colors.White : Color.FromArgb("#2C2C2C");
        button.BorderColor = selecionado ? Color.FromArgb("#B08D57") : Color.FromArgb("#E7DED2");
        button.BorderWidth = 1;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);

        if (width <= 0)
            return;

        var buttonWidth = (width - 112) / 7;
        foreach (var child in CalendarioGrid.Children.OfType<Button>())
        {
            child.WidthRequest = Math.Max(44, buttonWidth);
        }
    }
}
