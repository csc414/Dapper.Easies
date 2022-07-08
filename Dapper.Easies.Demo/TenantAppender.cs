using System;
using System.Linq.Expressions;

namespace Dapper.Easies.Demo
{
    public class TenantAppender : IAppender<ITenant>, IAppender<Student>
    {
        Expression<Func<ITenant, bool>> IAppender<ITenant>.Append()
        {
            return o => o.TenantId == Guid.NewGuid();
        }

        Expression<Func<Student, bool>> IAppender<Student>.Append()
        {
            return o => o.Age > 18;
        }
    }
}
