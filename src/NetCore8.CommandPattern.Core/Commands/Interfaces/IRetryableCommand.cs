
namespace NetCore8.CommandPattern.Core.Commands.Interfaces
{
    public interface IRetryableCommand 
    {
        int MaxRetries { get; }
        TimeSpan RetryDelay { get; }
        bool ShouldRetry(Exception ex);
    }
}