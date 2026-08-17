using CQRS.Core.Commands;
using CQRS.Core.Infrastructure;

namespace Post.Cmd.Infrastructure.Dispatchers;

public class CommandDispatcher : ICommandDispatcher
{
    private readonly Dictionary<Type, Func<BaseCommand, Task>> _handlers = [];

    public void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand
    {
        var type = typeof(T);

        if (_handlers.ContainsKey(type))
            throw new InvalidOperationException($"You can not register the same handler twice (handler for {type.Name}");

        _handlers.Add(type, x => handler((T)x));
    }

    public async Task SendAsync(BaseCommand command)
    {
        if (_handlers.TryGetValue(command.GetType(), out var handler))
            await handler(command);
        else
            ArgumentNullException.ThrowIfNull(handler, "No command handler was registered!");
    }
}
