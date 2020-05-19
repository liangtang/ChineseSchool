USE [ChineseSchool]
GO

/****** Object:  StoredProcedure [dbo].[RegisterStudent]    Script Date: 11/7/2017 9:12:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

Create Procedure [dbo].[RegisterStudent]
	@StudentId  int,
	@IfRegister bit,
	@classId int,
	@EnrichmentClassId int,
	@UserID nvarchar(128),
	@RegistrationDate Date
AS
	
	DECLARE @currentSemesterID int
	DECLARE @parentID int
	DECLARE @EndUserID NVARCHAR(128)
	DECLARE @alreadyRegistered int
	DECLARE @price1 decimal(8,2)
	DECLARE @price2 decimal(8,2)
	DECLARE @price3 decimal(8,2)
	DECLARE @price4 decimal(8,2)
	DECLARE @price5 decimal(8,2)
	DECLARE @ErrorMsg NVARCHAR(4000)

	BEGIN TRAN
	BEGIN TRY
		SELECT  @currentSemesterID=MAX(SemesterID),
				@price1=MAX(Price1),
				@price2=MAX(Price2),
				@price3=MAX(Price3),
				@price4=MAX(Price4),
				@price5=MAX(Price5)
			 FROM Semester where ActiveFlg=1
		SET @parentID = (SELECT parentsID FROM Student WHERE StudentID = @StudentId)
		SET @EndUserID = (SELECT UserID FROM Parent WHERE ParentsId = @parentID)
		SET @alreadyRegistered = (SELECT COUNT(distinct s.studentId) FROM Student s  join ClassRegistration c
									ON c.StudentId=s.StudentId WHERE s.ParentsId = @parentID 
									AND s.Deleted=0 AND c.SemesterId=@currentSemesterID and c.ActiveFlag=1)

		IF (@IfRegister =1)
		BEGIN
			INSERT INTO ClassRegistration
			Values
			(
				@StudentId,
				@currentSemesterID,
				NULLIF(@classId,0),
				CASE WHEN @alreadyRegistered = 0 THEN @price1 
					 WHEN @alreadyRegistered=1 THEN @price2
					 WHEN @alreadyRegistered=2 THEN @price3
					 WHEN @alreadyRegistered=3 THEN @price4
					ELSE @price5
				END,
				NULL,
				1,
				GETDATE(),
				@UserID,
				GETDATE(),
				@UserID
					
			)

			INSERT INTO Transactions
			VALUES
			(
				@EndUserID,
				@parentID,
				'CHARGE', 
				@RegistrationDate,
				CASE WHEN @alreadyRegistered = 0 THEN 'First Child Tuition'
					 WHEN @alreadyRegistered=1 THEN 'Second child Tuition'
					 WHEN @alreadyRegistered=2 THEN  'Third Child Tuition'
					 WHEN @alreadyRegistered=3 THEN 'Fourth Child Tuition'
					ELSE 'Fifth or more Child Tuition'
				END,
				CASE WHEN @alreadyRegistered = 0 THEN @price1 
					 WHEN @alreadyRegistered=1 THEN @price2
					 WHEN @alreadyRegistered=2 THEN @price3
					 WHEN @alreadyRegistered=3 THEN @price4
					ELSE @price5
				END,
				1,
				GETDATE(),
				@UserID,
				GETDATE(),
				@UserID,
				@currentSemesterID
			)
		END

		IF NULLIF(@EnrichmentClassId,0) IS NOT NULL
		BEGIN 
			INSERT INTO EnrichmentClassRegistration
			Values
			(
				@StudentId,
				@currentSemesterID,
				@EnrichmentClassId,
				(SELECT 
					CASE WHEN (SELECT COUNT(ClassRegistrationId) FROM ClassRegistration WHERE StudentId = @StudentId AND SemesterId = @currentSemesterID and ActiveFlag = 1)>0 THEN Price1
					ELSE Price2 END
				 From EnrichmentClass WHERE ClassID = @EnrichmentClassId),
				 NULL,
				 1,
				 GETDATE(),
				@UserID,
				GETDATE(),
				@UserID
			)

			INSERT INTO Transactions
			VALUES
			(
				@EndUserID,
				@parentID,
				'CHARGE', 
				@RegistrationDate,
				'Enrichment Class Registration Fee',
				 (SELECT 
					CASE WHEN (SELECT COUNT(ClassRegistrationId) FROM ClassRegistration WHERE StudentId = @StudentId AND SemesterId = @currentSemesterID and ActiveFlag = 1)>0 THEN Price1
					ELSE Price2 END
				 From EnrichmentClass WHERE ClassID = @EnrichmentClassId),
				 1,
				GETDATE(),
				@UserID,
				GETDATE(),
				@UserID,
				@currentSemesterID
		
			)

		END

		IF (@IfRegister =1) OR (@EnrichmentClassId IS NOT NULL)
		BEGIN
			IF (SELECT COUNT(*) FROM Transactions WHERE UserId = @EndUserID AND SemesterID = @currentSemesterID AND TransactionDescription = 'Registration Fee' AND ActiveFlg =1)=0
			BEGIN
				INSERT INTO Transactions
				VALUES
			(
				@EndUserID,
				@parentID,
				'CHARGE', 
				@RegistrationDate,
				'Registration Fee',
				 (SELECT 
					CASE WHEN @RegistrationDate<=(SELECT MAX(RegisterEndDate) FROM Semester WHERE SemesterID = @currentSemesterID AND ActiveFlg=1) THEN RegistrationFeeBeforeEndDate
					ELSE RegistrationFeeAfterEndDate END
				 From Semester WHERE SemesterID = @currentSemesterID),
				 1,
				GETDATE(),
				@UserID,
				GETDATE(),
				@UserID,
				@currentSemesterID
		
			)
			END

			IF (SELECT COUNT(*) FROM Transactions WHERE UserId = @EndUserID AND SemesterID = @currentSemesterID AND TransactionDescription = 'Volunteer Deposit' AND ActiveFlg =1)=0
			BEGIN
				INSERT INTO Transactions
				VALUES
			(
				@EndUserID,
				@parentID,
				'CHARGE', 
				@RegistrationDate,
				'Volunteer Deposit',
				 (SELECT 
					VolunteerDeposit FROM Semester WHERE SemesterID = @currentSemesterID),
				 1,
				GETDATE(),
				@UserID,
				GETDATE(),
				@UserID,
				@currentSemesterID
		
			)
			END
		END

		COMMIT TRAN
	END TRY
	BEGIN CATCH
		SET @ErrorMsg=ERROR_MESSAGE()
		IF @@TRANCOUNT>0
		BEGIN
			ROLLBACK
		END
		RAISERROR(@ErrorMsg,16,1)
	END CATCH
	
GO


