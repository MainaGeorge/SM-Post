using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Cmd.Infrastructure.Config;

namespace Post.Cmd.Infrastructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly IMongoCollection<EventModel> _eventStoreCollection;

    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        var mongoUrlBuilder = new MongoUrlBuilder(config.Value.ConnectionString) { Username = config.Value.Username, Password = config.Value.Password};
        var mongoClient = new MongoClient(mongoUrlBuilder.ToMongoUrl());
        var mongoDatabase = mongoClient.GetDatabase(config.Value.Database);

        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(config.Value.Collection);
    }
    public async Task<List<EventModel>> FindByAggregateId(Guid aggregateId) => 
        await _eventStoreCollection.Find(x => x.AggregateIdentifier == aggregateId).ToListAsync().ConfigureAwait(false);


    public async Task SaveAsync(EventModel @event) => 
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
}
