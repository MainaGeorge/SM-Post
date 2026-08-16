using Post.Query.Domain.Entitites;

namespace Post.Query.Domain.Repository;

public interface IPostRepository
{
    Task CreateAsync(PostEntity entity);
    Task UpdateAsync(PostEntity entity);
    Task DeleteAsync(Guid postId);
    Task<List<PostEntity>> ListAllAsync();
    Task<List<PostEntity>> ListByAuthorAsync(string author);
    Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes);
    Task<List<PostEntity>> ListWithCommentsAsync();
    Task<PostEntity> GetByIdAsync(Guid postId);
}
