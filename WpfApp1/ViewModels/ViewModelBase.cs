using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    protected async Task ExecuteAsync(Func<Task> operation, Action onError = null)
    {
        try
        {
            await operation();
        }
        catch (Exception ex)
        {
            onError?.Invoke();
            Console.WriteLine($"Operation failed: {ex.Message}");
            throw;
        }
    }
}