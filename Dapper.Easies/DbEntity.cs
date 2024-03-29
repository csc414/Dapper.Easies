﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dapper.Easies
{
    public sealed class DbEntity<T> : IDbQuery<T> where T : IDbTable
    {
        private readonly IEasiesProvider _provider;

        public QueryContext Context => _provider.From<T>().Context;

        internal DbEntity(IEasiesProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// 根据主键获取实体对象
        /// </summary>
        /// <param name="ids">主键，若复合主键请参考模型中主键的顺序依次传入</param>
        /// <returns></returns>
        public Task<T> GetAsync(params object[] ids) => _provider.GetAsync<T>(ids);

        /// <summary>
        /// 新增实体对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<bool> InsertAsync(T entity) => _provider.InsertAsync(entity);

        /// <summary>
        /// 批量新增实体对象，批量新增无法回填实体自增Id
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(IEnumerable<T> entities) => _provider.InsertAsync(entities);

        /// <summary>
        /// 删除实体对象
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(T entity) => _provider.DeleteAsync(entity);

        /// <summary>
        /// 批量删除实体对象
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public Task<int> DeleteAsync(IEnumerable<T> entities) => _provider.DeleteAsync(entities);

        /// <summary>
        /// 更新实体对象除主键外的所有字段
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(T entity) => _provider.UpdateAsync(entity);

        /// <summary>
        /// 批量更新实体对象除主键外的所有字段
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(IEnumerable<T> entities) => _provider.UpdateAsync(entities);

        public ISelectedDbQuery<TResult> Select<TResult>(Expression<Func<T, TResult>> selector) => _provider.From<T>().Select(selector);

        public ISelectedDbQuery<TResult> Select<TResult>() => _provider.From<T>().Select<TResult>();

        public IDbQuery<T> Where(Expression<Func<T, bool>> predicate) => _provider.From<T>().Where(predicate);

        public IDbQuery<T> Where(Expression<Func<T, string>> expression) => _provider.From<T>().Where(expression);

        public IDbQuery<T, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, JoinType type = JoinType.Inner) where TJoin : class => _provider.From<T>().Join(query, type);

        public IDbQuery<T, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T, TJoin, bool>> on, JoinType type = JoinType.Inner) where TJoin : class => _provider.From<T>().Join(query, on, type);

        public IDbQuery<T, TJoin> Join<TJoin>(ISelectedDbQuery<TJoin> query, Expression<Func<T, TJoin, string>> on, JoinType type = JoinType.Inner) where TJoin : class => _provider.From<T>().Join(query, on, type);

        public IDbQuery<T, TJoin> Join<TJoin>(JoinType type = JoinType.Inner) where TJoin : IDbObject => _provider.From<T>().Join<TJoin>(type);

        public IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, bool>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject => _provider.From<T>().Join(on, type);

        public IDbQuery<T, TJoin> Join<TJoin>(Expression<Func<T, TJoin, string>> on = null, JoinType type = JoinType.Inner) where TJoin : IDbObject => _provider.From<T>().Join(on, type);

        public IOrderedDbQuery<T> OrderBy(Expression<Func<T, object>> orderFields) => _provider.From<T>().OrderBy(orderFields);

        public IOrderedDbQuery<T> OrderByDescending(Expression<Func<T, object>> orderFields) => _provider.From<T>().OrderByDescending(orderFields);

        public IGroupingDbQuery<T> GroupBy(Expression<Func<T, object>> fields) => _provider.From<T>().GroupBy(fields);

        public Task<T> FirstAsync() => _provider.From<T>().FirstAsync();

        public Task<TResult> FirstAsync<TResult>() where TResult : struct
            => _provider.From<T>().FirstAsync<TResult>();

        public Task<T> FirstOrDefaultAsync() => _provider.From<T>().FirstOrDefaultAsync();

        public Task<TResult?> FirstOrDefaultAsync<TResult>() where TResult : struct => _provider.From<T>().FirstOrDefaultAsync<TResult>();

        public Task<IEnumerable<T>> QueryAsync() => _provider.From<T>().QueryAsync();

        public Task<IEnumerable<TResult>> QueryAsync<TResult>() where TResult : struct => _provider.From<T>().QueryAsync<TResult>();

        public Task<bool> ExistAsync() => _provider.From<T>().ExistAsync();

        public Task<int> DeleteAsync() => _provider.From<T>().DeleteAsync();

        public Task<int> UpdateAsync(Expression<Func<T>> updateFields) => _provider.From<T>().UpdateAsync(updateFields);

        public Task<int> UpdateAsync(Expression<Func<T, T>> updateFields) => _provider.From<T>().UpdateAsync(updateFields);

        public Task<long> CountAsync() => _provider.From<T>().CountAsync();

        public Task<long> CountAsync(Expression<Func<T, object>> field) => _provider.From<T>().CountAsync(field);

        public Task<TResult> MaxAsync<TResult>(Expression<Func<T, TResult>> field) => _provider.From<T>().MaxAsync(field);

        public Task<TResult> MinAsync<TResult>(Expression<Func<T, TResult>> field) => _provider.From<T>().MinAsync(field);

        public Task<decimal> AvgAsync<TField>(Expression<Func<T, TField>> field) => _provider.From<T>().AvgAsync(field);

        public Task<TField> SumAsync<TField>(Expression<Func<T, TField>> field) => _provider.From<T>().SumAsync(field);

        public IDbQuery<T> Skip(int count) => _provider.From<T>().Skip(count);

        public IDbQuery<T> Take(int count) => _provider.From<T>().Take(count);

        public IDbQuery<T> Distinct() => _provider.From<T>().Distinct();

        public Task<(IEnumerable<T> data, long total, int max_page)> GetPagerAsync(int page, int size) => _provider.From<T>().GetPagerAsync(page, size);

        public Task<(IEnumerable<TResult> data, long total, int max_page)> GetPagerAsync<TResult>(int page, int size) where TResult : struct => _provider.From<T>().GetPagerAsync<TResult>(page, size);

        public Task<(IEnumerable<T> data, long total)> GetLimitAsync(int skip, int take) => _provider.From<T>().GetLimitAsync(skip, take);

        public Task<(IEnumerable<TResult> data, long total)> GetLimitAsync<TResult>(int skip, int take) where TResult : struct => _provider.From<T>().GetLimitAsync<TResult>(skip, take);
    }
}
