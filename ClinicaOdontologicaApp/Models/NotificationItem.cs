namespace ClinicaOdontologicaApp.Models;

public class NotificationItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CriadoEm { get; set; } = DateTime.Now;
    public string Titulo { get; set; } = "";
    public string Mensagem { get; set; } = "";
}
