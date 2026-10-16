using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Yahtzee.Models;
using Yahtzee.Data.Interfaces;

namespace Yahtzee.Data.Repositories;

/// <summary>
/// Repository class for managing projects in the database.
/// </summary>
public class ProjectRepository : IRepository<Project>
{
    private bool _hasBeenInitialized = false;
    private readonly ILogger _logger;
    private readonly TaskRepository _taskRepository;
    private readonly TagRepository _tagRepository;

    public ProjectRepository(TaskRepository taskRepository, TagRepository tagRepository, ILogger<ProjectRepository> logger)
    {
        _taskRepository = taskRepository;
        _tagRepository = tagRepository;
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
            CREATE TABLE IF NOT EXISTS Project (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT NOT NULL,
                Icon TEXT NOT NULL,
                CategoryID INTEGER NOT NULL
            );";
            await createTableCmd.ExecuteNonQueryAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error creating Project table");
            throw;
        }

        _hasBeenInitialized = true;
    }

    public async Task<List<Project>> ListAsync(int categoryId)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();

        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Project WHERE CategoryID = @categoryId";
        selectCmd.Parameters.AddWithValue("@categoryId", categoryId);
        List<Project> projects = new List<Project>();

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            projects.Add(new Project
            {
                ID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Icon = reader.GetString(3),
                CategoryID = reader.GetInt32(4)
            });
        }

        foreach (Project project in projects)
        {
            project.Tags = await _tagRepository.ListAsync(project.ID);
            project.Tasks = await _taskRepository.ListAsync(project.ID);
        }

        return projects;
    }

    public async Task<List<Project>> ListAsync()
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();
        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Project";
        List<Project> projects = new List<Project>();

        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            projects.Add(new Project
            {
                ID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Icon = reader.GetString(3),
                CategoryID = reader.GetInt32(4)
            });
        }

        foreach (Project project in projects)
        {
            project.Tags = await _tagRepository.ListAsync(project.ID);
            project.Tasks = await _taskRepository.ListAsync(project.ID);
        }

        return projects;
    }

    public async Task<Project?> GetAsync(int id)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();
        SqliteCommand selectCmd = connection.CreateCommand();
        selectCmd.CommandText = "SELECT * FROM Project WHERE ID = @id";
        selectCmd.Parameters.AddWithValue("@id", id);
        SqliteDataReader reader = await selectCmd.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            Project project = new Project
            {
                ID = reader.GetInt32(0),
                Name = reader.GetString(1),
                Description = reader.GetString(2),
                Icon = reader.GetString(3),
                CategoryID = reader.GetInt32(4)
            };

            project.Tags = await _tagRepository.ListAsync(project.ID);
            project.Tasks = await _taskRepository.ListAsync(project.ID);

            return project;
        }

        return null;
    }

    public async Task<int> SaveItemAsync(Project item)
    {
        await Init();
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();
        SqliteCommand saveCmd = connection.CreateCommand();
        if (item.ID == 0)
        {
            saveCmd.CommandText = @"
                INSERT INTO Project (Name, Description, Icon, CategoryID)
                VALUES (@Name, @Description, @Icon, @CategoryID);
                SELECT last_insert_rowid();";
        }
        else
        {
            saveCmd.CommandText = @"
                UPDATE Project
                SET Name = @Name, Description = @Description, Icon = @Icon, CategoryID = @CategoryID
                WHERE ID = @ID";
            saveCmd.Parameters.AddWithValue("@ID", item.ID);
        }

        saveCmd.Parameters.AddWithValue("@Name", item.Name);
        saveCmd.Parameters.AddWithValue("@Description", item.Description);
        saveCmd.Parameters.AddWithValue("@Icon", item.Icon);
        saveCmd.Parameters.AddWithValue("@CategoryID", item.CategoryID);

        object? result = await saveCmd.ExecuteScalarAsync();
        if (item.ID == 0)
        {
            item.ID = Convert.ToInt32(result);
        }

        return item.ID;
    }

    public async Task<int> DeleteItemAsync(Project item)
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);

        SqliteCommand deleteCmd = connection.CreateCommand();
        deleteCmd.CommandText = "DELETE FROM Project WHERE ID = @ID";
        deleteCmd.Parameters.AddWithValue("@ID", item.ID);

        int result = await deleteCmd.ExecuteNonQueryAsync();
        return result;
    }

    public async Task DropTableAsync()
    {
        await using SqliteConnection connection = new SqliteConnection(Constants.DatabasePath);
        await connection.OpenAsync();
        SqliteCommand dropCmd = connection.CreateCommand();
        dropCmd.CommandText = "DROP TABLE IF EXISTS Project";
        await dropCmd.ExecuteNonQueryAsync();
    }
}
