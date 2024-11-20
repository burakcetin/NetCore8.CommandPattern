
namespace NetCore8.CommandPattern.Core.Commands.Interfaces
{
    public interface IRateLimitedCommand 
    {
        string RateLimitKey { get; }
        int MaxRequests { get; }
        TimeSpan TimeWindow { get; }
    }
}