
using System;

namespace NetCore8.CommandPattern.Core.Commands
{
    public interface ICacheableCommand : ICommand
    {
        string CacheKey { get; }
        TimeSpan CacheDuration { get; }
    }
}