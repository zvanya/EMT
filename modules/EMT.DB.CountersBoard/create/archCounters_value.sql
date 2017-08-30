CREATE TABLE [dbo].[archCounters_value]
(
	[id] INT NOT NULL IDENTITY(1,1),
	[original_id] INT NOT NULL,
	[id_counter] INT NULL,
	[dt] DATETIME NULL,
	[value] FLOAT NULL,
	CONSTRAINT [PK_archCounters_value] PRIMARY KEY ([id]),
	CONSTRAINT [FK_archCounters_value_Counters] FOREIGN KEY([id_counter]) REFERENCES [dbo].[Counters] ([id])
);