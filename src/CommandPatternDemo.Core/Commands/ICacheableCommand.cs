
using System;

namespace CommandPatternDemo.Core.Commands
{
    public interface ICacheableCommand : ICommand
    {
        string CacheKey { get; }
        TimeSpan CacheDuration { get; }
    }
}