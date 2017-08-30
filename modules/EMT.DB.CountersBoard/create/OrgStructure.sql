CREATE TABLE [dbo].[OrgStructure]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[ParentId] INT NULL,
	[Path] VARCHAR(1024) NULL,
	[Name] NVARCHAR(512) NOT NULL,
	[IsLine] BIT NOT NULL,
	CONSTRAINT [PK_OrgStructure_Id PRIMARY KEY] PRIMARY KEY ([Id]),
	CONSTRAINT [FK_OrgStructure_ParentId_OrgStructure_Id] FOREIGN KEY ([ParentId]) REFERENCES [dbo].[OrgStructure]([Id])
);