
using NetCore8.CommandPattern.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Infrastructure
{
    public class TransactionManager : ITransactionManager
    {
        private readonly DbContext _dbContext;

        public TransactionManager(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var result = await action();
                await transaction.CommitAsync();
                return result;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}