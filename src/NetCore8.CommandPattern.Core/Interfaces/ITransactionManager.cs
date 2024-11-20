
using System;
using System.Threading.Tasks;

namespace NetCore8.CommandPattern.Core.Interfaces
{
    public interface ITransactionManager
    {
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}