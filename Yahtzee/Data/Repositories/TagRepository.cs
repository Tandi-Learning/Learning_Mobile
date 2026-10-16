using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Yahtzee.Models;
using Yahtzee.Data.Interfaces;

namespace Yahtzee.Data.Repositories;

/// <summary>
/// Repository class for managing tags in the database.
/// </summary>
public class TagRepository : IRepository<Tag>
{
    private bool _hasBeenInitialized = false;
    private readonly ILogger _logger;

    public TagRepository(ILogger<TagRepository> logger)
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
            CREATE TABLE IF NOT EXISTS Tag (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Color TEXT NOT NULL
            );";
            await createTableCmd.ExecuteNonQueryAsync();

            createTableCmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS ProjectsTags (
                ProjectID INTEGER NOT NULL,
                TagID INTEGER NOT NULL,
                PRIMARY KEY(ProjectID, TagID)
            );";
            await createTableCmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating tables");
            throw;
        }

        _hasBeenInitialized = true;
    }

    public async Task<List<Tag>> ListAsync()
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Tag";
        List<Tag> tags = new List<Tag>();

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tags.Add(new Tag
            {
                ID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Color = reader.GetString(2)
            });
        }

        return tags;
    }

    public async Task<List<Tag>> ListAsync(int projectID)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = @"
        SELECT t.*
        FROM Tag t
        JOIN ProjectsTags pt ON t.ID = pt.TagID
        WHERE pt.ProjectID = @ProjectID";
        selectCmd.Parameters.AddWithValue("ProjectID", projectID);

        List<Tag> tags = new List<Tag>();

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            tags.Add(new Tag
            {
                ID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Color = reader.GetString(2)
            });
        }

        return tags;
    }

    public async Task<Tag?> GetAsync(int id)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Tag WHERE ID = @id";
        selectCmd.Parameters.AddWithValue("@id", id);

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Tag
            {
                ID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Color = reader.GetString(2)
            };
        }

        return null;
    }

    public async Task<int> SaveItemAsync(Tag item)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand saveCmd = connection.CreateCommand();
        if (item.ID == 0)
        {
            saveCmd.CommandText = @"
            INSERT INTO Tag (Title, Color) VALUES (@Title, @Color);
            SELECT last_insert_rowid();";
        }
        else
        {
            saveCmd.CommandText = @"
            UPDATE Tag SET Title = @Title, Color = @Color WHERE ID = @ID";
            saveCmd.Parameters.AddWithValue("@ID", item.ID);
        }

        saveCmd.Parameters.AddWithValue("@Title", item.Title);
        saveCmd.Parameters.AddWithValue("@Color", item.Color);

        object? result = await saveCmd.ExecuteScalarAsync();
        if (item.ID == 0)
        {
            item.ID = Convert.ToInt32(result);
        }

        return item.ID;
    }

    public async Task<int> SaveItemAsync(Tag item, int projectID)
    {
        await Init();

        var isAssociated = await IsAssociated(item, projectID);
        if (isAssociated)
            return 0;

        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand saveCmd = connection.CreateCommand();
        saveCmd.CommandText = @"
            INSERT INTO ProjectsTags (ProjectID, TagID) VALUES (@ProjectID, @TagID);";
        saveCmd.Parameters.AddWithValue("@ProjectID", projectID);
        saveCmd.Parameters.AddWithValue("@TagID", item.ID);

        int result = await saveCmd.ExecuteNonQueryAsync();
        return result;
    }

    public async Task<int> SaveItemAsync(Tag item, int projectID, bool force)
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand saveCmd = connection.CreateCommand();
        saveCmd.CommandText = @"
            INSERT INTO ProjectsTags (ProjectID, TagID) VALUES (@ProjectID, @TagID);";
        saveCmd.Parameters.AddWithValue("@ProjectID", projectID);
        saveCmd.Parameters.AddWithValue("@TagID", item.ID);

        int result = await saveCmd.ExecuteNonQueryAsync();
        return result;
    }

    private async Task<bool> IsAssociated(Tag item, int projectID)
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand checkCmd = connection.CreateCommand();
        checkCmd.CommandText = @"
            SELECT COUNT(1)
            FROM ProjectsTags
            WHERE ProjectID = @ProjectID AND TagID = @TagID";
        checkCmd.Parameters.AddWithValue("@ProjectID", projectID);
        checkCmd.Parameters.AddWithValue("@TagID", item.ID);

        object? result = await checkCmd.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }

    public async Task<int> SaveAssociationAsync(Tag item, int projectID)
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand saveCmd = connection.CreateCommand();
        saveCmd.CommandText = @"
            INSERT INTO ProjectsTags (ProjectID, TagID) VALUES (@ProjectID, @TagID);";
        saveCmd.Parameters.AddWithValue("@ProjectID", projectID);
        saveCmd.Parameters.AddWithValue("@TagID", item.ID);

        int result = await saveCmd.ExecuteNonQueryAsync();
        return result;
    }

    public async Task<int> DeleteAssociationAsync(Tag item, int projectID)
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM ProjectsTags WHERE ProjectID = @ProjectID AND TagID = @TagID";
        deleteCmd.Parameters.AddWithValue("@ID", item.ID);

        int result = await deleteCmd.ExecuteNonQueryAsync();
        return result;
    }

    public async Task<int> DeleteItemAsync(Tag item)
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM Tag WHERE ID = @ID";
        deleteCmd.Parameters.AddWithValue("@ID", item.ID);

        int result = await deleteCmd.ExecuteNonQueryAsync();
        return result;
    }

    public async Task DropTableAsync()
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();
        SqliteCommand dropTableCmd = connection.CreateCommand();
        dropTableCmd.CommandText = "DROP TABLE IF EXISTS Tag";
        await dropTableCmd.ExecuteNonQueryAsync();
    }
}
