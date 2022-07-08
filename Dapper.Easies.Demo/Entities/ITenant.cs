using System;
using System.Collections.Generic;
using System.Text;

namespace Dapper.Easies.Demo
{
    public interface ITenant
    {
        public Guid? TenantId { get; set; }
    }
}
