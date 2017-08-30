using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EMT.Web.Tools.Generator.Models
{
    public class OrgStructureUnitModel
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public bool IsLine { get; set; }
    }
}