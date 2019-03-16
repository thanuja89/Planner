USE [PlannerDb]
GO


/****** Object:  UserDefinedTableType [dbo].[ScheduledTaskType]    Script Date: 3/16/2019 12:23:05 PM ******/
CREATE TYPE [dbo].[ScheduledTaskType] AS TABLE(
		[Id] [uniqueidentifier] NOT NULL,
		[ClientUpdatedOnUtc] [datetime2](7) NULL,
		[Title] [varchar](255) NULL,
		[Note] [nvarchar](max) NULL,
		[Importance] [nvarchar](max) NOT NULL,
		[Repeat] [nvarchar](max) NOT NULL,
		[Start] [datetime2](7) NOT NULL,
		[End] [datetime2](7) NOT NULL,
		[IsAlarm] [bit] NOT NULL
	)
GO


