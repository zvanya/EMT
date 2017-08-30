using EMT.Common.Services.Base;
using EMT.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMT.Common.Extentions;

namespace EMT.DAL.Sql.Repositories
{
    public class SqlOrgStructureRepository: IOrgStructureRepository
    {
        #region Queries

        private const string SelectAllQuery = "SELECT [Id],[ParentId],[Path],[Name],[IsLine] FROM [dbo].[OrgStructure]";

        #endregion

        #region Fields

        private readonly IDatabaseSettings _databaseSettings;
        private readonly IProfilerService _profilerService;

        #endregion

        #region Constructors

        public SqlOrgStructureRepository(IDatabaseSettings databaseSettings, IProfilerService profilerService)
        {
            _databaseSettings = databaseSettings;
            _profilerService = profilerService;
        }

        #endregion

        #region IStructureEntityRepository

        public IEnumerable<OrgStructureUnit> GetAll()
        {
            using (_profilerService.Step("SqlStructureEntityRepository.GetAll()"))
            {
                using (SqlConnection connection = new SqlConnection(_databaseSettings.ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(SelectAllQuery, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<OrgStructureUnit> structureEntities = new List<OrgStructureUnit>();
                            while (reader.Read())
                            {
                                var structureEntity = new OrgStructureUnit();
                                structureEntity.Id = reader.GetValue<int>("Id");
                                structureEntity.ParentId = reader.GetNullableValue<int?>("ParentId");
                                structureEntity.Path = reader.GetValue<string>("Path");
                                structureEntity.Name = reader.GetValue<string>("Name");
                                structureEntity.IsLine = reader.GetValue<bool>("IsLine");
                                structureEntities.Add(structureEntity);
                            }
                            return structureEntities;
                        }
                    }
                }
            }

        }

        #endregion
    }
}
