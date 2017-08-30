CREATE PROCEDURE [dbo].[spArchiveCounterValues]	
	@Before DATETIME
AS
BEGIN
	DECLARE @StartTime DATETIME;
	DECLARE @FinishTime DATETIME;
	DECLARE @ItemsCount INT;
	DECLARE @ItemsTable TABLE
	(
		[original_id] INT NOT NULL,
		[id_counter] INT NULL,
		[dt] DATETIME NULL,
		[value] FLOAT NULL
	);

	BEGIN TRY

		BEGIN TRAN;

		SET @StartTime = GETDATE();

		INSERT INTO @ItemsTable 
			([original_id], [id_counter], [dt], [value])
		SELECT 
			[id], [id_counter], [dt], [value] 
		FROM [dbo].[Counters_value]
		WHERE [dt] < @Before;

		SELECT @ItemsCount = COUNT(*) FROM @ItemsTable;

		DELETE FROM [dbo].[Counters_value] WHERE [id] IN (SELECT [original_id] FROM @ItemsTable);

		INSERT INTO [dbo].[archCounters_value]
			([original_id], [id_counter], [dt], [value])
		SELECT 
			[original_id], [id_counter], [dt], [value] 
		FROM @ItemsTable

		SET @FinishTime = GETDATE();

		INSERT INTO [dbo].[ArchiveCounterValuesHistory] 
			([BeforeTime], [ItemsCount], [ArchStartTime], [ArchFinishTime])
		VALUES 
			(@Before, @ItemsCount, @StartTime, @FinishTime);

		COMMIT TRAN;

	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
		BEGIN
			ROLLBACK TRAN;
		END;

		DECLARE @ErrorMessage NVARCHAR(4000);
		DECLARE @ErrorSeverity INT;
		DECLARE @ErrorState INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(),
			@ErrorSeverity = ERROR_SEVERITY(),
			@ErrorState = ERROR_STATE();
		
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);

	END CATCH;
END;