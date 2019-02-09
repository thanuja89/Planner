using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Planner.Mobile.Core.Data
{
    public class PlannerDatabase
    {
        private static PlannerDatabase database;
        private readonly SQLiteAsyncConnection _connection;

        public PlannerDatabase(string path)
        {
            _connection = new SQLiteAsyncConnection(path);

            InitDatabase();
        }

        public static PlannerDatabase Instance
        {
            get
            {
                if (database == null)
                {
                    database = new PlannerDatabase(
                      Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PlannerSQLite.db3"));
                }
                return database;
            }
        }

        public AsyncTableQuery<T> GetAll<T>() where T : new ()
        {
            return _connection.Table<T>();
        }

        public Task<List<T>> QueryAll<T>(string query, params object[] objs) where T : new()
        {
            return _connection.QueryAsync<T>(query, objs);
        }

        public Task InsertAsync<T>(T item) where T : new()
        {
            return _connection.InsertAsync(item);
        }

        public Task UpdateAsync<T>(T item) where T : new()
        {
            return _connection.UpdateAsync(item);
        }

        public Task DeleteAsync<T>(T item) where T : new()
        {
            return _connection.DeleteAsync(item);
        }

        private void InitDatabase()
        {
            _connection.CreateTableAsync<ScheduledTask>().Wait();
        }
    }
}
