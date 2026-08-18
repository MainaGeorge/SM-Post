using Post.Query.Domain.Entitites;
using Post.Query.Domain.Repository;

namespace Post.Query.Api.Queries;

public class QueryHandler(IPostRepository repository) : IQueryHandler
{
    public async Task<List<PostEntity>> HandleAsync(FindAllPostQuery query) =>  await repository.ListAllAsync();
    public async Task<List<PostEntity>> HandleAsync(FindPostByAuthorQuery query) => await repository.ListByAuthorAsync(query.Author);
    public async Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query) => [await repository.GetByIdAsync(query.Id)];
    public async Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query) => await repository.ListWithLikesAsync(query.MinimumNumberOfLikes);
    public async Task<List<PostEntity>> HandleAsync(FindPostWithCommentsQuery query) => await repository.ListWithCommentsAsync();
}
