using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class JsonImportService
{
    public async Task<List<Dictionary<string, object>>> ImportMedicalDataAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Medical import file not found", filePath);

            var json = await File.ReadAllTextAsync(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var importData = JsonSerializer.Deserialize<MedicalExportTemplate<JsonElement>>(json, options);

            if (importData?.Records == null)
                throw new InvalidDataException("Invalid medical JSON format or empty data");

            var result = new List<Dictionary<string, object>>();

            foreach (var record in importData.Records)
            {
                var dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

                foreach (var prop in record.EnumerateObject())
                {
                    dict[prop.Name] = prop.Value.ValueKind switch
                    {
                        JsonValueKind.String => prop.Value.GetString(),
                        JsonValueKind.Number => prop.Value.GetDecimal(),
                        JsonValueKind.True => true,
                        JsonValueKind.False => false,
                        JsonValueKind.Null => null,
                        _ => prop.Value.ToString()
                    };
                }

                result.Add(dict);
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Medical import error: {ex.Message}");
            throw;
        }
    }
}