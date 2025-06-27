using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfApp1;

public class MainViewModel : ViewModelBase
{
    private readonly IDataService _dataService;
    private readonly IUserService _userService;
    private bool _isLoading;
    private DatabaseTable _selectedTable;

    public ObservableCollection<DatabaseTable> Tables { get; }

    public DatabaseTable SelectedTable
    {
        get => _selectedTable;
        set
        {
            if (SetProperty(ref _selectedTable, value) && value != null)
            {
                OpenTableEditor();
            }
        }
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadTablesCommand { get; }
    public ICommand LoadMedicalStaffCommand { get; }


    public MainViewModel(IDataService dataService, IUserService userService)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));

        Tables = new ObservableCollection<DatabaseTable>();

        LoadTablesCommand = new RelayCommand(async _ => await LoadTablesAsync());

        LoadTablesAsync();

    }
    private async Task LoadTablesAsync()
    {

        IsLoading = true;

        if (_dataService == null)
            throw new InvalidOperationException("Сервис данных не инициализирован");

        var tables = await _dataService.GetDatabaseTablesAsync();
        Application.Current.Dispatcher.Invoke(() =>
        {
            Tables.Clear();
            foreach (var table in tables?.OrderBy(t => t.Name) ?? Enumerable.Empty<DatabaseTable>())
            {
                Tables.Add(table);
            }
        });
    }

    private async void OpenTableEditor()
    {
        if (SelectedTable == null || string.IsNullOrEmpty(SelectedTable.Name))
            return;

        try
        {
            IsLoading = true;

            var tableData = await _dataService.GetTableDataAsync(SelectedTable.Name);

            if (tableData == null)
                throw new Exception("Не удалось получить данные таблицы");

            var editorWindow = new TableEditorWindow(SelectedTable.Name, tableData)
            {
                Owner = Application.Current.MainWindow
            };

            editorWindow.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось открыть редактор таблицы: {ex.Message}",
                          "Ошибка",
                          MessageBoxButton.OK,
                          MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }
}