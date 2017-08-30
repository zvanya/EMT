CREATE PROCEDURE [dbo].[spGetCounterValues]
	@CounterId INT,
	@TimeFrom DATETIME,
	@TimeTo DATETIME
AS
BEGIN
	DECLARE @ArchiveLatest DATETIME;

	SELECT @ArchiveLatest = MAX([BeforeTime]) FROM [dbo].[ArchiveCounterValuesHistory];

	IF @TimeTo IS NOT NULL
	BEGIN
		IF @TimeFrom >= @ArchiveLatest
		BEGIN
			SELECT [dt], [value] FROM [dbo].[Counters_value] WHERE [id_counter] = @CounterId AND [dt] >= @TimeFrom AND [dt] <= @TimeTo ORDER BY [dt];
		END
		ELSE
		BEGIN
			SELECT [dt], [value] FROM [dbo].[Counters_value] WHERE [id_counter] = @CounterId AND [dt] >= @TimeFrom AND [dt] <= @TimeTo
			UNION
			SELECT [dt], [value] FROM [dbo].[archCounters_value] WHERE [id_counter] = @CounterId AND [dt] >= @TimeFrom AND [dt] <= @TimeTo			
			ORDER BY [dt];
		END;
	END
	ELSE
	BEGIN
		IF @TimeFrom >= @ArchiveLatest
		BEGIN
			SELECT [dt], [value] FROM [dbo].[Counters_value] WHERE [id_counter] = @CounterId AND [dt] > @TimeFrom ORDER BY [dt];
		END
		ELSE
		BEGIN
			SELECT [dt], [value] FROM [dbo].[Counters_value] WHERE [id_counter] = @CounterId AND [dt] > @TimeFrom 
			UNION
			SELECT [dt], [value] FROM [dbo].[archCounters_value] WHERE [id_counter] = @CounterId AND [dt] > @TimeFrom 
			ORDER BY [dt];
		END;
	END;
END;