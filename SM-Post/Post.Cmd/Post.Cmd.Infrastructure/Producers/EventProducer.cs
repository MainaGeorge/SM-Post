using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Post.Cmd.Infrastructure.Producers;

public class EventProducer(IOptions<ProducerConfig> config) : IEventProducer
{
    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(config.Value)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        var eventType = @event.GetType();

        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, eventType)
        };

        var deliveryResult = await producer.ProduceAsync(topic, message);

        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            throw new Exception($"could not produce {eventType.Name} message to topic - {topic} due to the following reason: {deliveryResult.Message}!");
    }
}
