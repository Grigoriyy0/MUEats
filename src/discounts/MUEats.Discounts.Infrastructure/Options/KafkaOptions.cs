namespace MUEats.Discounts.Infrastructure.Options;

public class KafkaOptions
{
    public string BootstrapServers { get; set; }
    
    public Dictionary<string, ConsumerOptions> ConsumerOptions { get; set; }
}