using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahtzee.Data;

public class YahtzeeDatabase
{
    public SQLiteAsyncConnection Database { get; set; }

    //SQLiteAsyncDatabasedatabase;

    async Task Init()
    {
        if (Database is not null)
            return;

        Database= new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        var result = await Database.CreateTableAsync<HighScore>();
    }
}