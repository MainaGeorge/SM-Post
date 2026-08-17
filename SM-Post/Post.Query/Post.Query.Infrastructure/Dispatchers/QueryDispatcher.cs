using CQRS.Core.Infrastructure;
using CQRS.Core.Queries;
using Post.Query.Domain.Entitites;

namespace Post.Query.Infrastructure.Dispatchers;

public class QueryDispatcher : IQueryDispatcher<PostEntity>
{
    private readonly Dictionary<Type, Func<BaseQuery, Task<List<PostEntity>>>> _handlers = [];
    public void RegisterHandler<TQuery>(Func<TQuery, Task<List<PostEntity>>> handler) where TQuery : BaseQuery
    {
        var type = typeof(TQuery);

        if(_handlers.ContainsKey(type))
            throw new InvalidOperationException($"You can not register the same handler twice (handler for {type.Name}");

        _handlers.Add(type, x => handler((TQuery)x));

    }

    public async Task<List<PostEntity>> SendAsync(BaseQuery query)
    {
        var type = query.GetType();

        if (_handlers.TryGetValue(type, out var handler))
            return await handler(query);

        throw new ArgumentNullException(nameof(type.Name), "No query handler was registered!");
    }
}
