using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    private readonly List<BaseEvent> _changes = [];
    protected Guid _id;
    public Guid Id => _id;
    public int Version { get; set; } = -1;
    public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes;
    public void MarkChangesAsCommitted() => _changes.Clear();
    public void ApplyChange(BaseEvent @event, bool isNew)
    {
        var eventType = @event.GetType();
        var method = GetType().GetMethod("Apply", [eventType]);

        ArgumentNullException.ThrowIfNull(method, $"The apply method was not found in the aggregate for {eventType.Name}");

        method.Invoke(this, [@event]);

        if (isNew)
            _changes.Add(@event);
    }
    protected void RaiseEvent(BaseEvent @event) => ApplyChange(@event, true);
    public void ReplayEvents(IEnumerable<BaseEvent> @events)
    {
        foreach(var @event in @events)
            ApplyChange(@event, false);
    }
}
