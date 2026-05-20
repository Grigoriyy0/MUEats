using Prometheus;

namespace MUEats.Infrastructure.Metrics;

public static class OutboxMetrics
{
    public static readonly Counter OutboxProcessed = Prometheus.Metrics.CreateCounter("outbox_processed_total", "Total processed outbox messages", "status");
    public static readonly Histogram OutboxLag = Prometheus.Metrics.CreateHistogram("outbox_queue_lag_seconds", "Lag between message creation and processing" , new HistogramConfiguration
    {
        Buckets = [1,5,15,30,60,300,900,3600],
    });
}