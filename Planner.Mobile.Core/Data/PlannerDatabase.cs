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

        private PlannerDatabase(string path)
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

        public Task RunInTransactionAsync<T>(Action<SQLiteConnection> action) where T : new()
        {
            return _connection.RunInTransactionAsync(action);
        }

        public AsyncTableQuery<T> GetAll<T>() where T : new()
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

        public Task InsertAllAsync<T>(IEnumerable<T> items) where T : new()
        {
            return _connection.InsertAllAsync(items);
        }

        public Task InsertOrUpdateAllAsync<T>(IEnumerable<T> items) where T : new()
        {
            return RunInTransactionAsync<T>(c => 
            {
                foreach (var item in items)
                {
                    c.InsertOrReplace(item);
                }
            });
        }

        public Task InsertOrUpdateAllTasksAsync(IEnumerable<ScheduledTask> items)
        {
            return RunInTransactionAsync<ScheduledTask>(c =>
            {
                foreach (var item in items)
                {
                    if (item.IsDeleted)
                    {
                        c.Delete(item);
                    }
                    else
                    {
                        c.InsertOrReplace(item);
                    }                                         
                }
            });
        }

        public Task UpdateAsync<T>(T item) where T : new()
        {
            return _connection.UpdateAsync(item);
        }

        public Task ExecuteCommandAsync(string command, params object[] objs)
        {
            return _connection.ExecuteAsync(command, objs);
        }

        public Task DeleteAsync<T>(T item) where T : new()
        {
            return _connection.DeleteAsync(item);
        }

        private void InitDatabase()
        {
            _connection.CreateTableAsync<ScheduledTask>().Wait();
        }

        private void Seed()
        {
            InsertAllAsync(new ScheduledTask[]
            {
                new ScheduledTask()
                {
                    Id = Guid.NewGuid(),
                    Note = "Note",
                    Title = "Title",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddDays(1)
                },
                new ScheduledTask()
                {
                    Id = Guid.NewGuid(),
                    Note = "1Note",
                    Title = "1Title",
                    Start = DateTime.Now,
                    End = DateTime.Now.AddDays(1)
                }
            }).Wait();
        }
    }
}
