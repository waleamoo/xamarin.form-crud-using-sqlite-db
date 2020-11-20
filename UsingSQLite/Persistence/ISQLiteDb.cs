using System;
using SQLite;

namespace UsingSQLite.Persistence
{
    public interface ISQLiteDb
    {
        SQLiteAsyncConnection GetConnection();
    }

}
