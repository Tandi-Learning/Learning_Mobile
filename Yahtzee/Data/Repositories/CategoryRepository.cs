using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Yahtzee.Models;
using Yahtzee.Data.Interfaces;

namespace Yahtzee.Data.Repositories;

/// <summary>
/// Repository class for managing categories in the database.
/// </summary>
public class CategoryRepository : IRepository<Category>
{
    private bool _hasBeenInitialized = false;
    private readonly ILogger _logger;

    public CategoryRepository(ILogger<CategoryRepository> logger)
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
            CREATE TABLE IF NOT EXISTS Category (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Title TEXT NOT NULL,
                Color TEXT NOT NULL
            );";
            await createTableCmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating Category table");
            throw;
        }

        _hasBeenInitialized = true;
    }

    public async Task<List<Category>> ListAsync()
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Category";
        List<Category> categories = new List<Category>();

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            categories.Add(new Category
            {
                ID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Color = reader.GetString(2)
            });
        }

        return categories;
    }

    public async Task<Category?> GetAsync(int id)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Category WHERE ID = @id";
        selectCmd.Parameters.AddWithValue("@id", id);

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            return new Category
            {
                ID = reader.GetInt32(0),
                Title = reader.GetString(1),
                Color = reader.GetString(2)
            };
        }

        return null;
    }

    public async Task<int> SaveItemAsync(Category item)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand saveCmd = connection.CreateCommand();
        if (item.ID == 0)
        {
            saveCmd.CommandText = @"
                INSERT INTO Category (Title, Color)
                VALUES (@Title, @Color);
                SELECT last_insert_rowid();";
        }
        else
        {
            saveCmd.CommandText = @"
                UPDATE Category SET Title = @Title, Color = @Color
                WHERE ID = @ID";
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

    public async Task<int> DeleteItemAsync(Category item)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM Category WHERE ID = @id";
        deleteCmd.Parameters.AddWithValue("@id", item.ID);

        int result = await deleteCmd.ExecuteNonQueryAsync();
        return result;
    }

    public async Task DropTableAsync()
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand dropTableCmd = connection.CreateCommand();
        dropTableCmd.CommandText = "DROP TABLE IF EXISTS Category";

        await dropTableCmd.ExecuteNonQueryAsync();
        _hasBeenInitialized = false;
    }
}
