using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;
using System.Text.Json;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer(IOptions<ConsumerConfig> config, IEventHandler eventHandler) : IEventConsumer
{
    public Task Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(config.Value)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);

        while (true)
        {
            var consumerResult = consumer.Consume();

            if (consumerResult?.Message == null) continue;

            var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options: new JsonSerializerOptions { Converters = { new EventJsonConverter() } });

            var handlerMethod = eventHandler.GetType().GetMethod("On", [@event.GetType()]);

            ArgumentNullException.ThrowIfNull(handlerMethod, "could not find handler event method!");

            handlerMethod.Invoke(eventHandler, [@event]);

            consumer.Commit(consumerResult);
        }
    }
}
