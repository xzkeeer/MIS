using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

public class JsonExportService : IJsonExportService
{
    public async Task ExportMedicalDataAsync<T>(string tableName, List<T> data, string directoryPath)
    {
        try
        {
            var exportData = new MedicalExportTemplate<T>
            {
                TableName = tableName,
                ExportDate = DateTime.Now,
                RecordCount = data.Count,
                Records = data
            };

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var fileName = $"{tableName}_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            var fullPath = Path.Combine(directoryPath, fileName);

            Directory.CreateDirectory(directoryPath);
            var json = JsonSerializer.Serialize(exportData, options);
            await File.WriteAllTextAsync(fullPath, json);

            Console.WriteLine($"Medical data exported successfully to {fullPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Medical export error: {ex.Message}");
            throw;
        }
    }
}

public class MedicalExportTemplate<T>
{
    public string TableName { get; set; }
    public DateTime ExportDate { get; set; }
    public int RecordCount { get; set; }
    public List<T> Records { get; set; }
}