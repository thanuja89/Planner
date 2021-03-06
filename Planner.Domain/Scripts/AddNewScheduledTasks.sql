USE [PlannerDb]
GO
/****** Object:  StoredProcedure [dbo].[SP_AddNewScheduledTasks]    Script Date: 6/1/2019 8:01:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

                ALTER     PROCEDURE [dbo].[SP_AddNewScheduledTasks](@Tasks [ScheduledTaskType] READONLY, @UserId VARCHAR(100)) 
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
								,[ClientUpdatedOnUtc]
                    			,[Title]
                    			,[Note]
                    			,[Importance]
                    			,[Repeat]
                    			,[Start]
                    			,[End]
                    			,[ApplicationUserId]
                    		)
                    		SELECT 
                    			 T.[Id]
                    			,GETUTCDATE()
                    			,GETUTCDATE()
								,T.ClientUpdatedOnUtc
                    			,T.[Title]
                    			,T.[Note]
                    			,T.[Importance]
                    			,T.[Repeat]
                    			,T.[Start]
                    			,T.[End]
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
                    				 ,[UpdatedOnUtc] = GETUTCDATE()
									 ,[ClientUpdatedOnUtc] = T.ClientUpdatedOnUtc
									 ,IsDeleted = T.IsDeleted
							FROM ScheduledTasks S
								INNER JOIN @Tasks T ON S.Id = T.Id
							WHERE T.[ClientUpdatedOnUtc] > S.[UpdatedOnUtc]
                    
                    		COMMIT TRANSACTION;
                    
                    	END TRY
                    
                    	BEGIN CATCH
                    
                    		ROLLBACK TRANSACTION;
                    
                    	END CATCH
                    
                    END