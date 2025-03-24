using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Data;

public class YahtzeeDatabase
{
    SQLiteAsyncConnection database;

    async Task Init()
    {
        if (database is not null)
            return;

        database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        var result = await database.CreateTableAsync<HighScore>();
    }

    public async Task<List<HighScore>> GetHighScoresAsync()
    {
        await Init();
        return await database.Table<HighScore>().ToListAsync();
    }

    //public async Task<List<HighScores>> GetItemsNotDoneAsync()
    //{
    //    await Init();
    //    return await database.Table<HighScores>().Where(t => t.Done).ToListAsync();

    //    // SQL queries are also possible
    //    //return await Database.QueryAsync<HighScores>("SELECT * FROM [HighScores] WHERE [Done] = 0");
    //}

    //public async Task<HighScores> GetItemAsync(int id)
    //{
    //    await Init();
    //    return await database.Table<HighScores>().Where(i => i.ID == id).FirstOrDefaultAsync();
    //}

    public async Task<int> AddHighScoreAsync(HighScore item)
    {
        await Init();
        //if (item.ID != 0)
        //    return await database.UpdateAsync(item);
        //else
        return await database.InsertAsync(item);
    }

    public async Task ResetHighScoresAsync()
    {
        await database.DeleteAllAsync<HighScore>();
    }
    

    //public async Task<int> DeleteItemAsync(HighScore item)
    //{
    //    await Init();
    //    return await database.DeleteAsync(item);
    //}
}