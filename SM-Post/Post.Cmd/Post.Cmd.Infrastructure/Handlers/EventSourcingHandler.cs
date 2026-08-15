using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Handlers;

public class EventSourcingHandler(IEventStore store) : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore = store;

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        var events = await _eventStore.GetEventsAsync(aggregateId);

        if(events is null || events.Count == 0)
            return aggregate;

        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(e => e.Version).Max();

        return aggregate;
    }

    public async Task SaveAsync(AggregateRoot aggregateRoot)
    {
        await _eventStore.SaveEventAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges(), aggregateRoot.Version);
        aggregateRoot.MarkChangesAsCommitted();
    }
}
