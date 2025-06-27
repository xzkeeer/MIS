using System.Data;

public interface IDataService
{
    Task<IEnumerable<DatabaseTable>> GetDatabaseTablesAsync();
    Task<DataTable> GetTableDataAsync(string tableName);
    Task UpdateTableAsync(DataTable table);
    
}