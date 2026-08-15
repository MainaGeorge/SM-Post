using CQRS.Core.Commands;

namespace CQRS.Core.Infrastructure;

public interface ICommandDispacher
{
    void RegisterHandler<T>(Func<T, Task> handler) where T : BaseCommand;
    Task SendAsync(BaseCommand command);
}
