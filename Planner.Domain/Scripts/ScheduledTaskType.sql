USE [PlannerDb]
GO

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
COMMIT TRAN


