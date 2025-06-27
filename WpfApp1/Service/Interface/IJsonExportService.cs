public interface IJsonExportService
{
    Task ExportMedicalDataAsync<T>(string tableName, List<T> data, string directoryPath);
}