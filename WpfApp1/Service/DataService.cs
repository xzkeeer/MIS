using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Npgsql;

public class DataService : BaseRepository, IDataService, IDatabaseService
{
    // Помогает экранировать идентификаторы (имена таблиц и колонок)
    private string EscapeIdentifier(string identifier) =>
        "\"" + identifier.Replace("\"", "\"\"") + "\"";
    public async Task<DataTable> GetTableDataAsync(string tableName)
    {
        const int limit = 1000; // можно хардкодить лимит, если нужно

        using var connection = await GetConnectionAsync();
        var escapedTableName = EscapeIdentifier(tableName);
        var query = $"SELECT * FROM {escapedTableName} LIMIT @Limit";

        var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("Limit", limit);

        var adapter = new NpgsqlDataAdapter(command);
        var dataTable = new DataTable();
        adapter.Fill(dataTable);
        dataTable.TableName = tableName;
        return dataTable;
    }
    

    public async Task<IEnumerable<DatabaseTable>> GetDatabaseTablesAsync()
    {
        try
        {
            var query = @"
                SELECT table_name as Name, 
                       (SELECT count(*) FROM information_schema.tables t WHERE t.table_name = tables.table_name) as RowCount
                FROM information_schema.tables
                WHERE table_schema = 'public'
                  AND table_name NOT LIKE 'pg_%'
                  AND table_name NOT LIKE 'sql_%'";

            return await QueryAsync<DatabaseTable>(query);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database error: {ex.Message}");
            return Enumerable.Empty<DatabaseTable>();
        }
    }

    public async Task UpdateTableAsync(DataTable table)
    {
        await UpdateMedicalTableAsync(table);
    }

