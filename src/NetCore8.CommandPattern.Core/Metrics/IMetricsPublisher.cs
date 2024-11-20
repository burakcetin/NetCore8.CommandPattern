
using System.Diagnostics.Metrics;

namespace NetCore8.CommandPattern.Core.Metrics
{
    public interface IMetricsPublisher
    {
        void Increment(string metric, string[] labels = null);
        void Decrement(string metric, string[] labels = null);
        void Record(string metric, double value, string[] labels = null);
    }
}