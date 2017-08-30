CREATE ROLE [emt_data_reader];
CREATE ROLE [emt_data_writer];

ALTER ROLE [db_datareader] ADD MEMBER [emt_data_reader];
ALTER ROLE [db_datawriter] ADD MEMBER [emt_data_writer];

ALTER ROLE [emt_data_reader] ADD MEMBER [webapiuser];
ALTER ROLE [emt_data_writer] ADD MEMBER [webapireceiveuser];

GRANT EXECUTE ON [dbo].[spGetCounterValues] TO [emt_data_reader];
GRANT EXECUTE ON [dbo].[spGetLineStateValues] TO [emt_data_reader];
GRANT EXECUTE ON [dbo].[spGetLineModeValues] TO [emt_data_reader];
GRANT EXECUTE ON [dbo].[spGetBrandsValues] TO [emt_data_reader];