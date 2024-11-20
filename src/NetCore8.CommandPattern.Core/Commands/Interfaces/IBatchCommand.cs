
namespace NetCore8.CommandPattern.Core.Commands.Interfaces
{
    public interface IBatchCommand<T> 
    {
        IEnumerable<T> Items { get; }
        int BatchSize { get; }
    }
}