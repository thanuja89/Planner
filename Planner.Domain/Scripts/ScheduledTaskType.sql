-- ================================
-- Create User-defined Table Type
-- ================================
USE PlannerDb
GO

BEGIN TRAN

	IF TYPE_ID(N'ScheduledTaskType') IS NOT NULL
		DROP TYPE [dbo].[ScheduledTaskType]
	
	GO
	
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
		[IsAlarm] [bit] NOT NULL
	)

ROLLBACK TRAN
