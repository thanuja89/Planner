using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Planner.Domain.Entities;

namespace Planner.Domain
{
    public class PlannerDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<ScheduledTask> SheduledTasks { get; set; }

        public PlannerDbContext(DbContextOptions<PlannerDbContext> options) : base(options)
        {
           
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var importanceConverter = new EnumToStringConverter<Importance>();
            var frequencyConverter = new EnumToStringConverter<Frequency>();

            builder.Entity<ScheduledTask>()
                .Property(t => t.Importance)
                .HasConversion(importanceConverter);

            builder.Entity<ScheduledTask>()
                .Property(t => t.Repeat)
                .HasConversion(frequencyConverter);

            base.OnModelCreating(builder);

            Seed();
        }

        private void Seed()
        {
            Database.ExecuteSqlCommand(@"
                USE PlannerDb;
                
                BEGIN TRAN
                
                	IF TYPE_ID(N'ScheduledTaskType') IS NOT NULL
                		DROP TYPE [dbo].[ScheduledTaskType]
                	
                	CREATE TYPE [dbo].[ScheduledTaskType] AS TABLE(
                		[Id] [uniqueidentifier] NOT NULL,
                		[CreatedOnUtc] [datetime2](7) NOT NULL,
                		[UpdatedOnUtc] [datetime2](7) NULL,
                		[Title] [varchar](255) NULL,
                		[Note] [nvarchar](max) NULL,
                		[Importance] [nvarchar](max) NOT NULL,
                		[Repeat] [nvarchar](max) NOT NULL,
                		[Start] [datetime2](7) NOT NULL,
                		[End] [datetime2](7) NOT NULL,
                		[IsAlarm] [bit] NOT NULL,
                		[ApplicationUserId] [nvarchar](450) NOT NULL
                	)
                
                ROLLBACK TRAN");

            Database.ExecuteSqlCommand(@"
                CREATE OR ALTER PROCEDURE SP_AddNewScheduledTasks(@Tasks [ScheduledTaskType] READONLY) 
                AS	
                BEGIN
                
                	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;

	                BEGIN TRY

	                	BEGIN TRANSACTION

	                	MERGE ScheduledTasks S
	                	USING @Tasks T
	                		ON T.Id = S.Id

	                	WHEN MATCHED THEN
	                	
	                		UPDATE SET
	                			  S.Title = T.Title
	                			 ,S.Note = T.Note
	                			 ,S.Importance = T.Importance
	                			 ,S.[Repeat] = T.[Repeat]
	                			 ,S.[Start] = T.[Start]
	                			 ,S.[End] = T.[End]
	                			 ,S.IsAlarm = T.IsAlarm
	                			 ,S.[UpdatedOnUtc] = GETUTCDATE()

	                	WHEN NOT MATCHED THEN

	                		INSERT (
	                			 [Id]
	                			,[CreatedOnUtc]
	                			,[UpdatedOnUtc]
	                			,[Title]
	                			,[Note]
	                			,[Importance]
	                			,[Repeat]
	                			,[Start]
	                			,[End]
	                			,[IsAlarm]
	                			,[ApplicationUserId]
	                		)
	                		VALUES (
	                			 T.[Id]
	                			,GETUTCDATE()
	                			,GETUTCDATE()
	                			,T.[Title]
	                			,T.[Note]
	                			,T.[Importance]
	                			,T.[Repeat]
	                			,T.[Start]
	                			,T.[End]
	                			,T.[IsAlarm]
	                			,@UserId
	                		);

	                	COMMIT TRANSACTION;

	                END TRY

	                BEGIN CATCH

	                	ROLLBACK TRANSACTION;

	                END CATCH
                
                END");
        }
    }
}
