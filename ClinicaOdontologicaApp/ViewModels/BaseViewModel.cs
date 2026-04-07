using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClinicaOdontologicaApp.ViewModels;

public abstract class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool SetProperty<T>(ref T backing, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(backing, value)) return false;
        backing = value;
        OnPropertyChanged(name);
        return true;
    }
}
