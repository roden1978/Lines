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

    public DBService(IContentProvider contentProvider)
    {
        _contentProvider = contentProvider;
    }
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
        CancellationTokenSource cancelationTokenSource = new();
        try
        {
            SqlConnection sqlConnection = GetSqlConnection();

            using (sqlConnection)
            {
                cancelationTokenSource.CancelAfter(2000);

                await sqlConnection.OpenAsync(SqlConnectionOverrides.OpenWithoutRetry, cancelationTokenSource.Token);

                using SqlCommand sqlCommand = new(_contentProvider.GetAccessStringByType(AccessTypes.Read), sqlConnection);
                sqlCommand.Parameters.AddWithValue("@game", Settings.GameName);

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    var resultString = string.Empty;

                    while (await reader.ReadAsync())
                    {
                        Result result = new();

                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            if (reader.GetValue(0) != DBNull.Value)
                                result.PlayerName = Convert.ToString(reader.GetValue(0)).Trim();

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
            if (cancelationTokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine($"Error Get request => {e.Message}");
                cancelationTokenSource.Token.ThrowIfCancellationRequested();
            }
            return false;
        }

        return _results.Count > 0;
    }

    private async Task<bool> Insert(Result result)
    {
        int count = 0;
        CancellationTokenSource cancelationTokenSource = new();
        try
        {
            SqlConnection sqlConnection = GetSqlConnection();

            using (sqlConnection)
            {
                cancelationTokenSource.CancelAfter(2000);
                await sqlConnection.OpenAsync(SqlConnectionOverrides.OpenWithoutRetry, cancelationTokenSource.Token);

                using SqlCommand sqlCommand = sqlConnection.CreateCommand();
                sqlCommand.CommandText = _contentProvider.GetAccessStringByType(AccessTypes.Insert);

                sqlCommand.Parameters.AddWithValue("@name", result.PlayerName);
                sqlCommand.Parameters.AddWithValue("@value", result.Value);
                sqlCommand.Parameters.AddWithValue("@game", result.Game);

                count = await sqlCommand.ExecuteNonQueryAsync();
            }
        }
        catch (Exception e)
        {
            if (cancelationTokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine($"Error Insert request => {e.Message}");
                cancelationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }
        return count > 0;
    }

    private async Task<bool> Update(Result result)
    {
        int count = 0;
        CancellationTokenSource cancelationTokenSource = new();
        try
        {
            SqlConnection sqlConnection = GetSqlConnection();
            using (sqlConnection)
            {
                try
                {
                    cancelationTokenSource.CancelAfter(2000);
                    await sqlConnection.OpenAsync(SqlConnectionOverrides.OpenWithoutRetry, cancelationTokenSource.Token);

                    using SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandText = _contentProvider.GetAccessStringByType(AccessTypes.Update);

                    sqlCommand.Parameters.AddWithValue("@name", result.PlayerName);
                    sqlCommand.Parameters.AddWithValue("@value", result.Value);
                    sqlCommand.Parameters.AddWithValue("@game", result.Game);

                    try
                    {
                        count = await sqlCommand.ExecuteNonQueryAsync();
                    }
                    catch (Exception e)
                    {
                        if (cancelationTokenSource.Token.IsCancellationRequested)
                        {
                            Console.WriteLine($"Error execute sql command request => {e.Message}");
                            cancelationTokenSource.Token.ThrowIfCancellationRequested();
                        }
                    }

                }
                catch (Exception e)
                {
                    if (cancelationTokenSource.Token.IsCancellationRequested)
                    {
                        Console.WriteLine($"Error Update request => {e.Message}");
                        cancelationTokenSource.Token.ThrowIfCancellationRequested();
                    }
                }

            }
        }
        catch (Exception e)
        {
            if (cancelationTokenSource.Token.IsCancellationRequested)
            {
                Console.WriteLine($"Error connection request => {e.Message}");
                cancelationTokenSource.Token.ThrowIfCancellationRequested();
            }
        }


        return count > 0;
    }
}