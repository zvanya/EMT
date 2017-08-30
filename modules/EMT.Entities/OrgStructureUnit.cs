using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMT.Entities
{
    public class OrgStructureUnit
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public bool IsLine { get; set; }
    }
}
