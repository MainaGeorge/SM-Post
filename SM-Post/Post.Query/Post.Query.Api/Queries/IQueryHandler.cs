using Post.Query.Domain.Entitites;

namespace Post.Query.Api.Queries;

public interface IQueryHandler
{
    Task<List<PostEntity>> HandleAsync(FindAllPostQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostByAuthorQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostByIdQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostsWithLikesQuery query);
    Task<List<PostEntity>> HandleAsync(FindPostWithCommentsQuery query);
}
