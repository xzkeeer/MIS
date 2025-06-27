using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

public class TableEditorViewModel : ViewModelBase
{
    private readonly IDatabaseService _dataService;
    private readonly IJsonExportService _exportService;
    private readonly string _tableName;
    private DataTable _tableData;
    private DataRowView _selectedRow;

    public DataTable TableData
    {
        get => _tableData;
        set => SetProperty(ref _tableData, value);
    }
    public string Title => $"Редактирование: {_tableName}";

    public DataRowView SelectedRow
    {
        get => _selectedRow;
        set => SetProperty(ref _selectedRow, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand ExportCommand { get; }
    public ICommand ImportCommand { get; }
    public ICommand DeleteRowCommand { get; }

    public TableEditorViewModel(string tableName, DataTable tableData,
                              IDatabaseService dataService,
                              IJsonExportService exportService)
    {
        _tableName = tableName;
        _tableData = tableData;
        _dataService = dataService;
        _exportService = exportService;

        // Инициализация команд
        SaveCommand = new RelayCommand(async _ => await SaveChangesAsync());
        ExportCommand = new RelayCommand(async _ => await ExportDataAsync());
        ImportCommand = new RelayCommand(async _ => await ImportDataAsync());
        DeleteRowCommand = new RelayCommand(_ => DeleteRow(), _ => CanDeleteRow());
    }

    private bool CanDeleteRow() => SelectedRow != null;

    private async Task SaveChangesAsync()
    {
        await ExecuteAsync(async () =>
        {
            ValidateMedicalData();
            await _dataService.UpdateMedicalTableAsync(TableData);
            MessageBox.Show("Changes saved successfully", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        },
        () => MessageBox.Show("Error saving data", "Error", MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void ValidateMedicalData()
    {
        if (_tableName == "patients" && TableData.Rows.Count > 0)
        {
            foreach (DataRow row in TableData.Rows)
            {
                if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                {
                    if (string.IsNullOrEmpty(row["insurance_policy"]?.ToString()))
                        throw new Exception("Insurance policy number is required");
                }
            }
        }
    }

    private async Task ExportDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                FileName = $"{_tableName}_export_{DateTime.Now:yyyyMMdd_HHmmss}.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                var filePath = saveFileDialog.FileName;

                var dataList = TableData.Rows.Cast<DataRow>()
                    .Select(row => TableData.Columns.Cast<DataColumn>()
                        .ToDictionary(col => col.ColumnName, col => row[col] == DBNull.Value ? null : row[col]))
                    .ToList();

                // Папка из пути файла
                var directoryPath = Path.GetDirectoryName(filePath);

                await _exportService.ExportMedicalDataAsync(_tableName, dataList, directoryPath);

                MessageBox.Show("Export completed successfully", "Export", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        },
        () => MessageBox.Show("Error exporting data", "Error", MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private async Task ImportDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var dialog = new OpenFileDialog
            {
                Filter = "JSON files (*.json)|*.json",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() == true)
            {
                var importService = new JsonImportService();
                var importedData = await importService.ImportMedicalDataAsync(dialog.FileName);

                foreach (var item in importedData)
                {
                    var newRow = TableData.NewRow();
                    foreach (var kvp in item)
                    {
                        if (TableData.Columns.Contains(kvp.Key))
                            newRow[kvp.Key] = kvp.Value ?? DBNull.Value;
                    }
                    TableData.Rows.Add(newRow);
                }

                MessageBox.Show("Import completed successfully", "Import", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        },
        () => MessageBox.Show("Error importing data", "Error", MessageBoxButton.OK, MessageBoxImage.Error));
    }

    private void DeleteRow()
    {
        if (SelectedRow == null) return;

        try
        {
            SelectedRow.Row.Delete();
            // УДАЛИ: TableData.AcceptChanges();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting row: {ex.Message}", "Error",
                           MessageBoxButton.OK, MessageBoxImage.Error);
            TableData.RejectChanges();
        }
    }
}