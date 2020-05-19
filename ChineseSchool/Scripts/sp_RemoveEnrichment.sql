create procedure dbo.sp_RemoveEnrichment
@StudentId int, @UserId AS NVARCHAR(128)
AS 
BEGIN 

	DECLARE @ErrMsg NVARCHAR(4000)
	DECLARE @ParentId AS INT
	SET @ParentId = (SELECT ParentsId FROM Student WHERE StudentId = @StudentId)
	DECLARE @ParentUserId NVARCHAR(128) = (SELECT UserId FROM dbo.Parent WHERE ParentsId = @ParentId)
	DECLARE @CurrentSemester AS INT 
	SET @CurrentSemester = (SELECT MAX(SemesterID) FROM Semester WHERE ActiveFlg = 1)
	DECLARE @EnrichmentRegID INT
	SET @EnrichmentRegID = (SELECT MAX(EnrichmentClassRegistrationId) FROM EnrichmentClassRegistration WHERE ActiveFlag = 1 AND StudentId = @StudentId AND SemesterId = @CurrentSemester)
	IF (@EnrichmentRegID IS NULL)
	BEGIN
		RETURN
	END
	DECLARE @PreviousBalance AS DECIMAL(18,2)
	SET @PreviousBalance = dbo.CalculateCharge(@ParentId)
	
	BEGIN TRAN
	BEGIN TRY
	DELETE EnrichmentClassRegistration WHERE EnrichmentClassRegistrationId = @EnrichmentRegID

	DECLARE @CurrentBalance AS DECIMAL(18,2)
	SET @CurrentBalance = dbo.CalculateCharge(@ParentId)

	INSERT INTO dbo.Transactions
	(
		UserID,
		ParentsID,
		TRansactionType,
		TransactionDate,
		TransactionDescription,
		TransactionAmount,
		ActiveFlg,
		CreateTimeStemp,
		CreateUserId,
		UpdateTimeStemp,
		UpdateUserId,
		SemesterID
	)
	Values
	(
		@ParentUserId,
		@ParentId,
		'Adjustment',
		GETDATE(),
		'Removed Enrichment Class Registration',
		@CurrentBalance-@PreviousBalance,
		1,
		GETDATE(),
		@UserId,
		GETDATE(),
		@UserID,
		@CurrentSemester

	)

	EXEC dbo.sp_updateinvoice @ParentId

	COMMIT TRAN

	END TRY
	BEGIN CATCH
		SET @ErrMsg = ERROR_MESSAGE()
		IF @@TRANCOUNT > 0 ROLLBACK TRAN
		RAISERROR(@ErrMsg,16,1)
	END CATCH 

END