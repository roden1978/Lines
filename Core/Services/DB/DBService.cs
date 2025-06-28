using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
public class DBService : IDBService
{
    public IReadOnlyList<Result> Results => _results;
    private readonly List<Result> _results = [];
    private readonly IContentProvider _contentProvider;
    
    private const int Delay = 2000;

    public DBService(IContentProvider contentProvider) => _contentProvider = contentProvider;
    private string GetConnectionString() => _contentProvider.GetAccessStringByType(AccessTypes.Connect);
    private SqlConnection GetSqlConnection() => new(GetConnectionString());

    public async Task<bool> Save(Result result, bool insert)
    {
        bool success;

        if (insert)
            success = await Insert(result);
        else
            success = await Update(result);

        return success;
    }

    public async Task<bool> Get()
    {
        _results.Clear();
        CancellationTokenSource cancellationTokenSource = new();
        try
        {
            SqlConnection sqlConnection = GetSqlConnection();

            await using (sqlConnection)
            {
                cancellationTokenSource.CancelAfter(Delay);

                await sqlConnection.OpenAsync(SqlConnectionOverrides.OpenWithoutRetry, cancellationTokenSource.Token);

                await using SqlCommand sqlCommand = new(_contentProvider.GetAccessStringByType(AccessTypes.Read), sqlConnection);
                sqlCommand.Parameters.AddWithValue("@game", Settings.GameName);

                SqlDataReader reader = await sqlCommand.ExecuteReaderAsync(cancellationTokenSource.Token);

                if (reader.HasRows)
                {
                    while (await reader.ReadAsync(cancellationTokenSource.Token))
                    {
                        Result result = new();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetValue(0) != DBNull.Value)
                                result.PlayerName = Convert.ToString(reader.GetValue(0))?.Trim();

                            if (reader.GetValue(1) != DBNull.Value)
                                result.Value = Convert.ToInt32(reader.GetValue(1));
                        }
                        _results.Add(result);
                    }

                }
            }
        }
        catch (Exception e)
        {
            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine($"Error Get request => {e.Message}");
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            return false;
        }

        return _results.Count > 0;
    }

    private async Task<bool> Insert(Result result)
    {
        int count = 0;
        CancellationTokenSource cancellationTokenSource = new();
        try
        {
            SqlConnection sqlConnection = GetSqlConnection();

            await using (sqlConnection)
            {
                cancellationTokenSource.CancelAfter(Delay);
                await sqlConnection.OpenAsync(SqlConnectionOverrides.OpenWithoutRetry, cancellationTokenSource.Token);

                await using SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _contentProvider.GetAccessStringByType(AccessTypes.Insert);

                sqlCommand.Parameters.AddWithValue("@name", result.PlayerName);
                sqlCommand.Parameters.AddWithValue("@value", result.Value);
                sqlCommand.Parameters.AddWithValue("@game", result.Game);

                count = await sqlCommand.ExecuteNonQueryAsync(cancellationTokenSource.Token);
            }
        }
        catch (Exception e)
        {
            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine($"Error Insert request => {e.Message}");
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }
        return count > 0;
    }

    private async Task<bool> Update(Result result)
    {
        int count = 0;
        CancellationTokenSource cancellationTokenSource = new();
        try
        {
            SqlConnection sqlConnection = GetSqlConnection();
            await using (sqlConnection)
            {
                try
                {
                    cancellationTokenSource.CancelAfter(Delay);
                    await sqlConnection.OpenAsync(SqlConnectionOverrides.OpenWithoutRetry, cancellationTokenSource.Token);

                    await using SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = _contentProvider.GetAccessStringByType(AccessTypes.Update);

                    sqlCommand.Parameters.AddWithValue("@name", result.PlayerName);
                    sqlCommand.Parameters.AddWithValue("@value", result.Value);
                    sqlCommand.Parameters.AddWithValue("@game", result.Game);

                    try
                    {
                        count = await sqlCommand.ExecuteNonQueryAsync(cancellationTokenSource.Token);
                    }
                    catch (Exception e)
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested)
                        {
                            Console.WriteLine($"Error execute sql command request => {e.Message}");
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }

                }
                catch (Exception e)
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        Console.WriteLine($"Error Update request => {e.Message}");
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                }

            }
        }
        catch (Exception e)
        {
            if (cancellationTokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine($"Error connection request => {e.Message}");
                cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }


        return count > 0;
    }
}