    public async Task UpdateMedicalTableAsync(DataTable table)
    {
        if (table == null) throw new ArgumentNullException(nameof(table));

        using var connection = await GetConnectionAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var primaryKeyColumns = await GetPrimaryKeyColumnsAsync(connection, table.TableName);
            var columnsInfo = await GetTableColumnsInfoAsync(connection, table.TableName);
            var escapedTableName = EscapeIdentifier(table.TableName);

            // Обработка добавленных и изменённых строк
            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    foreach (var pkColumn in primaryKeyColumns)
                    {
                        if (row[pkColumn] == DBNull.Value || (row[pkColumn] is int && (int)row[pkColumn] == 0))
                        {
                            var seqQuery = $"SELECT nextval(pg_get_serial_sequence('{escapedTableName}', '{pkColumn}'))";
                            var newId = await connection.ExecuteScalarAsync<int>(seqQuery, transaction: transaction);
                            row[pkColumn] = newId;
                        }
                    }

                    foreach (DataColumn column in table.Columns)
                    {
                        if (row[column] == DBNull.Value &&
                            columnsInfo.TryGetValue(column.ColumnName, out var info) &&
                            !info.IsNullable)
                        {
                            row[column] = GetDefaultValueForType(info.DataType);
                        }
                    }
                }
                else if (row.RowState == DataRowState.Modified)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        if (row[column] == DBNull.Value &&
                            columnsInfo.TryGetValue(column.ColumnName, out var info) &&
                            !info.IsNullable)
                        {
                            row[column] = GetDefaultValueForType(info.DataType);
                        }
                    }
                }
                // НЕ обрабатываем Deleted здесь
            }

            // Удаляем строки
            if (table.GetChanges(DataRowState.Deleted) is DataTable deletedTable)
            {
                foreach (DataRow deletedRow in deletedTable.Rows)
                {
                    await DeleteRowAsync(connection, transaction, table.TableName, deletedRow, primaryKeyColumns);
                }
            }

            // Добавляем и обновляем строки
            foreach (DataRow row in table.Rows)
            {
                if (row.RowState == DataRowState.Added)
                {
                    await InsertRowAsync(connection, transaction, table.TableName, row);
                }
                else if (row.RowState == DataRowState.Modified)
                {
                    await UpdateRowAsync(connection, transaction, table.TableName, row, primaryKeyColumns);
                }
            }

            await transaction.CommitAsync();
            table.AcceptChanges();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            table.RejectChanges();
            throw new Exception($"Error updating table {table.TableName}: {ex.Message}", ex);
        }
    }

    public async Task<Dictionary<string, (string DataType, bool IsNullable)>> GetTableColumnsInfoAsync(NpgsqlConnection connection, string tableName)
    {
        var query = @"
            SELECT column_name, data_type, is_nullable
            FROM information_schema.columns
            WHERE table_name = @TableName";

        var columns = await connection.QueryAsync<(string column_name, string data_type, string is_nullable)>(query, new { TableName = tableName });

        return columns.ToDictionary(
            c => c.column_name,
            c => (c.data_type, c.is_nullable == "YES")
        );
    }

    private object GetDefaultValueForType(string dataType)
    {
        return dataType.ToLower() switch
        {
            "integer" or "bigint" => 0,
            "boolean" => false,
            "character varying" or "text" => string.Empty,
            "timestamp without time zone" or "timestamp with time zone" => DateTime.Now,
            _ => null
        };
    }

    public async Task<List<string>> GetPrimaryKeyColumnsAsync(NpgsqlConnection connection, string tableName)
    {
        var query = $@"
        SELECT a.attname
        FROM pg_index i
        JOIN pg_attribute a ON a.attrelid = i.indrelid AND a.attnum = ANY(i.indkey)
        WHERE i.indrelid = '{tableName}'::regclass
        AND i.indisprimary";

        var primaryKeys = await connection.QueryAsync<string>(query);
        return primaryKeys.ToList();
    }

    private async Task InsertRowAsync(NpgsqlConnection connection, NpgsqlTransaction transaction,
                                      string tableName, DataRow row)
    {
        var columns = new List<string>();
        var parameters = new DynamicParameters();

        foreach (DataColumn column in row.Table.Columns)
        {
            if (row[column] != DBNull.Value)
            {
                columns.Add(EscapeIdentifier(column.ColumnName));
                parameters.Add(column.ColumnName, row[column]);
            }
        }

        var columnsList = string.Join(", ", columns);
        var valuesList = string.Join(", ", columns.Select(c => "@" + c.Trim('"')));

        var escapedTableName = EscapeIdentifier(tableName);
        var query = $"INSERT INTO {escapedTableName} ({columnsList}) VALUES ({valuesList})";

        await connection.ExecuteAsync(query, parameters, transaction);
    }

    private async Task UpdateRowAsync(NpgsqlConnection connection, NpgsqlTransaction transaction,
                                      string tableName, DataRow row, List<string> primaryKeyColumns)
    {
        var setClauses = new List<string>();
        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        foreach (DataColumn column in row.Table.Columns)
        {
            if (!primaryKeyColumns.Contains(column.ColumnName))
            {
                var escapedCol = EscapeIdentifier(column.ColumnName);
                setClauses.Add($"{escapedCol} = @{column.ColumnName}");
                parameters.Add(column.ColumnName, row[column]);
            }
        }

        foreach (var pkColumn in primaryKeyColumns)
        {
            var escapedPk = EscapeIdentifier(pkColumn);
            whereClauses.Add($"{escapedPk} = @pk_{pkColumn}");
            parameters.Add($"pk_{pkColumn}", row[pkColumn, DataRowVersion.Original]);
        }

        var escapedTableName = EscapeIdentifier(tableName);
        var query = $"UPDATE {escapedTableName} SET {string.Join(", ", setClauses)} WHERE {string.Join(" AND ", whereClauses)}";

        await connection.ExecuteAsync(query, parameters, transaction);
    }

    private async Task DeleteRowAsync(IDbConnection connection, IDbTransaction transaction, string tableName, DataRow row, List<string> primaryKeyColumns)
    {
        var whereClauses = new List<string>();
        var parameters = new DynamicParameters();

        foreach (var pkColumn in primaryKeyColumns)
        {
            whereClauses.Add($"{EscapeIdentifier(pkColumn)} = @{pkColumn}");
            // Используем DataRowVersion.Original, т.к. строка в состоянии Deleted
            parameters.Add(pkColumn, row[pkColumn, DataRowVersion.Original]);
        }

        var whereClause = string.Join(" AND ", whereClauses);
        var sql = $"DELETE FROM {EscapeIdentifier(tableName)} WHERE {whereClause}";

        await connection.ExecuteAsync(sql, parameters, transaction);
    }

    public async Task<bool> DeleteRowByPkAsync(string tableName, Dictionary<string, object> pkValues)
    {
        using var connection = await GetConnectionAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var whereClauses = new List<string>();
            var parameters = new DynamicParameters();

            foreach (var kvp in pkValues)
            {
                var escapedKey = EscapeIdentifier(kvp.Key);
                whereClauses.Add($"{escapedKey} = @{kvp.Key}");
                parameters.Add(kvp.Key, kvp.Value);
            }

            var escapedTableName = EscapeIdentifier(tableName);
            var checkQuery = $"SELECT 1 FROM {escapedTableName} WHERE {string.Join(" AND ", whereClauses)} LIMIT 1";

            var exists = await connection.ExecuteScalarAsync<int?>(checkQuery, parameters, transaction);
            if (!exists.HasValue) return false;

            var deleteQuery = $"DELETE FROM {escapedTableName} WHERE {string.Join(" AND ", whereClauses)}";
            int affectedRows = await connection.ExecuteAsync(deleteQuery, parameters, transaction);

            await transaction.CommitAsync();
            return affectedRows > 0;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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

    public async Task<int> ImportDataAsync(string tableName, IEnumerable<Dictionary<string, object>> records)
    {
        using var connection = await GetConnectionAsync();
        using var transaction = await connection.BeginTransactionAsync();

        try
        {
            int importedCount = 0;
            var columns = await GetMedicalTableColumnsAsync(tableName);

            var escapedTableName = EscapeIdentifier(tableName);

            foreach (var record in records)
            {
                var columnsToInsert = record.Keys.Where(k => columns.Contains(k)).ToList();
                var columnNames = string.Join(", ", columnsToInsert.Select(EscapeIdentifier));
                var valuePlaceholders = string.Join(", ", columnsToInsert.Select(c => "@" + c));

                var sql = $"INSERT INTO {escapedTableName} ({columnNames}) VALUES ({valuePlaceholders})";

                importedCount += await connection.ExecuteAsync(sql, record, transaction);
            }

            await transaction.CommitAsync();
            return importedCount;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}