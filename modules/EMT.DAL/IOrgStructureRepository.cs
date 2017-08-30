using EMT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.DAL
{
    public interface IOrgStructureRepository
    {
        IEnumerable<OrgStructureUnit> GetAll();
    }
}
