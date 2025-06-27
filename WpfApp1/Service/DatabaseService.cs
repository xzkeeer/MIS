using Npgsql;
using Dapper;
using System.Configuration;
using System.Data;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class DatabaseService : BaseRepository
{
    public async Task UpdateMedicalTableAsync(DataTable table)
    {
        if (table == null) throw new ArgumentNullException(nameof(table));

        using var connection = await GetConnectionAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var primaryKeyColumns = await GetPrimaryKeyColumnsAsync(connection, table.TableName);
            var columnsInfo = await GetTableColumnsInfoAsync(connection, table.TableName);

            // Обработка автоинкрементных полей для новых строк
            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    foreach (var pkColumn in primaryKeyColumns)
                    {
                        if (row[pkColumn] == DBNull.Value || (row[pkColumn] is int && (int)row[pkColumn] == 0))
                        {
                            var sequenceQuery = $"SELECT nextval(pg_get_serial_sequence('{table.TableName}', '{pkColumn}'))";
                            var newId = await connection.ExecuteScalarAsync<int>(sequenceQuery, transaction: transaction);
                            row[pkColumn] = newId;
                        }
                    }
                }
            }

            // Обработка удаленных строк
            var deletedRows = table.GetChanges(DataRowState.Deleted)?.Rows;
            if (deletedRows != null)
            {
                foreach (DataRow row in deletedRows)
                {
                    await DeleteMedicalRowAsync(connection, transaction, table.TableName, row, primaryKeyColumns);
                }
            }

            // Обработка добавленных и измененных строк
            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    await InsertMedicalRowAsync(connection, transaction, table.TableName, row);
                }
                else if (row.RowState == DataRowState.Modified)
                {
                    await UpdateMedicalRowAsync(connection, transaction, table.TableName, row, primaryKeyColumns);
                }
            }

            await transaction.CommitAsync();
            table.AcceptChanges();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            table.RejectChanges();
            throw new Exception($"Error updating medical table {table.TableName}: {ex.Message}", ex);
        }
    }
    private async Task<List<string>> GetPrimaryKeyColumnsAsync(NpgsqlConnection connection, string tableName)
    {
        var query = @"
            SELECT column_name
            FROM information_schema.key_column_usage
            WHERE table_name = @TableName
            AND constraint_name LIKE '%pkey'";

        var columns = await connection.QueryAsync<string>(query, new { TableName = tableName });
        return columns.ToList();
    }

    private async Task<Dictionary<string, string>> GetTableColumnsInfoAsync(NpgsqlConnection connection, string tableName)
    {
        var query = @"
            SELECT column_name, data_type
            FROM information_schema.columns
            WHERE table_name = @TableName";

        var columns = await connection.QueryAsync<(string Name, string Type)>(query, new { TableName = tableName });
        return columns.ToDictionary(c => c.Name, c => c.Type);
    }
    private async Task InsertMedicalRowAsync(NpgsqlConnection connection, NpgsqlTransaction transaction,
                                          string tableName, DataRow row)
    {
        var columns = new List<string>();
        var parameters = new DynamicParameters();

        foreach (DataColumn column in row.Table.Columns)
        {
            if (row[column] != DBNull.Value)
            {
                columns.Add(column.ColumnName);
                parameters.Add(column.ColumnName, row[column]);
            }
        }

        var columnsList = string.Join(", ", columns);
        var valuesList = string.Join(", ", columns.Select(c => $"@{c}"));

        var query = $"INSERT INTO {tableName} ({columnsList}) VALUES ({valuesList})";
        await connection.ExecuteAsync(query, parameters, transaction);
    }

    private async Task UpdateMedicalRowAsync(NpgsqlConnection connection, NpgsqlTransaction transaction,
                                           string tableName, DataRow row, List<string> primaryKeyColumns)
    {
        var setClauses = new List<string>();
        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        foreach (DataColumn column in row.Table.Columns)
        {
            if (!primaryKeyColumns.Contains(column.ColumnName))
            {
                setClauses.Add($"{column.ColumnName} = @{column.ColumnName}");
                parameters.Add(column.ColumnName, row[column]);
            }
        }

        foreach (var pkColumn in primaryKeyColumns)
        {
            whereClauses.Add($"{pkColumn} = @pk_{pkColumn}");
            parameters.Add($"pk_{pkColumn}", row[pkColumn, DataRowVersion.Original]);
        }

        var query = $"UPDATE {tableName} SET {string.Join(", ", setClauses)} " +
                    $"WHERE {string.Join(" AND ", whereClauses)}";
        await connection.ExecuteAsync(query, parameters, transaction);
    }

    private async Task DeleteMedicalRowAsync(NpgsqlConnection connection, NpgsqlTransaction transaction,
                                          string tableName, DataRow row, List<string> primaryKeyColumns)
    {
        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        foreach (var pkColumn in primaryKeyColumns)
        {
            whereClauses.Add($"{pkColumn} = @{pkColumn}");
            parameters.Add(pkColumn, row[pkColumn, DataRowVersion.Original]);
        }

        var query = $"DELETE FROM {tableName} WHERE {string.Join(" AND ", whereClauses)}";
        await connection.ExecuteAsync(query, parameters, transaction);
    }

    public async Task<List<string>> GetMedicalTableColumnsAsync(string tableName)
    {
        using var connection = await GetConnectionAsync();
        var query = @"
            SELECT column_name
            FROM information_schema.columns
            WHERE table_name = @TableName
            ORDER BY ordinal_position";

        var columns = await connection.QueryAsync<string>(query, new { TableName = tableName });
        return columns.ToList();
    }
}