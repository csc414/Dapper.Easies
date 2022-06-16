using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Tests.Entities
{
    [DbObject("tb_menus")]
    public class Menu : IDbTable
    {
        [DbProperty(PrimaryKey = true)]
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }
    }
}
