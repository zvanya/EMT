CREATE TABLE [dbo].[ArchiveCounterValuesHistory]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[BeforeTime] DATETIME NOT NULL,
	[ItemsCount] INT NOT NULL,
	[ArchStartTime] DATETIME NOT NULL,
	[ArchFinishTime] DATETIME NOT NULL,
	CONSTRAINT [PK_ArchiveCounterValuesHistory_Id] PRIMARY KEY ([Id])
);

GO

CREATE INDEX [IX_ArchiveCounterValuesHistory_BeforeTime] ON [dbo].[ArchiveCounterValuesHistory] ([BeforeTime]);

GO