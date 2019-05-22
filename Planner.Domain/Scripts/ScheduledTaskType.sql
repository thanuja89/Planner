USE [PlannerDb]
GO

/****** Object:  UserDefinedTableType [dbo].[ScheduledTaskType]    Script Date: 5/22/2019 11:10:35 AM ******/
DROP TYPE [dbo].[ScheduledTaskType]
GO

/****** Object:  UserDefinedTableType [dbo].[ScheduledTaskType]    Script Date: 5/22/2019 11:10:35 AM ******/
CREATE TYPE [dbo].[ScheduledTaskType] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[ClientUpdatedOnUtc] [datetime2](7) NULL,
	[Title] [varchar](255) NULL,
	[Note] [nvarchar](max) NULL,
	[Importance] [nvarchar](max) NOT NULL,
	[Repeat] [nvarchar](max) NOT NULL,
	[Start] [datetime2](7) NOT NULL,
	[End] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL
)
GO


