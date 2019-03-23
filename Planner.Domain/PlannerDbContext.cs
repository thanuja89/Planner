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
                BEGIN TRAN
                
                	IF TYPE_ID(N'ScheduledTaskType') IS NOT NULL
                    BEGIN
						IF EXISTS ( SELECT  *
							FROM    sys.objects
							WHERE   object_id = OBJECT_ID(N'SP_AddNewScheduledTasks')
								AND type IN ( N'P', N'PC' ) ) 
							DROP PROCEDURE [dbo].[SP_AddNewScheduledTasks]

                		DROP TYPE [dbo].[ScheduledTaskType]                       
                    END 
                    CREATE TYPE [dbo].[ScheduledTaskType] AS TABLE(
                    		[Id] [uniqueidentifier] NOT NULL,
                    		[ClientUpdatedOnUtc] [datetime2](7) NULL,
                    		[Title] [varchar](255) NULL,
                    		[Note] [nvarchar](max) NULL,
                    		[Importance] [nvarchar](max) NOT NULL,
                    		[Repeat] [nvarchar](max) NOT NULL,
                    		[Start] [datetime2](7) NOT NULL,
                    		[End] [datetime2](7) NOT NULL,
                    		[IsAlarm] [bit] NOT NULL,
                    		[IsDeleted] [bit] NOT NULL
                    	)
                COMMIT TRAN");

            Database.ExecuteSqlCommand(@"
                CREATE OR ALTER   PROCEDURE [dbo].[SP_AddNewScheduledTasks](@Tasks [ScheduledTaskType] READONLY, @UserId VARCHAR(100)) 
                    AS	
                    BEGIN
                    
                    	SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
                    
                    	BEGIN TRY
                    
                    		BEGIN TRANSACTION
                    
                    		-- Insert

							INSERT INTO ScheduledTasks  
							(
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
                    		SELECT 
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
                    		
							FROM @Tasks T
							WHERE NOT EXISTS 
								(
									SELECT 1 
									FROM ScheduledTasks
									WHERE Id = T.Id
								)
								AND T.IsDeleted = 0;
							
							
							-- Update & Delete

							UPDATE S 
								SET
                    				  Title = T.Title
                    				 ,Note = T.Note
                    				 ,Importance = T.Importance
                    				 ,[Repeat] = T.[Repeat]
                    				 ,[Start] = T.[Start]
                    				 ,[End] = T.[End]
                    				 ,IsAlarm = T.IsAlarm
                    				 ,[UpdatedOnUtc] = GETUTCDATE()
									 ,IsDeleted = T.IsDeleted
							FROM ScheduledTasks S
								INNER JOIN @Tasks T ON S.Id = T.Id
							WHERE T.[ClientUpdatedOnUtc] > S.[UpdatedOnUtc]
                    
                    		COMMIT TRANSACTION;
                    
                    	END TRY
                    
                    	BEGIN CATCH
                    
                    		ROLLBACK TRANSACTION;
                    
                    	END CATCH
                    
                    END");
        }
    }
}
