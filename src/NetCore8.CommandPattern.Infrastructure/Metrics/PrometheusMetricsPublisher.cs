using NetCore8.CommandPattern.Core.Metrics;
using Prometheus;

namespace NetCore8.CommandPattern.Infrastructure.Metrics
{
    public class PrometheusMetricsPublisher : IMetricsPublisher
    {
        private readonly Counter _commandExecutions;
        private readonly Counter _commandSuccesses;
        private readonly Counter _commandFailures;
        private readonly Histogram _commandDuration;
        private readonly Histogram _commandSize;

        public PrometheusMetricsPublisher()
        {
            _commandExecutions = global::Prometheus.Metrics.CreateCounter(
                "command_executions_total",
                "Total number of command executions",
                new CounterConfiguration { LabelNames = new[] { "command" } });

            _commandSuccesses = global::Prometheus.Metrics.CreateCounter(
                "command_successes_total",
                "Total number of successful command executions",
                new CounterConfiguration { LabelNames = new[] { "command" } });

            _commandFailures = global::Prometheus.Metrics.CreateCounter(
                "command_failures_total",
                "Total number of failed command executions",
                new CounterConfiguration { LabelNames = new[] { "command" } });

            _commandDuration = global::Prometheus.Metrics.CreateHistogram(
                "command_duration_seconds",
                "Command execution duration in seconds",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "command" },
                    Buckets = new[] { .005, .01, .025, .05, .075, .1, .25, .5, .75, 1, 2.5, 5, 7.5, 10 }
                });

            _commandSize = global::Prometheus.Metrics.CreateHistogram(
                "command_size_bytes",
                "Command size in bytes",
                new HistogramConfiguration
                {
                    LabelNames = new[] { "command" },
                    Buckets = global::Prometheus.Histogram.ExponentialBuckets(start: 1, factor: 2, count: 10)
                });
        }

        public void Increment(string metric, string[] labels = null)
        {
            switch (metric)
            {
                case "command_executions_total":
                    _commandExecutions.WithLabels(labels).Inc();
                    break;
                case "command_successes_total":
                    _commandSuccesses.WithLabels(labels).Inc();
                    break;
                case "command_failures_total":
                    _commandFailures.WithLabels(labels).Inc();
                    break;
            }
        }

        public void Decrement(string metric, string[] labels = null)
        {
            // Prometheus counters cannot be decremented
        }

        public void Record(string metric, double value, string[] labels = null)
        {
            switch (metric)
            {
                case "command_duration_seconds":
                    _commandDuration.WithLabels(labels).Observe(value);
                    break;
                case "command_size_bytes":
                    _commandSize.WithLabels(labels).Observe(value);
                    break;
            }
        }
    }
}