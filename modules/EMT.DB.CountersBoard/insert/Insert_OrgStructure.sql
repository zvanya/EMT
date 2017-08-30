SET IDENTITY_INSERT [dbo].[OrgStructure] ON

INSERT INTO [dbo].[OrgStructure] ([Id], [ParentId], [Path], [Name], [IsLine]) 
	VALUES 
	(16, NULL, '16.',  'Харьков[KHR]', 0),
	(17, NULL, '17.', 'Десна[CHR]', 0),
	(1, 16, '16.1.', 'PET 1', 1),
	(2, 16, '16.2.', 'RGB 2', 1),
	(3, 16, '16.3.', 'PET 3', 1),
	(4, 16, '16.4.', 'RGB 3', 1),
	(5, 16, '16.5.', '_', 1),
	(11, 17, '17.11.', 'RGB 1', 1),
	(12, 17, '17.12.', 'PET 1', 1),
	(13, 17, '17.13.', 'RGB 2', 1),
	(14, 17, '17.14.', 'CAN 1', 1);

SET IDENTITY_INSERT [dbo].[OrgStructure] ON