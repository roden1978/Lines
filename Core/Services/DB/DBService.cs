using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
public class DBService : IDBService
{
    public IReadOnlyList<Result> Results => _results;
    private const int Delay = 5000;
    private readonly List<Result> _results = [];
    private readonly IContentProvider _contentProvider;

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

    public async Task<bool> Get(IReadOnlyList<StringSource> leadersData, PersistentData persistentData)
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
                int index = 0;
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync(cancellationTokenSource.Token))
                    {
                        string playerName = string.Empty;
                        if (reader.GetValue(0) != DBNull.Value)
                        {
                            playerName = Convert.ToString(reader.GetValue(0))?.Trim();
                            persistentData.Results[index].PlayerName = playerName;
                        }

                        if (reader.GetValue(1) != DBNull.Value)
                        {
                            int value = Convert.ToInt32(reader.GetValue(1));
                            persistentData.Results[index].Value = value;
                            leadersData[index].Value = $"{playerName} {value}";
                        }

                        persistentData.Results[index].Game = Settings.GameName;

                        index++;
                    }
                }
            }
            return true;
        }
        catch (Exception e)
        {
            if (cancellationTokenSource.Token.IsCancellationRequested)
                Console.WriteLine($"Error Get request => {e.Message}");
            return false;
        }
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
                Console.WriteLine($"Error Insert request => {e.Message}");
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
                            Console.WriteLine($"Error execute sql command request => {e.Message}");
                    }

                }
                catch (Exception e)
                {
                    if (cancellationTokenSource.Token.IsCancellationRequested)
                        Console.WriteLine($"Error Update request => {e.Message}");
                }

            }
        }
        catch (Exception e)
        {
            if (cancellationTokenSource.Token.IsCancellationRequested)
                Console.WriteLine($"Error connection request => {e.Message}");
        }

        return count > 0;
    }
}