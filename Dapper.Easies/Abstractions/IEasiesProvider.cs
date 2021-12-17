﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Dapper.Easies
{
    public interface IEasiesProvider : IConnection
    {
		IDbQuery<T> Query<T>() where T : IDbObject;

        Task<bool> InsertAsync<T>(T entity) where T : IDbTable;

        Task<int> DeleteAsync<T>(Expression<Predicate<T>> predicate = null) where T : IDbTable;

        Task<int> DeleteAsync(IDbQuery query);

        Task<int> DeleteCorrelationAsync(IDbQuery query);

        Task<int> UpdateAsync<T>(Expression<Func<T>> updateFields, Expression<Predicate<T>> predicate = null) where T : IDbTable;

        Task<int> UpdateAsync<T>(T entity) where T : IDbTable;
    }
}
