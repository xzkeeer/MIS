using System.Data;

public interface IDatabaseService : IDataService
{
    Task UpdateMedicalTableAsync(DataTable table);
    Task<List<string>> GetMedicalTableColumnsAsync(string tableName);
}