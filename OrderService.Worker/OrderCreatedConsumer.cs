using MassTransit;
using OrderService.Shared.NewFolder;
using StackExchange.Redis;

namespace OrderService.Worker.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreatedEvent>
{
    private readonly IDatabase _redis;

    public OrderCreatedConsumer(IConnectionMultiplexer redis)
    {
        _redis = redis.GetDatabase();
    }

    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        var order = context.Message;

        Console.WriteLine($"[Worker] Sipariş alındı: {order.OrderId} → işleniyor...");

        await Task.Delay(3000); // işlem simülasyonu

        var logKey = $"processed_order:{order.OrderId}";
        var logValue = $"İşlendi: {DateTime.UtcNow:O}";

        await _redis.StringSetAsync(logKey, logValue);

        Console.WriteLine($"[Worker] Sipariş işlendi ve Redis'e loglandı → {logKey}");
    }
}
