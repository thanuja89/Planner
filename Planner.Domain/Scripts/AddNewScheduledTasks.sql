USE [PlannerDb]
GO
/****** Object:  StoredProcedure [dbo].[SP_AddNewScheduledTasks]    Script Date: 3/10/2019 3:32:57 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER   PROCEDURE [dbo].[SP_AddNewScheduledTasks](@Tasks [ScheduledTaskType] READONLY, @UserId VARCHAR(100)) 
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

END
