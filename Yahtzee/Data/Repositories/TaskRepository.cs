using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Yahtzee.Models;
using Yahtzee.Data.Interfaces;

namespace Yahtzee.Data.Repositories;

/// <summary>
/// Repository class for managing tasks in the database.
/// </summary>
public class TaskRepository : IRepository<ProjectTask>
{
    private bool _hasBeenInitialized = false;
    private readonly ILogger _logger;

    public TaskRepository(ILogger<TaskRepository> logger)
    {
        _logger = logger;
    }

    private async Task Init()
    {
        if (_hasBeenInitialized)
            return;

        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        try
        {
            SqliteCommand createTableCmd = connection.CreateCommand();
            createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Task (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                IsCompleted INTEGER NOT NULL,
                ProjectID INTEGER NOT NULL
            );";
            await createTableCmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating Task table");
            throw;
        }

        _hasBeenInitialized = true;
    }

    public async Task<List<ProjectTask>> ListAsync()
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Task";
        List<ProjectTask> tasks = new List<ProjectTask>();

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new ProjectTask
            {
                ID = reader.GetInt32(0),
                Title = reader.GetString(1),
                IsCompleted = reader.GetBoolean(2),
                ProjectID = reader.GetInt32(3)
            });
        }

        return tasks;
    }

    public async Task<List<ProjectTask>> ListAsync(int projectId)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Task WHERE ProjectID = @projectId";
        selectCmd.Parameters.AddWithValue("@projectId", projectId);
        List<ProjectTask> tasks = new List<ProjectTask>();

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tasks.Add(new ProjectTask
            {
                ID = reader.GetInt32(0),
                Title = reader.GetString(1),
                IsCompleted = reader.GetBoolean(2),
                ProjectID = reader.GetInt32(3)
            });
        }

        return tasks;
    }

    public async Task<ProjectTask?> GetAsync(int id)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Task WHERE ID = @id";
        selectCmd.Parameters.AddWithValue("@id", id);

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new ProjectTask
            {
                ID = reader.GetInt32(0),
                Title = reader.GetString(1),
                IsCompleted = reader.GetBoolean(2),
                ProjectID = reader.GetInt32(3)
            };
        }

        return null;
    }

    public async Task<int> SaveItemAsync(ProjectTask item)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();
        SqliteCommand saveCmd = connection.CreateCommand();
        if (item.ID == 0)
        {
            saveCmd.CommandText = @"
            INSERT INTO Task (Title, IsCompleted, ProjectID) VALUES (@title, @isCompleted, @projectId);
            SELECT last_insert_rowid();";
        }
        else
        {
            saveCmd.CommandText = @"
            UPDATE Task SET Title = @title, IsCompleted = @isCompleted, ProjectID = @projectId WHERE ID = @id";
            saveCmd.Parameters.AddWithValue("@id", item.ID);
        }

        saveCmd.Parameters.AddWithValue("@title", item.Title);
        saveCmd.Parameters.AddWithValue("@isCompleted", item.IsCompleted);
        saveCmd.Parameters.AddWithValue("@projectId", item.ProjectID);

        object? result = await saveCmd.ExecuteScalarAsync();
        if (item.ID == 0)
        {
            item.ID = Convert.ToInt32(result);
        }

        return item.ID;
    }

    public async Task<int> DeleteItemAsync(ProjectTask item)
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);

        SqliteCommand deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM Task WHERE ID = @ID";
        deleteCmd.Parameters.AddWithValue("@ID", item.ID);

        int result = await deleteCmd.ExecuteNonQueryAsync();
        return result;
    }

    public async Task DropTableAsync()
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();
        SqliteCommand dropTableCmd = connection.CreateCommand();
        dropTableCmd.CommandText = "DROP TABLE IF EXISTS Task";
        await dropTableCmd.ExecuteNonQueryAsync();
    }
}
