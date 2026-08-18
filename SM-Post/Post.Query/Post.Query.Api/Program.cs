using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entitites;
using Post.Query.Domain.Repository;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAccess;
using Post.Query.Infrastructure.Dispatchers;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddDbContext<DatabaseContext>(opts => opts.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
builder.Services.AddSingleton(new DatabaseContextFactory(opts => opts.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"))));
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IQueryHandler, QueryHandler>();
builder.Services.AddScoped<IEventHandler, Post.Query.Infrastructure.Handlers.EventHandler>();
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddHostedService<ConsumerHostedService>();

builder.Services.AddSingleton<IQueryDispatcher<PostEntity>>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    var dispatcher = new QueryDispatcher();

    // Helper method to create a new scope for every command execution
    async Task<List<PostEntity>> ExecuteScoped<TQuery>(TQuery query, Func<IQueryHandler, TQuery, Task<List<PostEntity>>> action)
    {
        using var scope = scopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IQueryHandler>();
        return await action(handler, query);
    }

    dispatcher.RegisterHandler<FindAllPostQuery>(c => ExecuteScoped(c, (q, h) => q.HandleAsync(h)));
    dispatcher.RegisterHandler<FindPostByAuthorQuery>(c => ExecuteScoped(c, (q, h) => q.HandleAsync(h)));
    dispatcher.RegisterHandler<FindPostByIdQuery>(c => ExecuteScoped(c, (q, h) => q.HandleAsync(h)));
    dispatcher.RegisterHandler<FindPostWithCommentsQuery>(c => ExecuteScoped(c, (q, h) => q.HandleAsync(h)));
    dispatcher.RegisterHandler<FindPostsWithLikesQuery>(c => ExecuteScoped(c, (q, h) => q.HandleAsync(h)));

    return dispatcher;
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
dbContext.Database.EnsureCreated();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
