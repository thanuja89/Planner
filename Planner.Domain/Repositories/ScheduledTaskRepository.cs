using Microsoft.EntityFrameworkCore;
using Planner.Domain.DataModels;
using Planner.Domain.Entities;
using Planner.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Planner.Domain.Repositories
{
    public class ScheduledTaskRepository : Repository<ScheduledTask>, IScheduledTaskRepository
    {
        public ScheduledTaskRepository(PlannerDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ScheduledTask>> GetScheduledTasksForUser(string userId)
        {
            return await Entities
                .AsNoTracking()
                .Where(t => t.ApplicationUserId == userId && !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<ScheduledTask>> GetNewScheduledTasksForUserAsync(string userId, DateTime lastSyncedOn)
        {
            return await Entities
                .Where(t => t.ApplicationUserId == userId
                    && t.UpdatedOnUtc > lastSyncedOn)
                .ToListAsync();
        }

        public Task AddOrUpdateScheduledTasksAsync(IEnumerable<ScheduledTaskDataModel> scheduledTasks, string userId)
        {
            if (scheduledTasks == null)
                return Task.CompletedTask;

            DataTable dataTable = MapToDataTable(scheduledTasks);

            var userIdParam = new SqlParameter("@UserId", userId);

            var tasksParam = new SqlParameter()
            {
                ParameterName = "@Tasks",
                TypeName = "ScheduledTaskType",
                SqlDbType = SqlDbType.Structured,
                Value = dataTable
            };

            return Context.Database.ExecuteSqlCommandAsync("SP_AddNewScheduledTasks @Tasks, @UserId"
                , new SqlParameter[] { tasksParam, userIdParam });

        }

        #region Helper methods

        private DataTable MapToDataTable(IEnumerable<ScheduledTaskDataModel> scheduledTasks)
        {
            var dataTable = new DataTable();

            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn()
                {
                    ColumnName = "Id",
                    DataType = typeof(Guid)
                },
                new DataColumn()
                {
                    ColumnName = "ClientUpdatedOnUtc",
                    DataType = typeof(DateTime)
                },
                new DataColumn()
                {
                    ColumnName = "Title",
                    DataType = typeof(string)
                },
                new DataColumn()
                {
                    ColumnName = "Note",
                    DataType = typeof(string)
                },
                new DataColumn()
                {
                    ColumnName = "Importance",
                    DataType = typeof(string)
                },
                new DataColumn()
                {
                    ColumnName = "Repeat",
                    DataType = typeof(string)
                },
                new DataColumn()
                {
                    ColumnName = "Start",
                    DataType = typeof(DateTime)
                },
                new DataColumn()
                {
                    ColumnName = "End",
                    DataType = typeof(DateTime)
                },
                new DataColumn()
                {
                    ColumnName = "IsAlarm",
                    DataType = typeof(bool)
                },
                new DataColumn()
                {
                    ColumnName = "IsDeleted",
                    DataType = typeof(bool)
                }
            });

            foreach (var task in scheduledTasks)
            {
                dataTable.Rows.Add(task.Id
                    , task.ClientUpdatedOn
                    , task.Title
                    , task.Note
                    , task.Importance
                    , task.Repeat
                    , task.Start
                    , task.End
                    , task.IsAlarm
                    , task.IsDeleted);
            }

            return dataTable;
        }

        #endregion
    }
}
