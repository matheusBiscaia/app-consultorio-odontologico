using System.Collections.ObjectModel;
using ClinicaOdontologicaApp.Models;

namespace ClinicaOdontologicaApp.Services;

public class NotificationService
{
    private readonly ObservableCollection<NotificationItem> _items = new();
    public ReadOnlyObservableCollection<NotificationItem> Items { get; }

    public NotificationService()
    {
        Items = new ReadOnlyObservableCollection<NotificationItem>(_items);
    }

    public void Add(string titulo, string mensagem)
    {
        _items.Insert(0, new NotificationItem
        {
            Titulo = titulo,
            Mensagem = mensagem,
            CriadoEm = DateTime.Now
        });
    }
}
