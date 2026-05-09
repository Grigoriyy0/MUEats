using Prometheus;

namespace MUEats.Infrastructure.Metrics;

public static class InboxMetrics
{
    public static readonly Counter InboxProcessed = Prometheus.Metrics.CreateCounter("inbox_processed_total", "Total processed inbox messages", "status");
    public static readonly Histogram InboxLag = Prometheus.Metrics.CreateHistogram("inbox_queue_lag_seconds", "Lag between message creation and processing" , new HistogramConfiguration
        {
            Buckets = [1,5,15,30,60,300,900,3600],
        });
}