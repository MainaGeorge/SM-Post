using Confluent.Kafka;
using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Post.Cmd.Api.Commands;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Config;
using Post.Cmd.Infrastructure.Dispatchers;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Producers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));
builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped<IEventProducer, EventProducer>();
builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
builder.Services.AddScoped<ICommandHandler, CommandHandler>();
builder.Services.AddSingleton<ICommandDispatcher>(sp =>
{
    // Resolve IServiceScopeFactory (Singleton) instead of the Scoped handler directly
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    var dispatcher = new CommandDispatcher();

    // Helper method to create a new scope for every command execution
    async Task ExecuteScoped<TCommand>(TCommand command, Func<ICommandHandler, TCommand, Task> action)
    {
        using var scope = scopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler>();
        await action(handler, command);
    }

    dispatcher.RegisterHandler<NewPostCommand>(cmd => ExecuteScoped(cmd, (h, c) => h.HandleAsync(c)));
    dispatcher.RegisterHandler<EditMessageCommand>(cmd => ExecuteScoped(cmd, (h, c) => h.HandleAsync(c)));
    dispatcher.RegisterHandler<LikePostCommand>(cmd => ExecuteScoped(cmd, (h, c) => h.HandleAsync(c)));
    dispatcher.RegisterHandler<AddCommentCommand>(cmd => ExecuteScoped(cmd, (h, c) => h.HandleAsync(c)));
    dispatcher.RegisterHandler<EditCommentCommand>(cmd => ExecuteScoped(cmd, (h, c) => h.HandleAsync(c)));
    dispatcher.RegisterHandler<RemoveCommentCommand>(cmd => ExecuteScoped(cmd, (h, c) => h.HandleAsync(c)));
    dispatcher.RegisterHandler<DeletePostCommand>(cmd => ExecuteScoped(cmd, (h, c) => h.HandleAsync(c)));
    dispatcher.RegisterHandler<DeleteCommentCommand>(cmd => ExecuteScoped(cmd, (h, c) => h.HandleAsync(c)));

    return dispatcher;
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();